using System.ComponentModel.DataAnnotations;

namespace Shop_store.Models
{
    public class User
    {
      
        public string UserId { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string FullName { get; set; }

        public ShoppingCart ShoppingCart { get; set; }
    }
}
