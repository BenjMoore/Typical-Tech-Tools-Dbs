using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TypicalTechTools.Controllers
{
    public class AdminController : Controller
    {
        private readonly DataAccessLayer DBAccess;

        public AdminController(DataAccessLayer sQLConnector)
        {
            DBAccess = sQLConnector;
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult AdminLogin(AdminUser user)
        {
            bool userAuthorised = DBAccess.ValidateAdminUser(user.UserName.Trim(), user.Password.Trim());
            if (userAuthorised)
            {
                var adminUser = DBAccess.GetAdminUser(user.UserName);

                // Remove or update existing cookies before setting new admin cookies
                if (Request.Cookies["UserID"] != null)
                {
                    Response.Cookies.Delete("Authenticated");
                    Response.Cookies.Delete("UserID");
                    Response.Cookies.Delete("AccessLevel");
                }

                // Set new admin user cookies
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30)
                };
                Response.Cookies.Append("Authenticated", "True", options);
                Response.Cookies.Append("UserID", adminUser.UserID.ToString(), options);
                Response.Cookies.Append("AccessLevel", adminUser.AccessLevel.ToString(), options);

                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(user);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Authenticated");
            Response.Cookies.Delete("UserID");
            Response.Cookies.Delete("AccessLevel");

            return RedirectToAction("AdminLogin");
        }
       /* [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SignUp(AdminUser user)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists
                bool userExists = _dataAccessLayer.CheckUserExists(user.UserName);
                if (userExists)
                {
                    ModelState.AddModelError("", "Username already exists");
                    return View(user);
                }

                // Create the new admin user
                _dataAccessLayer.AddUser(user);

                // After successful registration, redirect to the login page
                return RedirectToAction("AdminLogin");
            }

            return View(user);
        }*/

        [HttpGet]
        public IActionResult AdminDashboard()
        {
            string authStatus = Request.Cookies["Authenticated"];
            int? accessLevel = int.TryParse(Request.Cookies["AccessLevel"], out int level) ? level : (int?)null;

            if (authStatus == "True" && accessLevel == 0)
            {
                return View();
            }

            return RedirectToAction("AdminLogin");
        }
    }
}
