using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Courses;

namespace AcademyERP.Domain.Entities.Programs;

public class Program : BaseEntity
{
    public string ProgramCode { get; set; } = string.Empty;

    public string ProgramName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
    public int DefaultDurationMinutes { get; set; } = 30;

    // Navigation
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}