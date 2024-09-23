using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;
using MoviesDatabase.Models.Test;

namespace MovieProjectWebServices.Controllers.Test
{
    [Route("api/[controller]")]
    [ApiController]

    public class TestWebController : ControllerBase
    {
        private readonly IRepository<TestModel> repo;

        public TestWebController(IRepository<TestModel> repo)
        {
            this.repo = repo;
        }

        [HttpGet("GetTests")]
        public async Task<IActionResult> Get() 
        {
            var tests = await repo.GetAll();
            if (tests.Count() > 0) {
                return Ok(tests);
            }
            else return BadRequest();

        }

        [HttpPost("CreateTest")]
        public async Task<IActionResult> Create([FromBody] TestModel input) 
        {
            if (input != null) 
            {
                (bool result, string message) = await repo.Create(input);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();
        }
    }
}