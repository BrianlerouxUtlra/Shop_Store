namespace Shop_store.Services
{
    public interface IRateLimitService
    {
        bool ExceedsLimit(string clientId);
    }
}
