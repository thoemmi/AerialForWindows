using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace AerialForWindows.Updates {
    public class GithubReleaseProvider : IReleaseProvider {
        private readonly string _clientName;
        private readonly string _repositoryOwner;
        private readonly string _repositoryName;

        public GithubReleaseProvider(string clientName, string repositoryOwner, string repositoryName) {
            _clientName = clientName;
            _repositoryOwner = repositoryOwner;
            _repositoryName = repositoryName;
        }

        public async Task<IReadOnlyCollection<ReleaseInfo>> GetReleaseInfosAsync() {
            var github = new GitHubClient(new ProductHeaderValue(_clientName));
            var result = new List<ReleaseInfo>();
            foreach (var release in await github.Release.GetAll(_repositoryOwner, _repositoryName)) {
                var r = new ReleaseInfo {
                    Name = String.IsNullOrWhiteSpace(release.Name) ? release.TagName : release.Name,
                    ReleaseNotes = release.Body,
                    PublishedAt = release.PublishedAt,
                    CreatedAt = release.CreatedAt,
                    HtmlUrl = release.HtmlUrl,
                    TagName = release.TagName,
                    IsPrerelease = release.Prerelease,
                };

                var assets = await github.Release.GetAllAssets(_repositoryOwner, _repositoryName, release.Id);
                var asset = assets.FirstOrDefault(a => a.Name.EndsWith(".msi"));
                if (asset != null) {
                    r.Filename = asset.Name;
                    r.DownloadUrl = asset.Url;
                }

                var match = Regex.Match(r.TagName, @"^v(?<major>\d+)\.(?<minor>\d+)(\.(?<patch>\d+))?$");
                if (match.Success) {
                    int major, minor, patch;
                    Int32.TryParse(match.Groups["major"].Value, out major);
                    Int32.TryParse(match.Groups["minor"].Value, out minor);
                    if (Int32.TryParse(match.Groups["patch"].Value, out patch)) {
                        r.Version = new Version(major, minor, patch);
                    } else {
                        r.Version = new Version(major, minor);
                    }
                }
                result.Add(r);
            }
            return result;
        }

        public async Task<string> DownloadReleasePackage(ReleaseInfo releaseInfo, Action<int> downloadProgressCallback,
            CancellationToken cancellationToken) {
            var destination = Path.Combine(Path.GetTempPath(), releaseInfo.Filename);
            if (File.Exists(destination)) {
                File.Delete(destination);
                Thread.Sleep(100);
            }
            using (var webClient = new WebClient()) {
                webClient.Headers.Add("Accept", "application/octet-stream");
                if (downloadProgressCallback != null) {
                    webClient.DownloadProgressChanged += (sender, args) => downloadProgressCallback(args.ProgressPercentage);
                }
                cancellationToken.Register(webClient.CancelAsync);
                await webClient.DownloadFileTaskAsync(releaseInfo.DownloadUrl, destination);
            }
            return destination;
        }
    }
}