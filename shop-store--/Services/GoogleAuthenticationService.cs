using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Shop_store.Services
{
    public class GoogleAuthenticationService : IGoogleAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IShoppingCartService _shoppingCartService;

        public GoogleAuthenticationService(IHttpContextAccessor httpContextAccessor, IShoppingCartService shoppingCartService)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _shoppingCartService = shoppingCartService ?? throw new ArgumentNullException(nameof(shoppingCartService));
        }

        public async Task<IActionResult> HandleGoogleResponseAsync()
        {
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var userId = result.Principal?.FindFirst("sub")?.Value;
                _shoppingCartService.AssociateUserWithShoppingCart(userId);
                return new OkResult();
            }

            return new BadRequestResult();
        }
    }
}