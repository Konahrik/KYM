using KnowYourMusic.Details;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        public static async Task DownloadPhoto(PhotoItem photo, String path, ProgressBar ProgressBar)
        {
            try
            {
                if (path[path.Length - 1] != '\\')
                {
                    path = path + "\\";
                }
                var fileName = photo.UrlPhoto;
                if (fileName.Length > 40)
                {
                    fileName = fileName.Substring(0, 40);
                }
                fileName = fileName.Replace(":", "").Replace("\\", "").Replace("/", "").Replace("*", "").Replace("?", "").Replace("\"", "");
                using (var client = new WebClient())
                {

                    client.DownloadProgressChanged += (o, args) =>
                    {
                        ProgressBar.Value = args.ProgressPercentage;
                    };
                    client.DownloadFileCompleted += (o, args) =>
                    {
                        ProgressBar.Value = 0;
                    };
                    await client.DownloadFileTaskAsync(new Uri(photo.UrlPhoto), path + fileName + ".jpg");
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
