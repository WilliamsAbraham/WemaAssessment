using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServiceApi.Core.Application.Response;
public static class ApiResponseFactory
{
    public static ApiResponse<T> CreateSuccessResponse<T>(T data, string message = "Request was successful.")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Code = "200",
            Errors = null
        };
    }

    public static ApiResponse<T> CreateErrorResponse<T>(string message, string code, List<string> errors = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Data = default,
            Errors = errors ?? new List<string>(),
            Code = code,
        };
    }

    public static PagedResponse<T> CreatePagedResponse<T>(T data, int pageNumber, int pageSize, int totalRecords, string message = "Request was successful.") => new PagedResponse<T>
    {
        IsSuccess = true,
        Message = message,
        Data = data,
        CurrentPage = pageNumber,
        PageSize = pageSize,
        TotalCount = totalRecords,
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),

        // Errors = null
    };

    public static ApiResponse<T> CreateValidationErrorResponse<T>(Dictionary<string, string[]> validationErrors)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = "Validation failed.",
            //Data = default(T),
            ValidationErrors = validationErrors
        };
    }

    //public static object CreateValidationErrorResponse<T>(object errors)
    //{
    //    throw new NotImplementedException();
    //}
}
