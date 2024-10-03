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
            try
            {
                (bool result, string message, ICollection<MovieModel> movies) = await repo.GetAll();
                if (result)
                {
                    return Ok(movies);
                }

                else
                {
                    return Problem("No Users");
                }
            }
            catch (Exception ex) { return Problem(ex.Message); }
        }

        [HttpGet("GetMovieWithId/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            (bool result, string message, var resultMovie) = await repo.GetWithId(id);
            if (result) return Ok(resultMovie);

            else return Problem(message);

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

                else return Problem(message);
            }

            else return NoContent();

        }

        [HttpDelete("DeleteMovie/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            (bool result, string message, var resultMovie) = await repo.GetWithId(id);
            if (result)
            {
                if (resultMovie != null)
                {
                    await repo.Delete(id);
                    return Ok();
                }

                else return Problem("Movie does not exist in db");
            }

            else return Problem(message);

        }
    }
}
