using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using Moq;
using System.Web.Mvc;

namespace SportsStore.WebUI.Tests
{
    /// <summary>
    /// AdminSecurityTests 的摘要说明
    /// </summary>
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_Valid_Credentials()
        {
            //准备
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "admin")).Returns(true);

            LoginViewModel model = new LoginViewModel
            {
                UserName = "admin",
                Password = "admin"
            };

            AccountController ctrl = new AccountController(mock.Object);

            //动作
            ActionResult result = ctrl.Login(model, "/MyUrl");

            //断言
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_Invalid_Credentials()
        {
            //准备
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "123")).Returns(false);

            LoginViewModel model = new LoginViewModel
            {
                UserName = "admin",
                Password = "123"
            };

            AccountController ctrl = new AccountController(mock.Object);

            //动作
            ActionResult result = ctrl.Login(model, "/MyUrl");

            //断言
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
