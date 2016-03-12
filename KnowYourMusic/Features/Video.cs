using KnowYourMusic.Details;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KnowYourMusic.Features
{
    public class Video
    {
        public static List<VideoResponse> LoadVideo(string userId)
        {
            try
            {
                var str = string.Format("https://api.vk.com/method/video.get?uid={0}&access_token={1}", userId, VkAccount.AccessToken);
                var responseText = Regex.Replace(General.VkRequest(str), "[\t|\r\n]", "");
                if (responseText.IndexOf("response\":[") != -1)
                {
                    int start = responseText.IndexOf('[') + 1;
                    int end = responseText.IndexOf(',', start);
                    responseText = responseText.Substring(0, start) + responseText.Substring(end + 1);
                }
                var result = JsonConvert.DeserializeObject<VkVideo>(responseText);
                return result.response;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static List<VideoResponse> SearchVideo(string q)
        {
            var str = string.Format("https://api.vk.com/method/video.search?q={0}&access_token={1}&count=1000", q, VkAccount.AccessToken);
            var responseText = General.VkRequest(str);
            var result = JsonConvert.DeserializeObject<VkVideo>(responseText);
            return result.response;
        }
        //public static async Task DownloadVideo(VideoResponse video, String path, ProgressBar ProgressBar)
        //{
        //    try
        //    {
        //        if (path[path.Length - 1] != '\\')
        //        {
        //            path = path + "\\";
        //        }
        //        var fileName = video.VideoTitle;
        //        if (fileName.Length > 40)
        //        {
        //            fileName = fileName.Substring(0, 40);
        //        }
        //        fileName = fileName.Replace(":", "").Replace("\\", "").Replace("/", "").Replace("*", "").Replace("?", "").Replace("\"", "");
        //        using (var client = new WebClient())
        //        {

        //            client.DownloadProgressChanged += (o, args) =>
        //            {
        //                ProgressBar.Value = args.ProgressPercentage;
        //            };
        //            client.DownloadFileCompleted += (o, args) =>
        //            {
        //                ProgressBar.Value = 0;
        //            };
        //            await client.DownloadFileTaskAsync(new Uri(video.VideoPlayer), path + fileName + ".mp4");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
    }
}
