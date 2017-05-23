using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Product> Products { get { return context.Products; } }

        public Product DeleteProduct(int productId)
        {
            Product onePro = context.Products.Find(productId);
            if (onePro != null)
            {
                context.Products.Remove(onePro);
                context.SaveChanges();
            }
            return onePro;
        }

        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)//新增
            {
                context.Products.Add(product);
            }
            else//修改
            {
                Product onePro = context.Products.Find(product.ProductID);
                if (onePro != null)
                {
                    onePro.Name = product.Name;
                    onePro.Description = product.Description;
                    onePro.Price = product.Price;
                    onePro.Category = product.Category;
                    onePro.ImageData = product.ImageData;
                    onePro.ImageMimeType = product.ImageMimeType;
                }
            }
            context.SaveChanges();
        }
    }
}
