using AcademyERP.Application.DTOs.ParentPortal;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class ParentPortalService : IParentPortalService
{
    private readonly ApplicationDbContext _context;


    public ParentPortalService(
        ApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<ParentDashboardResponse?> GetDashboardAsync(
        Guid applicationUserId,
        string parentName)
    {

        var parent = await _context.Parents
            .Include(x => x.StudentParents)
                .ThenInclude(x => x.Student)
                    .ThenInclude(x => x.Enrollments)
                        .ThenInclude(x => x.Course)

            .Include(x => x.StudentParents)
                .ThenInclude(x => x.Student)
                    .ThenInclude(x => x.Enrollments)
                        .ThenInclude(x => x.Teacher)

            .FirstOrDefaultAsync(x =>
                x.ApplicationUserId == applicationUserId);



        if (parent == null)
            return null;



        var response = new ParentDashboardResponse
        {
            ParentName = parentName,

            ChildrenCount =
                parent.StudentParents.Count
        };



        foreach (var studentParent in parent.StudentParents)
        {

            var student = studentParent.Student;


            var enrollment =
                student.Enrollments
                .FirstOrDefault(x =>
                    x.Status == EnrollmentStatus.Active);



            if (enrollment == null)
                continue;



            var attendanceRecords =
                await _context.Attendances

                .Where(a =>
                    a.ScheduledClass
                     .TeachingSchedule
                     .EnrollmentId == enrollment.Id)

                .ToListAsync();



            var totalAttendance =
                attendanceRecords.Count;



            var presentCount =
                attendanceRecords.Count(x =>
                    x.Status == AttendanceStatus.Present
                    ||
                    x.Status == AttendanceStatus.Late);



            decimal attendancePercentage = 0;


            if(totalAttendance > 0)
            {
                attendancePercentage =
                    Math.Round(
                        (decimal)presentCount /
                        totalAttendance * 100,
                        2);
            }




            var progressRecords =
                await _context.ClassProgresses

                .Where(x =>
                    x.ScheduledClass
                     .TeachingSchedule
                     .EnrollmentId == enrollment.Id)

                .ToListAsync();



            decimal progressPercentage = 0;


            if(progressRecords.Any())
            {
                var completed =
                    progressRecords.Count(x =>
                        x.IsLessonCompleted);


                progressPercentage =
                    Math.Round(
                        (decimal)completed /
                        progressRecords.Count * 100,
                        2);
            }




            response.Children.Add(
    new ParentChildSummaryDto
    {
        StudentId = student.Id,

        StudentName = student.FullName,

        CourseName =
            enrollment.Course.CourseName,

        TeacherName =
            enrollment.Teacher?.FullName
            ?? "Not Assigned",

        AttendancePercentage =
            attendancePercentage,

        ProgressPercentage =
            progressPercentage,

        HasAttendanceRecords =
            totalAttendance > 0,

        HasProgressRecords =
            progressRecords.Any()
    });
        }



        response.PendingFeeAmount =
            await _context.FeeInvoices

            .Where(x =>
                x.Enrollment.Student.StudentParents
                .Any(sp =>
                    sp.ParentId == parent.Id))

            .SumAsync(x =>
                x.BalanceAmount);



        response.UpcomingClasses =
    await _context.ScheduledClasses

    .Where(x =>
        x.ClassDate >= DateOnly.FromDateTime(DateTime.Today)

        &&

        x.TeachingSchedule.Enrollment.Student
        .StudentParents
        .Any(sp =>
            sp.ParentId == parent.Id))

    .OrderBy(x => x.ClassDate)
    .ThenBy(x => x.StartTime)

    .Take(5)

   .Select(x => new UpcomingClassDto
{
    ClassDate =
        x.ClassDate.ToString("dd MMM yyyy"),

    StudentName =
        x.TeachingSchedule
         .Enrollment
         .Student
         .FullName,

    CourseName =
        x.TeachingSchedule
         .Enrollment
         .Course
         .CourseName,

    TeacherName =
        x.TeachingSchedule
         .Teacher
         .FullName,

    StartTime =
        x.StartTime.ToString("hh:mm tt")
})

    .ToListAsync();



        return response;
    }
}