namespace AcademyERP.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Role { get; }
}