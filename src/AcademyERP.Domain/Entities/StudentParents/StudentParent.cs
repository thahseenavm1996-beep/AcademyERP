using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Parents;

namespace AcademyERP.Domain.Entities.StudentParents;

public class StudentParent : BaseEntity
{
    public Guid StudentId { get; set; }

    public Guid ParentId { get; set; }

    public ParentRelationship Relationship { get; set; }

    public bool IsPrimaryContact { get; set; }

    public Student Student { get; set; } = null!;

    public Parent Parent { get; set; } = null!;
}