﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Comment text is required")]
        [StringLength(250, ErrorMessage = "Comment text cannot exceed 250 characters")]
        public string CommentText { get; set; }
       
        [StringLength(50, ErrorMessage = "Product Code cannot exceed 50 characters")]
        [Required]
        public string ProductCode { get; set; }

        [StringLength(36, ErrorMessage = "UserID text cannot exceed 36 characters")]

        public string UserID { get; set; }
       

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}
