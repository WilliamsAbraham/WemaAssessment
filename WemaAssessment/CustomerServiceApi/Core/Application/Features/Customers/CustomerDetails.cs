namespace CustomerServiceApi.Core.Application.Features.Customers
{
    public class CustomerDetails
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string StateOfResidence { get; set; } = string.Empty;
        public string LGA { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }
}
