using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowYourMusic.Details
{
    public class VkVideo
    {
        public List<VideoResponse> response { get; set; }
    }

    public class VideoResponse
    {
        [JsonProperty("title")]
        public string VideoTitle { get; set; }
        [JsonConverter(typeof(SecondsToStringConverter))]
        [JsonProperty("duration")]
        public string VideoDuration { get; set; }
        [JsonProperty("player")]
        public string VideoPlayer { get; set; }
    }
}
