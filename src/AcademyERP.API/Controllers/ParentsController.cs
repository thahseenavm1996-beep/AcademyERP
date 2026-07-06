using Microsoft.AspNetCore.Mvc;
using AcademyERP.Application.DTOs.Parents;
using AcademyERP.Application.Services;
using AcademyERP.Application.Common;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParentsController : ControllerBase
{
    private readonly IParentService _parentService;

    public ParentsController(IParentService parentService)
    {
        _parentService = parentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetParents([FromQuery] ParentQueryRequest request)
    {
        var parents = await _parentService.GetAllAsync(request);

        return Ok(ApiResponse<PagedResponse<ParentResponse>>.SuccessResponse(
            parents,
            "Parents retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetParent(Guid id)
    {
        var parent = await _parentService.GetByIdAsync(id);

        if (parent == null)
            return NotFound();

        return Ok(ApiResponse<ParentResponse>.SuccessResponse(
            parent,
            "Parent retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> CreateParent(CreateParentRequest request)
    {
        var parent = await _parentService.CreateAsync(request);

        return Ok(ApiResponse<ParentResponse>.SuccessResponse(
            parent,
            "Parent created successfully."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateParent(Guid id, UpdateParentRequest request)
    {
        var parent = await _parentService.UpdateAsync(id, request);

        if (parent == null)
            return NotFound();

        return Ok(ApiResponse<ParentResponse>.SuccessResponse(
            parent,
            "Parent updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteParent(Guid id)
    {
        var deleted = await _parentService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok(ApiResponse<string>.SuccessResponse(
            "Deleted",
            "Parent deleted successfully."));
    }
    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(
    Guid id,
    [FromBody] ResetParentPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }

        try
        {
            var success = await _parentService.ResetPasswordAsync(id, request);

            if (!success)
                return NotFound("Parent or User not found.");

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}