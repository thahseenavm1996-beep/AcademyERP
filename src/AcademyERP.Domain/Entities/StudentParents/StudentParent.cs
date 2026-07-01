using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.StudentParents;

public class StudentParent : BaseEntity
{
    public Guid StudentId { get; set; }

    public Guid ParentId { get; set; }

    public ParentRelationship Relationship { get; set; }

    public bool IsPrimaryContact { get; set; }
}