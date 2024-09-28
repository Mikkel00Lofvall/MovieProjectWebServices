using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesDatabase;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.DTO;
using MoviesDatabase.Repos;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieRepository repo;

        public MovieController(MovieRepository repo)
        {
            this.repo = repo;
        }


        [HttpGet("GetMovies")]
        public async Task<IActionResult> Get() 
        {
            var movies = await repo.GetAll();
            
            return Ok(movies);
        }

        [HttpGet("GetMovieWithId/{id}")]
        public async Task<IActionResult> Get(int id) 
        {
            (bool result, string message, var resultMovie) = await repo.GetWithId(id);
            if (result) return Ok(resultMovie);

            else return BadRequest(message);

        }

        [HttpPost("CreateMovie")]
        public async Task<IActionResult> Create([FromBody] MovieDTO inputMovie)
        {
            if (inputMovie != null) 
            {
                MovieModel newMovie = new MovieModel();
                newMovie.Name = inputMovie.Name;
                newMovie.Description = inputMovie.Description;
                newMovie.FrontPageImage = inputMovie.FrontPageImage;
                newMovie.TrailerLink = inputMovie.TrailerLink;
                newMovie.ImagesBlobs = inputMovie.ImagesBlobs;
                newMovie.Details = inputMovie.Details;

                (bool result, string message) = await repo.Create(newMovie);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();

        }

        [HttpDelete("DeleteMovie")]
        public async Task<IActionResult> Delete([FromBody] MovieModel movie)
        {
            (bool result, string message, var resultMovie) = await repo.GetWithId(movie.id);
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
