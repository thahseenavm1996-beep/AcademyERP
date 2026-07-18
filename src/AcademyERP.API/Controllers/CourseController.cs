using AcademyERP.Application.DTOs.Courses;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/courses")]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly ICourseService _service;

    public CourseController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var course = await _service.GetByIdAsync(id);

        if (course == null)
            return NotFound();

        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseRequest request)
    {
        var course = await _service.CreateAsync(request);

        return Ok(course);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCourseRequest request)
    {
        var course = await _service.UpdateAsync(id, request);

        return Ok(course);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}