using Microsoft.AspNetCore.Mvc;
using AcademyERP.Application.DTOs.Teachers;
using AcademyERP.Application.Services;
using AcademyERP.Application.Common;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeachers([FromQuery] TeacherQueryRequest request)
    {
        var teachers = await _teacherService.GetAllAsync(request);

        return Ok(ApiResponse<PagedResponse<TeacherResponse>>.SuccessResponse(
            teachers,
            "Teachers retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTeacher(Guid id)
    {
        var teacher = await _teacherService.GetByIdAsync(id);

        if (teacher == null)
        {
            return NotFound();
        }

        return Ok(ApiResponse<TeacherResponse>.SuccessResponse(
            teacher,
            "Teacher retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeacher(CreateTeacherRequest request)
    {
        var teacher = await _teacherService.CreateAsync(request);

        return Ok(ApiResponse<TeacherResponse>.SuccessResponse(
            teacher,
            "Teacher created successfully."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTeacher(
        Guid id,
        UpdateTeacherRequest request)
    {
        var teacher = await _teacherService.UpdateAsync(id, request);

        if (teacher == null)
        {
            return NotFound();
        }

        return Ok(ApiResponse<TeacherResponse>.SuccessResponse(
            teacher,
            "Teacher updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTeacher(Guid id)
    {
        var deleted = await _teacherService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok(ApiResponse<string>.SuccessResponse(
            "Deleted",
            "Teacher deleted successfully."));
    }
}