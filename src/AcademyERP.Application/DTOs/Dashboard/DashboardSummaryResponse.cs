namespace AcademyERP.Application.DTOs.Dashboard;

public class DashboardSummaryResponse
{
    public int TotalStudents { get; set; }

    public int TotalTeachers { get; set; }

    public int TotalParents { get; set; }

    public int TotalPrograms { get; set; }

    public int NewAdmissions { get; set; }

    public int NewRegistrations { get; set; }

    public int ActiveStudents { get; set; }

    public int ActiveTeachers { get; set; }

    // Finance summary
    public decimal TotalCollected { get; set; }

    public decimal PendingFees { get; set; }

    public decimal TotalInvoiced { get; set; }

    public decimal OverdueFees { get; set; }

    public List<RecentStudentDto> RecentStudents { get; set; }
        = new();

        public int TodayClasses { get; set; }

public int TodayCompletedClasses { get; set; }

public int TodayUpcomingClasses { get; set; }

public int TodayMissedClasses { get; set; }

public int TodayTeachers { get; set; }

public int TodayStudents { get; set; }

public List<RecentActivityDto> RecentActivities { get; set; }
    = new();

    public List<ProgramPerformanceDto> ProgramPerformance { get; set; }
    = new();
    
    public List<TodayTeacherDto> TodayTeacherList { get; set; } = new();

public List<TodayStudentDto> TodayStudentList { get; set; } = new();

public List<DashboardEnrollmentDto> MostEnrolledProgramEnrollments { get; set; } = new();

public List<DashboardEnrollmentDetailDto> RecentEnrollmentDetails { get; set; } = new();

public List<DashboardFeeDetailDto> RecentFeeDetails { get; set; } = new();

public List<DashboardRegistrationDetailDto> RecentRegistrationDetails { get; set; } = new();
}

public class TodayTeacherDto
{
    public Guid Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

public class TodayStudentDto
{
    public Guid Id { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

public class DashboardEnrollmentDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string TimeSlotName { get; set; } = string.Empty;
    public string ClassDurationName { get; set; } = string.Empty;
}

public class DashboardEnrollmentDetailDto
{
    public Guid Id { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string TeacherName { get; set; } = string.Empty;

    public string TimeSlotName { get; set; } = string.Empty;

    public string ClassDurationName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public decimal MonthlyFee { get; set; }

    public decimal FinalMonthlyFee { get; set; }

    public DateTime? NextBillingDate { get; set; }

    public string Status { get; set; } = string.Empty;
}

public class DashboardFeeDetailDto
{
    public Guid Id { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal BalanceAmount { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = string.Empty;
}

public class DashboardRegistrationDetailDto
{
    public Guid Id { get; set; }

    public string ParentName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string StudentNames { get; set; } = string.Empty;

    public int StudentCount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }
}