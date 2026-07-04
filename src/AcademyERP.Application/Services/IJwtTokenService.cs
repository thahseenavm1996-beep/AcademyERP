using AcademyERP.Domain.Entities.Identity;

namespace AcademyERP.Application.Services;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}