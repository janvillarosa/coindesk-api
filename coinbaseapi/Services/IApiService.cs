using System.Threading.Tasks;
using coinbaseapi.Models;

public interface IApiService
{
    public Task<Price[]> GetCurrentCoinPrice();
    public Task StartPollingCoindesk();
}