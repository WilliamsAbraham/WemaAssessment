namespace CustomerServiceApi.Core.Application.Interfaces
{
    public interface IOtpService
    {
        Task<bool> VerifyOtp(string phoneNumber, string otp);
        string GenerateOtp(string phoneNumber);
    }
}
