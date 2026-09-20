using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Admissions;

public class ApproveRegistrationRequest
{
    public Guid RegistrationRequestId { get; set; }

    public List<ApproveStudentRequest> Students { get; set; }
        = new();
}

public class ApproveStudentRequest
{
    public Guid RegistrationStudentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid? TeacherId { get; set; }

    public Guid? TimeSlotId { get; set; }

    public Guid? ClassDurationId { get; set; }

    public List<DayOfWeek> DaysOfWeek { get; set; }
        = new();
}