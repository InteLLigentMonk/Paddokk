using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Models;

namespace Paddokk.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult FromError(Error error) => error.Type switch
    {
        ErrorType.NotFound => NotFound(new { error = error.Message }),
        ErrorType.Unauthorized => Forbid(),
        ErrorType.Conflict => Conflict(new { error = error.Message }),
        ErrorType.Validation => BadRequest(new { error = error.Message }),
        _ => StatusCode(500, new { error = error.Message })
    };

    protected ActionResult<T> OkOrError<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : FromError(result.Error);

    protected ActionResult OkOrError(Result result) =>
        result.IsSuccess ? NoContent() : FromError(result.Error);
}
