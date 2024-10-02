using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.DTO;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemeController : ControllerBase
    {
        private readonly ThemeRepository repo;

        public ThemeController(ThemeRepository repo)
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

            else return Problem(message);


        }

        [HttpGet("GetThemesWithMovieID/{movieID}")]
        public async Task<IActionResult> GetThemesWithMovieID(int movieID)
        {
            (bool result, string message, var Themes) = await repo.GetThemesWithMovieID(movieID);
            if (result) return Ok(Themes);
            else return Problem(message);
        }

        [HttpPost("UpdateMovieWithThemes/{movieID}")]
        public async Task<IActionResult> UpdateMovieWithThemes(int movieID, [FromBody] UpdateThemeDTO DTO)
        {
            if (DTO == null) return BadRequest("No UpdateThemeDTO");

            (bool result, string message) = await repo.UpdateMovieWithThemes(movieID, DTO);
            if (result) return Ok(result);
            else return Problem(message);
        }

        [HttpPost("CreateTheme")]
        public async Task<IActionResult> Create([FromBody] ThemeDTO themeInput)
        {
            if (themeInput != null)
            {
                ThemeModel newTheme = new ThemeModel(); 
                newTheme.Name = themeInput.Name;

                (bool result, string message) = await repo.Create(newTheme);
                if (result) return Ok();

                else return Problem(message);
            }

            else return NoContent();

        }

        [HttpDelete("DeleteTheme/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            (bool result, string message, var theme) = await repo.GetWithId(id);
            if (result)
            {
                if (theme != null)
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
