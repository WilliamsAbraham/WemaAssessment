using CustomerServiceApi.Core.Application.Interfaces;
using CustomerServiceApi.Core.Domain;
using CustomerServiceApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceApi.Infrastructure.Implementation
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OtpService> _logger;
        public OtpService(ApplicationContext context, IUnitOfWork unitOfWork, ILogger<OtpService> logger)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public string GenerateOtp(string phoneNumber)
        {
            string otp = string.Empty;
            try
            {
                otp = new Random().Next(100000, 999999).ToString();
                _context.Otps.Add(new Otp { PhoneNumber = phoneNumber, Code = otp });
                Console.WriteLine($"Mock OTP for {phoneNumber}: {otp}");
            }
            catch (Exception ex)
            {
                _unitOfWork.Dispose();
                _logger.LogError(ex.Message, ex);
                throw new Exception("An internal error has occured");
            }

            _unitOfWork.Complete();
            return otp;
        }

        public async Task<bool> VerifyOtp(string phoneNumber, string otp)
        {
            return await _context.Otps.AnyAsync(o => o.PhoneNumber == phoneNumber);
        }
    }

}
