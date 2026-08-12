using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Domain.Entities.Finance;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<FeePayment> Payments { get; set; }
        = new List<FeePayment>();
}