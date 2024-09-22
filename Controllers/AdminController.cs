using Azure;
using InstaHub.Dto;
using InstaHub.Services;
using InstaHub.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Threading.Tasks;

namespace InstaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminService _adminService, IAuthService _authService, IOptions<JwtSettings> _jwt,ILogger<AdminController> _logger) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || !ModelState.IsValid)
                return BadRequest("Invalid login request.");

            try
            {
                var token = await _authService.Login(loginDto);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false, // Set to false if you want to access via JavaScript
                    Secure = false,    // Only use this if you're on HTTPS, otherwise set to false for localhost
                    SameSite = SameSiteMode.None, // None if you need cross-origin requests
                    Expires = DateTime.Now.AddDays(7), // Set cookie expiration
                    IsEssential = true, // Ensures the cookie isn't affected by consent policy
                };

                Response.Cookies.Append("access_token", token, cookieOptions);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized login attempt for user: {Username}", loginDto.Username);
                return Unauthorized("Invalid username or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            try
            {
                var admins = await _adminService.GetAllAdminsAsync();
                if (admins == null || !admins.Any())
                    return Ok(new List<AdminDto>());

                return Ok(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching admins.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminDto adminDto)
        {
            if (adminDto == null || !ModelState.IsValid)
                return BadRequest("Invalid admin data.");

            try
            {
                var createdAdmin = await _adminService.AddAdminAsync(adminDto);
                return CreatedAtAction(nameof(GetAdminById), new { adminId = createdAdmin.Id }, createdAdmin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating admin.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{adminId}")]
        public async Task<IActionResult> GetAdminById(int adminId)
        {
            try
            {
                var admin = await _adminService.GetAdminByIdAsync(adminId);
                if (admin == null)
                    return NotFound($"Admin with ID {adminId} not found.");

                return Ok(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching admin by ID.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{adminId}")]
        public async Task<IActionResult> DeleteAdminById(int adminId)
        {
            try
            {
                var adminExists = await _adminService.GetAdminByIdAsync(adminId);
                if (adminExists == null)
                    return NotFound($"Admin with ID {adminId} not found.");

                await _adminService.DeleteAdminByIdAsync(adminId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting admin by ID.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{adminId}/password")]
        public async Task<IActionResult> UpdatePassword(int adminId, [FromBody] UpdatePasswordDto updatePasswordDto)
        {
            if (updatePasswordDto == null)
            {
                return BadRequest("Password update details cannot be null.");
            }

            if (string.IsNullOrEmpty(updatePasswordDto.NewPassword) || string.IsNullOrEmpty(updatePasswordDto.OldPassword))
            {
                return BadRequest("Old and new passwords must be provided.");
            }

            try
            {
                var result = await _adminService.UpdatePasswordAsync(adminId, updatePasswordDto);

                if (!result)
                {
                    return BadRequest("Password update failed. Please check your old password and try again.");
                }

                return Ok(new { success = true, message = "Admin password updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin password.");
                return StatusCode(500, "An error occurred while updating the admin password.");
            }
        }
        [HttpPost("delete-photo")]
        public async Task<IActionResult> DeletePhoto(int AdminId)
        {
            try
            {
                await _adminService.DeletePhotoAsync(AdminId);
                return StatusCode(200, new { message = "photo deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting photo.");
                return StatusCode(500, new { message = "An error occurred while deleting the photo." });
            }
        }

        [HttpPost("update-photo")]
        public async Task<IActionResult> UpdatePhoto(int AdminId,string url)
        {
            try
            {
                await _adminService.UpdatePhotoAsync(AdminId,url);
                return StatusCode(200, new { message = "photo deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting photo.");
                return StatusCode(500, new { message = "An error occurred while deleting the photo." });
            }
        }
        // ## email & number => return 
        //// Placeholder for the forget password functionality
        //[HttpPost("{adminId}/forget-password")]
        //public IActionResult ForgetPassword(int adminId)
        //{
        //    return StatusCode(501, "This functionality is not yet implemented.");
        //}
        // 1. forget password => Return ID => validate OTP => reset password 



        /// GET Ticket by admin Id

    }
}