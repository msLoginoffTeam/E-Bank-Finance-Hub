using Microsoft.AspNetCore.Mvc;
using User_Api.Data.DTOs.Responses;

namespace User_Api.Data
{
    public static class InvalidModelResponse
    {
        public static IActionResult MakeValidationResponse(ActionContext context)
        {
            var validationProblemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
            };

            var problemDetails = new ErrorListResponse((int)validationProblemDetails.Status);

            foreach (var error in validationProblemDetails.Errors)
            {
                problemDetails.addError(error.Key, error.Value.First());
            }

            var result = new BadRequestObjectResult(problemDetails);

            result.ContentTypes.Add("application/problem+json");

            return result;
        }
    }
}
