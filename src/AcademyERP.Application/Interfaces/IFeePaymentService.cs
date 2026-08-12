using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Payments;

namespace AcademyERP.Application.Interfaces;

public interface IFeePaymentService
{
    Task<PagedResponse<FeePaymentResponse>> GetAllAsync(
        FeePaymentQueryRequest request);

    Task<FeePaymentResponse?> GetByIdAsync(Guid id);

    Task<FeePaymentResponse> CreateAsync(
        CreateFeePaymentRequest request);

    Task<FeePaymentResponse?> UpdateAsync(
        Guid id,
        UpdateFeePaymentRequest request);

    Task<bool> DeleteAsync(Guid id);
}