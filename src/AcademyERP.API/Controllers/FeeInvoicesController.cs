using AcademyERP.Application.Common;
using AcademyERP.Application.Constants;
using AcademyERP.Application.DTOs.Fees;
using AcademyERP.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

//[Authorize(Roles = Roles.SuperAdmin + "," + Roles.Administrator)]
[ApiController]
[Route("api/[controller]")]
public class FeeInvoicesController : ControllerBase
{
    private readonly IFeeInvoiceService _feeInvoiceService;

    public FeeInvoicesController(
        IFeeInvoiceService feeInvoiceService)
    {
        _feeInvoiceService = feeInvoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] FeeInvoiceQueryRequest request)
    {
        var result =
            await _feeInvoiceService.GetAllAsync(request);

        return Ok(
            ApiResponse<PagedResponse<FeeInvoiceResponse>>
            .SuccessResponse(
                result,
                "Fee invoices retrieved successfully."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var invoice =
            await _feeInvoiceService.GetByIdAsync(id);

        if (invoice == null)
            return NotFound();

        return Ok(
            ApiResponse<FeeInvoiceResponse>
            .SuccessResponse(
                invoice,
                "Fee invoice retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateFeeInvoiceRequest request)
    {
        var invoice =
            await _feeInvoiceService.CreateAsync(request);

        return Ok(
            ApiResponse<FeeInvoiceResponse>
            .SuccessResponse(
                invoice,
                "Fee invoice created successfully."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateFeeInvoiceRequest request)
    {
        var invoice =
            await _feeInvoiceService.UpdateAsync(
                id,
                request);

        if (invoice == null)
            return NotFound();

        return Ok(
            ApiResponse<FeeInvoiceResponse>
            .SuccessResponse(
                invoice,
                "Fee invoice updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted =
            await _feeInvoiceService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok(
            ApiResponse<string>
            .SuccessResponse(
                "Deleted",
                "Fee invoice deleted successfully."));
    }
}