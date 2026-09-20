using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.TeacherPayments;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeacherPaymentsController : ControllerBase
{
    private readonly ITeacherPaymentService _service;

    public TeacherPaymentsController(
        ITeacherPaymentService service)
    {
        _service = service;
    }

    [HttpGet("configurations")]
    public async Task<IActionResult> GetConfigurations(
        [FromQuery] Guid? teacherId = null,
        [FromQuery] Guid? enrollmentId = null)
    {
        var result = await _service.GetConfigurationsAsync(
            teacherId,
            enrollmentId);

        return Ok(
            ApiResponse<List<TeacherPaymentConfigurationResponse>>
                .SuccessResponse(
                    result,
                    "Teacher payment configurations retrieved successfully."));
    }

    [HttpGet("configurations/{id:guid}")]
    public async Task<IActionResult> GetConfiguration(Guid id)
    {
        var result =
            await _service.GetConfigurationByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(
            ApiResponse<TeacherPaymentConfigurationResponse>
                .SuccessResponse(
                    result,
                    "Teacher payment configuration retrieved successfully."));
    }

    [HttpPost("configurations")]
    public async Task<IActionResult> CreateConfiguration(
        CreateTeacherPaymentConfigurationRequest request)
    {
        try
        {
            var result =
                await _service.CreateConfigurationAsync(request);

            return Ok(
                ApiResponse<TeacherPaymentConfigurationResponse>
                    .SuccessResponse(
                        result,
                        "Teacher payment configuration created successfully."));
        }
        catch (InvalidOperationException ex)
        {
           return BadRequest(ex.Message);
        }
    }

    [HttpPut("configurations/{id:guid}")]
    public async Task<IActionResult> UpdateConfiguration(
        Guid id,
        UpdateTeacherPaymentConfigurationRequest request)
    {
        try
        {
            var result =
                await _service.UpdateConfigurationAsync(
                    id,
                    request);

            if (result == null)
                return NotFound();

            return Ok(
                ApiResponse<TeacherPaymentConfigurationResponse>
                    .SuccessResponse(
                        result,
                        "Teacher payment configuration updated successfully."));
        }
        catch (InvalidOperationException ex)
        {
           return BadRequest(ex.Message);
        }
    }

    [HttpDelete("configurations/{id:guid}")]
    public async Task<IActionResult> DeleteConfiguration(
        Guid id)
    {
        try
        {
            var deleted =
                await _service.DeleteConfigurationAsync(id);

            if (!deleted)
                return NotFound();

            return Ok(
                ApiResponse<string>
                    .SuccessResponse(
                        "Deleted",
                        "Teacher payment configuration deleted successfully."));
        }
        catch (InvalidOperationException ex)
        {
           return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments(
        [FromQuery] TeacherPaymentQueryRequest request)
    {
        var result =
            await _service.GetPaymentsAsync(request);

        return Ok(
            ApiResponse<PagedResponse<TeacherPaymentResponse>>
                .SuccessResponse(
                    result,
                    "Teacher payments retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var result =
            await _service.GetPaymentByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(
            ApiResponse<TeacherPaymentResponse>
                .SuccessResponse(
                    result,
                    "Teacher payment retrieved successfully."));
    }

    [HttpPost("{id:guid}/mark-paid")]
    public async Task<IActionResult> MarkPaid(
        Guid id,
        MarkTeacherPaymentPaidRequest request)
    {
        try
        {
            var result =
                await _service.MarkPaidAsync(
                    id,
                    request);

            if (result == null)
                return NotFound();

            return Ok(
                ApiResponse<TeacherPaymentResponse>
                    .SuccessResponse(
                        result,
                        "Teacher payment marked as paid successfully."));
        }
        catch (InvalidOperationException ex)
        {
           return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var result =
                await _service.CancelAsync(id);

            if (result == null)
                return NotFound();

            return Ok(
                ApiResponse<TeacherPaymentResponse>
                    .SuccessResponse(
                        result,
                        "Teacher payment cancelled successfully."));
        }
        catch (InvalidOperationException ex)
        {
           return BadRequest(ex.Message);
        }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(
        [FromQuery] DateTime? asOfDate = null)
    {
        var date = asOfDate ?? DateTime.UtcNow;

        var created =
            await _service.GeneratePendingCyclesAsync(date);

        return Ok(
            ApiResponse<int>
                .SuccessResponse(
                    created,
                    $"{created} pending teacher payment cycle(s) generated successfully."));
    }

[HttpPost("configurations/{id}/stop")]
public async Task<IActionResult> StopConfiguration(
    Guid id,
    [FromQuery] DateTime? stopDate)
{
    try
    {
        var date = stopDate?.Date ?? DateTime.UtcNow.Date;

        var result = await _service
            .StopConfigurationAsync(id, date);

        if (result == null)
            return NotFound("Payment configuration not found.");

        return Ok(new ApiResponse<TeacherPaymentConfigurationResponse>
        {
            Success = true,
            Data = result
        });
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}

[HttpPost("configurations/{id:guid}/pause")]
public async Task<IActionResult> PauseConfiguration(
    Guid id,
    [FromQuery] DateTime pauseStartDate,
    [FromQuery] string? reason)
{
    try
    {
        var result = await _service.PauseConfigurationAsync(
            id,
            pauseStartDate.Date,
            reason);

        if (result == null)
            return NotFound("Payment configuration not found.");

        return Ok(
            ApiResponse<TeacherPaymentConfigurationResponse>
                .SuccessResponse(
                    result,
                    "Teacher payment configuration paused successfully."));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}


[HttpPost("configurations/{id:guid}/resume")]
public async Task<IActionResult> ResumeConfiguration(
    Guid id,
    [FromQuery] DateTime resumeDate)
{
    try
    {
        var result = await _service.ResumeConfigurationAsync(
            id,
            resumeDate.Date);

        if (result == null)
            return NotFound("Payment configuration not found.");

        return Ok(
            ApiResponse<TeacherPaymentConfigurationResponse>
                .SuccessResponse(
                    result,
                    "Teacher payment configuration resumed successfully."));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}

}
