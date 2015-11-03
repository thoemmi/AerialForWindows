using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AerialForWindows.Updates {
    public interface IReleaseProvider {
        Task<IReadOnlyCollection<ReleaseInfo>> GetReleaseInfosAsync();

        Task<string> DownloadReleasePackage(ReleaseInfo releaseInfo, Action<int> downloadProgressCallback, CancellationToken cancellationToken);
    }
}