using AcademyERP.Application.Constants;
using AcademyERP.Application.DTOs.FeePaymentSubmissions;
using AcademyERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/fee-payment-submissions")]
public class FeePaymentSubmissionsController : ControllerBase
{
    private readonly IFeePaymentSubmissionService _service;
private readonly IWebHostEnvironment _environment;
    public FeePaymentSubmissionsController(
    IFeePaymentSubmissionService service,
    IWebHostEnvironment environment)
{
    _service = service;
    _environment = environment;
}

    // ============================================================
    // TEACHER
    // ============================================================

    [Authorize(Roles = Roles.Teacher)]
    [HttpPost]
    public async Task<IActionResult> Submit(
        CreateFeePaymentSubmissionRequest request)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _service.SubmitAsync(
                Guid.Parse(userId),
                request);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.Teacher)]
    [HttpGet("my")]
    public async Task<IActionResult> GetMySubmissions()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _service.GetByTeacherAsync(
            Guid.Parse(userId));

        return Ok(result);
    }

    // ============================================================
    // ADMIN
    // ============================================================

    [Authorize(
        Roles = Roles.Administrator + "," +
                Roles.SuperAdmin)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [Authorize(
        Roles = Roles.Administrator + "," +
                Roles.SuperAdmin)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [Authorize(
        Roles = Roles.Administrator + "," +
                Roles.SuperAdmin)]
    [HttpPut("{id:guid}/review")]
    public async Task<IActionResult> Review(
        Guid id,
        ReviewFeePaymentSubmissionRequest request)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _service.ReviewAsync(
                id,
                Guid.Parse(userId),
                request);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.Teacher)]
[HttpPost("{id:guid}/proof")]
[RequestSizeLimit(5_242_880)]
[Consumes("multipart/form-data")]
public async Task<IActionResult> UploadProof(
    Guid id,
    IFormFile file)
{
    var userId = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
        return Unauthorized();

    if (file == null || file.Length == 0)
        return BadRequest("Please select a payment proof image.");

    try
    {
        await using var stream =
    file.OpenReadStream();

        var result =
            await _service.UploadProofAsync(
                id,
                Guid.Parse(userId),
                stream,
                file.FileName,
                file.ContentType);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}

[Authorize(Roles = Roles.Administrator + "," + Roles.SuperAdmin)]
[HttpGet("{id:guid}/proof")]
public async Task<IActionResult> GetProof(Guid id)
{
    var submission = await _service.GetByIdAsync(id);

    if (submission == null)
        return NotFound();

    if (string.IsNullOrWhiteSpace(submission.ProofFilePath))
        return NotFound("Payment proof not found.");

    var relativePath = submission.ProofFilePath
        .Replace("/", Path.DirectorySeparatorChar.ToString());

    var fullPath = Path.Combine(
        _environment.ContentRootPath,
        relativePath);

    if (!System.IO.File.Exists(fullPath))
        return NotFound("Payment proof file not found.");

    var extension = Path.GetExtension(fullPath).ToLowerInvariant();

    var contentType = extension switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };

    var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);

    return File(bytes, contentType);
}
}