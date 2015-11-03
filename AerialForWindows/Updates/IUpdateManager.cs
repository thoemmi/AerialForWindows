using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AerialForWindows.Updates {
    public interface IUpdateManager {
        Task CheckForUpdatesAsync();
        IReadOnlyCollection<ReleaseInfo> Releases { get; }
        event EventHandler UpdatesAvailable;
        Task<string> DownloadReleaseAsync(ReleaseInfo releaseInfo, Action<int> downloadProgressCallback, CancellationToken cancellationToken);
    }
}