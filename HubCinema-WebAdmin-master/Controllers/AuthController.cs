using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HubCinemaAdmin.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri(LinkHost.Url);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", login);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var tokenObj = JsonSerializer.Deserialize<TokenResponse>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tokenObj != null && !string.IsNullOrEmpty(tokenObj.Token))
                {
                    HttpContext.Session.SetString("Token", tokenObj.Token);
                    return RedirectToAction("Dashboard", "Dashboard");
                }

                ViewBag.Error = "Không nhận được token từ API.";
                return View(login);
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View(login);
        }
    }
}
