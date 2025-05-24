using Mubayaa.DTOs;
using Mubayaa.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mubayaa.Controllers
{
    public class UserController : Controller
    {
        private MubayaaDBEntities db = new MubayaaDBEntities();


        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(CustomerDTO customer)
        {
            if (ModelState.IsValid)
            {
                if(customer.IsEmployee)
                {
                    return RedirectToAction("EmployeeRegistration", customer);
                }
                else
                {
                    return RedirectToAction("CustomerRegistration", customer);
                }
            }
            return View(customer);
        }

        public ActionResult CustomerRegistration(CustomerDTO customer)
        {
            var data = Convert(customer, 1);   // Convert DTO to EF
            db.Customers.Add(data);
            db.SaveChanges();

            var Login = new Login() // Create Login
            {
                Email = customer.Email,
                Password = customer.Password,
                Status = 1,
                UserId = data.Id,
                LastLogin = DateTime.Now,
            };
            db.Logins.Add(Login);
            db.SaveChanges();
            TempData["Msg"] = "Registration Successful. Please login to continue.";
            TempData["Class"] = "success";
            return RedirectToAction("Login", "User");
        }

        public ActionResult EmployeeRegistration(CustomerDTO employee)
        {
            var data = Convert(employee);   // Convert DTO to EF
            db.Employees.Add(data);
            db.SaveChanges();

            var Login = new Login() // Create Login
            {
                Email = employee.Email,
                Password = employee.Password,
                Status = 2,
                UserId = data.Id,
                LastLogin = DateTime.Now,
            };
            db.Logins.Add(Login);
            db.SaveChanges();
            TempData["Msg"] = "Registration Successful. Please login to continue.";
            TempData["Class"] = "success";
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(String email, String password)
        {
            var user = (from u in db.Logins
                       where u.Email == email && u.Password == password
                       select u).SingleOrDefault();
            if (email != "" && password != "" && user != null)
            {
                Session["User"] = user;
                if (user.Status == 1)
                {
                    return RedirectToAction("Home", "Order");
                }
                else if (user.Status == 2)
                {
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    TempData["Msg"] = "Invalid User";
                    TempData["Class"] = "danger";
                }
            }
            else
            {
                TempData["Msg"] = "Please enter valid credenitals";
                TempData["Class"] = "danger";
            }
            return View();
        }



        public static Customer Convert(CustomerDTO cd, int status)
        {
            return new Customer()
            {
                Id = cd.Id,
                Name = cd.FirstName + " " + cd.LastName,
                Email = cd.Email,
                Address = cd.Address,
                Password = cd.Password,
                Status = 1,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };
        }

        public static Employee Convert(CustomerDTO ed)
        {
            return new Employee()
            {
                Id = ed.Id,
                Name = ed.FirstName + " " + ed.LastName,
                Email = ed.Email,
                Address = ed.Address,
                Password = ed.Password,
                Status = 1,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now
            };
        }
    }
}