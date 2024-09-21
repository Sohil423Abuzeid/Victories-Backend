using InstaHub.Dto;
using InstaHub.Models;
using Microsoft.AspNetCore.Authorization;

namespace InstaHub.Services
{
    public interface IOwnerService
    {
        Task<Admin> GetOwnerAsync();
        Task<Admin> UpdateOwnerInformationAsync(UpdateOwnerDto updateOwnerDto);
    }
}