using KnowYourMusic.Details;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KnowYourMusic.Features
{
    public class General
    {
        public static VkUsers GetUsersInfo(string userId)
        {
            try
            {
                var str = string.Format("https://api.vk.com/method/users.get?uids={0}", userId);
                var responseText = General.VkRequest(str);
                var users = JsonConvert.DeserializeObject<VkUsers>(responseText);
                return users;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string VkRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var responseText = reader.ReadToEnd();
            return responseText;
        }
    }
}
