using Microsoft.AspNetCore.Mvc;

namespace BrewMVC.Controllers
{
    public class BaseController : Controller
    {
        // TempData keys for consistency
        const string SuccessMessage = "SuccessMessage";
        const string ErrorMessage = "ErrorMessage";

        protected void SetSuccessMessage(string message)
        {
            TempData[SuccessMessage] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData[ErrorMessage] = message;
        }

        // Handle API errors 
        protected void HandleApiError(HttpResponseMessage response, string defaultMessage = "An error occurred")
        {
            if (response.IsSuccessStatusCode) return;

            string errorMessage;
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    errorMessage = "The requested item was not found";
                    break;

                case System.Net.HttpStatusCode.Unauthorized:
                    errorMessage = "You are not authorized to perform this action";
                    break;

                case System.Net.HttpStatusCode.Forbidden:
                    errorMessage = "Access to this resource is forbidden";
                    break;

                case System.Net.HttpStatusCode.Conflict:
                    errorMessage = "A conflict occurred while processing your request";
                    break;

                case System.Net.HttpStatusCode.BadRequest:
                    errorMessage = "Invalid request data";
                    break;

                default:
                    errorMessage = defaultMessage;
                    break;
            }

            ModelState.AddModelError("", errorMessage);
        }

        // Handle API errors when for redirects user from controller actions
        protected void HandleApiErrorWithTempData(HttpResponseMessage response, string defaultMessage = "An error occurred")
        {
            if (response.IsSuccessStatusCode) return;

            string errorMessage;
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NotFound:
                    errorMessage = "The requested item was not found";
                    break;

                case System.Net.HttpStatusCode.Unauthorized:
                    errorMessage = "You are not authorized to perform this action";
                    break;

                case System.Net.HttpStatusCode.Forbidden:
                    errorMessage = "Access to this resource is forbidden";
                    break;

                case System.Net.HttpStatusCode.Conflict:
                    errorMessage = "A conflict occurred while processing your request";
                    break;

                case System.Net.HttpStatusCode.BadRequest:
                    errorMessage = "Invalid request data";
                    break;

                default:
                    errorMessage = defaultMessage;
                    break;
            }

            SetErrorMessage(errorMessage);
        }
    }
}