using Application.DTOs.Auth;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AuthController(
            IAuthService authService,
            AppDbContext context,
            IEmailService emailService)
        {
            _authService = authService;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            if (result == "Email already exists")
                return BadRequest(new
                {
                    success = false,
                    message = result
                });

            return Ok(new
            {
                success = true,
                message = result
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.LoginAsync(request);

            if (user == null)
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid email or password"
                });

            var code = new Random().Next(100000, 999999).ToString();

            user.TwoFactorCode = code;
            user.TwoFactorCodeExpiresAt = DateTime.Now.AddMinutes(5);

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Smart Home Verification Code",
                $@"
                <h2>Smart Home System</h2>
                <p>Your verification code is:</p>
                <h1>{code}</h1>
                <p>This code will expire in 5 minutes.</p>
                "
            );

            return Ok(new
            {
                success = true,
                message = "Verification code sent to your email"
            });
        }

        [HttpPost("verify-login-code")]
        public async Task<IActionResult> VerifyLoginCode(VerifyLoginCodeDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid verification code"
                });

            if (user.TwoFactorCode != request.Code ||
                user.TwoFactorCodeExpiresAt == null ||
                user.TwoFactorCodeExpiresAt < DateTime.Now)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid or expired verification code"
                });
            }

            user.TwoFactorCode = null;
            user.TwoFactorCodeExpiresAt = null;

            await _context.SaveChangesAsync();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_MY_SECRET_KEY_FOR_SMART_HOME_SYSTEM_2026")
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                success = true,
                message = "Login successful",
                token = tokenString,
                user = new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.Role
                }
            });
        }
    }
}