
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace coinbaseapi.Hubs
{
    public class CoinHub : Hub
    {
        private readonly IApiService _apiService;
        private readonly IMemoryCache _memoryCache;

        public CoinHub(IApiService apiService, IMemoryCache memoryCache)
        {
            _apiService = apiService;
            _memoryCache = memoryCache;
        }

        public override async Task OnConnectedAsync()
        {
            await InitializeBtcPolling();
            await base.OnConnectedAsync();
        }

        private async Task InitializeBtcPolling()
        {
            var priceList = _memoryCache.Get("PriceList");
            if (priceList == null)
            {
                await _apiService.StartPollingCoindesk();
            }
        }

    }
}