namespace CustomerServiceApi.Core.Application.Features.Customers
{
    using System.ComponentModel.DataAnnotations;

    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "State of residence is required.")]
        public string StateOfResidence { get; set; } = string.Empty;

        [Required(ErrorMessage = "LGA is required.")]
        public string LGA { get; set; } = string.Empty;
    }

}
