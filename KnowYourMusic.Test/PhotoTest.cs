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
        public void VK_Load_Photo_Request()
        {
            var expected = @"[{""title"":""Фотографии со страницы Павла"",""size"":""8""},{""title"":""Фотографии на стене Павла"",""size"":""205""},{""title"":""Сохранённые фотографии Павла"",""size"":""42""},{""title"":""Instagram"",""size"":""0""},{""title"":""Здесь будут новые фотографии для прессы-службы"",""size"":""9""}]";
            var result = PhotoAlbums.LoadPhotoAlbums("1"); //1 - uid Павла Дурова

            Assert.AreEqual(expected, JsonConvert.SerializeObject(result));
        }
    }
}
