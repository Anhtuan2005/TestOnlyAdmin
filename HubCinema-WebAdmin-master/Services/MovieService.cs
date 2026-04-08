using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HubCinemaAdmin.Services
{
    public class MovieService : IMovieService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;

        public MovieService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
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

        public async Task<List<MovieDTO>> GetAllMoviesAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetMovies");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Không thể lấy danh sách phim.");

            var json = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<MovieDTO>>(json);
            return movies ?? new List<MovieDTO>();
        }

        public async Task<bool> CreateMovieAsync(MovieDTO movie)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Admin/CreateMovie", movie);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateMovieAsync(MovieDTO movie)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PutAsJsonAsync(LinkHost.Url + $"/Admin/UpdateMovie/{movie.IDMovie}", movie);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteMovieAsync(int movieId)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync(LinkHost.Url + $"/Admin/DeleteMovie/{movieId}");
            return response.IsSuccessStatusCode;
        }
    }
}