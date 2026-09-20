using AcademyERP.Application.Common;
using AcademyERP.Application.Constants;
using AcademyERP.Application.DTOs.Fees;
using AcademyERP.Application.Interfaces;
using AcademyERP.Application.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeeInvoicesController : ControllerBase
{
    private readonly IFeeInvoiceService _feeInvoiceService;
    private readonly IPdfService _pdfService;

    public FeeInvoicesController(
        IFeeInvoiceService feeInvoiceService,
        IPdfService pdfService)
    {
        _feeInvoiceService = feeInvoiceService;
        _pdfService = pdfService;
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FeeInvoiceQueryRequest request)
    {
        var result =
            await _feeInvoiceService.GetAllAsync(request);

        return Ok(
            ApiResponse<PagedResponse<FeeInvoiceResponse>>
                .SuccessResponse(
                    result,
                    "Fee invoices retrieved successfully."));
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var invoice =
            await _feeInvoiceService.GetByIdAsync(id);

        if (invoice == null)
            return NotFound();

        return Ok(
            ApiResponse<FeeInvoiceResponse>
                .SuccessResponse(
                    invoice,
                    "Fee invoice retrieved successfully."));
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateFeeInvoiceRequest request)
    {
        var invoice =
            await _feeInvoiceService.CreateAsync(request);

        return Ok(
            ApiResponse<FeeInvoiceResponse>
                .SuccessResponse(
                    invoice,
                    "Fee invoice created successfully."));
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateFeeInvoiceRequest request)
    {
        var invoice =
            await _feeInvoiceService.UpdateAsync(
                id,
                request);

        if (invoice == null)
            return NotFound();

        return Ok(
            ApiResponse<FeeInvoiceResponse>
                .SuccessResponse(
                    invoice,
                    "Fee invoice updated successfully."));
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted =
            await _feeInvoiceService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok(
            ApiResponse<string>
                .SuccessResponse(
                    "Deleted",
                    "Fee invoice deleted successfully."));
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromQuery] DateTime? asOfDate = null)
    {
        var generated =
            await _feeInvoiceService
                .GeneratePendingCyclesAsync(
                    asOfDate ?? DateTime.UtcNow);

        return Ok(
            ApiResponse<int>.SuccessResponse(
                generated,
                $"{generated} due fee invoice(s) generated successfully."));
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpPost("enrollments/{enrollmentId:guid}/pause")]
    public async Task<IActionResult> PauseEnrollment(
        Guid enrollmentId,
        [FromQuery] DateTime pauseStartDate,
        [FromQuery] string? reason)
    {
        try
        {
            var result =
                await _feeInvoiceService.PauseEnrollmentAsync(
                    enrollmentId,
                    pauseStartDate,
                    reason);

            return result == null
                ? NotFound("Enrollment not found.")
                : Ok(
                    ApiResponse<AcademyERP.Application.DTOs.Enrollments.EnrollmentResponse>
                        .SuccessResponse(
                            result,
                            "Fee generation paused successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpPost("enrollments/{enrollmentId:guid}/resume")]
    public async Task<IActionResult> ResumeEnrollment(
        Guid enrollmentId,
        [FromQuery] DateTime resumeDate)
    {
        try
        {
            var result =
                await _feeInvoiceService.ResumeEnrollmentAsync(
                    enrollmentId,
                    resumeDate);

            return result == null
                ? NotFound("Enrollment not found.")
                : Ok(
                    ApiResponse<AcademyERP.Application.DTOs.Enrollments.EnrollmentResponse>
                        .SuccessResponse(
                            result,
                            "Fee generation resumed successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
    [HttpGet("{id:guid}/export/pdf")]
    public async Task<IActionResult> ExportPdf(Guid id)
    {
        var invoice =
            await _feeInvoiceService.GetByIdAsync(id);

        if (invoice == null)
            return NotFound();

        var pdf =
            _pdfService.GenerateFeeInvoiceReport(invoice);

        var fileName =
            $"invoice-{invoice.InvoiceNumber}.pdf";

        return File(
            pdf,
            "application/pdf",
            fileName);
    }

    [Authorize(Roles = Roles.Teacher)]
    [HttpGet("teacher-outstanding")]
    public async Task<IActionResult> GetTeacherOutstandingInvoices()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result =
            await _feeInvoiceService
                .GetTeacherOutstandingInvoicesAsync(
                    Guid.Parse(userId));

        return Ok(result);
    }
}
