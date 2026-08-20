using AcademyERP.Application.Constants;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademyERP.API.Controllers;

[Authorize(Roles = Roles.Parent)]
[ApiController]
[Route("api/parent-portal")]
public class ParentPortalController : ControllerBase
{
    private readonly IParentPortalService _service;


    public ParentPortalController(
        IParentPortalService service)
    {
        _service = service;
    }



    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);


        if (string.IsNullOrEmpty(userId))
            return Unauthorized();


        var parentName = User.Identity?.Name ?? "Parent";

var dashboard =
    await _service.GetDashboardAsync(
        Guid.Parse(userId),
        parentName);


        if (dashboard == null)
            return NotFound();


        return Ok(dashboard);
    }
}