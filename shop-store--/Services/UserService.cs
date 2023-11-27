namespace Shop_store.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetUserId()
        {
           return _httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value;
           
        }
    }
}
