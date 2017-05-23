using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private IProductRepository repository;

        public AdminController(IProductRepository repo)
        {
            this.repository = repo;
        }

        // GET: Admin
        public ViewResult Index()
        {
            return View(repository.Products);
        }

        public ViewResult Create()
        {
            //此处会跳转到Edit视图，但是新增商品，提交的时候如果使用默认的Html.BeginForm()，则会被提交到Create动作方法
            //应当将Html.BeginForm()——>改为Html.BeginForm("Edit","Admin")
            return View("Edit", new Product());
        }

        public ViewResult Edit(int productId)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product,HttpPostedFileBase image=null)
        {
            if (ModelState.IsValid)
            {
                if (image!=null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }
                repository.SaveProduct(product);
                TempData["msg"] = string.Format("{0} 已保存", product.Name);//TempData在http请求结束后会被删除
                return RedirectToAction("Index");
            }
            else
            {
                //数据有误，再次渲染该视图，用户可以修正
                return View(product);
            }
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product delPro = repository.DeleteProduct(productId);
            if (delPro!=null)
            {
                TempData["msg"] = string.Format("{0} 已被删除", delPro.Name);
            }
            return RedirectToAction("Index");
        }
    }
}