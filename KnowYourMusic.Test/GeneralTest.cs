using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnowYourMusic.Features;
using Newtonsoft.Json;
using KnowYourMusic.Details;

namespace KnowYourMusic.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void VK_Request()
        {
            string result = General.VkRequest("https://api.vk.com/method/users.get?uids=durov");

            Assert.AreEqual("{\"response\":[{\"uid\":1,\"first_name\":\"Павел\",\"last_name\":\"Дуров\"}]}", result);
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void VK_Request_Exception_Is_Thrown()
        {
            string result = General.VkRequest("Wrong String!");
        }

        [TestMethod]
        public void Correct_Users_Info()
        {
            var result = General.GetUsersInfo("durov");

            var expected = JsonConvert.DeserializeObject<VkUsers>(@"{""response"":[{""uid"":1,""first_name"":""Павел"",""last_name"":""Дуров""}]}");
            Assert.AreEqual(expected.response[0].uid, result.response[0].uid);
        }
    }
}
