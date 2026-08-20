using AcademyERP.Application.DTOs.ClassProgress;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClassProgressController : ControllerBase
{
    private readonly IClassProgressService _service;


    public ClassProgressController(
        IClassProgressService service)
    {
        _service = service;
    }



    [HttpPost]
    public async Task<IActionResult> Create(
        CreateClassProgressRequest request)
    {
        var result =
            await _service.CreateAsync(request);

        return Ok(result);
    }



    [HttpGet("{scheduledClassId:guid}")]
    public async Task<IActionResult> Get(
        Guid scheduledClassId)
    {
        var result =
            await _service.GetByScheduledClassIdAsync(
                scheduledClassId);


        return result == null
            ? NotFound()
            : Ok(result);
    }
}