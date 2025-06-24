using Kottatar.Entities.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Kottatar.Endpoint.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;
            
            var statusCode = StatusCodes.Status500InternalServerError;
            
            // Determine appropriate status code based on exception type
            if (exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
            }
            else if (exception is KeyNotFoundException || exception is InvalidOperationException)
            {
                statusCode = StatusCodes.Status404NotFound;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
            }
            
            var path = HttpContext.Request.Path;
            var errorModel = new ErrorModel(
                exception?.Message ?? "An unexpected error occurred",
                statusCode,
                path
            );
            
            return StatusCode(statusCode, errorModel);
        }
    }
}