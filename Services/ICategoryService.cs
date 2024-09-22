using InstaHub.Models;

namespace InstaHub.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
    }
}
