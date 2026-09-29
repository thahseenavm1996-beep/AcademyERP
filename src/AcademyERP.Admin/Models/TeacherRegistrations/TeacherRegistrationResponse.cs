namespace AcademyERP.Admin.Models.TeacherRegistrations;

public class TeacherRegistrationResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Gender { get; set; }

    public string MobileNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Address { get; set; }

    public string Qualification { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }

    public string? TeachingExperience { get; set; }
    public string? Remarks { get; set; }

    public int Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewRemarks { get; set; }

    public List<TeacherRegistrationAvailabilityResponse> Availability { get; set; } = new();
    public List<TeacherRegistrationClassResponse> CurrentClasses { get; set; } = new();
}

public class TeacherRegistrationAvailabilityResponse
{
    public Guid Id { get; set; }
    public int DayOfWeek { get; set; }
    public Guid TimeSlotId { get; set; }
    public string TimeSlotName { get; set; } = string.Empty;
}

public class TeacherRegistrationClassResponse
{
    public Guid Id { get; set; }

  public Guid? StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? AdmissionNumber { get; set; }

    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? CourseCode { get; set; }

    public int DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }
    public string TimeSlotName { get; set; } = string.Empty;

    public Guid ClassDurationId { get; set; }
    public string ClassDurationName { get; set; } = string.Empty;

    public string? ClassType { get; set; }
}