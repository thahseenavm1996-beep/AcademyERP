using AcademyERP.Application.Common;
using AcademyERP.Application.Constants;
using AcademyERP.Application.DTOs.Payments;
using AcademyERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

//[Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
[ApiController]
[Route("api/[controller]")]
public class FeePaymentsController : ControllerBase
{
    private readonly IFeePaymentService _service;

    public FeePaymentsController(
        IFeePaymentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FeePaymentQueryRequest request)
    {
        var result =
            await _service.GetAllAsync(request);

        return Ok(
            ApiResponse<PagedResponse<FeePaymentResponse>>
            .SuccessResponse(
                result,
                "Payments retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var payment =
            await _service.GetByIdAsync(id);

        if (payment == null)
            return NotFound();

        return Ok(
            ApiResponse<FeePaymentResponse>
            .SuccessResponse(
                payment,
                "Payment retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateFeePaymentRequest request)
    {
        var payment =
            await _service.CreateAsync(request);

        return Ok(
            ApiResponse<FeePaymentResponse>
            .SuccessResponse(
                payment,
                "Payment created successfully."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateFeePaymentRequest request)
    {
        var payment =
            await _service.UpdateAsync(
                id,
                request);

        if (payment == null)
            return NotFound();

        return Ok(
            ApiResponse<FeePaymentResponse>
            .SuccessResponse(
                payment,
                "Payment updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted =
            await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                "Deleted",
                "Payment deleted successfully."));
    }
}