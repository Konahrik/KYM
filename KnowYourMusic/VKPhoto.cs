using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowYourMusic.Details
{
    public class VKPhoto
    {
        public List<PhotoResponse> response { get; set; }
    }
    public class PhotoResponse
    {

        [JsonProperty("aid")]
        public int Aid { get; set; }

        [JsonProperty("created")]
        public int Created { get; set; }

        [JsonProperty("owner_id")]
        public int Owner_id { get; set; }

        [JsonProperty("user_id")]
        public int User_id { get; set; }

        [JsonProperty("pid")]
        public int Pid { get; set; }

        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("src_big")]
        public string Src_big { get; set; }

        [JsonProperty("src_small")]
        public string Src_small { get; set; }

        [JsonProperty("post_id")]
        public string Post_id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}

public class PhotoItem
{
    public string UrlPhoto { get; set; }
}

