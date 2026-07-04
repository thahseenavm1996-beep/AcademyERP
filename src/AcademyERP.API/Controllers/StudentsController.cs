using Microsoft.AspNetCore.Mvc;
using AcademyERP.Application.DTOs.Students;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Application.Services;
using AcademyERP.Application.Common;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents([FromQuery] StudentQueryRequest request)
    {
        var students = await _studentService.GetAllAsync(request);

        return Ok(ApiResponse<PagedResponse<StudentResponse>>.SuccessResponse(
    students,
    "Students retrieved successfully."));
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStudent(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return Ok(ApiResponse<StudentResponse>.SuccessResponse(
    student,
    "Student retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent(CreateStudentRequest request)
    {
        var student = await _studentService.CreateAsync(request);

        return Ok(ApiResponse<StudentResponse>.SuccessResponse(
            student,
            "Student created successfully."));
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStudent(
    Guid id,
    UpdateStudentRequest request)
    {
        var student = await _studentService.UpdateAsync(id, request);

        if (student == null)
        {
            return NotFound();
        }

        return Ok(ApiResponse<StudentResponse>.SuccessResponse(
    student,
    "Student updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStudent(Guid id)
    {
        var deleted = await _studentService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok(ApiResponse<string>.SuccessResponse(
    "Deleted",
    "Student deleted successfully."));
    }
}