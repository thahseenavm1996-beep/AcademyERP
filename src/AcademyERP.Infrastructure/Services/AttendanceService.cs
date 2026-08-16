using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Attendance;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Attendance;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AttendanceService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResponse<AttendanceResponse>> GetHistoryAsync(AttendanceQueryRequest request)
    {
        var query = ApplyFilters(GetAttendanceQuery(), request);
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var totalCount = await query.CountAsync();
        var records = await query
            .OrderByDescending(x => x.AttendanceDate)
            .ThenBy(x => x.Enrollment.Student.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<AttendanceResponse>
        {
            Items = _mapper.Map<List<AttendanceResponse>>(records),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<AttendanceResponse?> GetByIdAsync(Guid id)
    {
        var record = await GetAttendanceQuery().FirstOrDefaultAsync(x => x.Id == id);
        return record is null ? null : _mapper.Map<AttendanceResponse>(record);
    }

    public async Task<AttendanceResponse> CreateAsync(CreateAttendanceRequest request)
    {
        var date = request.AttendanceDate.Date;
        var enrollment = await _context.Enrollments
            .Include(x => x.Student)
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == request.EnrollmentId);

        if (enrollment is null)
            throw new ArgumentException("The selected enrollment was not found.");

        if (enrollment.TeacherId is null || enrollment.Teacher is null)
            throw new ArgumentException("Attendance cannot be marked until a teacher is assigned to this enrollment.");

        var exists = await _context.Attendances.AnyAsync(x =>
    x.EnrollmentId == request.EnrollmentId &&
    x.AttendanceDate.Date == date);

        if (exists)
            throw new InvalidOperationException("Attendance has already been marked for this student and date. Edit the existing record instead.");

       var record = new AcademyERP.Domain.Entities.Attendance.Attendance
{
    EnrollmentId = enrollment.Id,
    AttendanceDate = date,
    Status = request.Status,
    Remarks = request.Remarks
};

       _context.Attendances.Add(record);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(record.Id)
            ?? throw new InvalidOperationException("Attendance was saved but could not be retrieved.");
    }

    public async Task<AttendanceResponse?> UpdateAsync(Guid id, UpdateAttendanceRequest request)
    {
       var record = await _context.Attendances.FirstOrDefaultAsync(x => x.Id == id);
        if (record is null)
            return null;

       record.Status = request.Status;
record.Remarks = request.Remarks;
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<AttendanceSummaryResponse> GetSummaryAsync(AttendanceQueryRequest request)
    {
        var totals = await ApplyFilters(GetAttendanceQuery(), request)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Total = group.Count(),
               Present = group.Count(x => x.Status == AttendanceStatus.Present),
                Absent = group.Count(x => x.Status == AttendanceStatus.Absent),
Late = group.Count(x => x.Status == AttendanceStatus.Late),
Leave = group.Count(x => x.Status == AttendanceStatus.Leave)
            })
            .FirstOrDefaultAsync();

        if (totals is null)
            return new AttendanceSummaryResponse();

        return new AttendanceSummaryResponse
        {
            Total = totals.Total,
            Present = totals.Present,
            Absent = totals.Absent,
            Late = totals.Late,
            Leave = totals.Leave,
            AttendanceRate = Math.Round((totals.Present + totals.Late) * 100m / totals.Total, 1)
        };
    }

    public Task<List<AttendancePersonSummaryResponse>> GetStudentReportAsync(AttendanceQueryRequest request) =>
        GetPersonReportAsync(request, true);

    public Task<List<AttendancePersonSummaryResponse>> GetTeacherReportAsync(AttendanceQueryRequest request) =>
        GetPersonReportAsync(request, false);

    private async Task<List<AttendancePersonSummaryResponse>> GetPersonReportAsync(
        AttendanceQueryRequest request, bool byStudent)
    {
       var records = await ApplyFilters(GetAttendanceQuery(), request)
    .Select(x => new
    {
        StudentId = x.Enrollment.StudentId,
        StudentName = x.Enrollment.Student.FullName,

        TeacherId = x.Enrollment.TeacherId!.Value,
        TeacherName = x.Enrollment.Teacher!.FullName,

        Status = x.Status
    })
    .ToListAsync();

                if (byStudent)
        {
            return records
                .GroupBy(x => new { x.StudentId, x.StudentName })
                .Select(x => BuildPersonSummary(
                    x.Key.StudentId,
                    x.Key.StudentName,
                    x.Select(r => r.Status)))
                .OrderByDescending(x => x.AttendanceRate)
                .ThenBy(x => x.PersonName)
                .ToList();
        }

        return records
            .GroupBy(x => new { x.TeacherId, x.TeacherName })
            .Select(x => BuildPersonSummary(
                x.Key.TeacherId,
                x.Key.TeacherName,
                x.Select(r => r.Status)))
            .OrderByDescending(x => x.AttendanceRate)
            .ThenBy(x => x.PersonName)
            .ToList();
    }

    private static AttendancePersonSummaryResponse BuildPersonSummary(
        Guid personId, string personName, IEnumerable<AttendanceStatus> statuses)
    {
        var values = statuses.ToList();
        var present = values.Count(x => x == AttendanceStatus.Present);
        var late = values.Count(x => x == AttendanceStatus.Late);
        return new AttendancePersonSummaryResponse
        {
            PersonId = personId,
            PersonName = personName,
            Total = values.Count,
            Present = present,
            Absent = values.Count(x => x == AttendanceStatus.Absent),
            Late = late,
            Leave = values.Count(x => x == AttendanceStatus.Leave),
            AttendanceRate = values.Count == 0 ? 0 : Math.Round((present + late) * 100m / values.Count, 1)
        };
    }

    private IQueryable<AcademyERP.Domain.Entities.Attendance.Attendance> GetAttendanceQuery() =>
    _context.Attendances
        .AsNoTracking()
        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Student)
        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Teacher)
        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Course)
                .ThenInclude(x => x.Program);

    private static IQueryable<AcademyERP.Domain.Entities.Attendance.Attendance> ApplyFilters(
    IQueryable<AcademyERP.Domain.Entities.Attendance.Attendance> query,
    AttendanceQueryRequest request)
    {
        if (request.StudentId.HasValue)
    query = query.Where(x => x.Enrollment.StudentId == request.StudentId.Value);
        if (request.TeacherId.HasValue)
    query = query.Where(x => x.Enrollment.TeacherId == request.TeacherId.Value);
        if (request.ProgramId.HasValue)
            query = query.Where(x => x.Enrollment.Course.ProgramId == request.ProgramId.Value);
        if (request.Status.HasValue)
    query = query.Where(x => x.Status == request.Status.Value);
       if (request.FromDate.HasValue)
    query = query.Where(x => x.AttendanceDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue)
    query = query.Where(x => x.AttendanceDate < request.ToDate.Value.Date.AddDays(1));
        return query;
    }
    public async Task<AttendanceResponse?> GetByEnrollmentDateAsync(
    Guid enrollmentId,
    DateTime date)
{
    var attendance = await GetAttendanceQuery()
        .FirstOrDefaultAsync(x =>
            x.EnrollmentId == enrollmentId &&
            x.AttendanceDate.Date == date.Date);

    return attendance == null
        ? null
        : _mapper.Map<AttendanceResponse>(attendance);
}
}
