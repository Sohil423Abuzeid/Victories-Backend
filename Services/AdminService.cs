using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static System.Net.WebRequestMethods;

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
                HashPassword = hashedPassword,
                RegisterationDate = DateTime.Now
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
        public async Task<int> GetAdminByNumberAndIdAsync(string number , string email)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.PhoneNumber == number && a.Email==email);
            if (admin == null)
            {
                throw new Exception($"Admin with Id {admin.Id} not found.");
            }
            return admin.Id;
        }
        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            var admins = await _context.Admins.ToListAsync();
            return admins;
        }

        public async Task<bool> UpdatePasswordAsync(int adminId, UpdatePasswordDto dto)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(a => a.Id == adminId);
            if (admin == null)
            {
                throw new InvalidOperationException("Owner not found.");
            }

            // Check if the old password matches the stored hashed password
            if (!_authService.VerifyPassword(admin, dto.OldPassword))
            {
                return false; // Old password is incorrect
            }

            var hashedNewPassword = _authService.HashPassword(dto.NewPassword);
            admin.HashPassword = hashedNewPassword;

            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();

            return true; // Password updated successfully
        }
        public async Task<bool> DeletePhotoAsync(int adminId)
        {
            var admin = await GetAdminByIdAsync(adminId);
            if (admin == null)
            {
                throw new InvalidOperationException("Owner not found.");
            }

            admin.PhotoUrl = "https://ibb.co/nm7PSgC";

            _context.Admins.Update(admin);
            
            await _context.SaveChangesAsync();

            return true; // photo deleted successfully
        }
        public async Task<bool> UpdatePhotoAsync(int adminId,string url)
        {
            var admin = await GetAdminByIdAsync(adminId);
            if (admin == null)
            {
                throw new InvalidOperationException("Owner not found.");
            }

            admin.PhotoUrl = url;

            _context.Admins.Update(admin);

            await _context.SaveChangesAsync();

            return true; // photo updated successfully
        }
       
    }
}
