namespace AcademyERP.Application.DTOs.EnrollmentSchedules;

public class CreateEnrollmentScheduleRequest
{
    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public Guid ClassDurationId { get; set; }
}