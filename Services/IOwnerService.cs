using InstaHub.Dto;
using InstaHub.Models;

namespace InstaHub.Services
{
    public interface IOwnerService
    {
        Task<Admin> GetOwnerAsync();
        Task<bool> UpdateOwnerInformationAsync(UpdateOwnerDto updateOwnerDto);
        Task<bool> UpdateOwnerPasswordAsync(UpdateOwnerPasswordDto updateOwnerPasswordDto);
    }
}