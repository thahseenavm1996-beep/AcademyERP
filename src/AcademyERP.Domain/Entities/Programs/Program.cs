using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Domain.Entities.Programs;

public class Program : BaseEntity
{
    public string ProgramCode { get; set; } = string.Empty;

    public string ProgramName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsGroupClassAllowed { get; set; }

    public bool IsActive { get; set; } = true;
}