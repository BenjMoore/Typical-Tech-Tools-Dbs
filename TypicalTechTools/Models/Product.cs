using System;
using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models
{
    public class Product
    {
        [Required(ErrorMessage = "Product code is required.")]
        [StringLength(50, ErrorMessage = "Product code can't be longer than 50 characters.")]
        public string ProductCode { get; set; }

        
        [StringLength(100, ErrorMessage = "Product name can't be longer than 100 characters.")]
        public string ProductName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be a positive value.")]
        public decimal ProductPrice { get; set; }

        [StringLength(500, ErrorMessage = "Product description can't be longer than 500 characters.")]
        [Required]
        public string ProductDescription { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
    }
}
