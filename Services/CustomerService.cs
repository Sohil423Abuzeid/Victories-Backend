using InstaHub.Models;

namespace InstaHub.Services
{
    public class CustomerService(AppDbContext _context) : ICustomerService
    {
        public async Task<IEnumerable<dynamic>>  GetAllCoustmers()
        {
            var customer =  _context.Customers.ToList();
            var customerjson = customer.Select((customer, index) => new
            {
                Id = index + 1,
                Number = customer.CustomerId,
                Date = customer.FirstTicketDate.ToString()
            }).ToList();
            return customerjson;
        }

        public Task<Customer>  GetCustomerByIdAsync(int customerId)
        {
            throw new NotImplementedException();

        }
    }
}
