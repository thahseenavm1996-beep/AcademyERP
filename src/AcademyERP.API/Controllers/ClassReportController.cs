using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassReportController : ControllerBase
{
    private readonly IClassReportService _service;

    public ClassReportController(IClassReportService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ClassReportQueryRequest request)
    {
        var result = await _service.GetAllAsync(request);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClassReportRequest request)
    {
        var result = await _service.CreateAsync(request);

        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateClassReportRequest request)
    {
        var result = await _service.UpdateAsync(id, request);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}