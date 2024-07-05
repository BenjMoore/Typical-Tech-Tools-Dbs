using TypicalTools.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TypicalTools.DataAccess
{
    public class CsvParser
    {
        private IWebHostEnvironment _hostingEnvironment;
        public CsvParser(IWebHostEnvironment environment)
        {
            _hostingEnvironment = environment;
        }

        #region Products

        public List<Product> ParseProducts()
        {
            string wwwRootPath = _hostingEnvironment.WebRootPath;

            string[] productLines = File.ReadAllLines(wwwRootPath + "\\data\\Products.csv");

            List<Product> productList = new List<Product>();

            foreach (var product in productLines)
            {
                string[] productSections = product.Split(',');

                Product parsedProduct = new Product
                {
                    ProductCode = int.Parse(productSections[0]),
                    ProductName = productSections[1],
                    ProductPrice = decimal.Parse(productSections[2]),
                    ProductDescription = productSections[3]
                };

                productList.Add(parsedProduct);
            }
            return productList;
        }

        public Product GetSingleProduct(int productCode)
        {
            var products = ParseProducts();

            return products.Where(c => c.ProductCode == productCode).FirstOrDefault();
        }

        #endregion

        #region Comments

        public List<Comment> ParseComments()
        {
            string wwwRootPath = _hostingEnvironment.WebRootPath;

            string[] commentLines = File.ReadAllLines(wwwRootPath + "\\data\\Comments.csv");

            List<Comment> commentList = new List<Comment>();

            foreach (var comment in commentLines)
            {
                string[] commentSections = comment.Split(',');

                Comment parsedComment = new Comment
                {
                    CommentId = int.Parse(commentSections[0]),
                    CommentText = commentSections[1],
                    ProductCode = int.Parse(commentSections[2]),
                    SessionId = commentSections[3]
                };

                commentList.Add(parsedComment);
            }
            return commentList;
        }

        public List<Comment> GetCommentsForProduct(int productCode)
        {
            if(productCode == 0)
            {
                return null;
            }

            var allComments = ParseComments();

            // Return all comments where the productcode matches the provided product code
            return allComments.Where(c => c.ProductCode == productCode).ToList();

        }

        public void AddComment(Comment comment, string sessionId)
        {
            string wwwRootPath = _hostingEnvironment.WebRootPath;

            var existingComments = ParseComments();

            int newID = 1;

            if (existingComments.Count != 0)
            {
                newID = existingComments.OrderByDescending(c => c.CommentId).FirstOrDefault().CommentId + 1;
            }

            string commentLine = $"{newID},{comment.CommentText},{comment.ProductCode},{sessionId}";

            File.AppendAllLines(wwwRootPath + "\\data\\Comments.csv", new string[] { commentLine });

        }

        public bool EditComment(Comment updatedComment)
        {
            string wwwRootPath = _hostingEnvironment.WebRootPath;

            var existingComments = ParseComments();

            bool exists = false;

            foreach (var comment in existingComments)
            {
                if (comment.CommentId == updatedComment.CommentId)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                Comment oldComment = existingComments.Where(c => c.CommentId == updatedComment.CommentId).FirstOrDefault();

                // find and remove the old comment
                int commentIndex = existingComments.IndexOf(oldComment);

                existingComments.RemoveAt(commentIndex);

                // insert the updated comment in the same list position
                existingComments.Insert(commentIndex, updatedComment);

                string[] comments = existingComments.Select(c => c.ToCSVString()).ToArray();

                File.WriteAllLines(wwwRootPath + "\\data\\Comments.csv", comments);
                return true;
            }

            return false;

        }

        public Comment GetSingleComment(int commentId)
        {
            var comments = ParseComments();

            return comments.Where(c => c.CommentId == commentId).FirstOrDefault();
        }

        public bool DeleteComment(int commentId)
        {
            string wwwRootPath = _hostingEnvironment.WebRootPath;

            var existingComments = ParseComments();

            bool exists = false;

            foreach (var comment in existingComments)
            {
                if (comment.CommentId == commentId)
                {
                    exists = true;
                }
            }

            if (exists)
            {
                Comment oldComment = existingComments.Where(c => c.CommentId == commentId).FirstOrDefault();

                existingComments.Remove(oldComment);

                string[] comments = existingComments.Select(c => c.ToCSVString()).ToArray();

                File.WriteAllLines(wwwRootPath + "\\data\\Comments.csv", comments);
                return true;
            }

            return false;
        }

        #endregion
    }
}
