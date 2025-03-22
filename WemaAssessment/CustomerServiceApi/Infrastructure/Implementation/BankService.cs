using CustomerServiceApi.Core.Application.Features.Banks;
using CustomerServiceApi.Core.Application.Interfaces;
using CustomerServiceApi.Infrastructure.Utilities;
using System.Reflection.Metadata;

namespace CustomerServiceApi.Infrastructure.Implementation
{
    public class BankService : IBankServcie
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<BankService> _logger;
        public BankService(IHttpClientFactory httpClient, ILogger<BankService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<BankResponse> GetBanksAsync()
        {
            try
            {
               using var client = _httpClient.CreateClient("banks");

                var clients = client;

                var response = await client.GetFromJsonAsync<BankResponse>(Constants.GetALLBANKS);
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
