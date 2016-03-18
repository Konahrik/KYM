using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnowYourMusic.Features;

namespace KnowYourMusic.Test
{
    [TestClass]
    public class VideoTest
    {
        [TestMethod]
        public void VK_Search_Video_Request_Failed_Auth()
        {
            var result = Video.SearchVideo("программирование");

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void VK_Load_Video_Request_Exception_Failed_Auth()
        {
            var result = Video.LoadVideo("durov");

            Assert.AreEqual(null, result);
        }
    }
}
