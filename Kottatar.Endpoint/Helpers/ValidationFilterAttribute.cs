using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kottatar.Entities.Helpers
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var statusCode = StatusCodes.Status400BadRequest;
                var path = context.HttpContext.Request.Path;
                var error = new ErrorModel(
                    String.Join(", ", context.ModelState.Values
                        .SelectMany(t => t.Errors.Select(z => z.ErrorMessage))
                        .ToArray()),
                    statusCode,
                    path
                );
                
                context.HttpContext.Response.StatusCode = statusCode;
                context.Result = new JsonResult(error);
            }
        }
        
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
