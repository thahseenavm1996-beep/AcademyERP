using AcademyERP.Application.DTOs.TeacherRegistrations;
using AcademyERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Application.Constants;
using AcademyERP.Application.Services.Pdf;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/teacher-registrations")]
public class TeacherRegistrationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITeacherRegistrationService _service;
    private readonly IPdfService _pdfService;

    public TeacherRegistrationsController(
        ITeacherRegistrationService service,
        ApplicationDbContext context,
        IPdfService pdfService)
    {
        _service = service;
        _context = context;
        _pdfService = pdfService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<TeacherRegistrationResponse>> Create(
        [FromBody] CreateTeacherRegistrationRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
[HttpGet("lookups")]
public async Task<IActionResult> GetLookups()
{
    var students = await _context.Students
        .AsNoTracking()
        .Where(x => !x.IsDeleted)
        .OrderBy(x => x.FullName)
        .Select(x => new
        {
            x.Id,
            x.AdmissionNumber,
            x.FullName
        })
        .ToListAsync();

    var courses = await _context.Courses
        .AsNoTracking()
        .Where(x => !x.IsDeleted && x.IsActive)
        .OrderBy(x => x.CourseName)
        .Select(x => new
        {
            x.Id,
            x.CourseCode,
            x.CourseName
        })
        .ToListAsync();
var programs = await _context.Programs
    .AsNoTracking()
    .Where(x => !x.IsDeleted && x.IsActive)
    .OrderBy(x => x.DisplayOrder)
    .ThenBy(x => x.ProgramName)
    .Select(x => new
    {
        x.Id,
        x.ProgramCode,
        x.ProgramName
    })
    .ToListAsync();
    var timeSlots = await _context.TimeSlots
        .AsNoTracking()
        .Where(x => !x.IsDeleted && x.IsActive)
        .OrderBy(x => x.StartTime)
        .Select(x => new
        {
            x.Id,
            x.Name,
            x.StartTime,
            x.EndTime
        })
        .ToListAsync();

    var classDurations = await _context.ClassDurations
        .AsNoTracking()
        .Where(x => !x.IsDeleted && x.IsActive)
        .OrderBy(x => x.Minutes)
        .Select(x => new
        {
            x.Id,
            x.Name,
            x.Minutes
        })
        .ToListAsync();

    return Ok(new
{
    students,
    courses,
    programs,
    timeSlots,
    classDurations
});
}

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpGet]
    public async Task<ActionResult<List<TeacherRegistrationResponse>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromQuery] string? search = null,
        [FromQuery] int gender = 0,
        [FromQuery] int status = 0)
    {
        var requests = await _service.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            requests = requests
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.FullName) &&
                    x.FullName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (gender != 0)
        {
            requests = requests
                .Where(x => (int)x.Gender == gender)
                .ToList();
        }

        if (status != 0)
        {
            requests = requests
                .Where(x => (int)x.Status == status)
                .ToList();
        }

        var pdf =
            _pdfService.GenerateTeacherRegistrationReport(
                requests);

        var fileName =
            $"Teacher-Registration-Requests-{DateTime.Now:yyyyMMdd-HHmmss}.pdf";

        return File(
            pdf,
            "application/pdf",
            fileName);
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpGet("pending")]
    public async Task<ActionResult<List<TeacherRegistrationResponse>>> GetPending()
    {
        return Ok(await _service.GetPendingAsync());
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeacherRegistrationResponse>> GetById(
        Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound(new { message = "Teacher registration not found." });

        return Ok(result);
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromBody] ApproveTeacherRegistrationRequest request)
    {
        try
        {
            var result = await _service.ApproveAsync(id, request);

            if (!result)
                return NotFound(new { message = "Teacher registration not found." });

            return Ok(new
            {
                message = "Teacher registration approved successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] string? reviewRemarks = null)
    {
        try
        {
            var result = await _service.RejectAsync(id, reviewRemarks);

            if (!result)
                return NotFound(new { message = "Teacher registration not found." });

            return Ok(new
            {
                message = "Teacher registration rejected successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
