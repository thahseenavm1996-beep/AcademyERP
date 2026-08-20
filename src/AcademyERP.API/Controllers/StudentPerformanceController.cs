using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;


[ApiController]
[Route("api/students/{studentId}/performance")]
[Authorize]
public class StudentPerformanceController : ControllerBase
{

    private readonly IStudentPerformanceService _service;


    public StudentPerformanceController(
        IStudentPerformanceService service)
    {
        _service = service;
    }



    [HttpGet]
    public async Task<IActionResult> Get(
        Guid studentId)
    {
        var result =
            await _service.GetAsync(studentId);


        return Ok(result);
    }

}