namespace AcademyERP.Application.DTOs.TeachingSchedules;

public class CreateTeachingScheduleRequest
{
    public Guid EnrollmentId { get; set; }

    public Guid TeacherId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public Guid ClassDurationId { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    public int MaximumStudents { get; set; } = 1;

    public string? Remarks { get; set; }
}