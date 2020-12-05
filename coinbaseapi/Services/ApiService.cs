using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using coinbaseapi.Hubs;
using coinbaseapi.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace coinbaseapi.Services
{
    public class ApiService : IApiService
    {
        private HttpClient _httpClient;
        private int _pollingInterval;
        private IMemoryCache _cache;
        private IHubContext<CoinHub> _hubContext;

        public ApiService(
          HttpClient httpClient,
          IMemoryCache cache,
          IHubContext<CoinHub> hubContext)
        {
            _httpClient = httpClient;
            _pollingInterval = 10000;
            _cache = cache;
            _hubContext = hubContext;
        }

        public async Task<Price[]> GetCurrentCoinPrice()
        {
            var response = "";
            const string URL = "https://api.coingecko.com/api/v3/simple/price?vs_currencies=nzd&include_last_updated_at=true&ids=bitcoin,ethereum";
            try
            {
                response = await _httpClient.GetStringAsync(URL);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                //return empty Price array
                return new Price[0];
            }
            Price[] prices = new Price[2];
            CurrentPriceResponse currentPrice = JsonConvert.DeserializeObject<CurrentPriceResponse>(response);
            prices[0] = new Price() { Value = currentPrice.bitcoin.nzd, Date = DateTimeOffset.Now.DateTime, Currency = "Bitcoin" };
            prices[1] = new Price() { Value = currentPrice.ethereum.nzd, Date = DateTimeOffset.Now.DateTime, Currency = "Ethereum" };
            return prices;
        }

        public async Task StartPollingCoindesk()
        {
            while (true)
            {
                Price[] currentPrices = await GetCurrentCoinPrice();
                foreach (Price currentPrice in currentPrices)
                {
                    AddPriceToListInMemory(currentPrice);
                    SendCurrentPriceToHub(currentPrice);
                }
                Thread.Sleep(_pollingInterval);
            }
        }

        private void SendCurrentPriceToHub(Price price)
        {
            _hubContext.Clients.All.SendAsync("ReceivePrice", price);
        }

        private void AddPriceToListInMemory(Price price)
        {
            IList<Price> priceList = _cache.Get("PriceList") as List<Price>;
            if (priceList == null)
            {
                _cache.CreateEntry("PriceList");
                _cache.Set("PriceList", new List<Price>() { price });
                priceList = _cache.Get("PriceList") as List<Price>;
            }

            if (priceList.Count == 5)
            {
                priceList.RemoveAt(0);
            }
            priceList.Add(price);
            _cache.Set("PriceList", priceList);
        }
    }
}