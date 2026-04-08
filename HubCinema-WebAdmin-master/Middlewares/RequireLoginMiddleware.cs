using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HubCinemaAdmin.Middlewares
{
    public class RequireLoginMiddleware
    {
        private readonly RequestDelegate _next;

        public RequireLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;

            bool isProtectedPath =
                (path.StartsWithSegments("/Auth/Login") && (method == "GET" || method == "POST")) ||
                path.StartsWithSegments("/css") ||
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/lib") ||
                path.StartsWithSegments("/images");


            if (isProtectedPath)
            {
                var token = context.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"Chặn truy cập: {path} → không có token");
                    context.Response.Redirect("/Auth/Login");
                    return;
                }
            }

            await _next(context);
        }

    }
}
