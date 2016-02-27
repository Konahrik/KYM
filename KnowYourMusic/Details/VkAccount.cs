using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowYourMusic.Details
{
    class VkAccount
    {
        [JsonProperty("access_token")]
        public static string AccessToken { get; set; }
        [JsonProperty("user_id")]
        public static string UserId { get; set; }
    }
}
