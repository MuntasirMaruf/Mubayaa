using Mubayaa.Auth;
using Mubayaa.DTOs;
using Mubayaa.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace Mubayaa.Controllers
{
    public class OrderController : Controller
    {
        private MubayaaDBEntities db = new MubayaaDBEntities();

        [HttpGet]
        public ActionResult Home()
        {
            var products = (from p in db.Products
                            where p.Status != 5
                            select p).ToList();
            return View(ProductController.Convert(products));
        }

        public ActionResult AddToCart(int id)
        {
            List<ProductDTO> cart = new List<ProductDTO>();          // Initialize the cart list of ProductDTO
            if (Session["cart"] == null)
            {
                cart = new List<ProductDTO>();                       // Create a new cart if it doesn't exist in session
            }
            else
            {
                cart = (List<ProductDTO>)Session["cart"];            // Retrieve the existing cart from session if it exists (Unboxing)
            }

            var procuct = db.Products.Find(id);                      // Find the product by id from the database
            var productDTO = ProductController.Convert(procuct);
            productDTO.Quantity = 1;
            cart.Add(productDTO);                                    // Add the product to the cart (Boxing)
            Session["cart"] = cart;                                  // Store the cart in session
            TempData["Msg"] = "Product " + productDTO.Name + " added to cart";
            TempData["Class"] = "success";
            return RedirectToAction("Home");
        }

        public ActionResult Cart()
        {
            if (Session["Cart"] == null)
            {
                TempData["Msg"] = "Cart is empty";
                TempData["Class"] = "danger";
                return RedirectToAction("Home");
            }
            var cart = (List<ProductDTO>)Session["Cart"];
            return View(cart);
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cart = (List<ProductDTO>)Session["Cart"];                               // Retrieve the cart from session (Unboxing)
            var pr = (from p in cart where p.Id == id select p).SingleOrDefault();      // Find the product in the cart by id
            cart.Remove(pr);                                                            // Remove the product from the cart
            Session["Cart"] = cart;                                                     // Update the session with the modified cart (Boxing)
            TempData["Msg"] = "Product " + pr.Name + " removed from cart";
            TempData["Class"] = "danger";
            return RedirectToAction("Cart");
        }

        public ActionResult PlaceOrder(decimal total)
        {
            if (Session["User"] == null)    // Checks the user is logged in or not
            {
                TempData["Msg"] = "Please login to place order";
                TempData["Class"] = "danger";
                return RedirectToAction("Login", "User");
            }
            else
            {
                var user = (Login)Session["User"];   // Retrieve the logged-in user from session if it exists/logged in (Unboxing)

                var order = new Order()
                {
                    Time = DateTime.Now,
                    Status = 1,
                    Total = total,
                    CustomerId = user.Id,
                };
                db.Orders.Add(order);
                db.SaveChanges();

                var cart = (List<ProductDTO>)Session["Cart"];   // Retrieve the cart from session (Unboxing)
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail()
                    {
                        OrderId = order.Id,
                        ProductId = item.Id,
                        Quantity = item.Quantity,
                        Price = item.Price,
                    };
                    db.OrderDetails.Add(orderDetail);
                }
                db.SaveChanges();

                TempData["Msg"] = "Order placed successfully";
                TempData["Class"] = "success";
                Session["Cart"] = null;   // Clear the cart from session
            }
                return RedirectToAction("Home");
        }

        public ActionResult CartDec(int id)
        {
            var cart = (List<ProductDTO>)Session["Cart"];
            var pr = (from p in cart where p.Id == id select p).SingleOrDefault();
            if (pr.Quantity > 1)
            {
                pr.Quantity--;
            }
            return RedirectToAction("Cart");
        }
        public ActionResult CartInc(int id)
        {
            var data = db.Products.Find(id);
            var cart = (List<ProductDTO>)Session["Cart"];
            var pr = (from p in cart where p.Id == id select p).SingleOrDefault();
            if (pr.Quantity < data.Quantity)
            {
                pr.Quantity++;
            }
            return RedirectToAction("Cart");
        }



        [UserAuth]
        public ActionResult MyOrders()
        {
            var user = (Login)Session["User"];
            var orders = (from o in db.Orders
                          where o.CustomerId == user.Id && o.Status != 6
                          select o).ToList();
            return View(orders);
        }

        [UserAuth]
        public ActionResult OrderDetails(int id)
        {
            var orderDetails = (from od in db.OrderDetails
                                where od.OrderId == id
                                select od).ToList();
            var order = db.Orders.Find(id);
            ViewBag.ID = order.Id;
            ViewBag.Total = order.Total;
            ViewBag.Status = order.Status;
            return View(orderDetails);
        }


        public ActionResult CancelOrder(int id)
        {
            var existingOrder = db.Orders.Find(id);
            existingOrder.Status = 6;
            db.SaveChanges();
            TempData["Msg"] = "Order candled";
            TempData["Class"] = "danger";
            return RedirectToAction("MyOrders");
        }
    }
}