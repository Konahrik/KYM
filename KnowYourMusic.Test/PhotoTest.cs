using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KnowYourMusic.Features;
using KnowYourMusic.Details;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KnowYourMusic.Test
{
    [TestClass]
    public class PhotoTest
    {
        [TestMethod]
        public void VK_Load_Photo_Albums_Request()
        {
            var expected = @"[{""title"":""Фотографии со страницы Павла"",""size"":""8"",""aid"":""-6""},{""title"":""Фотографии на стене Павла"",""size"":""205"",""aid"":""-7""},{""title"":""Сохранённые фотографии Павла"",""size"":""42"",""aid"":""-15""},{""title"":""Instagram"",""size"":""0"",""aid"":""207791859""},{""title"":""Здесь будут новые фотографии для прессы-службы"",""size"":""9"",""aid"":""136592355""}]";
            var result = PhotoAlbums.LoadPhotoAlbums("1"); //1 - uid Павла Дурова

            Assert.AreEqual(expected, JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        public void VK_Load_Photo_Request()
        {
            var expected = @"{""response"":[]}";
            var result = Photos.LoadPhoto("1", "207791859"); //1 - uid Павла Дурова

            Assert.AreEqual(expected, JsonConvert.SerializeObject(result));
        }
    }
}
