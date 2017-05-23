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
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.WebUI.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Pagiante()
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

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //动作
            //IEnumerable<Product> result = (IEnumerable<Product>)controller.List(2).Model;
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //断言
            Product[] proArr = result.Products.ToArray();
            Assert.IsTrue(proArr.Length == 2);
            Assert.AreEqual(proArr[0].Name, "篮球2");
            Assert.AreEqual(proArr[1].Name, "篮球3");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //准备
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //动作
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            //断言
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>" +
                @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>" +
                @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
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

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //动作
            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            //断言
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25,Category="zzz" },
                new Product { ProductID=2, Name = "篮球", Price = 179,Category="zzz" },
                new Product { ProductID=3, Name = "羽毛球", Price = 95,Category="zzz" },
                new Product { ProductID=4, Name = "篮球2", Price = 179,Category="zzz" },
                new Product { ProductID=5, Name = "篮球3", Price = 179,Category="zzz" }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //动作
            Product[] result = ((ProductsListViewModel)controller.List("zzz", 1).Model).Products.ToArray();

            //断言

            Assert.AreEqual(result.Length, 3);
            Assert.IsTrue(result[0].Name == "足球" && result[0].Category == "zzz");
            Assert.IsTrue(result[2].Name == "羽毛球" && result[2].Category == "zzz");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25,Category="zzz" },
                new Product { ProductID=2, Name = "篮球", Price = 179,Category="sss" },
                new Product { ProductID=3, Name = "羽毛球", Price = 95,Category="qqq" },
                new Product { ProductID=4, Name = "篮球2", Price = 179,Category="sss" },
                new Product { ProductID=5, Name = "篮球3", Price = 179,Category="zzz" }
            });

            NavController controller = new NavController(mock.Object);

            //动作
            string[] result = ((IEnumerable<string>)controller.Menu().Model).ToArray();

            //断言

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0], "qqq");
            Assert.AreEqual(result[1], "sss");
            Assert.AreEqual(result[2], "zzz");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25,Category="zzz" },
                new Product { ProductID=2, Name = "篮球", Price = 179,Category="sss" },
                new Product { ProductID=3, Name = "羽毛球", Price = 95,Category="qqq" },
                new Product { ProductID=4, Name = "篮球2", Price = 179,Category="sss" },
                new Product { ProductID=5, Name = "篮球3", Price = 179,Category="zzz" }
            });

            NavController controller = new NavController(mock.Object);

            string selectCategory = "zzz";

            //动作
            string result = (string)controller.Menu(selectCategory).ViewBag.SelectedCategory;

            //断言

            Assert.AreEqual(selectCategory, result);
        }

        [TestMethod]
        public void Generate_Specific_Product_Count()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25,Category="zzz" },
                new Product { ProductID=2, Name = "篮球", Price = 179,Category="sss" },
                new Product { ProductID=3, Name = "羽毛球", Price = 95,Category="qqq" },
                new Product { ProductID=4, Name = "篮球2", Price = 179,Category="sss" },
                new Product { ProductID=5, Name = "篮球3", Price = 179,Category="zzz" }
            });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //动作
            int res1 = ((ProductsListViewModel)controller.List("zzz").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)controller.List("sss").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)controller.List("qqq").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            //断言

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
