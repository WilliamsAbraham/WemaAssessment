using CustomerServiceApi.Core.Application.Features.Customers;

namespace CustomerServiceApi.Core.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<bool> ExistsWithEmailAsync(string email);
        Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber);
        Task<string> CreateCustomerAsync(CreateCustomerDto request);
        Task<string> VerifyCustomer(string phone, string otp);
        Task<List<CustomerDetails>> GetListAsync(CancellationToken cancellationToken);
    }
}
