using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/timeslots")]
//[Authorize]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _service;

    public TimeSlotsController(
        ITimeSlotService service)
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