using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services.Authentication;
using Microsoft.EntityFrameworkCore;

namespace InstaHub.Services
{
    public class OwnerService(AppDbContext _context, IAuthService _authService) : IOwnerService
    {
        
        public async Task<Admin> GetOwnerAsync()
        {
            // the owner is only one, and will be by default the one we will insert in our db first 
            var owner = await _context.Admins.FirstOrDefaultAsync(a => a.Id == 1);
            return owner!;
        }

        public async Task<Admin> UpdateOwnerInformationAsync(UpdateOwnerDto updateOwnerDto)
        {
            var owner = await GetOwnerAsync();

            // Update owner information
            owner.FirstName = updateOwnerDto.FirstName;
            owner.LastName = updateOwnerDto.LastName;
            owner.Email = updateOwnerDto.Email;

            _context.Update(owner);
            await _context.SaveChangesAsync();
            return owner;
        }

       

    }
}