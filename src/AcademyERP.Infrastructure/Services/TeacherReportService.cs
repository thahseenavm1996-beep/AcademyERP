using AcademyERP.Application.DTOs.TeacherReports;

namespace AcademyERP.Infrastructure.Services;

public class TeacherReportService
{
    public async Task<TeacherReportDashboardResponse> GetDashboardAsync()
    {
        var response = new TeacherReportDashboardResponse();

        response.TotalTeachers = 4;
        response.ActiveTeachers = 4;

        response.AveragePerformance = 94;

        response.CompletedClasses = 320;


        response.BestPerformingTeacher =
            new TeacherPerformanceDto
            {
                TeacherId = Guid.NewGuid(),
                TeacherName = "Asna Rahman",
                Score = 98,
                ClassesCompleted = 120,
                AttendancePercentage = 99
            };


        response.PerformanceTrend =
            new List<PerformanceTrendDto>
            {
                new()
                {
                    Month="Jan",
                    Score=82
                },

                new()
                {
                    Month="Feb",
                    Score=87
                },

                new()
                {
                    Month="Mar",
                    Score=91
                },

                new()
                {
                    Month="Apr",
                    Score=96
                },

                new()
                {
                    Month="May",
                    Score=98
                }
            };


        

        response.TeacherRanking = new List<TeacherRankingDto>
{
    new()
    {
        Rank = 1,
        TeacherName = "Thahseena",
        PerformanceScore = 98,
        CompletedClasses = 120,
        AttendancePercentage = 99,
        StudentsCount = 25
    },

    new()
    {
        Rank = 2,
        TeacherName = "Arif",
        PerformanceScore = 94,
        CompletedClasses = 110,
        AttendancePercentage = 97,
        StudentsCount = 22
    },

    new()
    {
        Rank = 3,
        TeacherName = "Ali",
        PerformanceScore = 91,
        CompletedClasses = 95,
        AttendancePercentage = 96,
        StudentsCount = 18
    },

    new()
    {
        Rank = 4,
        TeacherName = "Asna Rahman",
        PerformanceScore = 88,
        CompletedClasses = 80,
        AttendancePercentage = 95,
        StudentsCount = 15
    }
};
            response.JoiningTrend = new List<JoiningTrendDto>
{
    new()
    {
        Month = "Jan",
        Count = 2
    },

    new()
    {
        Month = "Feb",
        Count = 1
    },

    new()
    {
        Month = "Mar",
        Count = 3
    },

    new()
    {
        Month = "Apr",
        Count = 4
    },

    new()
    {
        Month = "May",
        Count = 2
    }
};
response.TeacherWorkload = new List<TeacherWorkloadDto>
{
    new()
    {
        TeacherName = "Thahseena",
        StudentsCount = 25,
        ActiveClasses = 120,
        WeeklyHours = 60
    },

    new()
    {
        TeacherName = "Arif",
        StudentsCount = 22,
        ActiveClasses = 110,
        WeeklyHours = 55
    },

    new()
    {
        TeacherName = "Ali",
        StudentsCount = 18,
        ActiveClasses = 95,
        WeeklyHours = 45
    },

    new()
    {
        TeacherName = "Asna Rahman",
        StudentsCount = 15,
        ActiveClasses = 80,
        WeeklyHours = 40
    }
};
response.ProgramDistribution = new List<ProgramTeacherDistributionDto>
{
    new()
    {
        ProgramName = "Quran",
        TeachersCount = 8,
        StudentsCount = 320
    },

    new()
    {
        ProgramName = "Hifz",
        TeachersCount = 5,
        StudentsCount = 80
    },

    new()
    {
        ProgramName = "Arabic",
        TeachersCount = 3,
        StudentsCount = 60
    },

    new()
    {
        ProgramName = "Fiqh",
        TeachersCount = 2,
        StudentsCount = 40
    }
};

        return response;
    }
}