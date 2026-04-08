using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace HubCinemaAdmin.Services
{
    public class FoodService : IFoodService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;

        public FoodService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
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
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<List<CinemaDTO>> GetCinemasAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetCinemas");

            if (!response.IsSuccessStatusCode)
                return new List<CinemaDTO>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CinemaDTO>>(json) ?? new List<CinemaDTO>();
        }

        public async Task<FoodDTO?> CreateFoodAsync(FoodDTO dto)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Admin/CreateFood", dto);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FoodDTO>(responseContent);
        }

        public async Task<bool> CreateComboForCinemasAsync(int foodId, List<int> cinemaIds)
        {
            var comboDto = new CreateComboCinema
            {
                IdFood = foodId,
                IdCinemapList = cinemaIds
            };

            var json = JsonConvert.SerializeObject(comboDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = CreateAuthorizedClient();
            var response = await client.PostAsync(LinkHost.Url + "/Admin/CreateComboForCinemas", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<FoodDTO>> GetFoodsByCinemaAsync(int cinemaId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + $"/Public/GetCombosByCinema/{cinemaId}");

            if (!response.IsSuccessStatusCode)
                return new List<FoodDTO>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FoodDTO>>(content) ?? new List<FoodDTO>();
        }

        public async Task<List<FoodDTO>> GetAllFoodsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetFoods");

            if (!response.IsSuccessStatusCode)
                return new List<FoodDTO>();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FoodDTO>>(content) ?? new List<FoodDTO>();
        }
        public async Task<bool> UpdateFoodAsync(FoodDTO foodDTO)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync(LinkHost.Url + $"/Admin/UpdateFood/{foodDTO.IDFood}", foodDTO);
            return response.IsSuccessStatusCode;
        }
    }
}
