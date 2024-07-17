using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WhatsAppAPI.Middleware
{
    public class ApiKeyMiddleware
    {

        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER_NAME = "WhatsApp-Api-Key";
        private const string API_KEY = "XQez0E3veIsWVz4Zk5aqq5BrsvPwUq4E"; // Replace with your actual API key

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            if (!API_KEY.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }

            await _next(context);
        }

    }
}
