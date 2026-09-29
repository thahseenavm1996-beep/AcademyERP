using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Attendance;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AttendanceService(
        ApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<PagedResponse<AttendanceResponse>> GetHistoryAsync(
        AttendanceQueryRequest request)
    {
        var query = ApplyFilters(
            GetAttendanceQuery(),
            request);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var totalCount = await query.CountAsync();

        var records = await query
            .OrderByDescending(x => x.ScheduledClass.ClassDate)
            .ThenBy(x => 
                x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .Student
                .FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        return new PagedResponse<AttendanceResponse>
        {
            Items = _mapper.Map<List<AttendanceResponse>>(records),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize)
        };
    }



    public async Task<AttendanceResponse?> GetByIdAsync(Guid id)
    {
        var attendance = await GetAttendanceQuery()
            .FirstOrDefaultAsync(x => x.Id == id);


        return attendance == null
            ? null
            : _mapper.Map<AttendanceResponse>(attendance);
    }



    public async Task<AttendanceResponse> CreateAsync(
        CreateAttendanceRequest request)
    {

        var scheduledClass =
            await _context.ScheduledClasses

            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)

            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Teacher)

            .FirstOrDefaultAsync(
                x => x.Id == request.ScheduledClassId);


        if (scheduledClass == null)
            throw new Exception(
                "Scheduled class not found.");


        var exists =
            await _context.Attendances.AnyAsync(
                x => x.ScheduledClassId ==
                     request.ScheduledClassId);


        if (exists)
            throw new InvalidOperationException(
                "Attendance already marked for this class.");



        var attendance =
            new AcademyERP.Domain.Entities.Attendance.Attendance
            {
                ScheduledClassId =
                    request.ScheduledClassId,

                Status = request.Status,

                Remarks = request.Remarks
            };


        _context.Attendances.Add(attendance);

        await _context.SaveChangesAsync();



        return await GetByIdAsync(attendance.Id)
            ?? throw new Exception(
                "Attendance saved but retrieval failed.");
    }




    public async Task<AttendanceResponse?> UpdateAsync(
        Guid id,
        UpdateAttendanceRequest request)
    {

        var attendance =
            await _context.Attendances
            .FirstOrDefaultAsync(x => x.Id == id);


        if (attendance == null)
            return null;



        attendance.Status = request.Status;
        attendance.Remarks = request.Remarks;


        await _context.SaveChangesAsync();


        return await GetByIdAsync(id);
    }




    public async Task<AttendanceSummaryResponse> GetSummaryAsync(
        AttendanceQueryRequest request)
    {

        var totals =
            await ApplyFilters(
                GetAttendanceQuery(),
                request)

            .GroupBy(x => 1)

            .Select(g => new
            {
                Total = g.Count(),

                Present =
                    g.Count(x =>
                    x.Status ==
                    AttendanceStatus.Present),

                Absent =
                    g.Count(x =>
                    x.Status ==
                    AttendanceStatus.Absent),

                Late =
                    g.Count(x =>
                    x.Status ==
                    AttendanceStatus.Late),

                Leave =
                    g.Count(x =>
                    x.Status ==
                    AttendanceStatus.Leave)

            })

            .FirstOrDefaultAsync();



        if (totals == null)
            return new AttendanceSummaryResponse();



        return new AttendanceSummaryResponse
        {
            Total = totals.Total,

            Present = totals.Present,

            Absent = totals.Absent,

            Late = totals.Late,

            Leave = totals.Leave,


            AttendanceRate =
                totals.Total == 0
                ? 0
                :
                Math.Round(
                (totals.Present + totals.Late)
                * 100m /
                totals.Total,
                1)
        };
    }




    public Task<List<AttendancePersonSummaryResponse>>
        GetStudentReportAsync(
            AttendanceQueryRequest request)
    {
        return GetPersonReportAsync(request, true);
    }




    public Task<List<AttendancePersonSummaryResponse>>
        GetTeacherReportAsync(
            AttendanceQueryRequest request)
    {
        return GetPersonReportAsync(request, false);
    }





    private async Task<List<AttendancePersonSummaryResponse>>
        GetPersonReportAsync(
            AttendanceQueryRequest request,
            bool byStudent)
    {


        var records =
            await ApplyFilters(
                GetAttendanceQuery(),
                request)

            .Select(x => new
            {

                StudentId =
                x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .StudentId,


                StudentName =
                x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .Student
                .FullName,


                TeacherId =
                x.ScheduledClass
                .TeachingSchedule
                .TeacherId,


                TeacherName =
                x.ScheduledClass
                .TeachingSchedule
                .Teacher
                .FullName,


                Status = x.Status

            })

            .ToListAsync();




        if (byStudent)
        {
            return records

            .GroupBy(x =>
            new
            {
                x.StudentId,
                x.StudentName
            })

            .Select(x =>
                BuildPersonSummary(
                    x.Key.StudentId,
                    x.Key.StudentName,
                    x.Select(y => y.Status)))

            .ToList();
        }



        return records

        .GroupBy(x =>
        new
        {
            x.TeacherId,
            x.TeacherName
        })

        .Select(x =>
            BuildPersonSummary(
                x.Key.TeacherId,
                x.Key.TeacherName,
                x.Select(y => y.Status)))

        .ToList();

    }





  private static AttendancePersonSummaryResponse
    BuildPersonSummary(
        Guid id,
        string name,
        IEnumerable<AttendanceStatus> statuses)
{
    var values = statuses.ToList();

    var present =
        values.Count(x =>
        x == AttendanceStatus.Present);

    var late =
        values.Count(x =>
        x == AttendanceStatus.Late);

    var consideredDays = values.Count;

    return new AttendancePersonSummaryResponse
    {
        PersonId = id,
        PersonName = name,

        Total = values.Count,

        Present = present,

        Absent =
            values.Count(x =>
            x == AttendanceStatus.Absent),

        Late = late,

        Leave =
            values.Count(x =>
            x == AttendanceStatus.Leave),

        AttendanceRate =
            consideredDays == 0
            ? 0
            :
            Math.Round(
                (present + late) * 100m / consideredDays,
                1)
    };
}





    private IQueryable<
        AcademyERP.Domain.Entities.Attendance.Attendance>
        GetAttendanceQuery()
    {

        return _context.Attendances

        .AsNoTracking()

        .Include(x => x.ScheduledClass)

            .ThenInclude(x =>
                x.TeachingSchedule)
.ThenInclude(x => x.ClassDuration)

.Include(x => x.ScheduledClass)
    .ThenInclude(x => x.TeachingSchedule)
    .ThenInclude(x => x.Teacher)

        .Include(x => x.ScheduledClass)

            .ThenInclude(x =>
                x.TeachingSchedule)

                .ThenInclude(x =>
                    x.Enrollment)

                    .ThenInclude(x =>
                        x.Student)


        .Include(x => x.ScheduledClass)

            .ThenInclude(x =>
                x.TeachingSchedule)

                .ThenInclude(x =>
                    x.Enrollment)

                    .ThenInclude(x =>
                        x.Course)

                        .ThenInclude(x =>
                            x.Program);

    }





    private static IQueryable<
        AcademyERP.Domain.Entities.Attendance.Attendance>
        ApplyFilters(
        IQueryable<
        AcademyERP.Domain.Entities.Attendance.Attendance>
        query,
        AttendanceQueryRequest request)
    {


        if(request.TeacherId.HasValue)

            query =
            query.Where(x =>
            x.ScheduledClass
            .TeachingSchedule
            .TeacherId
            ==
            request.TeacherId.Value);



        if(request.StudentId.HasValue)

            query =
            query.Where(x =>
            x.ScheduledClass
            .TeachingSchedule
            .Enrollment
            .StudentId
            ==
            request.StudentId.Value);



        if (request.Status.HasValue)
    query = query.Where(x =>
        x.Status == request.Status.Value);

if (request.ProgramId.HasValue)
    query = query.Where(x =>
        x.ScheduledClass
            .TeachingSchedule
            .Enrollment
            .Course
            .ProgramId == request.ProgramId.Value);

if (request.FromDate.HasValue)
    query = query.Where(x =>
        x.ScheduledClass.ClassDate >=
        DateOnly.FromDateTime(request.FromDate.Value));

if (request.ToDate.HasValue)
    query = query.Where(x =>
        x.ScheduledClass.ClassDate <=
        DateOnly.FromDateTime(request.ToDate.Value));

return query;
    }
