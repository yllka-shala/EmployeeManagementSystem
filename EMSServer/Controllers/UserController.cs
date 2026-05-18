using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using EMSServer.Helpers;
using EMSServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMSServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserRepository userRepository) : ControllerBase
    {
        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUsersAsync()
        {
            var result = await userRepository.GetUsers();
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(User user)
        {
            if (user is null)
                return BadRequest("Bad Request Made");

            var result = await userRepository.UpdateUser(user);
            return Ok(result);
        }

        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Request Sent!");

            var result = await userRepository.DeleteUser(id);
            return Ok(result);
        }

        [HttpGet("user-image/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserImage(int id)
        {
            var result = await userRepository.GetUserImage(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UserProfile userProfile)
        {
            var result = await userRepository.UpdateProfile(userProfile);
            return Ok(result);
        }

        [HttpDelete("delete-profile-picture/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProfilePicture(int id)
        {
            var result = await userRepository.DeleteProfilePicture(id);
            return Ok(result);
        }
    }
}
