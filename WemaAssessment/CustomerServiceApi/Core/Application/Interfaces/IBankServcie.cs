using CustomerServiceApi.Core.Application.Features.Banks;

namespace CustomerServiceApi.Core.Application.Interfaces
{
    public interface IBankServcie
    {
        Task<BankResponse> GetBanksAsync();
    }
}
