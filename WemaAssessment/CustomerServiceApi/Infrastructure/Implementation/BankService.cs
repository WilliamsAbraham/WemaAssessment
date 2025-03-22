using CustomerServiceApi.Core.Application.Features.Banks;
using CustomerServiceApi.Core.Application.Interfaces;
using CustomerServiceApi.Infrastructure.Utilities;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;

namespace CustomerServiceApi.Infrastructure.Implementation
{
    public class BankService : IBankServcie
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<BankService> _logger;
        private readonly BankSettings _bankSettings;
        public BankService(IHttpClientFactory httpClient, ILogger<BankService> logger, IOptions<BankSettings> bankSettings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _bankSettings = bankSettings.Value;
        }

        public async Task<BankResponse> GetBanksAsync()
        {
            try
            {
               using var client = _httpClient.CreateClient("banks");

                var clients = client;

                var response = await client.GetFromJsonAsync<BankResponse>(_bankSettings.GetAllBanks);
                return response.HasError ? new BankResponse { HasError = true, ErrorMessage = response.ErrorMessage } : response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception("An internal error has occured");
            }
  
        }
    }

}
