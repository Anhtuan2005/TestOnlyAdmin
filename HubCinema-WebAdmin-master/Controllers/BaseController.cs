using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class BaseController : Controller
    {
        protected string? Token => HttpContext.Session.GetString("Token");

        protected bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        protected HttpClient CreateAuthorizedClient(IHttpClientFactory factory)
        {
            var client = factory.CreateClient();
            if (!string.IsNullOrEmpty(Token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
            return client;
        }
    }
}
