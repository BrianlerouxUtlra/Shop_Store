using Shop_store.Models;

namespace Shop_store.Services
{
    public interface IShoppingCartService
    {
        Task<ServiceResult> AddToCart(string userId, ShoppingCartItemDto itemDto);
        Task<IEnumerable<ShoppingCartItemDto>> GetCartItems(string userId, int page, int pageSize);
        Task<ServiceResult> UpdateCartItem(string userId, int itemId, ShoppingCartItemDto updatedItemDto);
        Task<ServiceResult> RemoveFromCart(string userId, int itemId);

        void AssociateUserWithShoppingCart(string userId);
    }
}
