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


        [HttpPost("CheckAuth")]
        public async Task<IActionResult> CheckAuth()
        {
            var cookieValue = Request.Cookies["MovieProjectCookeAuth-00594907-acbc-4cb0-bcc4-3e918f709f35"];
            if (cookieValue != null)
            {
                return Ok("Cookie exists: " + cookieValue);
            }

            return NoContent();
        }


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

                else return Problem(message);
            }

            else return NoContent();

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            (bool isAdmin, string message) = await _repo.GetAdminUser(loginDTO.Username, loginDTO.Password);

            if (isAdmin)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginDTO.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                // Sign in the user
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new { success = true, message = "Login successful." });
            }

            return Unauthorized(new { success = false, message = message });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (Request.Cookies["MovieProjectCookeAuth-00594907-acbc-4cb0-bcc4-3e918f709f35"] != null)
            {
                Response.Cookies.Append("MovieProjectCookeAuth-00594907-acbc-4cb0-bcc4-3e918f709f35", "", new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Path = "/",
                    HttpOnly = true,
                    Secure = true
                });
            }

            return Ok(new { message = "Logged out" });
        }
    }

}
