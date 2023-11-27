using Microsoft.Extensions.Options;
using Shop_store.Config;

namespace Shop_store.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly Dictionary<string, int> _requestCounts = new Dictionary<string, int>();
        private readonly int _limit;
        private readonly TimeSpan _resetInterval;

        public RateLimitService(IOptions<RateLimitingOptions> options)
        {
            _limit = options.Value.Limit;
            _resetInterval = TimeSpan.FromMinutes(options.Value.ResetIntervalMinutes);
        }

        public bool ExceedsLimit(string clientId)
        {
            if (!_requestCounts.TryGetValue(clientId, out int count))
            {
                count = 0;
            }

            if (count >= _limit)
            {
                return true;
            }

            _requestCounts[clientId] = count + 1;

            Task.Delay(_resetInterval).ContinueWith(_ => ResetCount(clientId));

            return false;
        }

        private void ResetCount(string clientId)
        {
            _requestCounts.Remove(clientId);
        }
    }
}
