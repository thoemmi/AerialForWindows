using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AerialForWindows.Updates {
    public class TestReleaseProvider : IReleaseProvider {
        public Task<IReadOnlyCollection<ReleaseInfo>> GetReleaseInfosAsync() {
            var releaseInfos = (IReadOnlyCollection<ReleaseInfo>) new[] {
                new ReleaseInfo {
                    Name = "first release",
                    Version = new Version(0, 1),
                    ReleaseNotes = "first release",
                    HtmlUrl = "https://github.com/thoemmi/AerialForWindows/releases/tag/v0.1",
                },
                new ReleaseInfo {
                    Name = "new release",
                    Version = new Version(0, 2),
                    ReleaseNotes = "added **Setup**\r\n\r\nshow error when movie playback fails",
                    HtmlUrl = "https://github.com/thoemmi/AerialForWindows/releases/tag/v0.2",
                }
            };
            return Task.FromResult(releaseInfos);
        }

        public Task<string> DownloadReleasePackage(ReleaseInfo releaseInfo, Action<int> downloadProgressCallback,
            CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}