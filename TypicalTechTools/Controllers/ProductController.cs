using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using System;

namespace TypicalTechTools.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataAccessLayer _parser;

        public ProductController(DataAccessLayer parser)
        {
            _parser = parser;
        }

        private bool SetUserCookies()
        {
            var cookie = Request.Cookies["UserID"];
            if (cookie == null)
            {
                var options = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(120),
                    Secure = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                };
                var userId = Guid.NewGuid().ToString();
                int accessLevel = 1;

                Response.Cookies.Append("UserID", userId, options);
                Response.Cookies.Append("Authenticated", "False", options);
                Response.Cookies.Append("AccessLevel", accessLevel.ToString(), options);

                return true;
            }
            return false;
        }

        // Show all products
        public IActionResult Index()
        {
            SetUserCookies();
            var products = _parser.GetProducts();
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                product.UpdatedDate = DateTime.Now;
                _parser.AddProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            // Check if the user is authenticated and has admin access
            if (Request.Cookies["Authenticated"] != "True" || int.Parse(Request.Cookies["AccessLevel"]) != 0)
            {
                return Unauthorized(); 
            }

            if (ModelState.IsValid)
            {
                var existingProduct = _parser.GetProductByCode(product.ProductCode);
                if (existingProduct != null)
                {
                    existingProduct.ProductPrice = product.ProductPrice;
                    existingProduct.UpdatedDate = DateTime.Now;
                    _parser.UpdateProduct(existingProduct);
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
