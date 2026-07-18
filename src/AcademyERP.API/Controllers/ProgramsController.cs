using AcademyERP.Application.DTOs.Programs;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Mvc;
using AcademyERP.Application.Constants;
using Microsoft.AspNetCore.Authorization;
namespace AcademyERP.API.Controllers;

[Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
[ApiController]
[Route("api/[controller]")]
public class ProgramsController : ControllerBase
{
    private readonly IProgramService _programService;

    public ProgramsController(IProgramService programService)
    {
        _programService = programService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var programs = await _programService.GetAllAsync();
        return Ok(programs);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var program = await _programService.GetByIdAsync(id);

        if (program == null)
            return NotFound();

        return Ok(program);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProgramRequest request)
    {
        var program = await _programService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = program.Id },
            program);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProgramRequest request)
    {
        var program = await _programService.UpdateAsync(id, request);

        if (program == null)
            return NotFound();

        return Ok(program);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _programService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}