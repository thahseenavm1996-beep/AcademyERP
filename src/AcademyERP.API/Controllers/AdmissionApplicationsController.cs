using AcademyERP.Application.DTOs.Admissions;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdmissionApplicationsController : ControllerBase
{
    private readonly IAdmissionApplicationService
        _admissionApplicationService;

    public AdmissionApplicationsController(
        IAdmissionApplicationService admissionApplicationService)
    {
        _admissionApplicationService = admissionApplicationService;
    }

    // Public website admission form
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAdmissionApplicationRequest request)
    {
        var application =
            await _admissionApplicationService.CreateAsync(request);

        return Ok(application);
    }

    // Admin - list all admission applications
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var applications =
            await _admissionApplicationService.GetAllAsync();

        return Ok(applications);
    }

    // Admin - view one admission application
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var application =
            await _admissionApplicationService.GetByIdAsync(id);

        if (application == null)
            return NotFound();

        return Ok(application);
    }
    [Authorize]
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
    Guid id,
    UpdateAdmissionStatusRequest request)
    {
        var application =
            await _admissionApplicationService.UpdateStatusAsync(
                id,
                request.Status);

        if (application == null)
            return NotFound();

        return Ok(application);
    }
}