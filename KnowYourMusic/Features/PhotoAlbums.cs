using KnowYourMusic.Details;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KnowYourMusic.Features
{
    public class PhotoAlbums
    {
        public static List<PhotoAlbumsResponse> LoadPhotoAlbums(string userId)
        {
           var str = string.Format("https://api.vk.com/method/photos.getAlbums?uid={0}&need_covers=1&need_system=1", userId);
                var responseText = General.VkRequest(str);
                var result = JsonConvert.DeserializeObject<VkPhotoAlbums>(responseText);
                return result.response;
        }
    }
}
