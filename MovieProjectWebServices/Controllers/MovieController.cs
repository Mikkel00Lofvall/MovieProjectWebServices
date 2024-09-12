using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesDatabase;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IRepository<MovieModel> repo;

        public MovieController(IRepository<MovieModel> repo)
        {
            this.repo = repo;
        }


        [HttpGet("GetMovies")]
        public async Task<IActionResult> Get() 
        {
            var movies = await repo.GetAll();
            
            return Ok(movies);
        }

        [HttpPost("GetMovieWithId")]
        public async Task<IActionResult> Get([FromBody] MovieModel movie) 
        {
            (bool result, string message, var resultMovie) = await repo.GetWithId(movie.id);
            if (result) return Ok(resultMovie);

            else return BadRequest(message);

        }

        [HttpPost("CreateMovie")]
        public async Task<IActionResult> Create([FromBody] MovieModel movie)
        {
            if (movie != null) 
            {
                (bool result, string message) = await repo.Create(movie);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();

        }

        [HttpDelete("DeleteMovie")]
        public async Task<IActionResult> Delete([FromBody] GenreModel genre)
        {
            (bool result, string message, var resultMovie) = await repo.GetWithId(genre.id);
            if (result)
            {
                if (resultMovie != null)
                {
                    await repo.Delete(resultMovie);
                    return Ok();
                }

                else return BadRequest("Movie does not exist in db");
            }

            else return BadRequest(message);

        }
    }
}
