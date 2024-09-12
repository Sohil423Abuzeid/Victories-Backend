using InstaHub.Dto;
using InstaHub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace InstaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAdminService _adminService, IAuthService _authService, ILogger<AdminController> _logger) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || !ModelState.IsValid)
                return BadRequest("Invalid login request.");

            try
            {
                var token = await _authService.Login(loginDto);
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

        [HttpGet]
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
        public async Task<IActionResult> PostAdmin([FromBody] AdminDto adminDto)
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
    }
}
