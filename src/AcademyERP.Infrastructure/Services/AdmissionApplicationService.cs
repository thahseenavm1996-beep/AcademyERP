using AcademyERP.Application.DTOs.Admissions;
using AcademyERP.Application.Interfaces;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Admissions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Enums;
namespace AcademyERP.Infrastructure.Services;

public class AdmissionApplicationService : IAdmissionApplicationService
{
    private readonly IRepository<AdmissionApplication> _repository;
    private readonly IMapper _mapper;

    public AdmissionApplicationService(
        IRepository<AdmissionApplication> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AdmissionApplicationResponse> CreateAsync(
        CreateAdmissionApplicationRequest request)
    {
        var application = _mapper.Map<AdmissionApplication>(request);

        await _repository.AddAsync(application);

        return _mapper.Map<AdmissionApplicationResponse>(application);
    }

    public async Task<List<AdmissionApplicationResponse>> GetAllAsync()
    {
        var applications = await _repository
            .Query()
            .Include(x => x.Program)
            .Include(x => x.Course)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<AdmissionApplicationResponse>>(
            applications);
    }

    public async Task<AdmissionApplicationResponse?> GetByIdAsync(
    Guid id)
    {
        var application = await _repository
            .Query()
            .Include(x => x.Program)
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (application == null)
            return null;

        return _mapper.Map<AdmissionApplicationResponse>(
            application);
    }
    public async Task<AdmissionApplicationResponse?> UpdateStatusAsync(
    Guid id,
    AdmissionStatus status)
{
    var application = await _repository
        .Query()
        .Include(x => x.Program)
        .Include(x => x.Course)
        .FirstOrDefaultAsync(x =>
            x.Id == id &&
            !x.IsDeleted);

    if (application == null)
        return null;

    var previousStatus = application.Status;

    application.Status = status;

    // Demo has just started
    if (status == AdmissionStatus.DemoStarted &&
        previousStatus != AdmissionStatus.DemoStarted)
    {
        application.DemoStartedAt = DateTime.UtcNow;
        application.DemoFeeReminderSentAt = null;
    }

    await _repository.UpdateAsync(application);

    return _mapper.Map<AdmissionApplicationResponse>(application);
}

public async Task<bool> MarkAsViewedAsync(Guid id)
{
    var application = await _repository
        .Query()
        .FirstOrDefaultAsync(x =>
            x.Id == id &&
            !x.IsDeleted);

    if (application == null)
        return false;

    if (application.ViewedByAdminAt == null)
    {
        application.ViewedByAdminAt = DateTime.UtcNow;
        await _repository.UpdateAsync(application);
    }

    return true;
}
}