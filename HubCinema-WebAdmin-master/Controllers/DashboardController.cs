using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HubCinemaAdmin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashboardController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Dashboard()
        {
            string token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.ApiError = "Bạn chưa đăng nhập. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(LinkHost.DashboardSummary);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ApiError = "Không thể lấy dữ liệu dashboard từ API.";
                return View(new AdminDashboardViewModel());
            }

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<AdminDashboardViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(model);
        }

    }
}
