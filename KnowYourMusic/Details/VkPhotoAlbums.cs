using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowYourMusic.Details
{
    public class VkPhotoAlbums
    {
        public List<PhotoAlbumsResponse> response { get; set; }
    }
    public class PhotoAlbumsResponse
    {
        [JsonProperty("title")]
        public string PhotoAlbumTitle { get; set; }
        [JsonProperty("size")]
        public string PhotoAlbumSize { get; set; }
    }
}
