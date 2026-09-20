using AcademyERP.Application.Constants;
using AcademyERP.Application.DTOs.ParentPortal;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademyERP.API.Controllers;

[Authorize(Roles = Roles.Parent)]
[ApiController]
[Route("api/parent-portal")]
public class ParentPortalController : ControllerBase
{
    private readonly IParentPortalService _service;

    public ParentPortalController(
        IParentPortalService service)
    {
        _service = service;
    }

    private bool TryGetUserId(out Guid userId)
    {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out userId);
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var parentName =
            User.Identity?.Name ?? "Parent";

        var result =
            await _service.GetDashboardAsync(
                userId,
                parentName);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("children")]
    public async Task<IActionResult> GetChildren()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result =
            await _service.GetChildrenAsync(userId);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("schedule")]
    public async Task<IActionResult> GetSchedule(
        Guid? studentId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result =
            await _service.GetScheduleAsync(
                userId,
                studentId,
                fromDate,
                toDate);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("attendance")]
    public async Task<IActionResult> GetAttendance(
        Guid? studentId = null,
        AttendanceStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result =
            await _service.GetAttendanceAsync(
                userId,
                studentId,
                status,
                fromDate,
                toDate);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("fees")]
    public async Task<IActionResult> GetFees(
        Guid? studentId = null)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result =
            await _service.GetFeesAsync(
                userId,
                studentId);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("progress")]
    public async Task<IActionResult> GetProgress(
        Guid? studentId = null)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result =
            await _service.GetProgressAsync(
                userId,
                studentId);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var result =
            await _service.GetProfileAsync(userId);

        return result == null
            ? NotFound()
            : Ok(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateParentProfileRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var updated =
            await _service.UpdateProfileAsync(
                userId,
                request);

        return updated
            ? NoContent()
            : BadRequest();
    }
}
