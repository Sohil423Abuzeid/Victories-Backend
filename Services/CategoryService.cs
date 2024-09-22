using InstaHub.Models;
using Microsoft.EntityFrameworkCore;
namespace InstaHub.Services
{
    public class CategoryService(AppDbContext _context) : ICategoryService
    {
        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        
    }
}
