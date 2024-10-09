using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using System;

namespace TypicalTechTools.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataAccessLayer _DBAccess;

        public ProductController(DataAccessLayer parser)
        {
            _DBAccess = parser;
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
            var products = _DBAccess.GetProducts();
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
                _DBAccess.AddProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Update Price
        [ValidateAntiForgeryToken]
      
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            // Check if the user is authenticated and has admin access
            if (Request.Cookies["Authenticated"] != "True" || int.Parse(Request.Cookies["AccessLevel"]) != 0)
            {
                return Unauthorized();
            }

            // Retrieve the full product data based on the provided ProductCode
            Product fullProduct = _DBAccess.GetProductByCode(product.ProductCode);
          
            if (fullProduct == null)
            {
                ModelState.AddModelError("", "The product could not be found.");
            }
            else
            {
                product.ProductName = fullProduct.ProductName ?? string.Empty;
                product.ProductDescription = fullProduct.ProductDescription ?? string.Empty;
                product.ProductCode = fullProduct.ProductCode ?? string.Empty;
                ModelState.Remove("ProductName");
                ModelState.Remove("ProductDescription");
                ModelState.Remove("ProductCode");
            }
           /* foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }*/

            if (ModelState.IsValid)
            {
                // Proceed with the update if validation succeeds
                fullProduct.ProductPrice = product.ProductPrice;
                fullProduct.UpdatedDate = DateTime.Now;
                _DBAccess.UpdateProduct(fullProduct);

                return RedirectToAction("Index");
            }

            return View(product);  // Redirect back to the edit view with validation messages
        }
        /*
        public IActionResult Remove() 
        {      
           return View("RemoveProduct");
        }

        [HttpPost]
        public IActionResult RemoveProduct(Product product)
        {
            int productCode = Convert.ToInt32(product.ProductCode);
            _parser.RemoveProduct(productCode);
            return RedirectToAction("Index");
        }
        */
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
