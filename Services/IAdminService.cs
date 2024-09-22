using InstaHub.Dto;
using InstaHub.Models;

namespace InstaHub.Services
{
    public interface IAdminService
    {
        Task<Admin> GetAdminByIdAsync(int adminId);
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<Admin> AddAdminAsync(AdminDto adminDto);
        Task<bool> DeleteAdminByIdAsync(int adminId);
        Task<bool> UpdatePasswordAsync(int adminId, UpdatePasswordDto updatePasswordDto);

    }
}
