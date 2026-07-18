using AcademyERP.Application.DTOs.Auth;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace AcademyERP.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return null;

        var validPassword =
            await _userManager.CheckPasswordAsync(user, request.Password);

        if (!validPassword)
            return null;

        if (!user.IsActive)
            return null;

        var token =
            await _jwtTokenService.GenerateTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return new LoginResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? string.Empty
        };
        /*  return new LoginResponse
          {
              Token = token,
              Expiration = DateTime.UtcNow.AddMinutes(60)
          };*/
    }
}