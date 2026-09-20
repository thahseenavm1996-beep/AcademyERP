namespace AcademyERP.Application.DTOs.Teachers;

public class TeacherAvailabilityResponse
{
    public Guid Id { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public string TimeSlotName { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}