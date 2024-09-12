using InstaHub.Dto;
using InstaHub.Models;

namespace InstaHub.Services
{
    public interface IAdminService
    {
        Task<Admin> GetAdminByIdAsync(int adminId);
        Task<IEnumerable<AdminDto>> GetAllAdminsAsync();
        Task<Admin> AddAdminAsync(AdminDto adminDto);
        Task<bool> DeleteAdminByIdAsync(int adminId);
    }
}
