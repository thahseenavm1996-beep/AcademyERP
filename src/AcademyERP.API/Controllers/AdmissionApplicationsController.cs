using AcademyERP.Application.DTOs.Admissions;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcademyERP.Persistence.Context;
using AcademyERP.Application.DTOs.Admissions;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Enums;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdmissionApplicationsController : ControllerBase
{
    private readonly IAdmissionApplicationService
        _admissionApplicationService;
    private readonly ApplicationDbContext _context;

    public AdmissionApplicationsController(
        IAdmissionApplicationService admissionApplicationService,
        ApplicationDbContext context)
    {
        _admissionApplicationService = admissionApplicationService;
        _context = context;
    }

    // Public website admission form
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAdmissionApplicationRequest request)
    {
        var application =
            await _admissionApplicationService.CreateAsync(request);

        return Ok(application);
    }

    // Admin - list all admission applications
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var applications =
            await _admissionApplicationService.GetAllAsync();

        return Ok(applications);
    }

    // Admin - view one admission application
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var application =
            await _admissionApplicationService.GetByIdAsync(id);

        if (application == null)
            return NotFound();

        return Ok(application);
    }

    [Authorize]
   [HttpGet("{id:guid}/available-teachers")]
public async Task<IActionResult> GetAvailableTeachers(Guid id)
{
    var application = await _context.AdmissionApplications
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id);

    if (application == null)
        return NotFound();

    if (string.IsNullOrWhiteSpace(application.PreferredTiming))
        return Ok(new List<AvailableTeacherResponse>());

    // Find the exact time slot selected in the admission inquiry.
    var timeSlot = await _context.TimeSlots
        .AsNoTracking()
        .FirstOrDefaultAsync(x =>
            x.IsActive &&
            x.Name == application.PreferredTiming);

    if (timeSlot == null)
        return Ok(new List<AvailableTeacherResponse>());

    var teacherAvailabilities = await _context.TeacherAvailabilities
        .AsNoTracking()
        .Include(x => x.Teacher)
        .Where(x =>
            x.IsAvailable &&
            x.TimeSlotId == timeSlot.Id)
        .ToListAsync();

    if (teacherAvailabilities.Count == 0)
        return Ok(new List<AvailableTeacherResponse>());

    var teacherIds = teacherAvailabilities
        .Select(x => x.TeacherId)
        .Distinct()
        .ToList();

    // Get active/future scheduled classes for these teachers.
    var today = DateOnly.FromDateTime(DateTime.Today);

    var scheduledClasses = await _context.ScheduledClasses
        .AsNoTracking()
        .Include(x => x.TeachingSchedule)
        .Where(x =>
            teacherIds.Contains(x.TeachingSchedule.TeacherId) &&
            x.ClassDate >= today &&
            x.Status != ScheduledClassStatus.Completed &&
            x.Status != ScheduledClassStatus.Cancelled &&
            x.Status != ScheduledClassStatus.Missed)
        .Select(x => new
        {
            TeacherId = x.TeachingSchedule.TeacherId,
            x.ClassDate,
            x.StartTime,
            x.EndTime
        })
        .ToListAsync();

    var availableTeachers = new List<AvailableTeacherResponse>();

    foreach (var teacherGroup in teacherAvailabilities.GroupBy(x => x.TeacherId))
    {
        var teacher = teacherGroup.First().Teacher;

        // A teacher is unavailable for this inquiry if they already
        // have a scheduled class overlapping the requested time.
        var hasConflict = scheduledClasses.Any(x =>
            x.TeacherId == teacherGroup.Key &&
            x.StartTime < timeSlot.EndTime &&
            x.EndTime > timeSlot.StartTime);

        if (hasConflict)
            continue;

        availableTeachers.Add(
            new AvailableTeacherResponse
            {
                Id = teacher.Id,
                FullName = teacher.FullName,
                EmployeeCode = teacher.EmployeeCode,
                Department = teacher.Department,
                MobileNumber = teacher.MobileNumber
            });
    }

    return Ok(availableTeachers);
}

    [Authorize]
    [Authorize]
[HttpPut("{id:guid}/assigned-teacher")]
public async Task<IActionResult> AssignTeacher(Guid id, [FromBody] Guid teacherId)
{
    var application = await _context.AdmissionApplications
        .FirstOrDefaultAsync(x => x.Id == id);

    if (application == null)
        return NotFound();

    if (string.IsNullOrWhiteSpace(application.PreferredTiming))
        return BadRequest("The admission application does not have a preferred time.");

    // Find the exact time slot requested by the parent.
    var timeSlot = await _context.TimeSlots
        .AsNoTracking()
        .FirstOrDefaultAsync(x =>
            x.IsActive &&
            x.Name == application.PreferredTiming);

    if (timeSlot == null)
        return BadRequest("The selected preferred time slot is invalid.");

    // The teacher must actually have this time slot configured.
    var teacherHasAvailability = await _context.TeacherAvailabilities
        .AsNoTracking()
        .AnyAsync(x =>
            x.TeacherId == teacherId &&
            x.IsAvailable &&
            x.TimeSlotId == timeSlot.Id);

    if (!teacherHasAvailability)
    {
        return BadRequest(
            "The selected teacher is not available for the requested time.");
    }

    // The teacher must not already have an overlapping scheduled class.
    var hasConflict = await _context.ScheduledClasses
        .AsNoTracking()
        .Include(x => x.TeachingSchedule)
        .AnyAsync(x =>
            x.TeachingSchedule.TeacherId == teacherId &&
            x.StartTime < timeSlot.EndTime &&
            x.EndTime > timeSlot.StartTime &&
            x.Status != ScheduledClassStatus.Completed &&
            x.Status != ScheduledClassStatus.Cancelled &&
            x.Status != ScheduledClassStatus.Missed);

    if (hasConflict)
    {
        return BadRequest(
            "The selected teacher already has a scheduled class during the requested time.");
    }

    application.AssignedTeacherId = teacherId;

    await _context.SaveChangesAsync();

    return NoContent();
}

   
    [Authorize]
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
    Guid id,
    UpdateAdmissionStatusRequest request)
    {
        var application =
            await _admissionApplicationService.UpdateStatusAsync(
                id,
                request.Status);

        if (application == null)
            return NotFound();

        return Ok(application);
    }


    [Authorize]
[HttpPut("{id:guid}/viewed")]
public async Task<IActionResult> MarkAsViewed(Guid id)
{
    var result = await _admissionApplicationService.MarkAsViewedAsync(id);

    if (!result)
        return NotFound();

    return Ok();
}
}
