using AcademyERP.Application.DTOs.Admissions;
using AcademyERP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class RegistrationRequestsController
    : ControllerBase
{
    private readonly
        IRegistrationRequestService
        _registrationService;

    public RegistrationRequestsController(
        IRegistrationRequestService registrationService)
    {
        _registrationService =
            registrationService;
    }
    
[HttpPost]
public async Task<IActionResult> Create(
    [FromBody] CreateRegistrationRequest request)
{
    var result =
        await _registrationService
            .CreateAsync(request);

    return Ok(result);
}
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var result =
        await _registrationService
            .GetAllAsync();

    return Ok(result);
}
[HttpGet("{id}")]
public async Task<IActionResult> GetById(
    Guid id)
{
    var result =
        await _registrationService
            .GetByIdAsync(id);

    if (result == null)
        return NotFound();

    return Ok(result);
}
[HttpPost("approve")]
public async Task<IActionResult> Approve(
    [FromBody] ApproveRegistrationRequest request)
{
    var result =
        await _registrationService
            .ApproveAsync(request);

    return Ok(result);
}
[HttpPost("{id}/reject")]
public async Task<IActionResult>
    Reject(
        Guid id,
        [FromBody] string remarks)
{
    var result =
        await _registrationService
            .RejectAsync(id, remarks);

    return Ok(result);
}
}