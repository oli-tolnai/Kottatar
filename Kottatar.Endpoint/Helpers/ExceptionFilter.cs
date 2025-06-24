using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kottatar.Entities.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kottatar.Entities.Helpers
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Set default status code
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = context.Exception.Message;

            // Determine appropriate status code based on exception type
            if (context.Exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
            }
            else if (context.Exception is KeyNotFoundException || context.Exception is InvalidOperationException)
            {
                statusCode = StatusCodes.Status404NotFound;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
            }

            var path = context.HttpContext.Request.Path;
            var error = new ErrorModel(message, statusCode, path);

            context.HttpContext.Response.StatusCode = statusCode;
            context.Result = new JsonResult(error);
            context.ExceptionHandled = true;
        }
    }
}
