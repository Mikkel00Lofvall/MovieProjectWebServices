using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly IRepository<ThemeModel> repo;

        public ThemeController(IRepository<ThemeModel> repo)
        {
            this.repo = repo;
        }


        [HttpGet("GetThemes")]
        public async Task<IActionResult> Get()
        {
            var themes = await repo.GetAll();

            return Ok(themes);
        }

        [HttpGet("GetThemeWithId/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            (bool result, string message, var theme) = await repo.GetWithId(id);
            if (result) return Ok(theme);

            else return BadRequest(message);


        }

        [HttpPost("CreateTheme")]
        public async Task<IActionResult> Create([FromBody] ThemeModel genre)
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
        public async Task<IActionResult> Delete([FromBody] ThemeModel genre)
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
