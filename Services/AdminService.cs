using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services.Authentication;
using Microsoft.EntityFrameworkCore;

namespace InstaHub.Services
{
    public class AdminService(IAuthService _authService, AppDbContext _context) : IAdminService
    {
        public async Task<Admin> AddAdminAsync(AdminDto adminDto)
        {
            var hashedPassword = _authService.HashPassword(adminDto.Password);
            var admin = new Admin
            {
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,
                UserName = adminDto.UserName,
                PhoneNumber = adminDto.PhoneNumber,
                Email = adminDto.Email,
                HashPassword = hashedPassword
            };

            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<bool> DeleteAdminByIdAsync(int adminId)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin == null)
                return false;

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Admin> GetAdminByIdAsync(int adminId)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin == null)
            {
                throw new Exception($"Admin with Id {adminId} not found.");
            }
            return admin;
        }

        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            var admins = await _context.Admins.ToListAsync();
            return admins;
        }

        public async Task<bool> UpdatePasswordAsync(int adminId, UpdatePasswordDto dto)
        {
            var owner = await _context.Admins.SingleOrDefaultAsync(a => a.Id == adminId);
            if (owner == null)
            {
                throw new InvalidOperationException("Owner not found.");
            }

            // Check if the old password matches the stored hashed password
            var hashedOldPassword = _authService.HashPassword(dto.OldPassword);
            if (owner.HashPassword != hashedOldPassword)
            {
                return false; // Old password is incorrect
            }

            var hashedNewPassword = _authService.HashPassword(dto.NewPassword);
            owner.HashPassword = hashedNewPassword;

            _context.Admins.Update(owner);
            await _context.SaveChangesAsync();

            return true; // Password updated successfully
        }
    }
}
