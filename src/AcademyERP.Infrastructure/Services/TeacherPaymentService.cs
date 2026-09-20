using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.TeacherPayments;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.TeacherPayments;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.TeachingSchedules;


namespace AcademyERP.Infrastructure.Services;

public class TeacherPaymentService : ITeacherPaymentService
{
    private readonly ApplicationDbContext _context;

    public TeacherPaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherPaymentConfigurationResponse>> GetConfigurationsAsync(
        Guid? teacherId = null,
        Guid? enrollmentId = null)
    {
       var query = _context.TeacherPaymentConfigurations
    .Include(x => x.Teacher)
    .Include(x => x.Pauses)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .AsQueryable();

        if (teacherId.HasValue)
        {
            query = query.Where(x =>
                x.TeacherId == teacherId.Value);
        }

        if (enrollmentId.HasValue)
        {
            query = query.Where(x =>
                x.EnrollmentId == enrollmentId.Value);
        }

        var configurations = await query
            .OrderByDescending(x => x.EffectiveFrom)
            .ToListAsync();

        return configurations
            .Select(MapConfigurationToResponse)
            .ToList();
    }

    public async Task<TeacherPaymentConfigurationResponse?>
        GetConfigurationByIdAsync(Guid id)
    {
       var configuration = await _context
    .TeacherPaymentConfigurations
    .Include(x => x.Teacher)
    .Include(x => x.Pauses)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        return configuration == null
            ? null
            : MapConfigurationToResponse(configuration);
    }

    public async Task<TeacherPaymentConfigurationResponse>
        CreateConfigurationAsync(
            CreateTeacherPaymentConfigurationRequest request)
    {
        if (request.PaymentAmount < 0)
            throw new InvalidOperationException(
                "Payment amount cannot be negative.");

        var teacherExists = await _context.Teachers
            .AnyAsync(x => x.Id == request.TeacherId);

        if (!teacherExists)
            throw new InvalidOperationException(
                "Teacher not found.");

        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(x =>
                x.Id == request.EnrollmentId);

        if (enrollment == null)
            throw new InvalidOperationException(
                "Enrollment not found.");

        if (request.EffectiveTo.HasValue &&
            request.EffectiveTo.Value < request.EffectiveFrom)
        {
            throw new InvalidOperationException(
                "Effective end date cannot be before the effective start date.");
        }

        if (enrollment.TeacherId.HasValue &&
            enrollment.TeacherId.Value != request.TeacherId)
        {
            throw new InvalidOperationException(
                "The selected teacher is not assigned to this enrollment.");
        }

        var overlapping = await _context
            .TeacherPaymentConfigurations
            .AnyAsync(x =>
                x.TeacherId == request.TeacherId &&
                x.EnrollmentId == request.EnrollmentId &&
                x.EffectiveFrom <=
                    (request.EffectiveTo ?? DateTime.MaxValue) &&
                (x.EffectiveTo ?? DateTime.MaxValue) >=
                    request.EffectiveFrom);

        if (overlapping)
            throw new InvalidOperationException(
                "Another payment configuration already overlaps this effective period.");

        var configuration = new TeacherPaymentConfiguration
        {
            TeacherId = request.TeacherId,
            EnrollmentId = request.EnrollmentId,
            PaymentAmount = request.PaymentAmount,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            Remarks = request.Remarks
        };

        _context.TeacherPaymentConfigurations.Add(configuration);

        await _context.SaveChangesAsync();

        return (await GetConfigurationByIdAsync(configuration.Id))!;
    }

