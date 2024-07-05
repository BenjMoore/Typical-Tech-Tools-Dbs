using System.ComponentModel.DataAnnotations;

namespace TypicalTools.Models
{
    public class Comment
    {

        public int CommentId { get; set; }
        [Display(Name = "Comment")]
        public string CommentText { get; set; }
        [Display(Name = "Product Code")]
        public int ProductCode { get; set; }
        public string SessionId { get; set; }

        /// <summary>
        /// Return a CSV formatted string of the a comment object
        /// </summary>
        /// <returns></returns>
        public string ToCSVString()
        {
            return $"{CommentId},{CommentText},{ProductCode},{SessionId}";
        }

    }
}
