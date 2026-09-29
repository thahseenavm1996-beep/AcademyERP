namespace AcademyERP.Application.DTOs.TeacherStudentRequests;

public class TeacherStudentRequestScheduleResponse
{
    public Guid Id { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public string TimeSlotName { get; set; } = string.Empty;

    public Guid ClassDurationId { get; set; }

    public string ClassDurationName { get; set; } = string.Empty;
}
