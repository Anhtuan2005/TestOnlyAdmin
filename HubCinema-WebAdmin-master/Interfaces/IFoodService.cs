using HubCinemaAdmin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HubCinemaAdmin.Services
{
    public interface IFoodService
    {
        Task<List<CinemaDTO>> GetCinemasAsync();
        Task<FoodDTO?> CreateFoodAsync(FoodDTO dto);
        Task<bool> CreateComboForCinemasAsync(int foodId, List<int> cinemaIds);
        Task<List<FoodDTO>> GetFoodsByCinemaAsync(int cinemaId);
        Task<List<FoodDTO>> GetAllFoodsAsync();
        Task<bool> UpdateFoodAsync(FoodDTO foodDTO);
    }
}