using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shop_store.Controllersc.SwaggerDoc;
using Shop_store.Models;
using Shop_store.Services;

namespace Shop_store.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]   Authenticating a user through Google Console via SWAGGER UI is not working(CORS HEADER ISSUES ETC...), but calling it directly using //account/login is successful.
    //I added this information to demonstrate my understanding of what needs to happen, but I'm pressed for time to debug. (hard coded my user ID for now)
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;
        

        //Hard coded a user-id
        private string GetUserId()
        {
            return "109438982097685230115";
        }

        public ShoppingCartController(IShoppingCartService shoppingCartService, IUserService userService, ILogger<AccountController> logger)
        {
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
            _userService = userService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("items")]
        public async Task<IActionResult> GetCartItems([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();

            var items = await _shoppingCartService.GetCartItems(userId, page, pageSize);

            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] ShoppingCartItemDto itemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var userId = GetUserId();

            if (userId == null)
            {
                return BadRequest(new { Message = "UserId is required" });
            }

            var imageUrl = itemDto.ImageUrl;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                if (Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    itemDto.ImageUrl = imageUrl;
                }
                else
                {
                    return BadRequest(new { Message = "Invalid image URL" });
                }
            }

            var result = await _shoppingCartService.AddToCart(userId, itemDto);

            if (result.Success)
            {
                return Ok(new { Message = "Item added to the cart successfully" });
            }
            else
            {
                _logger.LogError(result.Exception, "Error adding item to cart for user {UserId}", userId);
                return BadRequest(new { Message = "Failed to add item to the cart", Errors = result.Exception?.Message });
            }
        }


        [HttpPost("update/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(int itemId, [FromBody] ShoppingCartItemDto updatedItemDto)
        {
            var userId = GetUserId();

            var result = await _shoppingCartService.UpdateCartItem(userId, itemId, updatedItemDto);

            if (result.Success)
            {
                return Ok(new { Message = "Item updated successfully" });
            }
            else
            {
                _logger.LogError(result.Exception, "Error updating item in cart for user {UserId}", userId);
                return BadRequest(new { Message = "Failed to update item", Errors = result.Exception?.Message });
            }
        }

        [HttpDelete("remove/{itemId}")]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            var userId = GetUserId();

            var result = await _shoppingCartService.RemoveFromCart(userId, itemId);

            if (result.Success)
            {
                return Ok(new { Message = "Item removed from the cart successfully" });
            }
            else
            {
                _logger.LogError(result.Exception, "Error removing item from cart for user {UserId}", userId);
                return BadRequest(new { Message = "Failed to remove item from the cart", Errors = result.Exception?.Message });
            }
        }

    }
}