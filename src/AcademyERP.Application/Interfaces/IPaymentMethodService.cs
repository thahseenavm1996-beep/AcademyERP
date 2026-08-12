using AcademyERP.Application.DTOs.PaymentMethods;

namespace AcademyERP.Application.Interfaces;

public interface IPaymentMethodService
{
    Task<List<PaymentMethodResponse>> GetAllAsync();
}