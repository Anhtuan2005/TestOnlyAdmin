using HubCinemaAdmin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HubCinemaAdmin.Services
{
    public interface IMovieService
    {
        Task<List<MovieDTO>> GetAllMoviesAsync()
;        Task<bool> CreateMovieAsync(MovieDTO movie);
        Task<bool> UpdateMovieAsync(MovieDTO movie);
        Task<bool> DeleteMovieAsync(int movieId);
    }
}
