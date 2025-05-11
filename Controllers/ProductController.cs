using Mubayaa.DTOs;
using Mubayaa.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mubayaa.Controllers
{
    public class ProductController : Controller
    {
        private MubayaaDBEntities db = new MubayaaDBEntities();
        // GET: Product

        [HttpGet]
        public ActionResult Index()
        {
            var products = (from p in db.Products
                           where p.Status != 5
                           select p).ToList();
            return View(Convert(products));
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var newProduct = Convert(product);
                db.Products.Add(newProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            var product = db.Products.Find(id);
            return View(Convert(product));
        }


        [HttpPost]
        public ActionResult EditProduct(ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                var existingObj = db.Products.Find(product.Id);
                existingObj.Name = product.Name;
                existingObj.Category = product.Category;
                existingObj.Description = product.Description;
                existingObj.Price = product.Price;
                existingObj.Quantity = product.Quantity;
                existingObj.Status = product.Status;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public ActionResult RemoveProduct(int id)
        {
            var existingObj = db.Products.Find(id);
            existingObj.Status = 5;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public static Product Convert(ProductDTO product)
        {
            return new Product()
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Status = 1
            };
        }

        public static ProductDTO Convert(Product product)
        {
            return new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Status = product.Status
            };
        }

        public static List<ProductDTO> Convert(List<Product> products)
        {
            List<ProductDTO> productDTOs = new List<ProductDTO>();
            foreach (var product in products)
            {
                productDTOs.Add(Convert(product));
            }
            return productDTOs;
        }
    }
}