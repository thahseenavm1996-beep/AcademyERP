using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class ScheduledClassesController : ControllerBase
{
    private readonly IScheduledClassService _service;

    public ScheduledClassesController(
        IScheduledClassService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        return Ok(result);
    }
}