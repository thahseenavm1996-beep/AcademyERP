using AcademyERP.Application.DTOs.PaymentMethods;
using AcademyERP.Application.Interfaces;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class PaymentMethodService
    : IPaymentMethodService
{
    private readonly ApplicationDbContext _context;

    public PaymentMethodService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentMethodResponse>>
        GetAllAsync()
    {
        return await _context.PaymentMethods
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new PaymentMethodResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }
}