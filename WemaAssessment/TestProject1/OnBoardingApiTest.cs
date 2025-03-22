using CustomerServiceApi.Core.Application.Features.Banks;
using CustomerServiceApi.Core.Application.Features.Customers;
using CustomerServiceApi.Core.Application.Features.Geolocations;
using CustomerServiceApi.Core.Application.Interfaces;
using CustomerServiceApi.Core.Application.Response;
using CustomerServiceApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CustomerServiceApi.Tests.Controllers
{
    public class CustomerOnBoardingControllerTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<IBankServcie> _bankServiceMock;
        private readonly Mock<IGeolocation> _geolocationMock;
        private readonly Mock<ILogger<CustomerOnBoardingController>> _loggerMock;
        private readonly CustomerOnBoardingController _controller;

        public CustomerOnBoardingControllerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _bankServiceMock = new Mock<IBankServcie>();
            _geolocationMock = new Mock<IGeolocation>();
            _loggerMock = new Mock<ILogger<CustomerOnBoardingController>>();

            // Create a mock HttpContext and set up RequestServices
            var httpContext = new Mock<HttpContext>();
            var serviceProvider = new Mock<IServiceProvider>();
            var loggerFactory = new Mock<ILoggerFactory>();

            // Set up the logger factory to return the mock logger
            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);
            serviceProvider.Setup(x => x.GetService(typeof(ILoggerFactory))).Returns(loggerFactory.Object);
            httpContext.Setup(x => x.RequestServices).Returns(serviceProvider.Object);

            // Create the controller and set the HttpContext
            _controller = new CustomerOnBoardingController(
                _customerServiceMock.Object,
                _bankServiceMock.Object,
                _geolocationMock.Object,
                _loggerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };
        }

        [Fact]
        public async Task GetAllCustomers_ShouldReturnListOfCustomers()
        {
            // Arrange
            var customers = new List<CustomerDetails>
            {
                new CustomerDetails
                {
                    Id = Guid.NewGuid(),
                    Email = "test1@example.com",
                    PhoneNumber = "1234567890"
                },
                new CustomerDetails
                {
                    Id = Guid.NewGuid(),
                    Email = "test2@example.com",
                    PhoneNumber = "0987654321"
                }
            };

            _customerServiceMock.Setup(x => x.GetListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            // Act
            var result = await _controller.GetAllCustomers(CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<List<CustomerDetails>>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result); // Unwrap the ActionResult<T>

            var apiResponse = Assert.IsType<ApiResponse<List<CustomerDetails>>>(okResult.Value);

            // Verify the response data
            Assert.Equal(customers.Count, apiResponse.Data.Count);
            Assert.Equal(customers[0].Id, apiResponse.Data[0].Id);
            Assert.Equal(customers[0].Email, apiResponse.Data[0].Email);
            Assert.Equal(customers[0].PhoneNumber, apiResponse.Data[0].PhoneNumber);

            Assert.Equal(customers[1].Id, apiResponse.Data[1].Id);
            Assert.Equal(customers[1].Email, apiResponse.Data[1].Email);
            Assert.Equal(customers[1].PhoneNumber, apiResponse.Data[1].PhoneNumber);
        }

        [Fact]
        public async Task GetAllCustomers_ShouldReturnInternalServerError_WhenServiceFails()
        {
            // Arrange
            _customerServiceMock.Setup(x => x.GetListAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service unavailable"));

            // Act
            var result = await _controller.GetAllCustomers(CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<List<CustomerDetails>>>>(result);
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<List<CustomerDetails>>>(statusCodeResult.Value);
            Assert.False(apiResponse.IsSuccess);
            Assert.Equal("An internal server error occurred.", apiResponse.Message);
        }

        [Fact]
        public async Task OnBoardCustomer_ShouldReturnSuccessResponse_WhenValidRequest()
        {
            // Arrange
            var request = new CreateCustomerDto
            {
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                StateOfResidence = "Lagos",
                LGA = "Ikeja",
                PasswordHash = "TestPassword123!"
            };

            var responseMessage = "User Registration successful";
            _customerServiceMock.Setup(x => x.CreateCustomerAsync(request))
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.OnBoardCustomer(request);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<string>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(responseMessage, apiResponse.Data);
        }

        [Fact]
        public async Task VerifyCustomer_ShouldReturnSuccessResponse_WhenValidOtp()
        {
            // Arrange
            var phone = "1234567890";
            var otp = "123456";
            var responseMessage = "Customer verified successfully";

            _customerServiceMock.Setup(x => x.VerifyCustomer(phone, otp))
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.VerifyCustomer(phone, otp);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<string>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(responseMessage, apiResponse.Data);
        }

        [Fact]
        public async Task GetBanks_ShouldReturnListOfBanks()
        {
            // Arrange
            var banks = new BankResponse
            {
                Result = new List<BankResult>
                {
                    new BankResult { BankName = "Bank A", BankCode = "001", BankLogo = "logo1.png" },
                    new BankResult { BankName = "Bank B", BankCode = "002", BankLogo = "logo2.png" }
                },
                ErrorMessage = string.Empty,
                ErrorMessages = new List<string>(),
                HasError = false,
                TimeGenerated = DateTime.UtcNow
            };

            _bankServiceMock.Setup(x => x.GetBanksAsync())
                .ReturnsAsync(banks);

            // Act
            var result = await _controller.GetBanks();

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<BankResponse>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            var apiResponse = Assert.IsType<ApiResponse<BankResponse>>(okResult.Value);
            Assert.Equal(banks.Result.Count, apiResponse.Data.Result.Count);
            Assert.Equal(banks.Result[0].BankName, apiResponse.Data.Result[0].BankName);
            Assert.Equal(banks.Result[0].BankCode, apiResponse.Data.Result[0].BankCode);
            Assert.Equal(banks.Result[0].BankLogo, apiResponse.Data.Result[0].BankLogo);
        }
    }
}