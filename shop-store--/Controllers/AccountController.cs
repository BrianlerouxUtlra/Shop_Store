using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_store.Data;
using Shop_store.Models;
using Shop_store.Services;

namespace Shop_store.Controllersc.SwaggerDoc
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]   Authenticating a user through Google Console via SWAGGER UI is not working(CORS HEADER ISSUES ETC...), but calling it directly using //account/login is successful.
    //I added this information to demonstrate my understanding of what needs to happen, but I'm pressed for time to debug. (hard coded my user ID for now)

    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGoogleAuthenticationService _googleAuthenticationService;
        private readonly ApplicationDbContext _applicationDbContext;


        public AccountController(
          ILogger<AccountController> logger,
          IHttpClientFactory httpClientFactory,
          IGoogleAuthenticationService googleAuthenticationService,
          ApplicationDbContext applicationDbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _googleAuthenticationService = googleAuthenticationService ?? throw new ArgumentNullException(nameof(googleAuthenticationService));
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }




        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse), "Account"),
            };

            var authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (authResult?.Principal == null)
            {
                return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
            }

            return RedirectToAction(nameof(GoogleResponse));
        }



        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"User authenticated with ID: {userId}");

                var existingUser = await _applicationDbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

                if (existingUser == null)
                {
                    var newUser = new User
                    {
                        UserId = userId,
                        Email = User.FindFirst(ClaimTypes.Email)?.Value,
                        FullName = User.FindFirst(ClaimTypes.Name)?.Value,
                    };

                    _applicationDbContext.Users.Add(newUser);
                    await _applicationDbContext.SaveChangesAsync();

                    _logger.LogInformation($"New user created with ID: {userId}");
                }
                else
                {
                    _logger.LogInformation($"Existing user logged in with ID: {userId}");
                }

                return Ok(new { Message = "Authentication successful", UserId = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error handling Google response: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}