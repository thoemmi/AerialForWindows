using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using SharpBits.Base;

namespace AerialForWindows.Services {
    public class MovieManager {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly List<Movie> _movies = new List<Movie>();

        private static readonly string MoviesPath = Path.Combine(AppEnvironment.DataFolder, "movies.json");

        private void EnsureLoaded() {
            if (File.Exists(MoviesPath)) {
                try {
                    var fileData = File.ReadAllText(MoviesPath);
                    JsonConvert.PopulateObject(fileData, _movies);
                    _logger.Debug("Movies loaded");
                } catch (Exception ex) {
                    _logger.Error(ex, "Loading movies failed");
                }
            }

            var assetClient = new AssetClient();
            foreach (var asset in assetClient.GetAssets()) {
                if (_movies.All(a => a.AssetId != asset.Id)) {
                    var movie = new Movie {
                        AssetId = asset.Id,
                        DownloadUrl = asset.Url,
                        Location = asset.AccessibilityLabel,
                        TimeOfDay = asset.TimeOfDay
                    };
                    _movies.Add(movie);
                }
            }

            if (Settings.Instance.ShouldCacheMovies) {
                DownloadMovies();
            }

            Save();
        }

        private void DownloadMovies() {
            using (var bitsManager = new BitsManager()) {
                bitsManager.OnInterfaceError += (sender, args) => _logger.Error("BITS error: {0}\n{1}", args.Message, args.Description);

                if (CheckRunningJob(bitsManager)) {
                    return;
                }

                var moviesToDownload = _movies
                    .Where(m => string.IsNullOrEmpty(m.LocalPath) || !File.Exists(m.LocalPath))
                    .Take(200)
                    .ToList();

                if (moviesToDownload.Any()) {
                    var job = bitsManager.CreateJob("AerialForWindows", JobType.Download);
                    job.Priority = JobPriority.Low;
                    job.MinimumRetryDelay = 5*60; // 5 minutes
                    job.NoProgressTimeout = 30*60; // 30 minutes

                    foreach (var movie in moviesToDownload) {
                        var localPath = Path.Combine(Settings.Instance.CachePath, Path.GetFileName(movie.DownloadUrl));
                        movie.LocalPath = localPath;
                        job.AddFile(movie.DownloadUrl, localPath);
                    }

                    job.Resume();
                    Settings.Instance.BitsJobId = job.JobId;
                    Settings.Instance.Save();

                    _logger.Debug($"Startet download of {moviesToDownload.Count} files");
                }
            }
        }

        public static void CancelRunningJob() {
            if (Settings.Instance.BitsJobId.HasValue) {
                using (var bitsManager = new BitsManager()) {
                    BitsJob runningJob;
                    if (bitsManager.EnumJobs().TryGetValue(Settings.Instance.BitsJobId.Value, out runningJob)) {
                        runningJob.Cancel();
                    }
                }
                Settings.Instance.BitsJobId = null;
                Settings.Instance.Save();
            }
        }

        private static bool CheckRunningJob(BitsManager bitsManager) {
            if (Settings.Instance.BitsJobId.HasValue) {
                BitsJob runningJob;
                if (bitsManager.EnumJobs().TryGetValue(Settings.Instance.BitsJobId.Value, out runningJob)) {
                    if (runningJob.State == JobState.Transferred) {
                        _logger.Debug("Download completed");
                        Settings.Instance.BitsJobId = null;
                        Settings.Instance.Save();
                        runningJob.Complete();
                    } else if (runningJob.State == JobState.Error) {
                        var error = runningJob.Error;
                        _logger.Error($"Download of {error.File.RemoteName} failed: {error.Description}\n{error.ContextDescription}");
                        Settings.Instance.BitsJobId = null;
                        Settings.Instance.Save();
                        runningJob.Cancel();
                    } else if (runningJob.State == JobState.TransientError) {
                        var error = runningJob.Error;
                        _logger.Warn("Download of {0} is failing: {1}\n{2}", error.File.RemoteName, error.Description,
                            error.ContextDescription);
                    } else if (runningJob.State == JobState.Transferring) {
                        var progress = runningJob.Progress;
                        _logger.Debug(
                            $"Download is in progress (files {progress.FilesTransferred} of {progress.FilesTotal}, bytes {progress.BytesTransferred} of {progress.BytesTotal})");
                    }
                    return true;
                } else {
                    Settings.Instance.BitsJobId = null;
                    Settings.Instance.Save();
                }
            }
            return false;
        }

        private void Save() {
            try {
                using (var textWriter = new StreamWriter(MoviesPath)) {
                    textWriter.WriteLine(JsonConvert.SerializeObject(_movies, Formatting.Indented));
                }
                _logger.Debug("Movies saved");
            } catch (Exception ex) {
                _logger.Error(ex, "Saving movies failed.");
            }
        }

        public string GetRandomAssetUrl(bool useTimeOfDay) {
            EnsureLoaded();

            if (!_movies.Any()) {
                return null;
            }

            var movies = _movies.ToArray();
            if (useTimeOfDay) {
                var timeOfDay = (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 19) ? TimeOfDay.Day : TimeOfDay.Night;
                movies = movies.Where(asset => asset.TimeOfDay == timeOfDay).ToArray();
            }

            var movie = movies[(new Random()).Next(movies.Length)];
            return !string.IsNullOrEmpty(movie.LocalPath) && File.Exists(movie.LocalPath)
                ? movie.LocalPath
                : movie.DownloadUrl;
        }
    }

    public class Movie {
        public string AssetId { get; set; }
        public string DownloadUrl { get; set; }
        public string LocalPath { get; set; }
        public string Location { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TimeOfDay TimeOfDay { get; set; }
    }
}