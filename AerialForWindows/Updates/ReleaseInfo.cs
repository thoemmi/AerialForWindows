using System;

namespace AerialForWindows.Updates {
    public class ReleaseInfo {
        public string Name { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTimeOffset? PublishedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Filename { get; set; }
        public string DownloadUrl { get; set; }
        public string HtmlUrl { get; set; }
        public string TagName { get; set; }
        public bool IsPrerelease { get; set; }
        public Version Version { get; set; }
    }
}