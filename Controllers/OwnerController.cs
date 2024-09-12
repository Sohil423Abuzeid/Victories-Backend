using InstaHub.Dto;
using InstaHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InstaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController(IOwnerService _ownerService, ILogger<OwnerController> _logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetOwner()
        {
            try
            {
                var owner = await _ownerService.GetOwnerAsync();

                if (owner == null)
                {
                    return NotFound("Owner not found.");
                }

                return Ok(owner);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving owner details.");

                return StatusCode(500, "An error occurred while retrieving the owner details.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOwnerInformation([FromBody] UpdateOwnerDto updateOwnerDto)
        {
            if (updateOwnerDto == null)
            {
                return BadRequest("Owner information cannot be null.");
            }

            try
            {
                // Assuming you have a service to handle updating owner details
                var result = await _ownerService.UpdateOwnerInformationAsync(updateOwnerDto);

                if (!result)
                {
                    return NotFound("Owner not found.");
                }

                return Ok(new { success = true, message = "Owner information updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error updating owner information.");

                return StatusCode(500, "An error occurred while updating the owner information.");
            }
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdateOwnerPassword([FromBody] UpdateOwnerPasswordDto updateOwnerPasswordDto)
        {
            if (updateOwnerPasswordDto == null)
            {
                return BadRequest("Password update details cannot be null.");
            }

            if (string.IsNullOrEmpty(updateOwnerPasswordDto.NewPassword) || string.IsNullOrEmpty(updateOwnerPasswordDto.OldPassword))
            {
                return BadRequest("Old and new passwords must be provided.");
            }

            try
            {
                // Assuming you have a service to handle password updates
                var result = await _ownerService.UpdateOwnerPasswordAsync(updateOwnerPasswordDto);

                if (!result)
                {
                    return BadRequest("Password update failed. Please check your old password and try again.");
                }

                return Ok(new { success = true, message = "Owner password updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error updating owner password.");

                return StatusCode(500, "An error occurred while updating the owner password.");
            }
        }

    }
}
