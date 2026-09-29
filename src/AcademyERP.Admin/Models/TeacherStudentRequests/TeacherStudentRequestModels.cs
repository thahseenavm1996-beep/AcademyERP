using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models.TeacherStudentRequests;

public class TeacherStudentRequestDto
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    public Guid? StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentPhoneNumber { get; set; } = string.Empty;
    public string? StudentAdmissionNumber { get; set; }

    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;

    public TeacherStudentRequestStatus Status { get; set; }

    public decimal? StudentFee { get; set; }
    public decimal? TeacherSalary { get; set; }

    public string? TeacherRemarks { get; set; }
    public string? AdminRemarks { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public Guid? EnrollmentId { get; set; }

    public List<TeacherStudentRequestScheduleDto> Schedules { get; set; } = new();
}

public class TeacherStudentRequestScheduleDto
{
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }
    public string TimeSlotName { get; set; } = string.Empty;

    public Guid ClassDurationId { get; set; }
    public string ClassDurationName { get; set; } = string.Empty;
}

public class CreateTeacherStudentRequestDto
{
    public Guid? StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string StudentPhoneNumber { get; set; } = string.Empty;

    public Guid CourseId { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Today;

    public decimal? StudentFee { get; set; }

    public string? TeacherRemarks { get; set; }

    public List<CreateTeacherStudentRequestScheduleDto> Schedules { get; set; } = new();
}

public class CreateTeacherStudentRequestScheduleDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public Guid TimeSlotId { get; set; }
    public Guid ClassDurationId { get; set; }
}

public class FindStudentResponseDto
{
    public Guid Id { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class LinkTeacherStudentRequestDto
{
    public Guid StudentId { get; set; }
}

public class ReviewTeacherStudentRequestDto
{
    public bool Approve { get; set; }
    public decimal StudentFee { get; set; }
    public decimal TeacherSalary { get; set; }
    public string? AdminRemarks { get; set; }
}
