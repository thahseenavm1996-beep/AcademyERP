using AcademyERP.Application.DTOs.Auth;

namespace AcademyERP.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}