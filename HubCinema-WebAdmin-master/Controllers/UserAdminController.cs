using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HubCinemaAdmin.Controllers
{
    public class UserAdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserAdminController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
        }

        // GET: /UserAdmin
        public async Task<IActionResult> Index(string? keyword)
        {
            string token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            string url = "http://api.dvxuanbac.com:2030/api/admin/users/all";
            if (!string.IsNullOrEmpty(keyword))
            {
                url += $"?keyword={keyword}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Không thể tải danh sách người dùng. Chi tiết: {errorDetail}";
                    return View(new List<UserDto>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDto>>(json) ?? new List<UserDto>();

                ViewBag.Keyword = keyword;
                return View(users);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi khi gọi API: {ex.Message}";
                return View(new List<UserDto>());
            }
        }

        // GET: /UserAdmin/Invoices/15
        public async Task<IActionResult> Invoices(int id)
        {
            string token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Bạn chưa đăng nhập. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Auth");
            }

            string url = $"http://api.dvxuanbac.com:2030/api/admin/users/{id}/invoices";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = $"Không thể tải hóa đơn người dùng. Chi tiết: {errorDetail}";
                    return View(new List<InvoiceDto>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(json) ?? new List<InvoiceDto>();

                ViewBag.UserId = id;
                return View(invoices);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Lỗi khi gọi API: {ex.Message}";
                return View(new List<InvoiceDto>());
            }
        }
    }
}