    public async Task<TeacherPaymentConfigurationResponse?>
        UpdateConfigurationAsync(
            Guid id,
            UpdateTeacherPaymentConfigurationRequest request)
    {
        if (request.PaymentAmount < 0)
            throw new InvalidOperationException(
                "Payment amount cannot be negative.");

        if (request.EffectiveTo.HasValue &&
            request.EffectiveTo.Value < request.EffectiveFrom)
        {
            throw new InvalidOperationException(
                "Effective end date cannot be before the effective start date.");
        }

        var configuration = await _context
            .TeacherPaymentConfigurations
            .FirstOrDefaultAsync(x => x.Id == id);

        if (configuration == null)
            return null;

        var overlapping = await _context
            .TeacherPaymentConfigurations
            .AnyAsync(x =>
                x.Id != id &&
                x.TeacherId == configuration.TeacherId &&
                x.EnrollmentId == configuration.EnrollmentId &&
                x.EffectiveFrom <=
                    (request.EffectiveTo ?? DateTime.MaxValue) &&
                (x.EffectiveTo ?? DateTime.MaxValue) >=
                    request.EffectiveFrom);

        if (overlapping)
            throw new InvalidOperationException(
                "Another payment configuration already overlaps this effective period.");

        configuration.PaymentAmount = request.PaymentAmount;
        configuration.EffectiveFrom = request.EffectiveFrom;
        configuration.EffectiveTo = request.EffectiveTo;
        configuration.Remarks = request.Remarks;

        await _context.SaveChangesAsync();

        return await GetConfigurationByIdAsync(id);
    }

