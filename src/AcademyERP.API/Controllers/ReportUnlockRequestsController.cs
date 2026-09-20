using AcademyERP.Application.DTOs.ReportUnlockRequests;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademyERP.API.Controllers;

[Authorize]
[ApiController]
[Route("api/report-unlock-requests")]
public class ReportUnlockRequestsController : ControllerBase
{
    private readonly IReportUnlockRequestService _service;

    public ReportUnlockRequestsController(
        IReportUnlockRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateReportUnlockRequest request)
    {
        var userId = GetUserId();

        if (!userId.HasValue)
            return Unauthorized();

        try
        {
            var result = await _service.CreateAsync(
                userId.Value,
                request);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("teacher")]
    public async Task<IActionResult> GetForTeacher()
    {
        var userId = GetUserId();

        if (!userId.HasValue)
            return Unauthorized();

        var result = await _service.GetForTeacherAsync(
            userId.Value);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ReportUnlockRequestStatus? status = null)
    {
        var result = await _service.GetAllAsync(status);

        return Ok(result);
    }

    [HttpPut("{id:guid}/review")]
    public async Task<IActionResult> Review(
        Guid id,
        [FromBody] ReviewReportUnlockRequest request)
    {
        var userId = GetUserId();

        if (!userId.HasValue)
            return Unauthorized();

        try
        {
            var result = await _service.ReviewAsync(
                id,
                userId.Value,
                request.Status,
                request.AdminRemarks);

            if (!result)
                return NotFound();

            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid? GetUserId()
    {
        var value =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var id)
            ? id
            : null;
    }
}

public class ReviewReportUnlockRequest
{
    public ReportUnlockRequestStatus Status { get; set; }

    public string? AdminRemarks { get; set; }
}
