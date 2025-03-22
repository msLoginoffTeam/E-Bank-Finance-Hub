using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Common.ErrorHandling;

namespace Auth_Service.Services.Utils
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled) return;

            context.Result = new RedirectToActionResult("Error", "Auth", new { message = (context.Exception as ErrorException).message });

            context.ExceptionHandled = true;
        }
    }
}
