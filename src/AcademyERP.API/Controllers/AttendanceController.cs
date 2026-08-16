using AcademyERP.Application.DTOs.Attendance;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;

    public AttendanceController(IAttendanceService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetHistory([FromQuery] AttendanceQueryRequest request) =>
        Ok(await _service.GetHistoryAsync(request));

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] AttendanceQueryRequest request) =>
        Ok(await _service.GetSummaryAsync(request));

    [HttpGet("reports/students")]
    public async Task<IActionResult> GetStudentReport([FromQuery] AttendanceQueryRequest request) =>
        Ok(await _service.GetStudentReportAsync(request));

    [HttpGet("reports/teachers")]
    public async Task<IActionResult> GetTeacherReport([FromQuery] AttendanceQueryRequest request) =>
        Ok(await _service.GetTeacherReportAsync(request));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttendanceRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpGet("by-enrollment-date")]
public async Task<IActionResult> GetByEnrollmentDate(
    Guid enrollmentId,
    DateTime date)
{
    var result = await _service.GetByEnrollmentDateAsync(
        enrollmentId,
        date);

    return result is null
        ? NotFound()
        : Ok(result);
}
}