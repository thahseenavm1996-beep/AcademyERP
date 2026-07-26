using AcademyERP.Application.Interfaces;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Admissions;
using AcademyERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class AdmissionConversionService : IAdmissionConversionService
{
    private readonly IRepository<AdmissionApplication> _repository;

    public AdmissionConversionService(
        IRepository<AdmissionApplication> repository)
    {
        _repository = repository;
    }

    public async Task ConvertToStudentAsync(Guid admissionId)
    {
        var admission = await _repository
            .Query()
            .Include(x => x.Program)
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == admissionId);

        if (admission == null)
            throw new Exception("Admission application not found.");

        if (admission.Status != AdmissionStatus.Approved)
            throw new Exception("Only approved applications can be converted.");

        if (admission.IsConverted)
            throw new Exception("This application has already been converted.");

        // Student creation will be added here in the next step.
    }
}