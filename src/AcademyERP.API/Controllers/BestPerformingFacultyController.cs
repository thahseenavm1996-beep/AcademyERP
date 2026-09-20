using AcademyERP.Application.DTOs.Reports;
using AcademyERP.Application.Services.Reports;
using AcademyERP.Domain.Enums;
using AcademyERP.Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/reports/best-performing-faculty")]
[Authorize]
public class BestPerformingFacultyController : ControllerBase
{
    private readonly IBestPerformingFacultyService _service;
    private readonly BestPerformingFacultyPdfService _pdf;
    private readonly BestPerformingFacultyImageService _image;

    public BestPerformingFacultyController(
        IBestPerformingFacultyService service,
        BestPerformingFacultyPdfService pdf,
        BestPerformingFacultyImageService image)
    {
        _service = service;
        _pdf = pdf;
        _image = image;
    }

    [HttpGet]
    public async Task<ActionResult<BestPerformingFacultyReportResponse>> Get(
        [FromQuery] string? courseName = null,
        [FromQuery] Gender? gender = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var result =
            await _service.GetReportAsync(
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
        var result =
            await _service.GetReportAsync(
                courseName,
                gender,
                fromDate,
                toDate);

        var bytes =
            _pdf.Generate(
                result,
                courseName,
                gender?.ToString(),
                fromDate,
                toDate);

        return File(
            bytes,
            "application/pdf",
            $"best-performing-faculty-{DateTime.Now:yyyyMMdd-HHmmss}.pdf");
    }

    [HttpGet("image")]
    public async Task<IActionResult> Image(
        [FromQuery] string? courseName = null,
        [FromQuery] Gender? gender = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null)
    {
        var result =
            await _service.GetReportAsync(
                courseName,
                gender,
                fromDate,
                toDate);

        var bytes =
            _image.Generate(
                result,
                courseName,
                gender?.ToString(),
                fromDate,
                toDate);

        return File(
            bytes,
            "image/png",
            $"best-performing-faculty-{DateTime.Now:yyyyMMdd-HHmmss}.png");
    }
}
