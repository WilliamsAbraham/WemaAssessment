using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServiceApi.Core.Application.Response;
public class BaseResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}

public class ApiResponse<T> : BaseResponse
{
    public T Data { get; set; } = default(T)!;
}

public class PagedResponse<T> : ApiResponse<T>
{
    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}