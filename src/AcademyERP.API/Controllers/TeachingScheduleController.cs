using AcademyERP.Application.DTOs.TeachingSchedules;
using AcademyERP.Application.Constants;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/teachingschedules")]
[Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
public class TeachingScheduleController : ControllerBase
{
    private readonly ITeachingScheduleService _service;

    public TeachingScheduleController(
        ITeachingScheduleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _service.GetAllAsync();

        return Ok(schedules);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTeachingScheduleRequest request)
    {
        try
        {
            var schedule = await _service.CreateAsync(request);

            return Ok(schedule);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}
