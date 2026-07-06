using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Parents;

namespace AcademyERP.Application.Services;

public interface IParentService
{
    Task<ParentResponse> CreateAsync(CreateParentRequest request);

    Task<PagedResponse<ParentResponse>> GetAllAsync(ParentQueryRequest request);

    Task<ParentResponse?> GetByIdAsync(Guid id);

    Task<ParentResponse?> UpdateAsync(Guid id, UpdateParentRequest request);

    Task<bool> DeleteAsync(Guid id);
    Task<bool> ResetPasswordAsync(
    Guid id,
    ResetParentPasswordRequest request);
}