using AcademyERP.Application.DTOs.StudentPerformance;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Application.DTOs.Students;

namespace AcademyERP.Infrastructure.Services;

public class StudentPerformanceService 
    : IStudentPerformanceService
{
    private readonly ApplicationDbContext _context;


    public StudentPerformanceService(
        ApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<StudentPerformanceResponse> GetAsync(
        Guid studentId)
    {

        var student =
            await _context.Students
            .FirstOrDefaultAsync(x => x.Id == studentId);


        if(student == null)
            throw new Exception("Student not found");



        var classes =
            await _context.ScheduledClasses
            .Where(x =>
                x.TeachingSchedule.Enrollment.StudentId 
                == studentId)
            .ToListAsync();



        var attendance =
            await _context.Attendances
            .Where(x =>
                x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .StudentId == studentId)
            .ToListAsync();



       var progress = await _context.ClassProgresses
    .Include(x => x.ScheduledClass)
    .Where(x =>
        x.ScheduledClass
        .TeachingSchedule
        .Enrollment
        .StudentId == studentId)
    .OrderByDescending(x => x.CreatedAt)
    .ToListAsync();


        var response =
            new StudentPerformanceResponse();


        response.StudentName =
            student.FullName;



        response.TotalClasses =
            classes.Count;



        response.CompletedClasses =
            classes.Count(x =>
                x.Status ==
                ScheduledClassStatus.Completed);



       // Total planned classes
response.TotalClasses = classes.Count;


// Completed classes
response.CompletedClasses =
    classes.Count(x =>
        x.Status == ScheduledClassStatus.Completed);


// Only classes that already happened
var conductedClasses = classes
    .Where(x => 
        x.ClassDate <= DateOnly.FromDateTime(DateTime.Today))
    .ToList();


// Attendance only for conducted classes
var conductedClassIds = conductedClasses
    .Select(x => x.Id)
    .ToList();


var conductedAttendance = attendance
    .Where(x => conductedClassIds.Contains(x.ScheduledClassId))
    .ToList();


if (conductedAttendance.Any())
{
    response.AttendancePercentage =
        Math.Round(
            conductedAttendance.Count(x =>
                x.Status == AttendanceStatus.Present)
            /
            (decimal)conductedAttendance.Count
            * 100,
            2);
}
else
{
    response.AttendancePercentage = 0;
}



        response.LearningScore =
            progress.Any()
            ?
            Math.Round(
                progress.Count(x =>
                x.IsLessonCompleted)
                /
                (decimal)progress.Count
                * 100,
                2)
            :
            0;



        response.HomeworkPercentage =
            progress.Any()
            ?
            Math.Round(
                progress.Count(x =>
                x.HomeworkCompleted)
                /
                (decimal)progress.Count
                * 100,
                2)
            :
            0;



        response.Strength =
            response.LearningScore >= 80
            ?
            "Good learning progress"
            :
            "Regular improvement needed";



        response.ImprovementArea =
            response.HomeworkPercentage < 70
            ?
            "Homework completion"
            :
            "Keep maintaining performance";



        response.RecentProgress =
            progress
            .Take(5)
            .Select(x => new RecentProgressResponse
            {
                Date =
                    DateOnly.FromDateTime(
                        x.CreatedAt),

                LessonTitle =
                    x.LessonTitle ?? "",

                Completed =
                    x.IsLessonCompleted

            })
            .ToList();



        response.TeacherFeedback =
            progress
            .Where(x =>
                !string.IsNullOrWhiteSpace(
                    x.TeacherNotes))
            .Select(x =>
                x.TeacherNotes!)
            .Take(5)
            .ToList();

response.MonthlyTrend =
    progress
.GroupBy(x => new
{
    x.ScheduledClass.ClassDate.Year,
    x.ScheduledClass.ClassDate.Month
})
    .Select(x => new StudentTrendResponse
    {
       Month =
    new DateTime(
        x.Key.Year,
        x.Key.Month,
        1,
        0,
        0,
        0)
    .ToString("MMM"),


        Score =
            x.Count() == 0
            ? 0
            :
            Math.Round(
                x.Count(y => y.IsLessonCompleted)
                * 100m /
                x.Count(),
                1)
    })
    .OrderBy(x => x.Month)
    .ToList();
    response.WeeklyTrend = new List<WeeklyPerformanceDto>
{
    new()
    {
        Week = "Week 1",
        AttendancePercentage = 80,
        LearningScore = 75,
        HomeworkPercentage = 90,
        OverallScore = 82
    },

    new()
    {
        Week = "Week 2",
        AttendancePercentage = 85,
        LearningScore = 82,
        HomeworkPercentage = 92,
        OverallScore = 86
    },

    new()
    {
        Week = "Week 3",
        AttendancePercentage = 95,
        LearningScore = 90,
        HomeworkPercentage = 96,
        OverallScore = 94
    },

    new()
    {
        Week = "Week 4",
        AttendancePercentage = 100,
        LearningScore = 95,
        HomeworkPercentage = 100,
        OverallScore = 98
    }
};

        return response;
    }
}