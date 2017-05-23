using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SportsStore.Domain.Entities;
using static SportsStore.Domain.Entities.Cart;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Controllers;
using System.Web.Mvc;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Tests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //准备
            Product p1 = new Product { ProductID = 1, Name = "zzz" };
            Product p2 = new Product { ProductID = 2, Name = "sss" };

            Cart cart = new Cart();

            //动作
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);

            CartLine[] result = cart.Lines.ToArray();

            //断言
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[1].Product, p2);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //准备
            Product p1 = new Product { ProductID = 1, Name = "zzz" };
            Product p2 = new Product { ProductID = 2, Name = "sss" };
            Product p3 = new Product { ProductID = 3, Name = "qqq" };

            Cart cart = new Cart();

            //动作
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 3);
            cart.AddItem(p3, 5);
            cart.AddItem(p2, 1);
            cart.RemoveLine(p2);

            //断言
            Assert.AreEqual(cart.Lines.Where(p => p.Product.Name == "p2").Count(), 0);
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //准备
            Product p1 = new Product { ProductID = 1, Name = "zzz" };
            Product p2 = new Product { ProductID = 2, Name = "sss" };

            Cart cart = new Cart();

            //动作
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 10);

            CartLine[] result = cart.Lines.OrderBy(p => p.Product.ProductID).ToArray();

            //断言
            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Quantity, 11);
            Assert.AreEqual(result[1].Quantity, 1);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //准备
            Product p1 = new Product { ProductID = 1, Name = "zzz", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "sss", Price = 50M };

            Cart cart = new Cart();

            //动作
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 3);

            decimal result = cart.ComputeTotalValue();

            //断言
            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Clear_Content()
        {
            //准备
            Product p1 = new Product { ProductID = 1, Name = "zzz", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "sss", Price = 50M };

            Cart cart = new Cart();

            //动作
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);

            cart.Clear();

            //断言
            Assert.AreEqual(cart.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25,Category="zzz" }
            }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);

            //动作
            controller.AddToCart(cart, 1, null);

            //断言
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_GoesTo_Cart_Screen()
        {
            //准备
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product { ProductID=1, Name = "足球", Price = 25,Category="zzz" }
            }.AsQueryable());

            Cart cart = new Cart();

            CartController controller = new CartController(mock.Object, null);

            //动作
            RedirectToRouteResult result = controller.AddToCart(cart, 1, "myUrl");

            //断言
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnurl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //准备
            Cart cart = new Cart();

            CartController controller = new CartController(null, null);

            //动作
            CartIndexViewModel result = (CartIndexViewModel)controller.Index(cart, "myUrl").ViewData.Model;

            //断言
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        /// <summary>
        /// 需要再好好理解一下这个用例！！！
        /// </summary>
        public void Cannot_Checkout_Cart_Empty()
        {
            //准备
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();

            ShippingDetail detail = new ShippingDetail();

            CartController ctrl = new CartController(null, mock.Object);

            //动作
            ViewResult result = ctrl.Checkout(cart, detail);

            //断言

            //检查，订单尚未提交给处理器
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetail>()), Times.Never());

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        public void Cannot_Checkout_Invalid_ShippingDetail()
        {
            //准备
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            ShippingDetail detail = new ShippingDetail();

            CartController ctrl = new CartController(null, mock.Object);

            //动作
            ViewResult result = ctrl.Checkout(cart, detail);

            //断言

            //检查，订单尚未提交给处理器
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetail>()), Times.Never());

            //是否返回默认视图
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            //准备
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            CartController controller = new CartController(null, mock.Object);

            //动作
            ViewResult result = controller.Checkout(cart, new ShippingDetail());

            //断言
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetail>()), Times.Once());

            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
