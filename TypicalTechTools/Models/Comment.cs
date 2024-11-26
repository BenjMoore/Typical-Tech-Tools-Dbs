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

        public string UserID { get; set; }
        public string SessionID { get; set; } // Store the session ID
        public DateTime CreatedDate { get; set; }
    }
}
