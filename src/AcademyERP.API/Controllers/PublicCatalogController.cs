using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/public/catalog")]
[AllowAnonymous]
public class PublicCatalogController : ControllerBase
{
    private readonly IProgramService _programService;
    private readonly ICourseService _courseService;

    public PublicCatalogController(
        IProgramService programService,
        ICourseService courseService)
    {
        _programService = programService;
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalog()
    {
        var programs = await _programService.GetAllAsync();
        var courses = await _courseService.GetAllAsync();

        var result = programs
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .Select(p => new
            {
                p.Id,
                p.ProgramCode,
                p.ProgramName,
                p.Description,

                Courses = courses
                    .Where(c =>
                        c.ProgramId == p.Id &&
                        c.IsActive)
                    .Select(c => new
                    {
                        c.Id,
                        c.CourseCode,
                        c.CourseName,
                        c.Description
                    })
                    .ToList()
            })
            .ToList();

        return Ok(result);
    }
}