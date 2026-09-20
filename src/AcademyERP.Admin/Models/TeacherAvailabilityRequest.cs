namespace AcademyERP.Admin.Models;

public class TeacherAvailabilityRequest
{
    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }
}