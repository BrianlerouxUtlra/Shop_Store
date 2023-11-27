using Microsoft.AspNetCore.Mvc;

namespace Shop_store.Services
{
    public interface IGoogleAuthenticationService
    {
        Task<IActionResult> HandleGoogleResponseAsync();
    }
}
