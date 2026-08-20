using AcademyERP.Application.DTOs.TeacherDashboard;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class TeacherDashboardService : ITeacherDashboardService
{
    private readonly ApplicationDbContext _context;

    public TeacherDashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

   public async Task<TeacherDashboardResponse> GetDashboardAsync(Guid applicationUserId)
{
    var indiaTimeZone =
    TimeZoneInfo.FindSystemTimeZoneById(
        "Asia/Kolkata");

var today =
    DateOnly.FromDateTime(
        TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            indiaTimeZone));


    // Find Teacher profile using logged-in ApplicationUserId
    var teacherId = await _context.Teachers
        .Where(x => x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();


    if (teacherId == Guid.Empty)
    {
        throw new Exception("Teacher profile not found.");
    }


    // Get today's classes assigned to this teacher
    var classes = await _context.ScheduledClasses

        .Include(x => x.TeachingSchedule)
            .ThenInclude(ts => ts.Enrollment)
                .ThenInclude(e => e.Student)

        .Include(x => x.TeachingSchedule)
            .ThenInclude(ts => ts.Enrollment)
                .ThenInclude(e => e.Course)

        .Where(x =>
            x.ClassDate == today &&
            x.TeachingSchedule.TeacherId == teacherId)

        .OrderBy(x => x.StartTime)

        .ToListAsync();



    var response = new TeacherDashboardResponse();



    response.TodayClasses = classes
        .Select(c => new TodayClassResponse
        {
            ScheduledClassId = c.Id,
Id = c.Id,
            StudentName =
                c.TeachingSchedule
                    .Enrollment
                    .Student
                    .FullName,

            CourseName =
                c.TeachingSchedule
                    .Enrollment
                    .Course
                    .CourseName,

            ClassDate = c.ClassDate,

            StartTime = c.StartTime,

            EndTime = c.EndTime,

            Status = c.Status

        })
        .ToList();



    response.Summary.TotalClassesToday =
        classes.Count;



    response.Summary.CompletedClasses =
        classes.Count(x =>
            x.Status == ScheduledClassStatus.Completed);



    response.Summary.PendingClasses =
        classes.Count(x =>
            x.Status == ScheduledClassStatus.Scheduled);



    response.Summary.MissedClasses =
        classes.Count(x =>
            x.Status == ScheduledClassStatus.Cancelled);



    if (response.Summary.TotalClassesToday > 0)
    {
        response.Summary.CompletionPercentage =
            (int)Math.Round(
                (double)response.Summary.CompletedClasses /
                response.Summary.TotalClassesToday *
                100);
    }



    return response;
}
}