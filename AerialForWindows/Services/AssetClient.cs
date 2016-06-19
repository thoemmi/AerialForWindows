﻿using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AerialForWindows.Services {
    public class AssetClient {
        private Asset[] _assets;
        private const string MovieViewUrl = "http://a1.phobos.apple.com/us/r1000/000/Features/atv/AutumnResources/videos/entries.json";

        public Asset[] GetAssets() {
            if (_assets == null) {
                try {
                    var webClient = new WebClient();
                    var content = webClient.DownloadString(MovieViewUrl);
                    var assetGroups = JsonConvert.DeserializeObject<AssetGroup[]>(content);
                    _assets = assetGroups.SelectMany(group => @group.Assets).ToArray();
                } catch {
                    _assets = new Asset[0];
                }
            }
            return _assets;
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