using Shared.Wrapper;
using Domain.ViewModels;
using Application.Common.Exceptions;

namespace Web.ExceptionHandler;

public static class FailureResponseHandlerExtension
{
    public static IResult FailureResponse(this Result result)
    {
        if (!result.IsFailure || result.Error is null)
        {
            throw new InvalidOperationException("FailureResponse should only be called on failed results.");
        }

        if (IsNotFoundError(result.Error.Code))
        {
            return Results.NotFound(new ApiResponse<object>
            {
                Suceeded = false,
                Data = null,
                Error = new ApiError
                {
                    Code = result.Error.Code,
                    Message = result.Error.Message
                }
            });
        }

        throw new GeneralApplicationException(result.Error.Message);
    }

    private static bool IsNotFoundError(string? code)
    {
        return !string.IsNullOrWhiteSpace(code) &&
               (code.EndsWith(".NotFound", StringComparison.OrdinalIgnoreCase) ||
                code.Equals("NotFound", StringComparison.OrdinalIgnoreCase));
    }
}