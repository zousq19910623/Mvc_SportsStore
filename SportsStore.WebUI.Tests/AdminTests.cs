using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using Moq;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Tests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Product()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25 },
                new Product { ProductID=2, Name = "篮球", Price = 179 },
                new Product { ProductID=3, Name = "羽毛球", Price = 95 },
                new Product { ProductID=4, Name = "篮球2", Price = 179 },
                new Product { ProductID=5, Name = "篮球3", Price = 179 }
            });

            AdminController controller = new AdminController(mock.Object);

            //动作
            Product[] result = ((IEnumerable<Product>)controller.Index().ViewData.Model).ToArray();

            //断言
            Assert.IsTrue(result.Length == 5);
            Assert.AreEqual(result.Length, 5);
            Assert.AreEqual(result[0].Name, "足球");
            Assert.AreEqual(result[1].Name, "篮球");
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25 },
                new Product { ProductID=2, Name = "篮球", Price = 179 },
                new Product { ProductID=3, Name = "羽毛球", Price = 95 },
                new Product { ProductID=4, Name = "篮球2", Price = 179 },
                new Product { ProductID=5, Name = "篮球3", Price = 179 }
            });

            AdminController controller = new AdminController(mock.Object);

            //动作
            Product p1 = controller.Edit(1).ViewData.Model as Product;
            Product p2 = controller.Edit(2).ViewData.Model as Product;
            Product p3 = controller.Edit(3).ViewData.Model as Product;

            //断言
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_No_Product()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25 },
                new Product { ProductID=2, Name = "篮球", Price = 179 },
                new Product { ProductID=3, Name = "羽毛球", Price = 95 },
                new Product { ProductID=4, Name = "篮球2", Price = 179 },
                new Product { ProductID=5, Name = "篮球3", Price = 179 }
            });

            AdminController controller = new AdminController(mock.Object);

            //动作
            Product p6 = controller.Edit(6).ViewData.Model as Product;

            //断言
            Assert.IsNull(p6);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            AdminController controller = new AdminController(mock.Object);

            Product product = new Product { Name = "test" };

            //动作
            ActionResult result = controller.Edit(product);

            //断言
            mock.Verify(m => m.SaveProduct(product));//什么作用?模拟调用SaveProduct方法？

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));//判断的是什么
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            AdminController controller = new AdminController(mock.Object);

            Product product = new Product { Name = "test" };
            controller.ModelState.AddModelError("error", "error");

            //动作
            ActionResult result = controller.Edit(product);

            //断言
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            Assert.IsInstanceOfType(result, typeof(ViewResult));//判断的是什么
        }

        [TestMethod]
        public void Can_Delete_Valid_Product()
        {
            //准备
            Product pro = new Product
            {
                ProductID = 2,
                Name = "test"
            };

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductID=1,Name="test1"},
                pro,
                new Product{ProductID=3,Name="test3"}
            });

            AdminController controller = new AdminController(mock.Object);

            //动作
            controller.Delete(pro.ProductID);

            //断言
            mock.Verify(m => m.DeleteProduct(pro.ProductID));

            //为什么不需要使用assert
        }
    }
}
