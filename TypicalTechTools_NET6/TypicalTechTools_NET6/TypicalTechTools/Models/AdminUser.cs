namespace TypicalTechTools.Models
{
    public class AdminUser
    {
        public string UserName { get; set; } // Assuming you have a username field
        public string Password { get; set; }
        public int UserID { get; set; }
        public int AccessLevel { get; set; }
    }
}
