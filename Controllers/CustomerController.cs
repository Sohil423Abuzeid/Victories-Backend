using InstaHub.Dto;
using InstaHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InstaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(ICustomerService _customerService, ILogger<CustomerController> _logger) : ControllerBase
    {
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer(int customerId)
        {
            if (customerId <= 0)
            {
                return BadRequest("Invalid customer ID.");
            }

            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(customerId);

                if (customer == null)
                {
                    return NotFound("Customer not found.");
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer details.");

                return StatusCode(500, "An error occurred while retrieving the customer details.");
            }
        }
        
    }
}
