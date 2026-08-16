using AcademyERP.Application.DTOs.Lookups;

namespace AcademyERP.Application.Services;

public interface IClassDurationService
{
    Task<List<ClassDurationResponse>> GetAllAsync();
}