using KnowYourMusic.Details;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowYourMusic.Features
{
    class Photos
    {
        public static VKPhoto LoadPhoto(string userId, string albumId)
        {
            var str = string.Format("https://api.vk.com/method/photos.get?owner_id={0}&album_id={1}", userId, albumId);
            var responseText = General.VkRequest(str);
            var result = JsonConvert.DeserializeObject<VKPhoto>(responseText);
            return result;
        }
    }
}
