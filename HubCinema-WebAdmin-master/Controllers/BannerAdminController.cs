using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HubCinemaAdmin.Controllers
{
    public class BannerAdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public BannerAdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://api.dvxuanbac.com:2030/");
        }

        private void AddBearerToken()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IActionResult> Index()
        {
            AddBearerToken();

            var banners = await _httpClient.GetFromJsonAsync<List<Banner>>("api/Banner/active");
            return View(banners ?? new List<Banner>());
        }

        public IActionResult Create()
        {
            return View(new Banner());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Banner model)
        {
            AddBearerToken();

            string? imagePath = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banners");
                Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                imagePath = $"{baseUrl}/uploads/banners/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                imagePath = model.ImageUrl;
            }

            model.ImageUrl = imagePath;
            model.CreatedAt = DateTime.Now;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Banner", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ViewBag.Error = "Tạo banner thất bại!";
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            AddBearerToken();

            var banner = await _httpClient.GetFromJsonAsync<Banner>($"api/Banner/{id}");
            if (banner == null)
                return NotFound();

            return View(banner);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] Banner model)
        {
            AddBearerToken();

            if (!ModelState.IsValid)
                return View(model);

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{model.ImageFile.FileName}";
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banners");
                Directory.CreateDirectory(folder);

                var fullPath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                model.ImageUrl = $"{baseUrl}/uploads/banners/{fileName}";
            }

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/Banner/{model.BannerId}", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError("", "Cập nhật banner thất bại.");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            AddBearerToken();

            await _httpClient.DeleteAsync($"api/Banner/{id}");
            return RedirectToAction("Index");
        }
    }
}
