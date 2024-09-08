using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesDatabase;
using MoviesDatabase.Interfaces;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepo repo;

        public MovieController(IMovieRepo repo)
        {
            this.repo = repo;
        }


        [HttpGet("GetMovies")]
        public async Task<IActionResult> Get() 
        {
            var movies = await repo.GetAllMovies();
            
            return Ok(movies);
        }
    }
}
