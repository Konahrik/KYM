using KnowYourMusic.Details;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KnowYourMusic.Features
{
    public class Audio
    {
        public static List<AudioResponse> LoadAudio(string userId)
        {
            try
            {
                var str = string.Format("https://api.vk.com/method/audio.get?uid={0}&access_token={1}", userId, VkAccount.AccessToken);
                var responseText = General.VkRequest(str);
                var result = JsonConvert.DeserializeObject<VkAudio>(responseText);
                return result.response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<AudioResponse> SearchAudio(string q)
        {
            try
            {
                var str = string.Format("https://api.vk.com/method/audio.search?q={0}&access_token={1}&count=1000&auto_complete=1", q, VkAccount.AccessToken);

                var responseText = Regex.Replace(General.VkRequest(str), "[\t|\r\n]", "");
                if (responseText.IndexOf("response\":[") != -1)
                {
                    int start = responseText.IndexOf('[') + 1;
                    int end = responseText.IndexOf(',', start);
                    responseText = responseText.Substring(0, start) + responseText.Substring(end + 1);
                }
                var result = JsonConvert.DeserializeObject<VkAudio>(responseText);
                return result.response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task DownloadAudio(AudioResponse composition, String path, ProgressBar ProgressBar)
        {
            try
            {
                if (path[path.Length - 1] != '\\')
                {
                    path = path + "\\";
                }
                var fileName = composition.Artist + " – " + composition.AudioTitle;
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
                    await client.DownloadFileTaskAsync(new Uri(composition.AudioUrl), path + fileName + ".mp3");
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
