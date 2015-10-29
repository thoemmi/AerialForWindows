using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AerialForWindows {
    public class MovieManager {
        private Asset[] _assets;
        private const string MovieViewUrl = "http://a1.phobos.apple.com/us/r1000/000/Features/atv/AutumnResources/videos/entries.json";

        private void EnsureAssets() {
            if (_assets != null && _assets.Length != 0) {
                return;
            }

            try {
                var webClient = new WebClient();
                var content = webClient.DownloadString(MovieViewUrl);
                var assetGroups = JsonConvert.DeserializeObject<AssetGroup[]>(content);
                _assets = assetGroups.SelectMany(group => @group.Assets).ToArray();
            } catch {
                _assets = new Asset[0];
            }
        }

        public string GetRandomAssetUrl(bool useTimeOfDay) {
            EnsureAssets();

            var assets = _assets;
            if (assets == null || assets.Length == 0) {
                return null;
            }

            if (useTimeOfDay) {
                var timeOfDay = (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 19) ? TimeOfDay.Day : TimeOfDay.Night;
                assets = assets.Where(asset => asset.TimeOfDay == timeOfDay).ToArray();
            }

            return assets[(new Random()).Next(assets.Length)].Url;
        }
    }

    public class AssetGroup {
        public string Id { get; set; }
        public Asset[] Assets { get; set; }
    }

    public class Asset {
        public string Id { get; set; }
        public string Url { get; set; }
        public string AccessibilityLabel { get; set; }
        public string Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TimeOfDay TimeOfDay { get; set; }
    }

    public enum TimeOfDay {
        Night,
        Day
    }
}