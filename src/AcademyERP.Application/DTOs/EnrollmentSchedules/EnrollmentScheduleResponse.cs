using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.EnrollmentSchedules;

public class EnrollmentScheduleResponse
{
    public Guid Id { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public string TimeSlotName { get; set; } = null!;

    public Guid ClassDurationId { get; set; }

    public string ClassDurationName { get; set; } = null!;
}