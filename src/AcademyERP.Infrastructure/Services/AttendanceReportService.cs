using AcademyERP.Application.DTOs.AttendanceReports;
using AcademyERP.Application.Services;

namespace AcademyERP.Infrastructure.Services;

public class AttendanceReportService : IAttendanceReportService
{
    public async Task<AttendanceReportDashboardResponse> GetDashboardAsync()
    {
        var response = new AttendanceReportDashboardResponse();


        // Temporary dashboard data
        // Will be replaced with EF Core queries later


        response.TotalClasses = 1200;

        response.CompletedClasses = 1050;

        response.TotalAttendanceRecords = 980;

        response.AverageAttendance = 93;


        response.AttendanceTrend =
        [
            new()
            {
                Month = "Jan",
                Percentage = 88
            },

            new()
            {
                Month = "Feb",
                Percentage = 91
            },

            new()
            {
                Month = "Mar",
                Percentage = 89
            },

            new()
            {
                Month = "Apr",
                Percentage = 95
            }
        ];


        response.StatusDistribution =
        [
            new()
            {
                Status = "Present",
                Count = 920
            },

            new()
            {
                Status = "Absent",
                Count = 50
            },

            new()
            {
                Status = "Late",
                Count = 20
            },

            new()
            {
                Status = "Leave",
                Count = 10
            }
        ];


        response.CourseAttendance =
        [
            new()
            {
                CourseName = "Quran",
                AttendancePercentage = 95
            },

            new()
            {
                CourseName = "Hifz",
                AttendancePercentage = 92
            },

            new()
            {
                CourseName = "Arabic",
                AttendancePercentage = 88
            },

            new()
            {
                CourseName = "Fiqh",
                AttendancePercentage = 90
            }
        ];


        response.StudentsNeedingAttention =
        [
            new()
            {
                StudentName = "Ahmed",
                CourseName = "Quran",
                AttendancePercentage = 62
            },

            new()
            {
                StudentName = "Fatima",
                CourseName = "Hifz",
                AttendancePercentage = 68
            }
        ];


        response.TeacherSummary =
        [
            new()
            {
                TeacherName = "Abdullah",
                TotalClasses = 120,
                AttendancePercentage = 96
            },

            new()
            {
                TeacherName = "Asna",
                TotalClasses = 100,
                AttendancePercentage = 94
            }
        ];


        return response;
    }
}