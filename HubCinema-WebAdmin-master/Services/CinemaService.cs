using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HubCinemaAdmin.Services
{
    public class CinemaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;

        public CinemaService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _contextAccessor = contextAccessor;
        }

        private HttpClient CreateAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = _contextAccessor.HttpContext?.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<bool> CreateCinemaAsync(CinemaDTO cinema)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Admin/CreateCinema", cinema);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CinemaDTO>> GetCinemasAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetCinemas");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Không thể lấy danh sách rạp.");

            var json = await response.Content.ReadAsStringAsync();
            var cinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(json);
            return cinemas ?? new List<CinemaDTO>();
        }

        public async Task<CinemaDTO?> GetCinemaByIdAsync(int id)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync(LinkHost.Url + $"/Public/GetCinemaById/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CinemaDTO>();
        }

        public async Task<bool> UpdateCinemaAsync(CinemaDTO cinema)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync(LinkHost.Url + $"/Admin/UpdateCinema/{cinema.IDCinema}", cinema);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteCinemaAsync(int idCinema)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync(LinkHost.Url + $"/Admin/DeleteCinema/{idCinema}");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {response.StatusCode}, Response: {content}");
            Console.WriteLine($"Request URL: {LinkHost.Url + $"/Admin/DeleteCinema/{idCinema}"}");
            return response.IsSuccessStatusCode;
        }

    }
}
