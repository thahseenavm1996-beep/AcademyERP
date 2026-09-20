using AcademyERP.Application.DTOs.Enrollments;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.EnrollmentSchedules;
using AcademyERP.Application.DTOs.Enrollments;
using AcademyERP.Application.DTOs.EnrollmentSchedules;
using AcademyERP.Domain.Entities.TeachingSchedules;
using AcademyERP.Application.DTOs.TeachingSchedules;
using AcademyERP.Domain.Entities.TeacherAvailabilities;

namespace AcademyERP.Infrastructure.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;
private readonly ITeachingScheduleService _teachingScheduleService;
    public EnrollmentService(
    ApplicationDbContext context,
    ITeachingScheduleService teachingScheduleService)
{
    _context = context;
    _teachingScheduleService = teachingScheduleService;
}

   public async Task<List<EnrollmentResponse>> GetAllAsync()
{
    return await _context.Enrollments
        .AsNoTracking()
        .Include(x => x.Student)
        .Include(x => x.Course)
        .Include(x => x.Teacher)
        .OrderByDescending(x => x.StartDate)
        .Select(x => new EnrollmentResponse
        {
            Id = x.Id,

            StudentId = x.StudentId,
            StudentName = x.Student.FullName,

            CourseId = x.CourseId,
            CourseName = x.Course.CourseName,

            TeacherId = x.TeacherId,
            TeacherName = x.Teacher != null
                ? x.Teacher.FullName
                : null,


            StartDate = x.StartDate,
            EndDate = x.EndDate,
Gender = x.Student.Gender.ToString(),
            MonthlyFee = x.MonthlyFee,

            ScholarshipAmount = x.ScholarshipAmount,
            ScholarshipReason = x.ScholarshipReason,

            DiscountAmount = x.DiscountAmount,
            DiscountReason = x.DiscountReason,

            FinalMonthlyFee = x.FinalMonthlyFee,

            NextBillingDate = x.NextBillingDate,

            IsFeePaused = x.FeePauses.Any(p => !p.ResumeDate.HasValue),
            FeePauseStartDate = x.FeePauses
                .Where(p => !p.ResumeDate.HasValue)
                .OrderByDescending(p => p.PauseStartDate)
                .Select(p => (DateTime?)p.PauseStartDate)
                .FirstOrDefault(),
            FeePauseReason = x.FeePauses
                .Where(p => !p.ResumeDate.HasValue)
                .OrderByDescending(p => p.PauseStartDate)
                .Select(p => p.Reason)
                .FirstOrDefault(),

            Status = x.Status,

            Remarks = x.Remarks,
TimeSlotName = x.EnrollmentSchedules
    .Select(s => s.TimeSlot.Name)
    .FirstOrDefault(),

ClassDurationName = x.EnrollmentSchedules
    .Select(s => s.ClassDuration.Name)
    .FirstOrDefault(),

            Schedules = x.EnrollmentSchedules
                .Select(s => new EnrollmentScheduleResponse
                {
                    Id = s.Id,

                    DayOfWeek = s.DayOfWeek,

                    TimeSlotId = s.TimeSlotId,

                    TimeSlotName = s.TimeSlot.Name,

                    ClassDurationId = s.ClassDurationId,

                    ClassDurationName = s.ClassDuration.Name
                })
                .ToList()
        })
        .ToListAsync();
}
    public async Task<EnrollmentResponse?> GetByIdAsync(Guid id)
{
    var result = await _context.Enrollments
        .AsNoTracking()
        .Where(x => x.Id == id)
        .Select(x => new EnrollmentResponse
        {
            Id = x.Id,

            StudentId = x.StudentId,
            StudentName = x.Student.FullName,

            CourseId = x.CourseId,
            CourseName = x.Course.CourseName,

            TeacherId = x.TeacherId,
            TeacherName = x.Teacher != null
                ? x.Teacher.FullName
                : null,

            StartDate = x.StartDate,
            EndDate = x.EndDate,

            MonthlyFee = x.MonthlyFee,

            ScholarshipAmount = x.ScholarshipAmount,
            ScholarshipReason = x.ScholarshipReason,

            DiscountAmount = x.DiscountAmount,
            DiscountReason = x.DiscountReason,

            FinalMonthlyFee = x.FinalMonthlyFee,

            NextBillingDate = x.NextBillingDate,

            IsFeePaused = x.FeePauses.Any(p => !p.ResumeDate.HasValue),
            FeePauseStartDate = x.FeePauses
                .Where(p => !p.ResumeDate.HasValue)
                .OrderByDescending(p => p.PauseStartDate)
                .Select(p => (DateTime?)p.PauseStartDate)
                .FirstOrDefault(),
            FeePauseReason = x.FeePauses
                .Where(p => !p.ResumeDate.HasValue)
                .OrderByDescending(p => p.PauseStartDate)
                .Select(p => p.Reason)
                .FirstOrDefault(),

            Status = x.Status,

            Remarks = x.Remarks,
TimeSlotName = x.EnrollmentSchedules
    .Select(s => s.TimeSlot.Name)
    .FirstOrDefault(),

ClassDurationName = x.EnrollmentSchedules
    .Select(s => s.ClassDuration.Name)
    .FirstOrDefault(),

            Schedules = x.EnrollmentSchedules
                .Select(s => new EnrollmentScheduleResponse
                {
                    Id = s.Id,

                    DayOfWeek = s.DayOfWeek,

                    TimeSlotId = s.TimeSlotId,

                    TimeSlotName = s.TimeSlot.Name,

                    ClassDurationId = s.ClassDurationId,

                    ClassDurationName = s.ClassDuration.Name
                })
                .ToList()
        })
        .FirstOrDefaultAsync();


    return result;
}
    public async Task<EnrollmentResponse> CreateAsync(
    CreateEnrollmentRequest request)
{
    ValidateFees(
        request.MonthlyFee,
        request.ScholarshipAmount,
        request.DiscountAmount);

    if (request.Schedules == null || !request.Schedules.Any())
    {
        throw new ArgumentException(
            "At least one class schedule is required.");
    }

    if (!request.TeacherId.HasValue)
    {
        throw new ArgumentException(
            "A teacher must be assigned before an enrollment can be created.");
    }
await ValidateEnrollmentSchedulesAsync(
    request.TeacherId.Value,
    request.Schedules,
    request.StartDate,
    request.EndDate);

    var enrollment = new Enrollment
    {
        StudentId = request.StudentId,

        CourseId = request.CourseId,

        TeacherId = request.TeacherId,

        StartDate = request.StartDate,

        EndDate = request.EndDate,


        MonthlyFee = request.MonthlyFee,

        ScholarshipAmount = request.ScholarshipAmount,

        ScholarshipReason = request.ScholarshipReason,


        DiscountAmount = request.DiscountAmount,

        DiscountReason = request.DiscountReason,


        FinalMonthlyFee =
            request.MonthlyFee
            - request.ScholarshipAmount
            - request.DiscountAmount,


        // Billing follows enrollment start date
        NextBillingDate =
            request.StartDate.AddMonths(1),


        Remarks = request.Remarks
    };


    foreach (var schedule in request.Schedules)
    {
        enrollment.EnrollmentSchedules.Add(
            new EnrollmentSchedule
            {
                DayOfWeek = schedule.DayOfWeek,

                TimeSlotId = schedule.TimeSlotId,

                ClassDurationId = schedule.ClassDurationId,

                IsActive = true
            });
    }


    _context.Enrollments.Add(enrollment);


    await _context.SaveChangesAsync();



await _context.Entry(enrollment)
    .Collection(x => x.EnrollmentSchedules)
    .Query()
    .Include(x => x.TimeSlot)
    .LoadAsync();


// Create TeachingSchedule automatically
foreach (var schedule in enrollment.EnrollmentSchedules)
{
    await _teachingScheduleService.CreateAsync(
        new CreateTeachingScheduleRequest
        {
            EnrollmentId = enrollment.Id,

            TeacherId = enrollment.TeacherId!.Value,

            DayOfWeek = schedule.DayOfWeek,

            StartTime = schedule.TimeSlot.StartTime,

            ClassDurationId = schedule.ClassDurationId,

            EffectiveFrom = enrollment.StartDate,

            EffectiveTo = enrollment.EndDate,

            IsActive = true,

            MaximumStudents = 1
        });
}


    return await GetByIdAsync(enrollment.Id)
        ?? throw new InvalidOperationException(
            "Enrollment was created but could not be retrieved.");
}

    public async Task<EnrollmentResponse?> UpdateAsync(
        Guid id,
        UpdateEnrollmentRequest request)
    {
        var enrollment = await _context.Enrollments
    .Include(x => x.EnrollmentSchedules)
    .FirstOrDefaultAsync(x => x.Id == id);

        if (enrollment == null)
            return null;

        ValidateFees(
            request.MonthlyFee,
            request.ScholarshipAmount,
            request.DiscountAmount);

        enrollment.TeacherId = request.TeacherId;
        //enrollment.TimeSlotId = request.TimeSlotId;
       // enrollment.ClassDurationId = request.ClassDurationId;

        enrollment.StartDate = request.StartDate;
        enrollment.EndDate = request.EndDate;

        enrollment.MonthlyFee = request.MonthlyFee;

        enrollment.ScholarshipAmount =
            request.ScholarshipAmount;

        enrollment.ScholarshipReason =
            request.ScholarshipReason;

        enrollment.DiscountAmount =
            request.DiscountAmount;

        enrollment.DiscountReason =
            request.DiscountReason;

        enrollment.FinalMonthlyFee =
            request.MonthlyFee
            - request.ScholarshipAmount
            - request.DiscountAmount;

        enrollment.Status = request.Status;
        enrollment.Remarks = request.Remarks;
// Remove existing schedules completely
var existingSchedules = enrollment.EnrollmentSchedules.ToList();
var oldTeachingSchedules =
    await _context.TeachingSchedules
    .Where(x => x.EnrollmentId == enrollment.Id)
    .ToListAsync();


_context.TeachingSchedules.RemoveRange(oldTeachingSchedules);
_context.EnrollmentSchedules.RemoveRange(existingSchedules);


// Add new schedules
foreach (var schedule in request.Schedules)
{
    enrollment.EnrollmentSchedules.Add(
        new AcademyERP.Domain.Entities.EnrollmentSchedules.EnrollmentSchedule
        {
            EnrollmentId = enrollment.Id,

            DayOfWeek = schedule.DayOfWeek,

            TimeSlotId = schedule.TimeSlotId,

            ClassDurationId = schedule.ClassDurationId,

            IsActive = true
        });
}
        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (enrollment == null)
            return false;

        _context.Enrollments.Remove(enrollment);

        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateFees(
        decimal monthlyFee,
        decimal scholarshipAmount,
        decimal discountAmount)
    {
        if (monthlyFee < 0 ||
            scholarshipAmount < 0 ||
            discountAmount < 0)
        {
            throw new ArgumentException(
                "Fee amounts cannot be negative.");
        }

        if (scholarshipAmount + discountAmount > monthlyFee)
        {
            throw new ArgumentException(
                "Scholarship and discount total cannot exceed the monthly fee.");
        }
    }


    private async Task ValidateEnrollmentSchedulesAsync(
    Guid teacherId,
    IEnumerable<CreateEnrollmentScheduleRequest> schedules,
    DateTime startDate,
    DateTime? endDate)
{
    var scheduleList = schedules.ToList();

    if (scheduleList.Count == 0)
        throw new ArgumentException(
            "At least one class schedule is required.");

    var teacherExists = await _context.Teachers
        .AsNoTracking()
        .AnyAsync(x => x.Id == teacherId);

    if (!teacherExists)
        throw new ArgumentException(
            "Selected teacher was not found.");

    var slotIds = scheduleList
        .Select(x => x.TimeSlotId)
        .Distinct()
        .ToList();

    var durationIds = scheduleList
        .Select(x => x.ClassDurationId)
        .Distinct()
        .ToList();

    var timeSlots = await _context.TimeSlots
        .AsNoTracking()
        .Where(x => slotIds.Contains(x.Id) && x.IsActive)
        .ToListAsync();

    var durations = await _context.ClassDurations
        .AsNoTracking()
        .Where(x => durationIds.Contains(x.Id))
        .ToListAsync();

    if (timeSlots.Count != slotIds.Count)
        throw new InvalidOperationException(
            "One or more selected time slots are inactive or invalid.");

    if (durations.Count != durationIds.Count)
        throw new InvalidOperationException(
            "One or more selected class durations are invalid.");

    var availabilities = await _context.TeacherAvailabilities
        .AsNoTracking()
        .Include(x => x.TimeSlot)
        .Where(x =>
            x.TeacherId == teacherId &&
            x.IsAvailable)
        .ToListAsync();

    // If availability has been configured for the teacher,
    // every enrollment schedule must respect it.
    if (availabilities.Count == 0)
    {
        throw new InvalidOperationException(
            "This teacher has no availability configured.");
    }

    var existingTeachingSchedules = await _context.TeachingSchedules
        .AsNoTracking()
        .Include(x => x.ClassDuration)
        .Where(x =>
            x.TeacherId == teacherId &&
            x.IsActive)
        .ToListAsync();

    var requestedRanges = new List<(DayOfWeek Day, int Start, int End)>();

    foreach (var schedule in scheduleList)
    {
        var slot = timeSlots.First(x => x.Id == schedule.TimeSlotId);
        var duration = durations.First(x => x.Id == schedule.ClassDurationId);

        var requestedStart = ToMinutes(slot.StartTime);
        var requestedEnd = requestedStart + duration.Minutes;

        if (requestedEnd > 24 * 60)
        {
            throw new InvalidOperationException(
                $"The class on {schedule.DayOfWeek} would continue past midnight.");
        }

        // Check teacher's configured availability.
        var dayAvailability = availabilities
            .Where(x =>
                x.DayOfWeek == schedule.DayOfWeek &&
                x.IsAvailable)
            .Select(x => new
            {
                Start = ToMinutes(x.TimeSlot.StartTime),
                End = ToMinutes(x.TimeSlot.EndTime)
            })
            .OrderBy(x => x.Start)
            .ToList();

        if (dayAvailability.Count == 0)
        {
            throw new InvalidOperationException(
                $"Teacher is not available on {schedule.DayOfWeek}.");
        }

        var coveredUntil = requestedStart;

        foreach (var available in dayAvailability)
        {
            var availableStart = available.Start;
            var availableEnd = available.End;

            // Handle 11 PM - 12 AM.
            if (availableEnd <= availableStart)
                availableEnd = 24 * 60;

            if (availableStart > coveredUntil)
                break;

            if (availableEnd > coveredUntil)
                coveredUntil = availableEnd;

            if (coveredUntil >= requestedEnd)
                break;
        }

        if (coveredUntil < requestedEnd)
        {
            throw new InvalidOperationException(
                $"Teacher is not available on {schedule.DayOfWeek} " +
                $"for the selected time and duration.");
        }

        // Check for overlapping schedules inside this same enrollment.
        var overlapsRequest = requestedRanges.Any(existing =>
            existing.Day == schedule.DayOfWeek &&
            requestedStart < existing.End &&
            requestedEnd > existing.Start);

        if (overlapsRequest)
        {
            throw new InvalidOperationException(
                $"The enrollment contains overlapping classes on " +
                $"{schedule.DayOfWeek}.");
        }

        requestedRanges.Add(
            (schedule.DayOfWeek, requestedStart, requestedEnd));

        // Check existing recurring teaching schedules.
        var hasConflict = existingTeachingSchedules.Any(existing =>
        {
            if (existing.DayOfWeek != schedule.DayOfWeek)
                return false;

            var existingStart = ToMinutes(existing.StartTime);
            var existingEnd =
                existingStart + existing.ClassDuration.Minutes;

            if (existingEnd > 24 * 60)
                existingEnd = 24 * 60;

            var datesOverlap =
                existing.EffectiveFrom.Date <=
                    (endDate ?? DateTime.MaxValue).Date
                &&
                (!existing.EffectiveTo.HasValue ||
                 existing.EffectiveTo.Value.Date >= startDate.Date);

            if (!datesOverlap)
                return false;

            return requestedStart < existingEnd &&
                   requestedEnd > existingStart;
        });

        if (hasConflict)
        {
            throw new InvalidOperationException(
                $"The selected teacher already has a class " +
                $"overlapping this time on {schedule.DayOfWeek}.");
        }
    }
}

private static int ToMinutes(TimeOnly time)
{
    return (time.Hour * 60) + time.Minute;
}
}
