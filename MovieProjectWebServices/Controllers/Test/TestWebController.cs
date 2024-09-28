using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;
using MoviesDatabase.Models.Test;
using Microsoft.AspNetCore.Authorization;
using System.Data;

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
        public async Task<IActionResult> Create() 
        {
            return Ok();
        }
    }
}