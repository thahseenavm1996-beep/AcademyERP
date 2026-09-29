using AcademyERP.Application.DTOs.TeacherStudentRequests;
using AcademyERP.Application.Interfaces.TeacherStudentRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeacherStudentRequestsController : ControllerBase
{
    private readonly ITeacherStudentRequestService _service;

    public TeacherStudentRequestsController(
        ITeacherStudentRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTeacherStudentRequestDto request)
    {
        var teacherId = GetCurrentUserId();

        return Ok(
            await _service.CreateAsync(
                teacherId,
                request));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests()
    {
        var teacherId = GetCurrentUserId();

        return Ok(
            await _service.GetForTeacherAsync(teacherId));
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        return Ok(
            await _service.GetPendingAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("find-student")]
    public async Task<IActionResult> FindStudent(
        [FromQuery] string name,
        [FromQuery] string phone)
    {
        var result = await _service.FindStudentAsync(name, phone);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("{id:guid}/link-student")]
    public async Task<IActionResult> LinkStudent(
        Guid id,
        LinkTeacherStudentRequestDto request)
    {
        return Ok(
            await _service.LinkStudentAsync(
                id,
                request.StudentId));
    }

    [HttpPost("{id:guid}/review")]
    public async Task<IActionResult> Review(
        Guid id,
        ReviewTeacherStudentRequestDto request)
    {
        var reviewerId = GetCurrentUserId();

        try
        {
            return Ok(
                await _service.ReviewAsync(
                    id,
                    reviewerId,
                    request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    private Guid GetCurrentUserId()
    {
        var claim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(claim, out var userId))
            throw new UnauthorizedAccessException(
                "Authenticated user ID is missing.");

        return userId;
    }
}
