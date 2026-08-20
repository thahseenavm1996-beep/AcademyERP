using System.Security.Claims;
using AcademyERP.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AcademyERP.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public Guid? UserId
    {
        get
        {
            var id =
                _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return Guid.TryParse(id, out var guid)
                ? guid
                : null;
        }
    }


    public string? Role
    {
        get
        {
            return _httpContextAccessor
                .HttpContext?
                .User?
                .FindFirstValue(
                    ClaimTypes.Role);
        }
    }
}