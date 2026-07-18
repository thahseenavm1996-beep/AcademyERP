using AcademyERP.Application.DTOs.Programs;

namespace AcademyERP.Application.Services;

public interface IProgramService
{
    Task<List<ProgramResponse>> GetAllAsync();

    Task<ProgramResponse?> GetByIdAsync(Guid id);

    Task<ProgramResponse> CreateAsync(CreateProgramRequest request);

    Task<ProgramResponse?> UpdateAsync(Guid id, UpdateProgramRequest request);

    Task<bool> DeleteAsync(Guid id);
}