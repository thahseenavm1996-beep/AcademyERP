using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.TeacherRegistrations;

public class CreateTeacherRegistrationRequest
{
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Address { get; set; }

    public string Qualification { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public string? TeachingExperience { get; set; }
    public string? Remarks { get; set; }

    public List<CreateTeacherRegistrationAvailability> Availability { get; set; }
        = new();

    public List<CreateTeacherRegistrationClass> CurrentClasses { get; set; }
        = new();
}

public class CreateTeacherRegistrationAvailability
{
    public DayOfWeek DayOfWeek { get; set; }
    public Guid TimeSlotId { get; set; }
}

public class CreateTeacherRegistrationClass
{
    public Guid? StudentId { get; set; }
    public string? StudentName { get; set; }
    public Guid CourseId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid ClassDurationId { get; set; }
    public string? ClassType { get; set; }
}
