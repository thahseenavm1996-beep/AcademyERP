using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.ClassReports;
using AcademyERP.Persistence.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class ClassReportService : IClassReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;


    public ClassReportService(
        ApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }



    public async Task<PagedResponse<ClassReportResponse>> GetAllAsync(
        ClassReportQueryRequest request)
    {

        var query = GetClassReportQuery();


        if (request.StudentId.HasValue)
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
        .TeachingSchedule
        .Enrollment
        .StudentId == request.StudentId.Value);
}


       if (request.TeacherId.HasValue)
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
        .TeachingSchedule
        .TeacherId == request.TeacherId.Value);
}



        var totalCount = await query.CountAsync();



        var reports = await query

    .OrderByDescending(x =>
        x.ScheduledClass != null
            ? x.ScheduledClass.ClassDate
            : DateOnly.MinValue)

    .Skip((request.Page - 1) * request.PageSize)

    .Take(request.PageSize)

    .ToListAsync();



        return new PagedResponse<ClassReportResponse>
        {
            Items =
                _mapper.Map<List<ClassReportResponse>>(reports),

            Page = request.Page,

            PageSize = request.PageSize,

            TotalCount = totalCount,

            TotalPages =
                (int)Math.Ceiling(
                    totalCount /
                    (double)request.PageSize)
        };
    }




    public async Task<ClassReportResponse?> GetByIdAsync(Guid id)
    {

        var report =
            await GetClassReportQuery()
            .FirstOrDefaultAsync(x => x.Id == id);



        if (report == null)
            return null;



        return _mapper.Map<ClassReportResponse>(report);
    }




    public async Task<ClassReportResponse> CreateAsync(
        CreateClassReportRequest request)
    {

var scheduledClass =
    await _context.ScheduledClasses
    .FirstOrDefaultAsync(x =>
        x.Id == request.ScheduledClassId);


if(scheduledClass == null)
{
    throw new Exception(
        "Scheduled class not found.");
}
       var exists =
    await _context.ClassReports.AnyAsync(x =>
        x.ScheduledClassId ==
        request.ScheduledClassId);



        if (exists)
        {
            throw new InvalidOperationException(
                "A class report already exists for this class.");
        }




        var report =
            new ClassReport
            {
                ScheduledClassId =
                    request.ScheduledClassId,


                PerformanceRating =
                    request.PerformanceRating,


                HomeworkStatus =
                    request.HomeworkStatus,


                BehaviourRating =
                    request.BehaviourRating,


                ClassOutcome =
                    request.ClassOutcome,


                LessonTaken =
                    request.LessonTaken,


                NextHomework =
                    request.NextHomework,


                TeacherRemarks =
                    request.TeacherRemarks,


                ActualDurationMinutes =
                    request.ActualDurationMinutes,


                SubmittedAt =
                    DateTime.UtcNow
            };



        _context.ClassReports.Add(report);


        await _context.SaveChangesAsync();



        var result =
            await GetByIdAsync(report.Id);



        return result ??
            throw new Exception(
                "Class report saved but retrieval failed.");
    }





    public async Task<ClassReportResponse?> UpdateAsync(
        Guid id,
        UpdateClassReportRequest request)
    {

        var report =
            await _context.ClassReports
            .FirstOrDefaultAsync(x =>
                x.Id == id);



        if (report == null)
            return null;



        report.PerformanceRating =
            request.PerformanceRating;


        report.HomeworkStatus =
            request.HomeworkStatus;


        report.BehaviourRating =
            request.BehaviourRating;


        report.ClassOutcome =
            request.ClassOutcome;


        report.LessonTaken =
            request.LessonTaken;


        report.NextHomework =
            request.NextHomework;


        report.TeacherRemarks =
            request.TeacherRemarks;


        report.ActualDurationMinutes =
            request.ActualDurationMinutes;



        await _context.SaveChangesAsync();



        return await GetByIdAsync(id);
    }




    public async Task<bool> DeleteAsync(Guid id)
    {

        var report =
            await _context.ClassReports
            .FirstOrDefaultAsync(x =>
                x.Id == id);



        if (report == null)
            return false;



        _context.ClassReports.Remove(report);


        await _context.SaveChangesAsync();



        return true;
    }





   private IQueryable<ClassReport> GetClassReportQuery()
{
    return _context.ClassReports

        .AsNoTracking()

        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Teacher)


        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)


        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Course);
}
}