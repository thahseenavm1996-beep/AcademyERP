using AcademyERP.Application.DTOs.Reports;
using AcademyERP.Application.Services.Reports;
using AcademyERP.Domain.Enums;
using AcademyERP.Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/reports/best-performing-students")]
[Authorize]
public class BestPerformingStudentsController : ControllerBase
{
    private readonly IBestPerformingStudentService _service;
    private readonly BestPerformingStudentsPdfService _pdfService;
    private readonly BestPerformingStudentsImageService _imageService;

    public BestPerformingStudentsController(
        IBestPerformingStudentService service,
        BestPerformingStudentsPdfService pdfService,
        BestPerformingStudentsImageService imageService)
    {
        _service = service;
        _pdfService = pdfService;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<ActionResult<BestPerformingStudentsReportResponse>> Get(
        [FromQuery] string? courseName = null,
        [FromQuery] Gender? gender = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var result = await _service.GetReportAsync(
            courseName,
            gender,
            fromDate,
            toDate);

        return Ok(result);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> Pdf(
        [FromQuery] string? courseName = null,
        [FromQuery] Gender? gender = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var result = await _service.GetReportAsync(
            courseName,
            gender,
            fromDate,
            toDate);

        var bytes = _pdfService.Generate(
            result,
            courseName,
            gender?.ToString(),
            fromDate,
            toDate);

        return File(
            bytes,
            "application/pdf",
            $"best-performing-students-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");
    }

    [HttpGet("image")]
    public async Task<IActionResult> Image(
        [FromQuery] string? courseName = null,
        [FromQuery] Gender? gender = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var result = await _service.GetReportAsync(
            courseName,
            gender,
            fromDate,
            toDate);

        var bytes = _imageService.Generate(
            result,
            courseName,
            gender?.ToString(),
            fromDate,
            toDate);

        return File(
            bytes,
            "image/png",
            $"best-performing-students-{DateTime.Now:yyyyMMdd-HHmmss}.png");
    }
}
