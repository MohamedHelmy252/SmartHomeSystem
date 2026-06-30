using Application.DTOs.Auth;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogService _logService;

        public AuthController(
            IAuthService authService,
            AppDbContext context,
            IEmailService emailService,
             ILogService logService)
        {
            _authService = authService;
            _context = context;
            _emailService = emailService;
            _logService = logService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _authService.RegisterAsync(request);

            if (result == "Email already exists")
            {
                await _logService.LogAsync(
                    eventType: "UserCreateFailed",
                    severity: "Warning",
                    riskScore: 5,
                    description: $"Admin tried to create user but email already exists: {request.Email}",
                    actorRole: "Admin",
                    userId: adminId,
                    entityName: "User",
                    statusCode: 400
                );

                return BadRequest(new { success = false, message = result });
            }

            var createdUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            await _logService.LogAsync(
                eventType: "UserCreated",
                severity: "Information",
                riskScore: 2,
                description: $"Admin created new user: {request.Email}",
                actorRole: "Admin",
                userId: adminId,
                entityName: "User",
                entityId: createdUser?.UserId,
                statusCode: 200
            );

            return Ok(new { success = true, message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _authService.LoginAsync(request);

            if (user == null)
            {
                await _logService.LogAsync(
                    eventType: "FailedLogin",
                    severity: "Warning",
                    riskScore: 7,
                    description: $"Failed login attempt for email: {request.Email}",
                    actorRole: "Unknown",
                    entityName: "Auth",
                    statusCode: 401
                );

                var fromTime = DateTime.Now.AddMinutes(-5);

                var lastFailedLogin = await _context.Logs
                    .Where(l => l.EventType == "FailedLogin")
                    .OrderByDescending(l => l.CreatedAt)
                    .FirstOrDefaultAsync();

                var ipAddress = lastFailedLogin?.IpAddress;

                var failedCount = await _context.Logs
                    .CountAsync(l =>
                        l.EventType == "FailedLogin" &&
                        l.IpAddress == ipAddress &&
                        l.CreatedAt >= fromTime);

                var alreadyDetected = await _context.Logs
                    .AnyAsync(l =>
                        l.EventType == "RepeatedFailedLoginFromSameIP" &&
                        l.IpAddress == ipAddress &&
                        l.CreatedAt >= fromTime);

                if (failedCount >= 5 && !alreadyDetected)
                {
                    await _logService.LogAsync(
                        eventType: "RepeatedFailedLoginFromSameIP",
                        severity: "Critical",
                        riskScore: 25,
                        description: $"Possible brute force detected. IP: {ipAddress}, FailedAttempts: {failedCount} within last 5 minutes.",
                        actorRole: "Unknown",
                        entityName: "Auth",
                        statusCode: 401
                    );
                }

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid email or password"
                });
            }

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

            await _logService.LogAsync(
                eventType: "LoginCodeSent",
                severity: "Information",
                riskScore: 2,
                description: $"2FA verification code sent to user: {user.Email}",
                actorRole: user.Role,
                userId: user.UserId,
                entityName: "Auth",
                statusCode: 200
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
            {
                await _logService.LogAsync(
                    eventType: "FailedVerifyLoginCode",
                    severity: "Warning",
                    riskScore: 8,
                    description: $"Verification failed. Email not found: {request.Email}",
                    actorRole: "Unknown",
                    entityName: "Auth",
                    statusCode: 401
                );

                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid verification code"
                });
            }

            if (user.TwoFactorCode != request.Code ||
                user.TwoFactorCodeExpiresAt == null ||
                user.TwoFactorCodeExpiresAt < DateTime.Now)
            {
                await _logService.LogAsync(
                    eventType: "FailedVerifyLoginCode",
                    severity: "Warning",
                    riskScore: 8,
                    description: $"Invalid or expired verification code for user: {user.Email}",
                    actorRole: user.Role,
                    userId: user.UserId,
                    entityName: "Auth",
                    statusCode: 401
                );

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
                expires: DateTime.Now.AddHours(30),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            await _logService.LogAsync(
                eventType: "SuccessfulLogin",
                severity: "Information",
                riskScore: 1,
                description: $"User logged in successfully: {user.Email}",
                actorRole: user.Role,
                userId: user.UserId,
                entityName: "Auth",
                statusCode: 200
            );

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