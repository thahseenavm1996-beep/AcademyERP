using AcademyERP.Application.DTOs.ClassProgress;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.AcademicProgress;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class ClassProgressService : IClassProgressService
{
    private readonly ApplicationDbContext _context;

    public ClassProgressService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<ClassProgressResponse> CreateAsync(
        CreateClassProgressRequest request)
    {
        var progress = new ClassProgress
        {
            ScheduledClassId = request.ScheduledClassId,

            LessonTitle = request.LessonTitle,

            ChapterName = request.ChapterName,

            PagesCovered = request.PagesCovered,

            IsLessonCompleted = request.IsLessonCompleted,

            TeacherNotes = request.TeacherNotes,

            Homework = request.Homework,

            HomeworkGiven = request.HomeworkGiven,

            HomeworkCompleted = request.HomeworkCompleted
        };


        _context.ClassProgresses.Add(progress);

        await _context.SaveChangesAsync();


        return new ClassProgressResponse
        {
            Id = progress.Id,

            ScheduledClassId =
                progress.ScheduledClassId,

            LessonTitle =
                progress.LessonTitle,

            ChapterName =
                progress.ChapterName,

            PagesCovered =
                progress.PagesCovered,

            IsLessonCompleted =
                progress.IsLessonCompleted,

            TeacherNotes =
                progress.TeacherNotes,

            Homework =
                progress.Homework,

            HomeworkGiven =
                progress.HomeworkGiven,

            HomeworkCompleted =
                progress.HomeworkCompleted
        };
    }



    public async Task<ClassProgressResponse?> GetByScheduledClassIdAsync(
        Guid scheduledClassId)
    {
        return await _context.ClassProgresses

            .Where(x =>
                x.ScheduledClassId == scheduledClassId)

            .Select(x => new ClassProgressResponse
            {
                Id = x.Id,

                ScheduledClassId =
                    x.ScheduledClassId,

                LessonTitle =
                    x.LessonTitle,

                ChapterName =
                    x.ChapterName,

                PagesCovered =
                    x.PagesCovered,

                IsLessonCompleted =
                    x.IsLessonCompleted,

                TeacherNotes =
                    x.TeacherNotes,

                Homework =
                    x.Homework,

                HomeworkGiven =
                    x.HomeworkGiven,

                HomeworkCompleted =
                    x.HomeworkCompleted
            })

            .FirstOrDefaultAsync();
    }
}