using AcademyERP.Application.DTOs.Admissions;

namespace AcademyERP.Application.Interfaces;

public interface IRegistrationRequestService
{
    Task<List<RegistrationRequestResponse>>
        GetAllAsync();

    Task<RegistrationRequestResponse?>
        GetByIdAsync(Guid id);

    Task<RegistrationRequestResponse>
        CreateAsync(
            CreateRegistrationRequest request);
            
Task<bool> ApproveAsync(
    ApproveRegistrationRequest request);
Task<bool> RejectAsync(
    Guid id,
    string remarks);
}