    public async Task<bool> DeleteConfigurationAsync(Guid id)
    {
        var configuration = await _context
            .TeacherPaymentConfigurations
            .FirstOrDefaultAsync(x => x.Id == id);

        if (configuration == null)
            return false;

        var hasPayments = await _context.TeacherPayments
            .AnyAsync(x =>
                x.TeacherId == configuration.TeacherId &&
                x.EnrollmentId == configuration.EnrollmentId &&
                x.CycleStartDate >= configuration.EffectiveFrom);

        if (hasPayments)
            throw new InvalidOperationException(
                "This configuration is linked to payment history and cannot be deleted.");

        _context.TeacherPaymentConfigurations.Remove(configuration);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<PagedResponse<TeacherPaymentResponse>>
        GetPaymentsAsync(TeacherPaymentQueryRequest request)
    {
        var query = _context.TeacherPayments
            .Include(x => x.Teacher)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .AsQueryable();

        if (request.TeacherId.HasValue)
        {
            query = query.Where(x =>
                x.TeacherId == request.TeacherId.Value);
        }

        if (request.EnrollmentId.HasValue)
        {
            query = query.Where(x =>
                x.EnrollmentId == request.EnrollmentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TeacherName))
        {
            var name = request.TeacherName.Trim();

            query = query.Where(x =>
                x.Teacher.FullName.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.StudentName))
        {
            var name = request.StudentName.Trim();

            query = query.Where(x =>
                x.Enrollment.Student.FullName.Contains(name));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x =>
                x.CycleStartDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x =>
                x.CycleEndDate <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync();

        var payments = await query
            .OrderByDescending(x => x.CycleStartDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<TeacherPaymentResponse>
        {
            Items = payments
                .Select(MapPaymentToResponse)
                .ToList(),

            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,

            TotalPages = (int)Math.Ceiling(
                (double)totalCount / request.PageSize)
        };
    }

    public async Task<TeacherPaymentResponse?>
        GetPaymentByIdAsync(Guid id)
    {
        var payment = await _context.TeacherPayments
            .Include(x => x.Teacher)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        return payment == null
            ? null
            : MapPaymentToResponse(payment);
    }

    public async Task<TeacherPaymentResponse?>
        MarkPaidAsync(
            Guid id,
            MarkTeacherPaymentPaidRequest request)
    {
        var payment = await _context.TeacherPayments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (payment == null)
            return null;

        if (payment.Status == TeacherPaymentStatus.Paid)
            throw new InvalidOperationException(
                "This payment has already been marked as paid.");

        if (payment.Status == TeacherPaymentStatus.Cancelled)
            throw new InvalidOperationException(
                "A cancelled payment cannot be marked as paid.");

        payment.Status = TeacherPaymentStatus.Paid;
        payment.PaidDate = request.PaidDate;
        payment.PaymentMethodId = request.PaymentMethodId;
        payment.TransactionReference =
            request.TransactionReference;

        if (!string.IsNullOrWhiteSpace(request.Remarks))
            payment.Remarks = request.Remarks;

        await _context.SaveChangesAsync();

        return await GetPaymentByIdAsync(id);
    }

    public async Task<TeacherPaymentResponse?>
        CancelAsync(Guid id)
    {
        var payment = await _context.TeacherPayments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (payment == null)
            return null;

        if (payment.Status == TeacherPaymentStatus.Paid)
            throw new InvalidOperationException(
                "A paid payment cannot be cancelled.");

        payment.Status = TeacherPaymentStatus.Cancelled;

        await _context.SaveChangesAsync();

        return await GetPaymentByIdAsync(id);
    }

    public async Task<int> GeneratePendingCyclesAsync(DateTime asOfDate)
{
    var configurations = await _context
        .TeacherPaymentConfigurations
        .Include(x => x.Enrollment)
        .Include(x => x.Pauses)
        .ToListAsync();

    var affectedCount = 0;
    var today = asOfDate.Date;

    foreach (var configuration in configurations)
    {
        var enrollment = configuration.Enrollment;

        var schedules = await _context.TeachingSchedules
            .Where(x =>
                x.EnrollmentId == configuration.EnrollmentId &&
                x.TeacherId == configuration.TeacherId &&
                x.IsActive)
            .ToListAsync();

        if (schedules.Count == 0)
            continue;

        var cycleStart = enrollment.StartDate.Date;

        while (cycleStart <= today)
        {
            /*
             * A salary cycle represents one month of ACTIVE teaching.
             *
             * Example:
             *
             * Normal:
             * Sep 1 -> Sep 30
             *
             * Pause:
             * Sep 11 -> Sep 18
             *
             * Extended:
             * Sep 1 -> Oct 7
             *
             * The next cycle therefore starts:
             * Oct 8
             */

            var extendedCycleEnd =
                GetExtendedCycleEnd(
                    cycleStart,
                    configuration.Pauses);

            var extendedNextCycleStart =
                extendedCycleEnd.AddDays(1);

            /*
             * An open pause means this cycle's final end date is not
             * known yet.  Do not create a payment that would later need
             * to be extended when the teacher resumes.
             */
            var hasOpenPauseInCycle = configuration.Pauses.Any(x =>
                !x.ResumeDate.HasValue &&
                x.PauseStartDate.Date <= extendedCycleEnd);

            if (hasOpenPauseInCycle)
                break;

            /*
             * If the cycle has not finished yet, do not generate it.
             *
             * This is especially important for an active pause because
             * the final cycle end cannot be known until the teacher
             * resumes.
             */

            if (extendedCycleEnd > today)
                break;

            /*
             * Configuration period.
             *
             * EffectiveTo is the FIRST non-payable day.
             */

            var configurationStart =
                configuration.EffectiveFrom.Date;

            var configurationEndExclusive =
                configuration.EffectiveTo?.Date
                ?? DateTime.MaxValue.Date;

            /*
             * Skip cycles completely outside the configuration.
             */

            if (configurationStart >= extendedNextCycleStart ||
                configurationEndExclusive <= cycleStart)
            {
                cycleStart = extendedNextCycleStart;
                continue;
            }

            /*
             * Full active cycle.
             *
             * IMPORTANT:
             *
             * The denominator excludes paused days.
             *
             * Therefore a normal completed cycle with a temporary
             * pause still receives the full configured monthly amount.
             */

            var cycleActiveStart =
                cycleStart > configurationStart
                    ? cycleStart
                    : configurationStart;

            var cycleActiveEnd =
                extendedCycleEnd;

            if (configurationEndExclusive != DateTime.MaxValue.Date &&
                configurationEndExclusive.AddDays(-1) < cycleActiveEnd)
            {
                cycleActiveEnd =
                    configurationEndExclusive.AddDays(-1);
            }

            if (enrollment.EndDate.HasValue &&
                enrollment.EndDate.Value.Date < cycleActiveEnd)
            {
                cycleActiveEnd =
                    enrollment.EndDate.Value.Date;
            }

            if (cycleActiveStart > cycleActiveEnd)
            {
                cycleStart = extendedNextCycleStart;
                continue;
            }

            /*
             * Count ACTIVE scheduled teaching days.
             *
             * Paused dates are excluded from BOTH:
             *
             * - denominator
             * - payable numerator
             *
             * This ensures a completed paused cycle still receives
             * the configured monthly salary.
             */

            var cycleScheduledDays =
                CountScheduledTeachingDaysExcludingPauses(
                    schedules,
                    cycleStart,
                    extendedCycleEnd,
                    configuration.Pauses);

            if (cycleScheduledDays <= 0)
            {
                cycleStart = extendedNextCycleStart;
                continue;
            }

            /*
             * Payable period.
             *
             * Normally this is the complete active cycle.
             *
             * If the configuration was stopped in the middle of
             * the cycle, payment is prorated up to the day before
             * EffectiveTo.
             */

            var payableFrom = cycleActiveStart;
            var payableTo = cycleActiveEnd;

            var payableScheduledDays =
                CountScheduledTeachingDaysExcludingPauses(
                    schedules,
                    payableFrom,
                    payableTo,
                    configuration.Pauses);

            /*
             * Full monthly salary when the whole active cycle is
             * payable.
             *
             * Partial configuration periods are prorated.
             */

            var amount =
                Math.Round(
                    configuration.PaymentAmount *
                    payableScheduledDays /
                    (decimal)cycleScheduledDays,
                    2,
                    MidpointRounding.AwayFromZero);

            /*
             * Find an existing payment by cycle START.
             *
             * We deliberately use CycleStartDate as the identity
             * because the CycleEndDate can change after a pause.
             *
             * This also repairs existing pending records created
             * before Pause/Resume support was introduced.
             */

            var payment =
                await _context.TeacherPayments
                    .FirstOrDefaultAsync(x =>
                        x.TeacherId ==
                            configuration.TeacherId &&
                        x.EnrollmentId ==
                            configuration.EnrollmentId &&
                        x.CycleStartDate.Date ==
                            cycleStart);

            /*
             * Older versions could create a pending payment before
             * the cycle/payable fields were populated.  Reuse that
             * clearly malformed record for the first due cycle rather
             * than leaving it as 01 Jan 0001 and creating a duplicate.
             */
            if (payment == null)
            {
                var pendingPayments = await _context.TeacherPayments
                    .Where(x =>
                        x.TeacherId == configuration.TeacherId &&
                        x.EnrollmentId == configuration.EnrollmentId &&
                        x.Status == TeacherPaymentStatus.Pending)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();

                payment = pendingPayments
                    .FirstOrDefault(IsMalformedPendingPayment);
            }

            if (payment == null)
            {
                payment = new TeacherPayment
                {
                    TeacherId =
                        configuration.TeacherId,

                    EnrollmentId =
                        configuration.EnrollmentId,

                    CycleStartDate =
                        cycleStart,

                    CycleEndDate =
                        extendedCycleEnd,

                    PayableFromDate =
                        payableFrom,

                    PayableToDate =
                        payableTo,

                    PayableDays =
                        payableScheduledDays,

                    CycleScheduledDays =
                        cycleScheduledDays,

                    DueDate =
                        extendedNextCycleStart,

                    Amount =
                        amount,

                    Status =
                        TeacherPaymentStatus.Pending,

                    Remarks =
                        configuration.Remarks
                };

                _context.TeacherPayments.Add(payment);

                affectedCount++;
            }
            else if (
                payment.Status ==
                TeacherPaymentStatus.Pending)
            {
                /*
                 * Recalculate pending payments.
                 *
                 * This is important when:
                 *
                 * - a pause was added
                 * - a pause was resumed
                 * - a stop was applied
                 * - an old record has 0/0 scheduled days
                 */

                payment.CycleEndDate =
                    extendedCycleEnd;

                payment.PayableFromDate =
                    payableFrom;

                payment.PayableToDate =
                    payableTo;

                payment.PayableDays =
                    payableScheduledDays;

                payment.CycleScheduledDays =
                    cycleScheduledDays;

                payment.DueDate =
                    extendedNextCycleStart;

                payment.Amount =
                    amount;

                affectedCount++;
            }

            /*
             * VERY IMPORTANT:
             *
             * The next cycle starts AFTER the EXTENDED cycle.
             */

            cycleStart =
                extendedNextCycleStart;
        }
    }

    if (affectedCount > 0)
        await _context.SaveChangesAsync();

    return affectedCount;
}

private static DateTime GetExtendedCycleEnd(
    DateTime cycleStart,
    ICollection<TeacherPaymentPause> pauses)
{
    /*
     * Start with the normal one-month cycle.
     */

    var normalNextCycleStart =
        cycleStart.Date.AddMonths(1);

    var extendedCycleEnd =
        normalNextCycleStart.AddDays(-1);

    /*
     * A pause can itself extend the cycle into a period where
     * another pause begins.
     *
     * Therefore we keep recalculating until the end date no
     * longer changes.
     */

    while (true)
    {
        var previousEnd =
            extendedCycleEnd;

        var cycleEndExclusive =
            extendedCycleEnd.AddDays(1);

        var totalPausedDays = 0;

        foreach (var pause in pauses)
        {
            if (!pause.ResumeDate.HasValue)
            {
                /*
                 * An active pause has no known end date yet.
                 * The cycle must wait until Resume is recorded.
                 */
                continue;
            }

            var pauseStart =
                pause.PauseStartDate.Date;

            var pauseEndExclusive =
                pause.ResumeDate.Value.Date;

            if (pauseEndExclusive <= pauseStart)
                continue;

            /*
             * Find overlap between the pause and the current
             * cycle window.
             */

            var overlapStart =
                pauseStart > cycleStart.Date
                    ? pauseStart
                    : cycleStart.Date;

            var overlapEndExclusive =
                pauseEndExclusive < cycleEndExclusive
                    ? pauseEndExclusive
                    : cycleEndExclusive;

            if (overlapStart < overlapEndExclusive)
            {
                totalPausedDays +=
                    (overlapEndExclusive - overlapStart).Days;
            }
        }

        /*
         * Extend the cycle by paused calendar days.
         */

        extendedCycleEnd =
            normalNextCycleStart
                .AddDays(totalPausedDays - 1);

        if (extendedCycleEnd == previousEnd)
            break;
    }

    return extendedCycleEnd;
}

    private static TeacherPaymentConfigurationResponse
        MapConfigurationToResponse(
            TeacherPaymentConfiguration configuration)
    {
        return new TeacherPaymentConfigurationResponse
        {
            Id = configuration.Id,

            TeacherId =
                configuration.TeacherId,

            TeacherName =
                configuration.Teacher.FullName,

            EnrollmentId =
                configuration.EnrollmentId,

            StudentName =
                configuration.Enrollment
                    .Student
                    .FullName,

            CourseName =
                configuration.Enrollment
                    .Course
                    .CourseName,

            PaymentAmount =
                configuration.PaymentAmount,

            EffectiveFrom =
                configuration.EffectiveFrom,

            EffectiveTo =
                configuration.EffectiveTo,
IsPaused = configuration.Pauses
    .Any(x => !x.ResumeDate.HasValue),

PauseStartDate = configuration.Pauses
    .Where(x => !x.ResumeDate.HasValue)
    .OrderByDescending(x => x.PauseStartDate)
    .Select(x => (DateTime?)x.PauseStartDate)
    .FirstOrDefault(),

PauseReason = configuration.Pauses
    .Where(x => !x.ResumeDate.HasValue)
    .OrderByDescending(x => x.PauseStartDate)
    .Select(x => x.Reason)
    .FirstOrDefault(),


            Remarks =
                configuration.Remarks
        };
    }

    private static TeacherPaymentResponse
        MapPaymentToResponse(
            TeacherPayment payment)
    {
        return new TeacherPaymentResponse
        {
            Id = payment.Id,

            TeacherId =
                payment.TeacherId,

            TeacherName =
                payment.Teacher.FullName,

            EnrollmentId =
                payment.EnrollmentId,

            StudentName =
                payment.Enrollment
                    .Student
                    .FullName,

            CourseName =
                payment.Enrollment
                    .Course
                    .CourseName,

            CycleStartDate =
                payment.CycleStartDate,

            CycleEndDate =
                payment.CycleEndDate,

            DueDate =
                payment.DueDate,

            Amount =
                payment.Amount,

            Status =
                payment.Status,

            PaidDate =
                payment.PaidDate,

            PaymentMethodId =
                payment.PaymentMethodId,

            TransactionReference =
                payment.TransactionReference,

            PayableFromDate =
                payment.PayableFromDate,

            PayableToDate =
                payment.PayableToDate,

            PayableDays =
                payment.PayableDays,

            CycleScheduledDays =
                payment.CycleScheduledDays,

            Remarks =
                payment.Remarks
        };
    }

    private static bool IsMalformedPendingPayment(
        TeacherPayment payment)
    {
        return payment.CycleStartDate.Date <= DateTime.MinValue.Date ||
               payment.CycleEndDate.Date <= DateTime.MinValue.Date ||
               payment.PayableFromDate.Date <= DateTime.MinValue.Date ||
               payment.PayableToDate.Date <= DateTime.MinValue.Date;
    }

public async Task<TeacherPaymentConfigurationResponse?>
    StopConfigurationAsync(
        Guid id,
        DateTime stopDate)
{
    var configuration =
        await _context.TeacherPaymentConfigurations
            .Include(x => x.Pauses)
            .FirstOrDefaultAsync(x => x.Id == id);

    if (configuration == null)
        return null;

    var date = stopDate.Date;

    if (date < configuration.EffectiveFrom.Date)
    {
        throw new InvalidOperationException(
            "Stop date cannot be before the configuration start date.");
    }

    configuration.EffectiveTo = date;

    // Stop date is the FIRST day that is NOT payable.
    // Therefore any cycle beginning on or after the
    // stop date must be cancelled.
    var futurePayments =
        await _context.TeacherPayments
            .Where(x =>
                x.TeacherId == configuration.TeacherId &&
                x.EnrollmentId == configuration.EnrollmentId &&
                x.Status == TeacherPaymentStatus.Pending &&
                x.CycleStartDate.Date >= date)
            .ToListAsync();

    foreach (var payment in futurePayments)
    {
        payment.Status =
            TeacherPaymentStatus.Cancelled;

        payment.Remarks =
            string.IsNullOrWhiteSpace(payment.Remarks)
                ? "Cancelled because teacher payment configuration was stopped."
                : payment.Remarks +
                  " Cancelled because teacher payment configuration was stopped.";
    }

    // If a pending payment overlaps the stop date,
    // recalculate its payable period and amount.
    var affectedPayments =
        await _context.TeacherPayments
            .Where(x =>
                x.TeacherId == configuration.TeacherId &&
                x.EnrollmentId == configuration.EnrollmentId &&
                x.Status == TeacherPaymentStatus.Pending &&
                x.CycleStartDate.Date < date &&
                x.CycleEndDate.Date >= date)
            .ToListAsync();

    var schedules =
        await _context.TeachingSchedules
            .Where(x =>
                x.EnrollmentId == configuration.EnrollmentId &&
                x.TeacherId == configuration.TeacherId &&
                x.IsActive)
            .ToListAsync();

    foreach (var payment in affectedPayments)
    {
        var payableFrom =
            payment.CycleStartDate.Date >
            configuration.EffectiveFrom.Date
                ? payment.CycleStartDate.Date
                : configuration.EffectiveFrom.Date;

        var payableTo =
            date.AddDays(-1) < payment.CycleEndDate.Date
                ? date.AddDays(-1)
                : payment.CycleEndDate.Date;

        var cycleScheduledDays =
            CountScheduledTeachingDaysExcludingPauses(
                schedules,
                payment.CycleStartDate,
                payment.CycleEndDate,
                configuration.Pauses);

        var payableScheduledDays =
            CountScheduledTeachingDaysExcludingPauses(
                schedules,
                payableFrom,
                payableTo,
                configuration.Pauses);

        if (cycleScheduledDays > 0)
        {
            payment.PayableFromDate =
                payableFrom;

            payment.PayableToDate =
                payableTo;

            payment.PayableDays =
                payableScheduledDays;

            payment.CycleScheduledDays =
                cycleScheduledDays;

            payment.Amount =
                Math.Round(
                    configuration.PaymentAmount *
                    payableScheduledDays /
                    (decimal)cycleScheduledDays,
                    2,
                    MidpointRounding.AwayFromZero);
        }
    }

    await _context.SaveChangesAsync();

    return await GetConfigurationByIdAsync(id);
}

public async Task<TeacherPaymentConfigurationResponse?>
    PauseConfigurationAsync(
        Guid id,
        DateTime pauseStartDate,
        string? reason)
{
    var configuration =
        await _context.TeacherPaymentConfigurations
            .Include(x => x.Pauses)
            .FirstOrDefaultAsync(x => x.Id == id);

    if (configuration == null)
        return null;

    var date = pauseStartDate.Date;

    if (date < configuration.EffectiveFrom.Date)
    {
        throw new InvalidOperationException(
            "Pause date cannot be before the configuration start date.");
    }

    if (configuration.EffectiveTo.HasValue &&
        date >= configuration.EffectiveTo.Value.Date)
    {
        throw new InvalidOperationException(
            "Pause date must be before the configuration stop date.");
    }

    var activePause = configuration.Pauses
        .FirstOrDefault(x => !x.ResumeDate.HasValue);

    if (activePause != null)
    {
        throw new InvalidOperationException(
            "This payment configuration is already paused.");
    }

    var overlappingPause = configuration.Pauses
        .Any(x =>
            date >= x.PauseStartDate.Date &&
            (!x.ResumeDate.HasValue ||
             date < x.ResumeDate.Value.Date));

    if (overlappingPause)
    {
        throw new InvalidOperationException(
            "The selected pause date overlaps an existing pause period.");
    }

    var pause = new TeacherPaymentPause
    {
        TeacherPaymentConfigurationId = configuration.Id,
        PauseStartDate = date,
        ResumeDate = null,
        Reason = reason
    };

    _context.TeacherPaymentPauses.Add(pause);

    await _context.SaveChangesAsync();

    return await GetConfigurationByIdAsync(id);
}


public async Task<TeacherPaymentConfigurationResponse?>
    ResumeConfigurationAsync(
        Guid id,
        DateTime resumeDate)
{
    var configuration =
        await _context.TeacherPaymentConfigurations
            .Include(x => x.Pauses)
            .FirstOrDefaultAsync(x => x.Id == id);

    if (configuration == null)
        return null;

    var date = resumeDate.Date;

    var activePause = configuration.Pauses
        .Where(x => !x.ResumeDate.HasValue)
        .OrderByDescending(x => x.PauseStartDate)
        .FirstOrDefault();

    if (activePause == null)
    {
        throw new InvalidOperationException(
            "This payment configuration is not currently paused.");
    }

    if (date <= activePause.PauseStartDate.Date)
    {
        throw new InvalidOperationException(
            "Resume date must be after the pause start date.");
    }

    if (configuration.EffectiveTo.HasValue &&
        date > configuration.EffectiveTo.Value.Date)
    {
        throw new InvalidOperationException(
            "Resume date cannot be after the configuration stop date.");
    }

    activePause.ResumeDate = date;

    await _context.SaveChangesAsync();

    return await GetConfigurationByIdAsync(id);
}

private static int CountScheduledTeachingDays(
    List<TeachingSchedule> schedules,
    DateTime fromDate,
    DateTime toDate)
{
    if (fromDate.Date > toDate.Date)
        return 0;

    var count = 0;

    for (var date = fromDate.Date;
         date <= toDate.Date;
         date = date.AddDays(1))
    {
        foreach (var schedule in schedules)
        {
            var scheduleFrom = schedule.EffectiveFrom.Date;

            var scheduleTo =
                schedule.EffectiveTo?.Date
                ?? DateTime.MaxValue.Date;

            if (date < scheduleFrom ||
                date > scheduleTo)
            {
                continue;
            }

            if (schedule.DayOfWeek == date.DayOfWeek)
            {
                count++;
                break;
            }
        }
    }

    return count;
}

private static int CountScheduledTeachingDaysExcludingPauses(
    List<TeachingSchedule> schedules,
    DateTime fromDate,
    DateTime toDate,
    ICollection<TeacherPaymentPause> pauses)
{
    if (fromDate.Date > toDate.Date)
        return 0;

    var count = 0;

    for (var date = fromDate.Date;
         date <= toDate.Date;
         date = date.AddDays(1))
    {
        /*
         * A paused date is not a payable teaching day.
         */
        var isPaused = pauses.Any(x =>
            date >= x.PauseStartDate.Date &&
            x.ResumeDate.HasValue &&
            date < x.ResumeDate.Value.Date);

        if (isPaused)
            continue;

        foreach (var schedule in schedules)
        {
            var scheduleFrom =
                schedule.EffectiveFrom.Date;

            var scheduleTo =
                schedule.EffectiveTo?.Date
                ?? DateTime.MaxValue.Date;

            if (date < scheduleFrom ||
                date > scheduleTo)
            {
                continue;
            }

            if (schedule.DayOfWeek == date.DayOfWeek)
            {
                count++;
                break;
            }
        }
    }

    return count;
}

}
