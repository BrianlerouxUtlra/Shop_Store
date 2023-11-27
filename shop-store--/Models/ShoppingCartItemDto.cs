using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shop_store.Models
{
    public class ShoppingCartItemDto

    {
        [JsonIgnore]
        public int Id { get; set; }

        public string UserId { get; set; }

        [Required(ErrorMessage = "ProductName is required")]
        public string ProductName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "ImageUrl is required")]
        public string ImageUrl { get; set; }

        public DateTime AddedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
