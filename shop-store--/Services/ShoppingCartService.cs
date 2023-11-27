using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Added for ILogger
using Shop_store.Data;
using Shop_store.Models;

namespace Shop_store.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ShoppingCartService> _logger;

        public ShoppingCartService(ApplicationDbContext dbContext, ILogger<ShoppingCartService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult> AddToCart(string userId, ShoppingCartItemDto itemDto)
        {
            if (itemDto == null)
            {
                return new ServiceResult { Success = false, Errors = new List<string> { "Shopping Cart item is null" } };
            }

            try
            {
                itemDto.UserId = userId;
                itemDto.AddedAt = DateTime.Now;

                _dbContext.ShoppingCartItems.Add(itemDto);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart for user {UserId}", userId);
                return new ServiceResult { Success = false, Errors = new List<string> { ex.Message } };
            }

            return new ServiceResult { Success = true };
        }

        public async Task<IEnumerable<ShoppingCartItemDto>> GetCartItems(string userId, int page, int pageSize)
        {
            try
            {

                int skipCount = (page - 1) * pageSize;

                var items = await _dbContext.ShoppingCartItems
                    .Where(item => item.UserId == userId)
                    .OrderBy(item => item.Id)
                    .Skip(skipCount)
                    .Take(pageSize)
                    .ToListAsync();

                var mappedItems = items.Select(item => new ShoppingCartItemDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ImageUrl = item.ImageUrl,
                    AddedAt = item.AddedAt,
                    UpdatedAt = item.UpdatedAt
                });

                return mappedItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items for user {UserId}", userId);
                return new List<ShoppingCartItemDto>();
            }
        }



        public async Task<ServiceResult> UpdateCartItem(string userId, int itemId, ShoppingCartItemDto updatedItemDto)
        {

            if (updatedItemDto == null)
            {
                return new ServiceResult { Success = false, Errors = new List<string> { "UpdatedItemDto is null" } };
            }

            try
            {
                var existingItem = await _dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(item => item.Id == itemId && item.UserId == userId);

                if (existingItem == null)
                {
                    return new ServiceResult { Success = false, Errors = new List<string> { "Item not found" } };
                }

                existingItem.ProductName = updatedItemDto.ProductName;
                existingItem.Price = updatedItemDto.Price;
                existingItem.Quantity = updatedItemDto.Quantity;
                existingItem.ImageUrl = updatedItemDto.ImageUrl;
                existingItem.UpdatedAt = DateTime.Now;

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
       
                _logger.LogError(ex, "Error updating cart item for user {UserId}, itemId {ItemId}", userId, itemId);
                return new ServiceResult { Success = false, Errors = new List<string> { ex.Message } };
            }

            return new ServiceResult { Success = true };
        }

        public async Task<ServiceResult> RemoveFromCart(string userId, int itemId)
        {
            try
            {
                var existingItem = await _dbContext.ShoppingCartItems
                    .FirstOrDefaultAsync(item => item.Id == itemId && item.UserId == userId);

                if (existingItem == null)
                {
                    return new ServiceResult { Success = false, Errors = new List<string> { "Item not found" } };
                }

                _dbContext.ShoppingCartItems.Remove(existingItem);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error removing cart item for user {UserId}, itemId {ItemId}", userId, itemId);
                return new ServiceResult { Success = false, Errors = new List<string> { ex.Message } };
            }

            return new ServiceResult { Success = true };
        }



        public void AssociateUserWithShoppingCart(string userId)
        {   
            var existingShoppingCart = _dbContext.ShoppingCarts.SingleOrDefault(sc => sc.UserId == userId);

            if (existingShoppingCart == null)
            {
                var newShoppingCart = new ShoppingCart
                {
                    UserId = userId,
                };

                _dbContext.ShoppingCarts.Add(newShoppingCart);
            }

            _dbContext.SaveChanges();
        }
    }
}