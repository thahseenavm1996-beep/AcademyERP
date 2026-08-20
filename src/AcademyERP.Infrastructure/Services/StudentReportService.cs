using AcademyERP.Application.DTOs.StudentReports;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class StudentReportService : IStudentReportService
{
    private readonly ApplicationDbContext _context;

    public StudentReportService(
        ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<StudentReportResponse?> GetStudentReportAsync(
        Guid studentId)
    {
        var student =
            await _context.Students
            .FirstOrDefaultAsync(x => x.Id == studentId);


        if (student == null)
            return null;


        var enrollments =
            await _context.Enrollments

            .Include(x => x.Course)

            .Include(x => x.Teacher)

            .Where(x =>
                x.StudentId == studentId)

            .ToListAsync();


        var attendance =
            await _context.Attendances

            .Include(x =>
                x.ScheduledClass)

            .Where(x =>
                x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .StudentId == studentId)

            .ToListAsync();



        var progress =
            await _context.ClassProgresses

            .Where(x =>
                x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .StudentId == studentId)

            .ToListAsync();



        return new StudentReportResponse
        {
            StudentId = student.Id,

            StudentName = student.FullName,


            Courses =
                enrollments
                .Select(x => new StudentCourseReportResponse
                {
                    CourseName =
                        x.Course.CourseName,

                    TeacherName =
                        x.Teacher.FullName,

                    StartDate =
    DateOnly.FromDateTime(x.StartDate),

                    Status = "Active"

                })
                .ToList(),



            Attendance =
            new AttendanceSummaryResponse
            {
                TotalClasses =
                    attendance.Count,

                Present =
                    attendance.Count(x =>
                    x.Status ==
                    Domain.Enums.AttendanceStatus.Present),

                Absent =
                    attendance.Count(x =>
                    x.Status ==
                    Domain.Enums.AttendanceStatus.Absent),

                Late =
                    attendance.Count(x =>
                    x.Status ==
                    Domain.Enums.AttendanceStatus.Late),

                Leave =
                    attendance.Count(x =>
                    x.Status ==
                    Domain.Enums.AttendanceStatus.Leave)
            },


            Progress =
            new LearningProgressSummaryResponse
            {
                TotalLessons =
                    progress.Count,

                CompletedLessons =
                    progress.Count(x =>
                    x.IsLessonCompleted),

                ChaptersCompleted =
                    progress.Count(x =>
                    x.IsLessonCompleted),

                PagesCovered =
                    string.Join(", ",
                    progress.Select(x =>
                    x.PagesCovered))
            }

        };
       
    }
}