using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcademyERP.Application.Services.Pdf;
using AcademyERP.Domain.Enums;
using AcademyERP.Application.Services.Reports;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassReportController : ControllerBase
{
    private readonly IClassReportService _service;
private readonly IPdfService _pdfService;
private readonly IClassReportImageService _classReportImageService;
    public ClassReportController(
    IClassReportService service,
    IPdfService pdfService,IClassReportImageService classReportImageService)
{
    _service = service;
    _pdfService = pdfService;
_classReportImageService = classReportImageService;
}

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ClassReportQueryRequest request)
    {
        var result = await _service.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
public async Task<IActionResult> Create(CreateClassReportRequest request)
{
    try
    {
        var result = await _service.CreateAsync(request);

        return CreatedAtAction(
            nameof(Get),
            new { id = result.Id },
            result);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new
        {
            message = ex.Message
        });
    }
}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateClassReportRequest request)
    {
        var result = await _service.UpdateAsync(id, request);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("scheduled-class/{scheduledClassId:guid}")]
public async Task<IActionResult> GetByScheduledClass(
    Guid scheduledClassId)
{
    var result =
        await _service.GetByScheduledClassIdAsync(
            scheduledClassId);

    if (result == null)
        return NotFound();

    return Ok(result);
}

[HttpGet("export/pdf")]
public async Task<IActionResult> ExportPdf(
    [FromQuery] ClassReportQueryRequest request)
{
    var reports =
        await _service.GetForExportAsync(request);

    var pdf =
        _pdfService.GenerateClassReportsReport(
            reports,
            request);

    var fileName =
        "class-reports-" +
        DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") +
        ".pdf";

    return File(
        pdf,
        "application/pdf",
        fileName);
}
[HttpGet("export/image")]
public async Task<IActionResult> ExportImage(
    [FromQuery] ClassReportQueryRequest request)
{
    var reports =
        await _service.GetForExportAsync(request);

    var image = _classReportImageService
        .GenerateClassReportsImage(
            reports,
            request);

    var fileName =
        "class-reports-" +
        DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") +
        ".png";

    return File(
        image,
        "image/png",
        fileName);
}

[HttpGet("teacher-monitoring")]
public async Task<IActionResult> GetTeacherReportMonitoring(
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate,
    [FromQuery] Guid? teacherId,
    [FromQuery] string? courseName,
    [FromQuery] Gender? gender,
    [FromQuery] bool? reportSubmitted)
{
   var result = await _service.GetTeacherReportMonitoringAsync(
        fromDate,
        toDate,
        teacherId,
        courseName,
        gender,
        reportSubmitted);

    return Ok(result);
}

[HttpGet("teacher-monitoring/export/pdf")]
public async Task<IActionResult> ExportTeacherReportMonitoringPdf(
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate,
    [FromQuery] Guid? teacherId,
    [FromQuery] string? courseName,
    [FromQuery] Gender? gender,
    [FromQuery] bool? reportSubmitted)
{
    var reports = await _service.GetTeacherReportMonitoringAsync(
        fromDate,
        toDate,
        teacherId,
        courseName,
        gender,
        reportSubmitted);

    var pdf = _pdfService.GenerateTeacherReportMonitoringReport(
        reports,
        fromDate,
        toDate,
        teacherId,
        courseName,
        gender,
        reportSubmitted);

    return File(
        pdf,
        "application/pdf",
        $"teacher-report-monitoring-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");
}
}
