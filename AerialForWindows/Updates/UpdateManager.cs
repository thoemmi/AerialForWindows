using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AerialForWindows.Updates {
    public sealed class UpdateManager : IUpdateManager {
        private readonly Version _currentVersion;
        private readonly IReleaseProvider _reader;
        private readonly List<ReleaseInfo> _releases = new List<ReleaseInfo>();
        private static IUpdateManager _instance;

        public static IUpdateManager Instance {
            get {
                if (_instance == null) {
                    var currentVersion = AssemblyName.GetAssemblyName(typeof(UpdateManager).Assembly.Location).Version;
                    var releaseProvider = new GithubReleaseProvider("AerialForWindows", "thoemmi", "AerialForWindows");
                    _instance = new UpdateManager(releaseProvider, currentVersion);

                }
                return _instance;
            }
        }

        private UpdateManager(IReleaseProvider reader, Version currentVersion) {
            _currentVersion = currentVersion;
            _reader = reader;
        }

        public async Task CheckForUpdatesAsync() {
            IReadOnlyCollection<ReleaseInfo> newReleases;
            try {
                newReleases = await _reader.GetReleaseInfosAsync();
            } catch {
                return;
            }

            // remove re-published versions
            foreach (var release in newReleases) {
                _releases.RemoveAll(r => r.Version == release.Version);
            }

            _releases.AddRange(newReleases);
            _releases.Sort((r1, r2) => r2.Version.CompareTo(r1.Version));
            if (_releases.Any(r => r.Version > _currentVersion)) {
                OnUpdatesAvailable();
            }
        }

        public IReadOnlyCollection<ReleaseInfo> Releases => _releases;

        public event EventHandler UpdatesAvailable;

        private void OnUpdatesAvailable() {
            UpdatesAvailable?.Invoke(this, EventArgs.Empty);
        }

        public Task<string> DownloadReleaseAsync(ReleaseInfo releaseInfo, Action<int> downloadProgressCallback,
            CancellationToken cancellationToken) {
            return _reader.DownloadReleasePackage(releaseInfo, downloadProgressCallback, cancellationToken);
        }
    }
}