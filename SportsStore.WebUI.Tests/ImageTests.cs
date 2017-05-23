using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using Moq;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.WebUI.Tests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            //准备
            Product pro = new Product
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] { },
                ImageMimeType = "image/jpeg"
            };

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductID=1,Name="test1"},
                pro,
                new Product{ProductID=3,Name="test3"}
            }.AsQueryable());

            ProductController ctrl = new ProductController(mock.Object);

            //动作
            ActionResult result = ctrl.GetImage(2);

            //断言
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(pro.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data_For_Invalid_Id()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductID=1,Name="test1"},
                new Product{ProductID=3,Name="test3"}
            }.AsQueryable());

            ProductController ctrl = new ProductController(mock.Object);

            //动作
            ActionResult result = ctrl.GetImage(2);

            //断言
            Assert.IsNull(result);
        }
    }
}
