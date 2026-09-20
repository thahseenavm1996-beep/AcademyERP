using AcademyERP.Application.Constants;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AcademyERP.Application.DTOs.Attendance;
using AcademyERP.Application.DTOs.TeacherPortal;
using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Application.Services.Pdf;
using AcademyERP.Domain.Enums;
using AcademyERP.Application.Services.Reports;

namespace AcademyERP.API.Controllers;

[Authorize(Roles = Roles.Teacher)]
[ApiController]
[Route("api/teacher-portal")]
public class TeacherPortalController : ControllerBase
{
    private readonly ITeacherPortalService _service;

private readonly IPdfService _pdfService;
private readonly IClassReportImageService _classReportImageService;


    public TeacherPortalController(
    ITeacherPortalService service,
    IPdfService pdfService,IClassReportImageService classReportImageService)
{
    _service = service;
    _pdfService = pdfService;
_classReportImageService = classReportImageService;
}



    [HttpGet("classes")]
    public async Task<IActionResult> GetClasses()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);


        if (string.IsNullOrEmpty(userId))
            return Unauthorized();



        var result =
            await _service.GetClassesAsync(
                Guid.Parse(userId));


        return Ok(result);
    }


    [HttpGet("students")]
public async Task<IActionResult> GetStudents()
{
    var userId =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier);


    if(string.IsNullOrEmpty(userId))
        return Unauthorized();



    var result =
        await _service.GetStudentsAsync(
            Guid.Parse(userId));


    return Ok(result);
}

[HttpGet("schedule")]
public async Task<IActionResult> GetSchedule()
{
    var userId =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier);


    if(string.IsNullOrEmpty(userId))
        return Unauthorized();


    var result =
        await _service.GetScheduleAsync(
            Guid.Parse(userId));


    return Ok(result);
}

[HttpGet("attendance")]
public async Task<IActionResult> GetAttendance()
{
    var userId =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier);


    if (string.IsNullOrEmpty(userId))
        return Unauthorized();


    var result =
        await _service.GetAttendanceAsync(
            Guid.Parse(userId));


    return Ok(result);
}

[HttpPost("attendance")]
public async Task<IActionResult> MarkAttendance(
    [FromBody] CreateAttendanceRequest request)
{
    var userId =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    try
    {
        var result =
            await _service.MarkAttendanceAsync(
                Guid.Parse(userId),
                request.ScheduledClassId,
                request.Status,
                request.Remarks);

        return Ok(result);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Forbid(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Conflict(ex.Message);
    }
}

[HttpGet("reports")]
public async Task<IActionResult> GetReports()
{
    var userId =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier);


    if (string.IsNullOrEmpty(userId))
        return Unauthorized();


    var result =
        await _service.GetReportsAsync(
            Guid.Parse(userId));


    return Ok(result);
}

[HttpGet("reports/pdf")]
public async Task<IActionResult> DownloadReportsPdf(
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
    [FromQuery] string? courseName = null,
    [FromQuery] Gender? gender = null)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    var reports = await _service.GetReportsForPdfAsync(
        Guid.Parse(userId),
        fromDate,
        toDate,
        courseName,
        gender);

   var pdf = _pdfService.GenerateClassReportsReport(
        reports,
        new ClassReportQueryRequest
        {
            FromDate = fromDate,
            ToDate = toDate,
            CourseName = courseName,
            Gender = gender
        });

    return File(
        pdf,
        "application/pdf",
        $"teacher-class-reports-{DateTime.Now:yyyyMMddHHmmss}.pdf");
}

[HttpPost("reports/bulk")]
public async Task<IActionResult> CreateBulkReports(
    [FromBody] CreateTeacherBulkReportsRequest request)
{
    var userId =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    try
    {
        var result =
            await _service.CreateBulkReportsAsync(
                Guid.Parse(userId),
                request);

        return Ok(result);
    }
    catch (UnauthorizedAccessException)
    {
        return Forbid();
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}

[HttpGet("profile")]

public async Task<IActionResult> GetProfile()
{
    var userId =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier);


    if (string.IsNullOrEmpty(userId))
        return Unauthorized();


    var result =
        await _service.GetProfileAsync(
            Guid.Parse(userId));


    if(result == null)
        return NotFound();


    return Ok(result);
}
[HttpGet("reports/image")]
public async Task<IActionResult> DownloadReportsImage(
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
    [FromQuery] string? courseName = null,
    [FromQuery] Gender? gender = null)
{
    var userId = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    var reports = await _service.GetReportsForPdfAsync(
        Guid.Parse(userId),
        fromDate,
        toDate,
        courseName,
        gender);

   var image = _classReportImageService.GenerateClassReportsImage(
    reports,
    new ClassReportQueryRequest
    {
        FromDate = fromDate,
        ToDate = toDate,
        CourseName = courseName,
        Gender = gender
    },
    hideTeacherColumn: true);

    return File(
        image,
        "image/png",
        $"teacher-class-reports-{DateTime.Now:yyyyMMddHHmmss}.png");
}
}
