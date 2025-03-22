
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using CustomerServiceApi.Core.Application.Response;
using CustomerServiceApi.Core.Application.Exceptions;
using System.ComponentModel.DataAnnotations;


namespace HobaxHrApi.Host.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
    private ILogger<BaseApiController>? _logger;
    private string? _databaseErrorMessage;

    //Property to lazily initialize the logger
    protected ILogger<BaseApiController> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger<BaseApiController>();

    protected async Task<ActionResult<ApiResponse<T>>> HandleApiOperationAsync<T>(
    Func<Task<ApiResponse<T>>> action,
    [CallerLineNumber] int lineNo = 0,
    [CallerMemberName] string method = "")
    {
        Logger.LogInformation($">>> ENTERS ({method}) >>> ");

        try
        {
            // Check if the ModelState is invalid (this will include validation errors)
            if (!ModelState.IsValid)
            {
                // Process the ModelState errors into a custom validation response
                var validationErrors = ModelState.ToDictionary(
                    entry =>
                    {
                        // Extract the field name from the ModelState entry key
                        string fieldName = entry.Key.Contains("[")
                            ? entry.Key.Substring(0, entry.Key.IndexOf('[')) + " (Item)"
                            : entry.Key.Split('.').Last();

                        return fieldName;
                    },
                    entry => entry.Value.Errors.Select(e =>
                    {
                        // Provide user-friendly error messages for common scenarios
                        var errorMessage = e.Exception?.Message ?? e.ErrorMessage;

                        return errorMessage switch
                        {
                            "The value '' is invalid." => $"{entry.Key}: is required and cannot be empty.",
                            "The field is invalid." => $"Please provide a valid value for {entry.Key}.",
                            var msg when msg.Contains("could not be converted to System.Guid") =>
                                $"{entry.Key}: Please provide a valid GUID for the employee ID.",
                            _ => !string.IsNullOrWhiteSpace(errorMessage)
                                 ? $"{entry.Key}: {errorMessage}"
                                 : $"{entry.Key}: An error occurred with this field."
                        };
                    }).ToArray()
                );

                // Create a custom validation error response using ApiResponseFactory
                var validationResponse = ApiResponseFactory.CreateValidationErrorResponse<T>(validationErrors);

                // Return a BadRequest (400) with the validation errors
                return BadRequest(validationResponse);
            }

            // Execute the provided action if validation passes
            var actionResponse = await action();

            // Return a success response (200 OK)
            return Ok(ApiResponseFactory.CreateSuccessResponse(actionResponse.Data, actionResponse.Message));
        }

        catch (NotFoundException ex)
        {
            Logger.LogWarning($"L{lineNo} - {ex.StatusCode}: {ex.Message}");
            return NotFound(ApiResponseFactory.CreateErrorResponse<T>(ex.Message, ex.StatusCode.ToString(), new List<string> { ex.Message }));
        }
       
        catch(ValidationException ex)
        {
            Logger.LogWarning($"L{lineNo} - {HttpStatusCode.NotAcceptable}: {ex.Message}");
            return BadRequest(ApiResponseFactory.CreateErrorResponse<T>(ex.Message, HttpStatusCode.BadRequest.ToString(), new List<string> { ex.Message }));
        }
        catch(FormatException ex)
        {
            Logger.LogWarning($"L{lineNo} - {HttpStatusCode.BadRequest}: {ex.Message}");
            return BadRequest(ApiResponseFactory.CreateErrorResponse<T>(ex.Message, HttpStatusCode.BadRequest.ToString(), new List<string> { ex.Message }));
        }

         catch(ArgumentNullException ex)
        {
            Logger.LogWarning($"L{lineNo} - {HttpStatusCode.BadRequest}: {ex.Message}");
            return BadRequest(ApiResponseFactory.CreateErrorResponse<T>(ex.Message, HttpStatusCode.BadRequest.ToString(), new List<string> { ex.Message }));
        }

        catch (Exception ex)
        {
            Logger.LogWarning($"L{lineNo} - DBV001: {ex.Message}");
            Logger.LogError(ex.Message, ex);
            return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponseFactory.CreateErrorResponse<T>(
                "An internal server error occurred.", "DBV001", new List<string> { "500" }));
        }
        finally
        {
            Logger.LogInformation($"<<< EXITS ({method}) <<< ");
        }
    }

    protected async Task<IActionResult> HandleFileApiOperationAsync(
    Func<Task<IActionResult>> action, [CallerLineNumber] int lineNo = 0, [CallerMemberName] string method = "")
    {
        Logger.LogInformation($">>> ENTERS ({method}) >>> ");

        try
        {
            // Check for model validation errors (if any)
            if (!ModelState.IsValid)
            {
                var validationResponse = new
                {
                    Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.Exception?.Message ?? e.ErrorMessage)
                            .ToList()
                };
                return BadRequest(validationResponse);  // Return 400 Bad Request for validation issues
            }

            // Execute the action that generates the file
            return await action();  // Directly handle file response or custom results
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"L{lineNo} - Error: {ex.Message}");
            Logger.LogError(ex.Message, ex);
            return StatusCode((int)HttpStatusCode.InternalServerError, new
            {
                ErrorCode = "500",
                Message = "An internal server error occurred.",
                Details = ex.Message
            });  // Return 500 Internal Server Error
        }
        finally
        {
            Logger.LogInformation($"<<< EXITS ({method}) <<< ");
        }
    }

    /// <summary>
    /// Read ModelError into string collection
    /// </summary>
    /// <returns></returns>
    private List<string> ListModelErrors
    {
        get
        {
            return ModelState.Values
              .SelectMany(x => x.Errors
                .Select(ie => ie.ErrorMessage))
                .ToList();
        }
    }
}
