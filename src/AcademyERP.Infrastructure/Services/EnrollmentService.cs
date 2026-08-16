using AcademyERP.Application.DTOs.Enrollments;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.EnrollmentSchedules;
using AcademyERP.Application.DTOs.Enrollments;
using AcademyERP.Application.DTOs.EnrollmentSchedules;

namespace AcademyERP.Infrastructure.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;

    public EnrollmentService(ApplicationDbContext context)
    {
        _context = context;
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

            MonthlyFee = x.MonthlyFee,

            ScholarshipAmount = x.ScholarshipAmount,
            ScholarshipReason = x.ScholarshipReason,

            DiscountAmount = x.DiscountAmount,
            DiscountReason = x.DiscountReason,

            FinalMonthlyFee = x.FinalMonthlyFee,

            NextBillingDate = x.NextBillingDate,

            Status = x.Status,

            Remarks = x.Remarks,


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

            Status = x.Status,

            Remarks = x.Remarks,


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
}