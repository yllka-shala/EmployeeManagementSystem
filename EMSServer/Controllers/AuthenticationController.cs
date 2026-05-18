using EMSBaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EMSServer.Repositories.Interfaces;
using EMSBaseLibrary.Entities;

namespace EMSServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAccountRepository accRepo) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync(Register user)
        {
            if (user == null)
                return BadRequest("Model is empty!");

            var result = await accRepo.CreateAsync(user);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> SignInAsync(Login user)
        {
            if (user == null)
                return BadRequest("Model is empty!");

            var result = await accRepo.SignInAsync(user);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenAsync(RefreshToken token)
        {
            if (token == null)
                return BadRequest("Model is empty!");

            var result = await accRepo.RefreshTokenAsync(token);
            return Ok(result);
        }
    }
}
