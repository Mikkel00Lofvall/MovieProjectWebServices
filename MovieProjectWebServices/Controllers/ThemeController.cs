using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IRepository<GenreModel> repo;

        public ThemeController(IRepository<GenreModel> repo)
        {
            this.repo = repo;
        }


        [HttpGet("GetThemes")]
        public async Task<IActionResult> Get()
        {
            var themes = await repo.GetAll();

            return Ok(themes);
        }

        [HttpPost("GetThemeWithId")]
        public async Task<IActionResult> Get([FromBody] GenreModel genre)
        {
            (bool result, string message, var theme) = await repo.GetWithId(genre.id);
            if (result) return Ok(theme);

            else return BadRequest(message);


        }

        [HttpPost("CreateTheme")]
        public async Task<IActionResult> Create([FromBody] GenreModel genre)
        {
            if (genre != null)
            {
                (bool result, string message) = await repo.Create(genre);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();

        }

        [HttpDelete("DeleteTheme")]
        public async Task<IActionResult> Delete([FromBody] GenreModel genre)
        {
            (bool result, string message, var theme) = await repo.GetWithId(genre.id);
            if (result)
            {
                if (theme != null)
                {
                    await repo.Delete(theme);
                    return Ok();
                }

                else return BadRequest("Movie does not exist in db");
            }

            else return BadRequest(message);

        }
    }
}
