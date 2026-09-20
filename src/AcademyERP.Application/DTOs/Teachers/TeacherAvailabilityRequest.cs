namespace AcademyERP.Application.DTOs.Teachers;

public class TeacherAvailabilityRequest
{
    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }
}