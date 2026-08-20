using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Mvc;
using AcademyERP.Application.DTOs.ScheduledClasses;
namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    var result = await _service.GetByIdAsync(id);

    if (result == null)
        return NotFound();

    return Ok(result);
}

    [HttpPost("{id}/start")]
    public async Task<IActionResult> StartClass(Guid id)
    {
        await _service.StartClassAsync(id);

        return Ok(new
        {
            message = "Class started successfully"
        });
    }
[HttpPost("{id}/complete")]
public async Task<IActionResult> CompleteClass(
    Guid id,
    CompleteScheduledClassRequest request)
{
    await _service.CompleteClassAsync(id, request);

    return Ok(new
    {
        message = "Class completed successfully"
    });
}
    [HttpPut("{id}")]
public async Task<IActionResult> Update(
    Guid id,
    UpdateScheduledClassRequest request)
{
    var result =
        await _service.UpdateAsync(id, request);


    if(result == null)
        return NotFound();


    return Ok(result);
}
}