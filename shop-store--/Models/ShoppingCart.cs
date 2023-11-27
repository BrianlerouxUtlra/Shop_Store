using System.ComponentModel.DataAnnotations;

namespace Shop_store.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }
  
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
