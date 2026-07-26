using AcademyERP.Application.DTOs.Admissions;

namespace AcademyERP.Application.Services;

public interface IAdmissionConversionService
{
    Task ConvertToStudentAsync(Guid admissionId);
}