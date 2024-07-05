using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using System;

namespace TypicalTools.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataAccessLayer _Parser;

        public ProductController(DataAccessLayer parser)
        {
            _Parser = parser;
        }

        // Show all products
        public IActionResult Index()
        {
            var products = _Parser.GetProducts();
            return View(products);
        }
        [HttpGet]
        public IActionResult AddProduct()
        {            
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
           
            if (ModelState.IsValid)
            {
                product.UpdatedDate = DateTime.Now;
                _Parser.AddProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(string productCode, decimal productPrice)
        {
            // Check if the user is authenticated and has admin access
            if (Request.Cookies["Authenticated"] != "True" || int.Parse(Request.Cookies["AccessLevel"]) != 0)
            {
                return Unauthorized(); // Only allow access if authenticated and access level is 0 (admin)
            }

            var product = _Parser.GetProductByCode(productCode);
            if (product != null)
            {
                product.ProductPrice = productPrice;
                product.UpdatedDate = DateTime.Now;
                _Parser.UpdateProduct(product);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
