using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Capteurs.Filters
{
    public class ApiKeyAuthAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _apiKey;

        public ApiKeyAuthAttribute(IConfiguration configuration)
        {
            _apiKey = configuration["ApiSettings:Key"];
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var providedApiKey))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "API Key is missing."
                };
                return;
            }

            if (!_apiKey.Equals(providedApiKey))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "Invalid API Key."
                };
            }
        }
    }
}
