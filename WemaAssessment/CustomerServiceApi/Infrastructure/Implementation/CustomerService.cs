using CustomerServiceApi.Core.Application.Exceptions;
using CustomerServiceApi.Core.Application.Features.Customers;
using CustomerServiceApi.Core.Application.Interfaces;
using CustomerServiceApi.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerServiceApi.Infrastructure.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly UserManager<Customer> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOtpService _otpService;
        private readonly IGeolocation _geolocation;
        private readonly ILogger<CustomerService> _logger;
        public CustomerService
            (UserManager<Customer> userManager, 
            IUnitOfWork unitOfWork,
            IOtpService otpService, 
            ILogger<CustomerService> logger,
            IGeolocation geolocation)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _otpService = otpService;
            _logger = logger;
            _geolocation = geolocation;
        }
        public async Task<string> CreateCustomerAsync(CreateCustomerDto request)
        {
            string otp = string.Empty;
            string response = string.Empty;

            CheckStatAndLga(request.StateOfResidence, request.LGA);

            if(await ExistsWithEmailAsync(request.Email))
                throw new ValidationException("Email already exists");

            if (await ExistsWithPhoneNumberAsync(request.PhoneNumber))
                throw new ValidationException("Phone number already exists");

            var user = new Customer
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                StateOfResidence = request.StateOfResidence,
                LGA = request.LGA
            };

             try
             {
                var result = await _userManager.CreateAsync(user, request.PasswordHash);
                if (result.Succeeded)
                {
                    otp = _otpService.GenerateOtp(request.PhoneNumber);
                    response = $"User Registraion successful, please veriry your phone number with this otp {otp}";
                }
                else
                {
                    response = "User registration failed, please try again";
                    _logger.LogError(result.Errors.FirstOrDefault()?.Description.ToString());
                    throw new Exception(response);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception("An internal error has occured");
            }

            return response;
        }
        public async Task<bool> ExistsWithPhoneNumberAsync(string phoneNumber)
        {

            return await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber) is Customer user;
        }

        public async Task<bool> ExistsWithEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email.Normalize()) is Customer user;
        }


        public async Task<List<CustomerDetails>> GetListAsync(CancellationToken cancellationToken)
        {
            var customers = await _userManager.Users.AsNoTracking().ToListAsync();

            return customers.Select(s => new CustomerDetails
            {
                Id = s.Id,
                Email = s.Email ?? string.Empty,
                PhoneNumber = s.PhoneNumber ?? string.Empty,
                StateOfResidence = s.StateOfResidence,
                LGA = s.LGA ?? string.Empty,

            }).ToList();
        }

        public async Task<string> VerifyCustomer(string phone, string otp)
        {
            string response = string.Empty;

            try
            {
                var exist = await _otpService.VerifyOtp(phone, otp);
                if(exist)
                {
                    var customer = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
                    if (customer != null)
                    {
                        customer.PhoneNumberConfirmed = true;
                        customer.IsVerified = true;

                        await _userManager.UpdateAsync(customer);
                        _unitOfWork.Complete();

                        response = $"Custmer with phone number {customer.PhoneNumber} is verified successfully!";
                    }
                    else
                    {
                        response = "Verification failed, please check the phone number supplied or the opt to be sure they are correct";

                    }
                }

                else
                {
                    response = "Verification failed, please check the phone number supplied or the opt to be sure they are correct";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception("An internal error has occured");
            }
            return response;
        }

        private void CheckStatAndLga(string state, string lga)
        {
            var states = _geolocation.GetStates().ToList();
            var isValidState = states.FirstOrDefault(s => s.Name == state) ?? throw new NotFoundException("The selected state does not exist");
            var lgas = _geolocation.GetLGAs(isValidState.Id).ToList();

            var isValidLga = lgas.Any(l => l.Name == lga);
            if (!isValidLga)
                throw new NotFoundException("The selected lga  does not belong to the selected state or it does not exist");

            //return isValidState.Equals(isValidLga);

        }
    }
}
