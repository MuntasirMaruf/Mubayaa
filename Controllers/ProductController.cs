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
            if (Session["User"] == null)    // Checks the user is logged in or not
            {
                TempData["Msg"] = "Please login to add products";
                TempData["Class"] = "danger";
                return RedirectToAction("Login", "User");
            }
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
                TempData["Msg"] = "Product added successfully";
                TempData["Class"] = "success";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            if (Session["User"] == null)    // Checks the user is logged in or not
            {
                TempData["Msg"] = "Please login to edit product";
                TempData["Class"] = "danger";
                return RedirectToAction("Login", "User");
            }
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
            if (Session["User"] == null)    // Checks the user is logged in or not
            {
                TempData["Msg"] = "Please login to remove product";
                TempData["Class"] = "danger";
                return RedirectToAction("Login", "User");
            }
            var existingObj = db.Products.Find(id);
            existingObj.Status = 5;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Orders()
        {
            if (Session["User"] == null)    // Checks the user is logged in or not
            {
                TempData["Msg"] = "Please login to place order";
                TempData["Class"] = "danger";
                return RedirectToAction("Login", "User");
            }
            var orders = from o in db.Orders
                         where o.Status != 5
                         select o;
            return View(orders);
        }

        public ActionResult Accept(int id)
        {
           var order = db.Orders.Find(id);
            order.Status = 2; // 2 = Processing
            db.SaveChanges();
            TempData["Msg"] = "Order Accepted";
            TempData["Class"] = "success";
            return RedirectToAction("Orders");
        }

        public ActionResult Reject(int id)
        {
            var order = db.Orders.Find(id);
            order.Status = 4; // 4 = Cancled

            var existingOrderDetails = (from od in db.OrderDetails
                                        where od.OrderId == id
                                        select od).ToList();

            foreach (var od in existingOrderDetails)
            {
                var existingStock = (from p in db.Products
                                     where p.Id == od.ProductId
                                     select p).SingleOrDefault();
                existingStock.Quantity -= od.Quantity;
                db.SaveChanges();
            }

            db.SaveChanges();
            TempData["Msg"] = "Order Rejected";
            TempData["Class"] = "danger";
            return RedirectToAction("Orders");
        }

        public ActionResult Details(int id)
        {
            var orderDetails = (from od in db.OrderDetails
                        where od.OrderId == id
                        select od).ToList();

            return View(orderDetails);
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