using InstaHub.Models;

namespace InstaHub.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<dynamic>> GetAllCoustmers();

    }
}