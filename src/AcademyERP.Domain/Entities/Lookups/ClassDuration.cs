using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Domain.Entities.Lookups;

public class ClassDuration : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int Minutes { get; set; }

    public bool IsActive { get; set; } = true;
}