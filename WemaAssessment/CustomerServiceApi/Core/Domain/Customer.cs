using Microsoft.AspNetCore.Identity;

namespace CustomerServiceApi.Core.Domain
{
    public class Customer : IdentityUser<Guid>
    {
        public string StateOfResidence { get; set; } = string.Empty;
        public string LGA { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }

}
