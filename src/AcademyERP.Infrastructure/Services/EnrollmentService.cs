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
using AcademyERP.Application.DTOs.TeacherPayments;
using AcademyERP.Domain.Entities.Finance;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Infrastructure.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;
private readonly ITeachingScheduleService _teachingScheduleService;
private readonly ITeacherPaymentService _teacherPaymentService;
    public EnrollmentService(
    ApplicationDbContext context,
    ITeachingScheduleService teachingScheduleService,
    ITeacherPaymentService teacherPaymentService)
{
    _context = context;
    _teachingScheduleService = teachingScheduleService;
    _teacherPaymentService = teacherPaymentService;
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

    if (request.TeacherPaymentAmount < 0)
    {
        throw new ArgumentException(
            "Teacher payment amount cannot be negative.");
    }

    await using var transaction =
    await _context.Database.BeginTransactionAsync();

try
{
    if (request.AutoCreateTeacherAvailability)
    {
        var requestedSchedules = request.Schedules.ToList();

        var requestedTimeSlotIds = requestedSchedules
            .Select(x => x.TimeSlotId)
            .Distinct()
            .ToList();

        var activeTimeSlotIds = await _context.TimeSlots
            .Where(x =>
                requestedTimeSlotIds.Contains(x.Id) &&
                x.IsActive)
            .Select(x => x.Id)
            .ToListAsync();

        if (activeTimeSlotIds.Count != requestedTimeSlotIds.Count)
        {
            throw new InvalidOperationException(
                "One or more selected time slots are inactive or invalid.");
        }

        var existingAvailability =
            await _context.TeacherAvailabilities
                .Where(x =>
                    x.TeacherId == request.TeacherId.Value &&
                    !x.IsDeleted &&
                    x.IsAvailable)
                .ToListAsync();

        foreach (var schedule in requestedSchedules)
        {
            var alreadyAvailable =
                existingAvailability.Any(x =>
                    x.DayOfWeek == schedule.DayOfWeek &&
                    x.TimeSlotId == schedule.TimeSlotId &&
                    x.IsAvailable);

            if (alreadyAvailable)
                continue;

            var availability = new TeacherAvailability
            {
                TeacherId = request.TeacherId.Value,
                DayOfWeek = schedule.DayOfWeek,
                TimeSlotId = schedule.TimeSlotId,
                IsAvailable = true
            };

            _context.TeacherAvailabilities.Add(availability);
            existingAvailability.Add(availability);
        }

        await _context.SaveChangesAsync();
    }

    var resolvedStartTimes =
        await ValidateEnrollmentSchedulesAsync(
            request.TeacherId.Value,
            request.Schedules,
            request.StartDate,
            request.EndDate);
        // ==========================================
        // CALCULATE FINAL STUDENT FEE
        // ==========================================

        var finalMonthlyFee =
            request.MonthlyFee
            - request.ScholarshipAmount
            - request.DiscountAmount;

        if (finalMonthlyFee < 0)
        {
            throw new ArgumentException(
                "Final monthly fee cannot be negative.");
        }

        // ==========================================
        // CREATE ENROLLMENT
        // ==========================================

        var enrollment = new Enrollment
        {
            StudentId =
                request.StudentId,

            CourseId =
                request.CourseId,

            TeacherId =
                request.TeacherId,

            StartDate =
                request.StartDate,

            EndDate =
                request.EndDate,

            MonthlyFee =
                request.MonthlyFee,

            ScholarshipAmount =
                request.ScholarshipAmount,

            ScholarshipReason =
                request.ScholarshipReason,

            DiscountAmount =
                request.DiscountAmount,

            DiscountReason =
                request.DiscountReason,

            FinalMonthlyFee =
                finalMonthlyFee,

            // Billing starts from enrollment date.
            NextBillingDate =
                request.StartDate.AddMonths(1),

            Remarks =
                request.Remarks
        };

        // ==========================================
        // CREATE ENROLLMENT SCHEDULES
        // ==========================================

        foreach (var schedule in request.Schedules)
        {
            enrollment.EnrollmentSchedules.Add(
                new EnrollmentSchedule
                {
                    DayOfWeek =
                        schedule.DayOfWeek,

                    TimeSlotId =
                        schedule.TimeSlotId,

                    ClassDurationId =
                        schedule.ClassDurationId,

                    IsActive =
                        true
                });
        }

        _context.Enrollments.Add(enrollment);

        // Save first so EnrollmentId is available.
        await _context.SaveChangesAsync();

        // ==========================================
        // LOAD TIME SLOTS
        // ==========================================

        await _context.Entry(enrollment)
            .Collection(x => x.EnrollmentSchedules)
            .Query()
            .Include(x => x.TimeSlot)
            .LoadAsync();

        // ==========================================
        // CREATE TEACHING SCHEDULES
        // ==========================================

        foreach (var schedule in enrollment.EnrollmentSchedules)
        {
            await _teachingScheduleService.CreateAsync(
                new CreateTeachingScheduleRequest
                {
                    EnrollmentId =
                        enrollment.Id,

                    TeacherId =
                        enrollment.TeacherId!.Value,

                    DayOfWeek =
                        schedule.DayOfWeek,

                    StartTime =
                        resolvedStartTimes[
                            (
                                schedule.DayOfWeek,
                                schedule.TimeSlotId,
                                schedule.ClassDurationId
                            )],

                    ClassDurationId =
                        schedule.ClassDurationId,

                    EffectiveFrom =
                        enrollment.StartDate,

                    EffectiveTo =
                        enrollment.EndDate,

                    IsActive =
                        true,

                    MaximumStudents =
                        1
                });
        }

        // ==========================================
        // CREATE FEE INVOICES
        // ==========================================
        //
        // If the enrollment is entered after its joining date,
        // completed historical billing cycles are treated as
        // already settled. Only the current cycle remains pending.
        //
        // Example:
        // Joining Date : 01 Aug
        // Created      : 29 Sep
        //
        // 01 Aug cycle -> Paid
        // 01 Sep cycle -> Pending
        // 01 Oct cycle -> Not created yet
        // ==========================================

        var billingCycleStart =
            request.StartDate.Date;

        var billingToday =
            DateTime.UtcNow.Date;

        while (billingCycleStart.AddMonths(1) <= billingToday)
        {
            var historicalInvoice =
                new FeeInvoice
                {
                    EnrollmentId =
                        enrollment.Id,

                    InvoiceNumber =
                        $"INV-{billingCycleStart:yyyyMMdd}-{enrollment.Id.ToString()[..8]}",

                    Amount =
                        enrollment.FinalMonthlyFee,

                    DiscountAmount =
                        enrollment.ScholarshipAmount
                        + enrollment.DiscountAmount,

                    PaidAmount =
                        enrollment.FinalMonthlyFee,

                    BalanceAmount =
                        0,

                    InvoiceDate =
                        billingCycleStart,

                    DueDate =
                        billingCycleStart,

                    PaidDate =
                        billingToday,

                    Status =
                        FeeStatus.Paid,

                    Remarks =
                        "Historical billing cycle settled during late enrollment creation."
                };

            _context.FeeInvoices.Add(historicalInvoice);

            billingCycleStart =
                billingCycleStart.AddMonths(1);
        }

        // Current billing cycle remains unpaid.
        var firstInvoice =
            new FeeInvoice
            {
                EnrollmentId =
                    enrollment.Id,

                InvoiceNumber =
                    $"INV-{billingCycleStart:yyyyMMdd}-{enrollment.Id.ToString()[..8]}",

                Amount =
                    enrollment.FinalMonthlyFee,

                DiscountAmount =
                    enrollment.ScholarshipAmount
                    + enrollment.DiscountAmount,

                PaidAmount =
                    0,

                BalanceAmount =
                    enrollment.FinalMonthlyFee,

                InvoiceDate =
                    billingCycleStart,

                DueDate =
                    billingCycleStart,

                Status =
                    FeeStatus.Pending,

                Remarks =
                    "Generated automatically for the current enrollment billing cycle."
            };

        _context.FeeInvoices.Add(firstInvoice);

        // Next cycle is one month after the current cycle.
        enrollment.NextBillingDate =
            billingCycleStart.AddMonths(1);

        await _context.SaveChangesAsync();

        // ==========================================
        // CREATE TEACHER PAYMENT CONFIGURATION
        // ==========================================

        await _teacherPaymentService.CreateConfigurationAsync(
            new CreateTeacherPaymentConfigurationRequest
            {
                TeacherId =
                    enrollment.TeacherId!.Value,

                EnrollmentId =
                    enrollment.Id,

                PaymentAmount =
                    request.TeacherPaymentAmount,

                EffectiveFrom =
                    enrollment.StartDate,

                EffectiveTo =
                    enrollment.EndDate,

                Remarks =
                    "Created automatically during enrollment creation."
            });

        // ==========================================
        // COMMIT EVERYTHING
        // ==========================================

        await transaction.CommitAsync();

        // ==========================================
        // RETURN CREATED ENROLLMENT
        // ==========================================

        return await GetByIdAsync(enrollment.Id)
            ?? throw new InvalidOperationException(
                "Enrollment was created but could not be retrieved.");
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
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


foreach (var schedule in oldTeachingSchedules)
{
    schedule.IsDeleted = true;
}
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


    private async Task<Dictionary<(DayOfWeek Day, Guid TimeSlotId, Guid ClassDurationId), TimeOnly>>
        ValidateEnrollmentSchedulesAsync(
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

        if (availabilities.Count == 0)
        {
            throw new InvalidOperationException(
                "This teacher has no availability configured.");
        }

        var existingTeachingSchedules =
            await _context.TeachingSchedules
                .AsNoTracking()
                .Include(x => x.ClassDuration)
                .Where(x =>
                    x.TeacherId == teacherId &&
                    x.IsActive)
                .ToListAsync();

        var requestedRanges =
            new List<(DayOfWeek Day, int Start, int End)>();

        var resolvedStartTimes =
            new Dictionary<(DayOfWeek Day, Guid TimeSlotId, Guid ClassDurationId), TimeOnly>();

        foreach (var schedule in scheduleList)
        {
            var slot = timeSlots.First(x =>
                x.Id == schedule.TimeSlotId);

            var duration = durations.First(x =>
                x.Id == schedule.ClassDurationId);

            var slotStart = ToMinutes(slot.StartTime);
            var slotEnd = ToMinutes(slot.EndTime);

            if (slotEnd <= slotStart)
                slotEnd = 24 * 60;

            if (slotStart + duration.Minutes > 24 * 60)
            {
                throw new InvalidOperationException(
                    $"The selected time slot on {schedule.DayOfWeek} " +
                    $"cannot accommodate the requested class duration.");
            }

            // ---------------------------------------------------------
            // Find teacher availability windows for this day.
            // ---------------------------------------------------------
            var dayAvailability = availabilities
                .Where(x =>
                    x.DayOfWeek == schedule.DayOfWeek &&
                    x.IsAvailable)
                .Select(x =>
                {
                    var start = ToMinutes(x.TimeSlot.StartTime);
                    var end = ToMinutes(x.TimeSlot.EndTime);

                    if (end <= start)
                        end = 24 * 60;

                    return new
                    {
                        Start = start,
                        End = end
                    };
                })
                .Where(x =>
                    x.End > slotStart &&
                    x.Start < slotEnd)
                .OrderBy(x => x.Start)
                .ToList();

            if (dayAvailability.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Teacher is not available on {schedule.DayOfWeek} " +
                    $"during the selected time slot.");
            }

            TimeOnly? resolvedStart = null;

            // ---------------------------------------------------------
            // Find the earliest free period inside:
            // TimeSlot + TeacherAvailability + Existing schedules.
            // ---------------------------------------------------------
            foreach (var availability in dayAvailability)
            {
                var windowStart =
                    Math.Max(slotStart, availability.Start);

                var windowEnd =
                    Math.Min(slotEnd, availability.End);

                if (windowEnd - windowStart < duration.Minutes)
                    continue;

                var candidate = windowStart;

                while (candidate + duration.Minutes <= windowEnd)
                {
                    var candidateEnd =
                        candidate + duration.Minutes;

                    // Existing recurring classes.
                    var existingConflict =
                        existingTeachingSchedules
                            .Where(existing =>
                                existing.DayOfWeek ==
                                schedule.DayOfWeek)
                            .Where(existing =>
                            {
                                var datesOverlap =
                                    existing.EffectiveFrom.Date <=
                                        (endDate ??
                                         DateTime.MaxValue).Date
                                    &&
                                    (!existing.EffectiveTo.HasValue ||
                                     existing.EffectiveTo.Value.Date >=
                                         startDate.Date);

                                return datesOverlap;
                            })
                            .Select(existing =>
                            {
                                var existingStart =
                                    ToMinutes(existing.StartTime);

                                var existingEnd =
                                    Math.Min(
                                        24 * 60,
                                        existingStart +
                                        existing.ClassDuration.Minutes);

                                return new
                                {
                                    Start = existingStart,
                                    End = existingEnd
                                };
                            })
                            .FirstOrDefault(existing =>
                                candidate < existing.End &&
                                candidateEnd > existing.Start);

                    if (existingConflict != null)
                    {
                        candidate = existingConflict.End;
                        continue;
                    }

                    // Another schedule in the same enrollment.
                    var requestConflict =
                        requestedRanges
                            .FirstOrDefault(existing =>
                                existing.Day ==
                                    schedule.DayOfWeek &&
                                candidate < existing.End &&
                                candidateEnd > existing.Start);

                    if (requestConflict != default)
                    {
                        candidate = requestConflict.End;
                        continue;
                    }

                    resolvedStart =
                        TimeOnly.FromTimeSpan(
                            TimeSpan.FromMinutes(candidate));

                    break;
                }

                if (resolvedStart.HasValue)
                    break;
            }

            if (!resolvedStart.HasValue)
            {
                throw new InvalidOperationException(
                    $"No {duration.Minutes}-minute free time is available " +
                    $"for the teacher on {schedule.DayOfWeek} " +
                    $"inside the selected time slot.");
            }

            var actualStart =
                ToMinutes(resolvedStart.Value);

            var actualEnd =
                actualStart + duration.Minutes;

            requestedRanges.Add(
                (schedule.DayOfWeek, actualStart, actualEnd));

            resolvedStartTimes[
                (
                    schedule.DayOfWeek,
                    schedule.TimeSlotId,
                    schedule.ClassDurationId
                )] = resolvedStart.Value;
        }

        return resolvedStartTimes;
    }

private static int ToMinutes(TimeOnly time)
{
    return (time.Hour * 60) + time.Minute;
}
}
