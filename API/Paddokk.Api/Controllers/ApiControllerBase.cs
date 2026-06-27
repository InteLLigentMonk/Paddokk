using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Api.OpenApi;
using Paddokk.Core.Models;

namespace Paddokk.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult FromError(Error error)
    {
        var status = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return StatusCode(status, new ApiErrorResponse(
            error.Code, error.Message, status, TraceId: HttpContext.TraceIdentifier));
    }

    protected ActionResult<T> OkOrError<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : FromError(result.Error);

    protected ActionResult OkOrError(Result result) =>
        result.IsSuccess ? NoContent() : FromError(result.Error);
}
