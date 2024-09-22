using InstaHub.Dto;
using InstaHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InstaHub.Services.Authentication
{
    public class AuthService(AppDbContext _context, IConfiguration _configuration) : IAuthService
    {
        public string HashPassword(string password)
        {
            using var hmac = new HMACSHA256();
            byte[] salt = hmac.Key;
            var saltedPassword = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            byte[] hash = hmac.ComputeHash(saltedPassword);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(Admin user, string password)
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
                throw new UnauthorizedAccessException("User not found.");
            }

            if (!VerifyPassword(user, loginDto.Password))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }

            var token = GenerateToken(user);
            return token;
        }

        private string GenerateToken(Admin user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = _configuration["Jwt:JwtSecret"]; 
            var expirationInMinutes = Convert.ToInt32(_configuration["Jwt:ExpirationInMinutes"]); 

            var key = Encoding.ASCII.GetBytes(jwtSecret); // Convert the secret to a byte array

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim("FirstName", user.FirstName),
                     new Claim("LastName", user.LastName),
                     new Claim(ClaimTypes.Name, user.UserName),
                     new Claim(ClaimTypes.Email, user.Email),
                     new Claim("phone_number", user.PhoneNumber),
                     new Claim("Id", user.Id.ToString()),
                     new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
