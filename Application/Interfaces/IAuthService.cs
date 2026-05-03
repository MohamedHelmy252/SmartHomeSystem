using Application.DTOs.Auth;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDTO request);
        Task<User?> LoginAsync(LoginRequestDTO request);
    }
}
