using InstaHub.Models;
using InstaHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace InstaHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(CategoryService _categoryService) : ControllerBase
    { 
        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetCategories();
            return Ok(categories);
        }
    }
}
