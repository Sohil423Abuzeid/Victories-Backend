using Azure.Core;
using Azure;
using InstaHub.Dto;
using InstaHub.Models;
using InstaHub.Services.ChannelsServices.WhatsAppServiceInfra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InstaHub.Services.Authentication
{
    public interface IAuthService
    {
        string HashPassword(string password);
        Task<string> Login(LoginDto loginDto);
    }

    public class AuthService(AppDbContext _context, IOptions<JwtSettings> _settings): IAuthService
    {
        public string HashPassword(string password)
        {
            using var hmac = new HMACSHA256();
            byte[] salt = hmac.Key;
            var saltedPassword = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            byte[] hash = hmac.ComputeHash(saltedPassword);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        private bool VerifyPassword(Admin user, string password)
        {
            var storedPasswordParts = user.HashPassword.Split('.');
            if (storedPasswordParts.Length != 2)
            {
                return false;
            }

            var salt = Convert.FromBase64String(storedPasswordParts[0]);
            var storedHash = storedPasswordParts[1];

            using var hmac = new HMACSHA256(salt);  // Use the stored salt as the key
            var saltedPassword = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            byte[] computedHash = hmac.ComputeHash(saltedPassword);

            var computedHashBase64 = Convert.ToBase64String(computedHash);
            return computedHashBase64 == storedHash;
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _context.Admins.SingleOrDefaultAsync(u => u.UserName == loginDto.Username);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            if (!verifyPassword(user, loginDto.Password))
                throw new UnauthorizedAccessException();
                
            var token = GenerateToken(user);
          

            return token;
        }
        private string GenerateToken(Admin user)
        {
            // Generate a JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.Value.JwtSecret);
            var role = user.Id == 1 ? "owner" : "admin";
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Id", user.Id.ToString()),
                new Claim("role", role)
            }),
                Expires = DateTime.UtcNow.AddMinutes(_settings.Value.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
