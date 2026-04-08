// Services/ShowtimeService.cs
using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HubCinemaAdmin.Services
{
    public class ShowtimeService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ShowtimeService(HttpClient httpClient, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        private HttpClient CreateAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
        public async Task<List<CinemaDTO>> GetCinemasAsync()
        {
            try
            {
                var url = $"{LinkHost.Url}/Public/GetCinemas";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var cinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(json);

                return cinemas ?? new List<CinemaDTO>();
            }
            catch
            {
                return new List<CinemaDTO>();
            }
        }
        public async Task<List<MovieDTO>> GetMoviesAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetMovies");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<MovieDTO>>(json);

            return movies?
                .Where(m => m.status == 0 || m.status == 1)
                .ToList() ?? new();
        }

        public async Task<List<RoomDTO>> GetRoomsAsync()
        {
            try
            {
                var url = $"{LinkHost.Url}/Public/GetRooms";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<RoomDTO>>(json);

                return rooms ?? new List<RoomDTO>();
            }
            catch
            {
                return new List<RoomDTO>();
            }
        }
        public async Task<List<RoomDTO>> GetRoomsByCinemaIdAsync(int cinemaId)
        {
            var client = _httpClientFactory.CreateClient();

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync(LinkHost.Url + $"/Admin/GetRoomsByCinemaId/{cinemaId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<RoomDTO>>(json);
                return rooms ?? new List<RoomDTO>();
            }

            return new List<RoomDTO>();
        }

        public async Task<bool> CreateScheduleAsync(ShowtimeDTO showtimeDTO)
        {
            var client = CreateAuthorizedClient();

            var json = JsonConvert.SerializeObject(showtimeDTO, Formatting.Indented);
            Console.WriteLine("[DEBUG - ShowtimeDTO JSON]");
            Console.WriteLine(json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{LinkHost.Url}/Schedule/CreateSchedule", content);
            var respContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[DEBUG] STATUS: {response.StatusCode}, BODY: {respContent}");

            return response.IsSuccessStatusCode;
        }

        public async Task<List<ShowtimeTimelineDTO>> GetTimelineAsync(string ngay, int maRap)
        {
            if (string.IsNullOrWhiteSpace(ngay) || maRap <= 0)
                return new List<ShowtimeTimelineDTO>();

            try
            {
                var url = $"{LinkHost.Url}/Schedule/GetTimeline?ngay={ngay}&maRap={maRap}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ShowtimeTimelineDTO>>(json);

                return list ?? new List<ShowtimeTimelineDTO>();
            }
            catch
            {
                return new List<ShowtimeTimelineDTO>();
            }
        }
    }
}
