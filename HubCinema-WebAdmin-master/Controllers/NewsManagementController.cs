using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using HubCinemaAdmin.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HubCinemaAdmin.Controllers
{
    public class NewsManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://localhost:5264/api/News";

        public NewsManagementController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        //Trang danh s�ch b�i vi?t (Index)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(_apiBaseUrl);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Kh�ng th? t?i danh s�ch b�i vi?t.";
                return View(new List<News>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var newsList = JsonSerializer.Deserialize<List<News>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(newsList);
        }

        //Trang t?o b�i vi?t
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categoryResponse = await _httpClient.GetAsync("http://localhost:5264/api/News/categories");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var json = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
            }
            else
            {
                ViewBag.Categories = new List<SelectListItem>();
                ViewBag.Error = "Kh�ng th? t?i danh m?c!";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] NewsCreateDTO dto)
        {

            string? thumbnailPath = null;

            //Upload ?nh n?u c�
            if (dto.ThumbnailFile != null && dto.ThumbnailFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ThumbnailFile.FileName);
                var saveFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "news");
                Directory.CreateDirectory(saveFolder);
                var savePath = Path.Combine(saveFolder, fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await dto.ThumbnailFile.CopyToAsync(stream);
                }

                //T?o du?ng d?n tuy?t d?i t? Request
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                thumbnailPath = $"{baseUrl}/uploads/news/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
            {
                //D�ng link ngu?i d�ng nh?p n?u kh�ng upload
                thumbnailPath = dto.ThumbnailUrl;
            }

            //Chu?n b? d? li?u g?i API
            var news = new News
            {
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                Slug = dto.Slug,
                Content = dto.Content,
                Status = dto.Status ?? "A",
                Category = dto.CategoryId ?? 1,
                Thumbnail = thumbnailPath
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(news), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiBaseUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            //N?u l?i: hi?n th�ng b�o + n?p l?i danh m?c
            await LoadCategoriesAsync();
            ViewBag.Error = "T?o b�i vi?t th?t b?i!";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
            return RedirectToAction("Index");
        }

        private async Task LoadCategoriesAsync()
        {
            var categoryResponse = await _httpClient.GetAsync("http://localhost:5264/api/News/categories");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var json = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
            }
            else
            {
                ViewBag.Categories = new List<SelectListItem>();
            }
        }

    }
}
