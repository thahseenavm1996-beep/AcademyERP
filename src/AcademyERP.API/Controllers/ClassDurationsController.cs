using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/classdurations")]
//[Authorize]
public class ClassDurationsController : ControllerBase
{
    private readonly IClassDurationService _service;

    public ClassDurationsController(
        IClassDurationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(
            await _service.GetAllAsync());
    }
}