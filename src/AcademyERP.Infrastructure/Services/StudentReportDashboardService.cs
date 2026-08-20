using AcademyERP.Application.DTOs.StudentReports;

namespace AcademyERP.Infrastructure.Services;

public class StudentReportDashboardService
{
    public async Task<StudentReportDashboardResponse> GetDashboardAsync()
    {
        var response = new StudentReportDashboardResponse();


        // Fake data for dashboard UI

        response.TotalStudents = 250;

        response.ActiveStudents = 230;

        response.AveragePerformance = 86;

        response.AverageAttendance = 92;


        response.BestStudent = new BestStudentDto
        {
            StudentName = "Muhammad Hasan",

            CourseName = "HIFZ",

            PerformanceScore = 98,

            AttendancePercentage = 100
        };
    response.PerformanceTrend =
[
    new()
    {
        Month = "Jan",
        Score = 75
    },

    new()
    {
        Month = "Feb",
        Score = 82
    },

    new()
    {
        Month = "Mar",
        Score = 90
    },

    new()
    {
        Month = "Apr",
        Score = 96
    }
];

response.AttendanceTrend =
[
    new()
    {
        Month = "Jan",
        Attendance = 88
    },

    new()
    {
        Month = "Feb",
        Attendance = 90
    },

    new()
    {
        Month = "Mar",
        Attendance = 93
    },

    new()
    {
        Month = "Apr",
        Attendance = 96
    }
];


response.CourseDistribution =
[
    new()
    {
        CourseName = "Quran",
        Students = 320
    },

    new()
    {
        CourseName = "Hifz",
        Students = 80
    },

    new()
    {
        CourseName = "Arabic",
        Students = 60
    },

    new()
    {
        CourseName = "Fiqh",
        Students = 40
    }
];


response.StudentRanking =
[
    new()
    {
        Rank = 1,
        StudentName = "Muhammad Hasan",
        CourseName = "HIFZ",
        PerformanceScore = 98,
        Attendance = 100
    },

    new()
    {
        Rank = 2,
        StudentName = "Ahmed",
        CourseName = "Quran",
        PerformanceScore = 96,
        Attendance = 98
    }
];


response.StudentStatus =
[
    new()
    {
        Status = "Active",
        Count = 230
    },

    new()
    {
        Status = "Completed",
        Count = 15
    },

    new()
    {
        Status = "Inactive",
        Count = 5
    }
];


response.LearningProgress =
[
    new()
    {
        StudentName = "Muhammad Hasan",
        CourseName = "HIFZ",
        LessonsCompleted = 85,
        PagesCovered = 240,
        ProgressPercentage = 90
    },

    new()
    {
        StudentName = "Ahmed",
        CourseName = "Quran",
        LessonsCompleted = 60,
        PagesCovered = 120,
        ProgressPercentage = 75
    },

    new()
    {
        StudentName = "Aisha",
        CourseName = "Arabic",
        LessonsCompleted = 45,
        PagesCovered = 90,
        ProgressPercentage = 65
    }
];


response.FeeStatus =
[
    new()
    {
        StudentName = "Muhammad Hasan",
        CourseName = "HIFZ",
        MonthlyFee = 3000,
        LastPaymentDate = new DateOnly(2026, 8, 10),
        NextDueDate = new DateOnly(2026, 9, 10),
        Status = "Paid"
    },

    new()
    {
        StudentName = "Ahmed",
        CourseName = "Quran",
        MonthlyFee = 2000,
        LastPaymentDate = new DateOnly(2026, 7, 15),
        NextDueDate = new DateOnly(2026, 8, 15),
        Status = "Pending"
    },

    new()
    {
        StudentName = "Aisha",
        CourseName = "Arabic",
        MonthlyFee = 2500,
        LastPaymentDate = new DateOnly(2026, 8, 5),
        NextDueDate = new DateOnly(2026, 9, 5),
        Status = "Paid"
    }
];

   response.AttendanceSummary =
new()
{
    PresentPercentage = 92,
    AbsentPercentage = 5,
    LeavePercentage = 2,
    LatePercentage = 1
};


response.FeeSummary =
new()
{
    TotalCollected = 450000,
    PendingAmount = 35000,
    PaidStudents = 230,
    PendingStudents = 20
};


response.LearningSummary =
new()
{
    AverageProgress = 78,
    TotalLessonsCompleted = 1240
};
     return response;
    }
}