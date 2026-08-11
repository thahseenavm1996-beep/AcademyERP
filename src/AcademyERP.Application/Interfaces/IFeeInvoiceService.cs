using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Fees;

namespace AcademyERP.Application.Interfaces;

public interface IFeeInvoiceService
{
    Task<PagedResponse<FeeInvoiceResponse>> GetAllAsync(
        FeeInvoiceQueryRequest request);

    Task<FeeInvoiceResponse?> GetByIdAsync(Guid id);

    Task<FeeInvoiceResponse> CreateAsync(
        CreateFeeInvoiceRequest request);

    Task<FeeInvoiceResponse?> UpdateAsync(
        Guid id,
        UpdateFeeInvoiceRequest request);

    Task<bool> DeleteAsync(Guid id);
}