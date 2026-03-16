using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TripManagerWebApi.Dtos;
using TripManagerData.Models;
using TripManagerWebApi.Security;

namespace TripManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TripManagerDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(TripManagerDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            if (_context.Users.Any(u => u.Username == registerDto.Username))
                return BadRequest("Username already exists.");

            var salt = PasswordHashProvider.GetSalt();
            var hash = PasswordHashProvider.GetHash(registerDto.Password, salt);

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                Password = hash,
                Salt = salt,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Role = "User",
                CreatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Registration successful.");
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginDto.Username);
            if (user == null)
                return BadRequest("No user found.");

            var hash = PasswordHashProvider.GetHash(loginDto.Password, user.Salt);

            if (user.Password != hash)
                return BadRequest("Invalid password.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var keyString = _config["JWT:SecureKey"];

            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT:SecureKey is not set.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }


        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto changePassDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == changePassDto.Username);
            if (user == null)
                return BadRequest("User not found.");

            var oldHash = PasswordHashProvider.GetHash(changePassDto.OldPassword, user.Salt);
            if (user.Password != oldHash)
                return BadRequest("Password is incorrect.");

            var newHash = PasswordHashProvider.GetHash(changePassDto.NewPassword, user.Salt);
            user.Password = newHash;

            _context.SaveChanges();

            return Ok("Password changed successfully.");
        }

        [HttpDelete("DeleteAccount")]
        public IActionResult DeleteAccount([FromBody] DeleteAccountDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null)
                return BadRequest("User not found.");

            var hash = PasswordHashProvider.GetHash(dto.Password, user.Salt);

            if (user.Password != hash)
                return BadRequest("Invalid password.");

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok("Account deleted.");
        }
    }
}
