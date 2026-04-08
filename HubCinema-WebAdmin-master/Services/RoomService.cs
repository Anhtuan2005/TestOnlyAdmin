using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HubCinemaAdmin.Services
{
    public class RoomService : IRoomService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoomService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
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
        public async Task<bool> CreateRoomAsync(RoomDTO roomDTO)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Admin/CreateRoom", roomDTO);
            var json = System.Text.Json.JsonSerializer.Serialize(roomDTO);
            Console.WriteLine("ROOM DTO JSON SENT: " + json);
            return response.IsSuccessStatusCode;
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
    }
}
