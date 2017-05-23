using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        private IProductRepository repository;

        public NavController(IProductRepository repo)
        {
            this.repository = repo;
        }

        //public PartialViewResult Menu(string category = null, bool menuHorizontal = false)
        //{
        //    //建议使用viewmodel的形式
        //    ViewBag.SelectedCategory = category;

        //    IEnumerable<string> categories = repository.Products
        //        .Select(x => x.Category)
        //        .Distinct()
        //        .OrderBy(x => x);

        //    string viewName = menuHorizontal ? "MenuHorizontal" : "Menu";
        //    return PartialView(viewName, categories);
        //}

        public PartialViewResult Menu(string category = null)
        {
            //建议使用viewmodel的形式
            ViewBag.SelectedCategory = category;

            IEnumerable<string> categories = repository.Products
                .Select(x => x.Category)
                .Distinct()
                .OrderBy(x => x);
            
            return PartialView("FlexMenu", categories);
        }
    }
}