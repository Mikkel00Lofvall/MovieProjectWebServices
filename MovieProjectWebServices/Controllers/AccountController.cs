using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MoviesDatabase.DTO;
using MoviesDatabase.Interfaces;
using MoviesDatabase.Models;
using MoviesDatabase.Repos;
using System.Security.Claims;

namespace MovieProjectWebServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AdminUserRepository _repo;
        public AccountController(AdminUserRepository repo) { this._repo = repo; }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] LoginDTO DTO)
        {
            if (DTO != null)
            {
                AdminUserModel adminUser = new AdminUserModel() 
                { 
                    Username = DTO.Username,
                    Password = DTO.Password,
                };

                (bool result, string message) = await _repo.Create(adminUser);
                if (result) return Ok();

                else return BadRequest(message);
            }

            else return BadRequest();

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {

            (bool isAdmin, string message) = await _repo.GetAdminUser(loginDTO.Username, loginDTO.Password);

            if (isAdmin) 
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "example")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {

                };

                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new { success = true });
            }
            
            return Ok();

        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true });
        }
    }

}
