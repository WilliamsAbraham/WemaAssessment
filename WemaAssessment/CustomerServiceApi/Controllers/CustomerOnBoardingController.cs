using CustomerServiceApi.Core.Application.Features.Banks;
using CustomerServiceApi.Core.Application.Features.Customers;
using CustomerServiceApi.Core.Application.Features.Geolocations;
using CustomerServiceApi.Core.Application.Interfaces;
using CustomerServiceApi.Core.Application.Response;
using HobaxHrApi.Host.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace CustomerServiceApi.Controllers
{
    public class CustomerOnBoardingController : BaseApiController
    {
        private readonly ICustomerService _customerService;
        private readonly IBankServcie _banServcie;
        private readonly IGeolocation _geoLocation;
        public CustomerOnBoardingController(ICustomerService customerService, IBankServcie bankServcie, IGeolocation geolocation, ILogger<CustomerOnBoardingController> @object)
        {
            _customerService = customerService;
            _banServcie = bankServcie;
            _geoLocation = geolocation;
        }

        [HttpPost("OnBoardCustomer")]
        public async Task<ActionResult<ApiResponse<string>>> OnBoardCustomer(CreateCustomerDto request)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var respone = await _customerService.CreateCustomerAsync(request);

                return ApiResponseFactory.CreateSuccessResponse(respone);
            }
            );
        }

        [HttpPut("verify")]
        public async Task<ActionResult<ApiResponse<string>>> VerifyCustomer(string phone, string otp)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var response = await _customerService.VerifyCustomer(phone, otp);

                return ApiResponseFactory.CreateSuccessResponse(response);
            });
        }

        [HttpGet("getBanks")]
        public async Task<ActionResult<ApiResponse<BankResponse>>> GetBanks()
        {
            return await HandleApiOperationAsync(async () =>
            {
                var response = await _banServcie.GetBanksAsync();

                return ApiResponseFactory.CreateSuccessResponse(response);
            });
        }

        [HttpGet("all-customers")]
        public async Task<ActionResult<ApiResponse<List<CustomerDetails>>>> GetAllCustomers(CancellationToken cancellationToken)
        {
            return await HandleApiOperationAsync(async () =>
            {
                var response = await _customerService.GetListAsync(cancellationToken);

                return ApiResponseFactory.CreateSuccessResponse(response);
            });
        }   
        [HttpGet("getStates")]
        public ActionResult<ApiResponse<List<StateDto>>> GetStats()
        {
            return HandleApiOperationAsync(() =>
            {
                var response = _geoLocation.GetStates().ToList();

                return Task.FromResult(ApiResponseFactory.CreateSuccessResponse(response));
            }).Result;
        }

        [HttpGet("getLgas")]
        public ActionResult<ApiResponse<List<LGADto>>> GetLgas(int stateId)
        {
            return HandleApiOperationAsync(() =>
            {
                var response = _geoLocation.GetLGAs(stateId).ToList();

                return Task.FromResult(ApiResponseFactory.CreateSuccessResponse(response));
            }).Result;
        }


    }
}
