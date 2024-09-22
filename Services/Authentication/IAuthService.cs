using Azure.Core;
using Azure;
using InstaHub.Dto;
using InstaHub.Services.ChannelsServices.WhatsAppServiceInfra;
using InstaHub.Models;

namespace InstaHub.Services.Authentication
{
    public interface IAuthService
    {
        string HashPassword(string password);
        Task<string> Login(LoginDto loginDto);
        bool VerifyPassword(Admin user, string password);
        string GenerateOTP();
    }
}