public async Task<AttendanceResponse?> GetByEnrollmentDateAsync(
    Guid enrollmentId,
    DateTime date)
{
    var attendance = await GetAttendanceQuery()
        .FirstOrDefaultAsync(x =>
            x.ScheduledClass.TeachingSchedule.EnrollmentId == enrollmentId
            &&
            x.ScheduledClass.ClassDate == DateOnly.FromDateTime(date));

    return attendance == null
        ? null
        : _mapper.Map<AttendanceResponse>(attendance);
}

public async Task<List<
    AcademyERP.Application.DTOs.AttendanceAlerts.StudentAbsenceAlertResponse>>
    GetStudentAbsenceAlertsAsync(int minimumConsecutiveClasses = 2)
{
    var attendances = await _context.Attendances
        .AsNoTracking()
        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
                        .ThenInclude(x => x.StudentParents)
                            .ThenInclude(x => x.Parent)
                            .Include(x => x.ScheduledClass)
    .ThenInclude(x => x.TeachingSchedule)
        .ThenInclude(x => x.Enrollment)
            .ThenInclude(x => x.Course)
        .Where(x =>
            x.ScheduledClass.TeachingSchedule.Enrollment.Student != null)
        .OrderByDescending(x => x.ScheduledClass.ClassDate)
        .ThenByDescending(x => x.ScheduledClass.StartTime)
        .ToListAsync();

    var results = new List<
        AcademyERP.Application.DTOs.AttendanceAlerts.StudentAbsenceAlertResponse>();

    var grouped = attendances
        .GroupBy(x => new
        {
            StudentId =
                x.ScheduledClass.TeachingSchedule.Enrollment.Student.Id,

            EnrollmentId =
                x.ScheduledClass.TeachingSchedule.Enrollment.Id
        });

    foreach (var group in grouped)
    {
        var ordered = group
            .OrderByDescending(x => x.ScheduledClass.ClassDate)
            .ThenByDescending(x => x.ScheduledClass.StartTime)
            .ToList();

        if (!ordered.Any())
            continue;

        var latest = ordered.First();

        // No active absence streak if the latest class was attended.
        if (latest.Status != AttendanceStatus.Absent &&
            latest.Status != AttendanceStatus.Leave)
        {
            continue;
        }

        var consecutive = 0;

        foreach (var attendance in ordered)
        {
            if (attendance.Status == AttendanceStatus.Absent ||
                attendance.Status == AttendanceStatus.Leave)
            {
                consecutive++;
                continue;
            }

            // Present/Late breaks the current consecutive streak.
            break;
        }

        if (consecutive < minimumConsecutiveClasses)
            continue;

        var enrollment =
            latest.ScheduledClass.TeachingSchedule.Enrollment;

        var student = enrollment.Student;

        var parent = student.StudentParents
            .OrderByDescending(x => x.IsPrimaryContact)
            .FirstOrDefault();

        var teacher = await _context.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == latest.ScheduledClass.TeachingSchedule.TeacherId);

        var lastAttendedDate = await _context.Attendances
            .AsNoTracking()
            .Where(x =>
                x.ScheduledClass.TeachingSchedule.Enrollment.Id ==
                    enrollment.Id &&
                (x.Status == AttendanceStatus.Present ||
                 x.Status == AttendanceStatus.Late) &&
                (
                    x.ScheduledClass.ClassDate <
                        latest.ScheduledClass.ClassDate
                    ||
                    (
                        x.ScheduledClass.ClassDate ==
                            latest.ScheduledClass.ClassDate &&
                        x.ScheduledClass.StartTime <
                            latest.ScheduledClass.StartTime
                    )
                ))
            .OrderByDescending(x => x.ScheduledClass.ClassDate)
            .ThenByDescending(x => x.ScheduledClass.StartTime)
            .Select(x => (DateOnly?)x.ScheduledClass.ClassDate)
            .FirstOrDefaultAsync();

        results.Add(
            new AcademyERP.Application.DTOs.AttendanceAlerts
                .StudentAbsenceAlertResponse
            {
                StudentId = student.Id,
                StudentName = student.FullName,

                ParentId = parent?.ParentId,
                ParentName = parent?.Parent?.FullName ?? string.Empty,
               ParentPhoneNumber = student.PhoneNumber,

                TeacherId =
                    latest.ScheduledClass.TeachingSchedule.TeacherId,

                TeacherName =
                    teacher?.FullName ?? string.Empty,

                CourseName =
                    enrollment.Course?.CourseName ?? string.Empty,

                ConsecutiveMissedClasses = consecutive,

                LastMissedDate =
                    latest.ScheduledClass.ClassDate,

                LastAttendedDate = lastAttendedDate,

                LastAttendanceStatus =
                    latest.Status.ToString(),

                AlertLevel = consecutive >= 3
                    ? "Critical"
                    : "Warning"
            });
    }

    return results
        .OrderByDescending(x => x.ConsecutiveMissedClasses)
        .ThenByDescending(x => x.LastMissedDate)
        .ToList();
}

}