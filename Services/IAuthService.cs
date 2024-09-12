using InstaHub.Dto;

namespace InstaHub.Services
{
    public interface IAuthService
    {
        Task<string> Login(LoginDto loginDto);
    }
}
