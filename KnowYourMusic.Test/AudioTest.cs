using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnowYourMusic.Features;

namespace KnowYourMusic.Test
{
    [TestClass]
    public class AudioTest
    {
        [TestMethod]
        public void VK_Search_Audio_Request_Failed_Auth()
        {
            var result = Audio.LoadAudio("durov");

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void VK_Load_Audio_Request_Exception_Failed_Auth()
        {
            var result = Audio.SearchAudio("OST");

            Assert.AreEqual(null, result);
        }
    }
}
