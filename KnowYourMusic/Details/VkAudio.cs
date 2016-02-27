using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowYourMusic.Details
{
    
    public class VkAudio
    {
        public List<AudioResponse> response { get; set; }
    }
    public class AudioResponse
    {
        [JsonProperty("artist")]
        public string artist { get; set; }
        [JsonProperty("title")]
        public string title { get; set; }
        [JsonConverter(typeof(SecondsToStringConverter))]
        public string duration { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
    }
    class SecondsToStringConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return String.Format("{0}:{1}", (long)reader.Value / 60, (long)reader.Value % 60);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof (int);
    }
}
}

