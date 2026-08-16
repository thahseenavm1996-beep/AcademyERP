namespace AcademyERP.Admin.Models.Enrollments;

public class CreateEnrollmentScheduleRequest
{
    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public Guid ClassDurationId { get; set; }
}