using AcademyERP.Application.DTOs.EnrollmentSchedules;
using AcademyERP.Application.DTOs.Enrollments;
using AcademyERP.Application.DTOs.TeacherStudentRequests;
using AcademyERP.Application.Interfaces;
using AcademyERP.Application.Services;
using AcademyERP.Application.Interfaces.TeacherStudentRequests;
using AcademyERP.Domain.Entities.TeacherStudentRequests;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class TeacherStudentRequestService : ITeacherStudentRequestService
{
    private readonly ApplicationDbContext _context;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IFeeInvoiceService _feeInvoiceService;
    private readonly ITeacherPaymentService _teacherPaymentService;

    public TeacherStudentRequestService(
        ApplicationDbContext context,
        IEnrollmentService enrollmentService,
        IFeeInvoiceService feeInvoiceService,
        ITeacherPaymentService teacherPaymentService)
    {
        _context = context;
        _enrollmentService = enrollmentService;
        _feeInvoiceService = feeInvoiceService;
        _teacherPaymentService = teacherPaymentService;
    }

    public async Task<TeacherStudentRequestResponse> CreateAsync(
        Guid teacherId,
        CreateTeacherStudentRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.StudentName))
            throw new ArgumentException("Student name is required.");

        if (string.IsNullOrWhiteSpace(request.StudentPhoneNumber))
            throw new ArgumentException("Student mobile number is required.");

        if (request.CourseId == Guid.Empty)
            throw new ArgumentException("Course is required.");

        if (request.Schedules == null || request.Schedules.Count == 0)
            throw new ArgumentException(
                "At least one class schedule is required.");

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(
                x => x.ApplicationUserId == teacherId &&
                     !x.IsDeleted);

        if (teacher == null)
            throw new ArgumentException(
                "Teacher profile was not found for the logged-in user.");

        var courseExists = await _context.Courses
            .AnyAsync(x => x.Id == request.CourseId && !x.IsDeleted);

        if (!courseExists)
            throw new ArgumentException("Course was not found.");

        Guid? studentId = null;

        if (request.StudentId.HasValue &&
            request.StudentId.Value != Guid.Empty)
        {
            var studentExists = await _context.Students
                .AnyAsync(
                    x => x.Id == request.StudentId.Value &&
                         !x.IsDeleted);

            if (!studentExists)
                throw new ArgumentException("Student was not found.");

            studentId = request.StudentId.Value;
        }

        var duplicate = await _context.TeacherStudentRequests
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.TeacherId == teacher.Id &&
                x.CourseId == request.CourseId &&
                (x.Status == TeacherStudentRequestStatus.Pending ||
                 x.Status == TeacherStudentRequestStatus.Approved) &&
                (
                    (studentId.HasValue &&
                     x.StudentId == studentId.Value)
                    ||
                    (!studentId.HasValue &&
                     x.StudentId == null &&
                     x.StudentName == request.StudentName.Trim() &&
                     x.StudentPhoneNumber == request.StudentPhoneNumber.Trim())
                ));

        if (duplicate)
        {
            throw new InvalidOperationException(
                "A pending or approved request already exists for this student and course.");
        }

        var entity = new TeacherStudentRequest
        {
            TeacherId = teacher.Id,
            StudentId = studentId,
            StudentName = request.StudentName.Trim(),
            StudentPhoneNumber = request.StudentPhoneNumber.Trim(),
            CourseId = request.CourseId,
            StartDate = request.StartDate.Date,
            StudentFee = request.StudentFee,
            TeacherRemarks = request.TeacherRemarks,
            Status = TeacherStudentRequestStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };

        foreach (var schedule in request.Schedules)
        {
            entity.Schedules.Add(
                new TeacherStudentRequestSchedule
                {
                    DayOfWeek = schedule.DayOfWeek,
                    TimeSlotId = schedule.TimeSlotId,
                    ClassDurationId = schedule.ClassDurationId
                });
        }

        _context.TeacherStudentRequests.Add(entity);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(entity.Id)
            ?? throw new InvalidOperationException(
                "Request was created but could not be retrieved.");
    }

    public async Task<List<TeacherStudentRequestResponse>> GetForTeacherAsync(
        Guid applicationUserId)
    {
        var teacher = await _context.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.ApplicationUserId == applicationUserId &&
                !x.IsDeleted);

        if (teacher == null)
            return new List<TeacherStudentRequestResponse>();

        return await BuildQuery()
            .Where(x => x.TeacherId == teacher.Id)
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<TeacherStudentRequestResponse>> GetPendingAsync()
    {
        return await BuildQuery()
            .Where(x => x.Status == TeacherStudentRequestStatus.Pending)
            .OrderBy(x => x.SubmittedAt)
            .ToListAsync();
    }

    public async Task<TeacherStudentRequestResponse?> GetByIdAsync(Guid id)
    {
        return await BuildQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FindStudentResponse?> FindStudentAsync(
        string studentName,
        string studentPhoneNumber)
    {
        var name = studentName.Trim();
        var phone = studentPhoneNumber.Trim();

        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(phone))
        {
            return null;
        }

        return await _context.Students
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.FullName == name &&
                x.PhoneNumber == phone)
            .Select(x => new FindStudentResponse
            {
                Id = x.Id,
                AdmissionNumber = x.AdmissionNumber,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TeacherStudentRequestResponse> LinkStudentAsync(
        Guid id,
        Guid studentId)
    {
        var entity = await _context.TeacherStudentRequests
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (entity == null)
            throw new KeyNotFoundException(
                "Teacher student request was not found.");

        if (entity.Status != TeacherStudentRequestStatus.Pending)
            throw new InvalidOperationException(
                "Only pending requests can be updated.");

        var student = await _context.Students
            .FirstOrDefaultAsync(x =>
                x.Id == studentId &&
                !x.IsDeleted);

        if (student == null)
            throw new KeyNotFoundException(
                "Student was not found.");

        entity.StudentId = student.Id;
        entity.StudentName = student.FullName;
        entity.StudentPhoneNumber = student.PhoneNumber;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(entity.Id)
            ?? throw new InvalidOperationException(
                "Request could not be retrieved after linking the student.");
    }

    public async Task<TeacherStudentRequestResponse> ReviewAsync(
        Guid id,
        Guid reviewerId,
        ReviewTeacherStudentRequestDto request)
    {
        var entity = await _context.TeacherStudentRequests
            .Include(x => x.Schedules)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (entity == null)
            throw new KeyNotFoundException(
                "Teacher student request was not found.");

        if (entity.Status != TeacherStudentRequestStatus.Pending)
            throw new InvalidOperationException(
                "Only pending requests can be reviewed.");

        if (!request.Approve)
        {
            entity.Status = TeacherStudentRequestStatus.Rejected;
            entity.AdminRemarks = request.AdminRemarks;
            entity.ReviewedAt = DateTime.UtcNow;
            entity.ReviewedBy = reviewerId;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id)
                ?? throw new InvalidOperationException(
                    "Request could not be retrieved after rejection.");
        }

        if (!entity.StudentId.HasValue ||
            entity.StudentId.Value == Guid.Empty)
        {
            throw new InvalidOperationException(
                "A student must be linked or created before approval.");
        }

        if (request.StudentFee < 0)
            throw new ArgumentException(
                "Student fee cannot be negative.");

        if (request.TeacherSalary < 0)
            throw new ArgumentException(
                "Teacher salary cannot be negative.");

        if (entity.Schedules.Count == 0)
            throw new InvalidOperationException(
                "The request has no class schedule.");

        var enrollment = await _enrollmentService.CreateAsync(
            new CreateEnrollmentRequest
            {
                StudentId = entity.StudentId.Value,
                CourseId = entity.CourseId,
                TeacherId = entity.TeacherId,
                StartDate = entity.StartDate,
                EndDate = null,
                MonthlyFee = request.StudentFee,
                TeacherPaymentAmount = request.TeacherSalary,
                ScholarshipAmount = 0,
                DiscountAmount = 0,
                Remarks =
                    $"Created from Teacher Student Request {entity.Id}",
                Schedules = entity.Schedules
                    .Where(x => !x.IsDeleted)
                    .Select(x =>
                        new CreateEnrollmentScheduleRequest
                        {
                            DayOfWeek = x.DayOfWeek,
                            TimeSlotId = x.TimeSlotId,
                            ClassDurationId = x.ClassDurationId
                        })
                    .ToList()
            });

        // Ensure the first fee cycle exists for this enrollment.
        // EnrollmentService already creates the initial invoice;
        // this call is idempotent and also advances billing state
        // correctly when required.
        await _feeInvoiceService.GeneratePendingCyclesAsync(
            entity.StartDate.Date);

        // Generate the first teacher salary/payment cycle for
        // this specific enrollment.
        await _teacherPaymentService.GeneratePendingCyclesAsync(
            entity.StartDate.Date,
            enrollment.Id);


        entity.StudentFee = request.StudentFee;
        entity.TeacherSalary = request.TeacherSalary;
        entity.AdminRemarks = request.AdminRemarks;
        entity.Status = TeacherStudentRequestStatus.Approved;
        entity.ReviewedAt = DateTime.UtcNow;
        entity.ReviewedBy = reviewerId;
        entity.EnrollmentId = enrollment.Id;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(entity.Id)
            ?? throw new InvalidOperationException(
                "Request was approved but could not be retrieved.");
    }

    private IQueryable<TeacherStudentRequestResponse> BuildQuery()
    {
        return _context.TeacherStudentRequests
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Teacher)
            .Include(x => x.Student)
            .Include(x => x.Course)
            .Include(x => x.Schedules)
                .ThenInclude(x => x.TimeSlot)
            .Include(x => x.Schedules)
                .ThenInclude(x => x.ClassDuration)
            .Select(x => new TeacherStudentRequestResponse
            {
                Id = x.Id,

                TeacherId = x.TeacherId,
                TeacherName = x.Teacher.FullName,

                StudentId = x.StudentId,
                StudentName = x.StudentName,
                StudentPhoneNumber = x.StudentPhoneNumber,
                StudentAdmissionNumber =
                    x.Student != null
                        ? x.Student.AdmissionNumber
                        : null,

                CourseId = x.CourseId,
                CourseName = x.Course.CourseName,

                Status = x.Status,

                StudentFee = x.StudentFee,
                TeacherSalary = x.TeacherSalary,

                TeacherRemarks = x.TeacherRemarks,
                AdminRemarks = x.AdminRemarks,

                StartDate = x.StartDate,
                SubmittedAt = x.SubmittedAt,
                ReviewedAt = x.ReviewedAt,

                EnrollmentId = x.EnrollmentId,

                Schedules = x.Schedules
                    .Where(s => !s.IsDeleted)
                    .Select(s =>
                        new TeacherStudentRequestScheduleResponse
                        {
                            Id = s.Id,
                            DayOfWeek = s.DayOfWeek,
                            TimeSlotId = s.TimeSlotId,
                            TimeSlotName = s.TimeSlot.Name,
                            ClassDurationId = s.ClassDurationId,
                            ClassDurationName = s.ClassDuration.Name
                        })
                    .ToList()
            });
    }
}
