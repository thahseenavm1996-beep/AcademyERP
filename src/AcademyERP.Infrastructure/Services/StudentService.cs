using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using AcademyERP.Application.DTOs.Students;
using AcademyERP.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AcademyERP.Application.Common;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using AcademyERP.Domain.Enums;
using AcademyERP.Application.Services;

namespace AcademyERP.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly IRepository<Student> _repository;
private readonly IMapper _mapper;
private readonly UserManager<ApplicationUser> _userManager;
private readonly ApplicationDbContext _context;
private readonly IFeeInvoiceService _feeInvoiceService;
private readonly ITeacherPaymentService _teacherPaymentService;
    public StudentService(
    IRepository<Student> repository,
    IMapper mapper,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IFeeInvoiceService feeInvoiceService,
    ITeacherPaymentService teacherPaymentService)
{
    _repository = repository;
    _mapper = mapper;
    _context = context;
    _userManager = userManager;
    _feeInvoiceService = feeInvoiceService;
    _teacherPaymentService = teacherPaymentService;
}
    public async Task<StudentProfileSummaryResponse> GetProfileSummaryAsync(Guid id)
{
    var student = await _context.Students
        .FirstOrDefaultAsync(x => x.Id == id);

    if (student == null)
        throw new Exception("Student not found");


    var programsCount = await _context.Enrollments
        .Where(x => x.StudentId == id)
        .Select(x => x.Course.ProgramId)
        .Distinct()
        .CountAsync();


    var teachersCount = await _context.Enrollments
        .Where(x => x.StudentId == id)
        .Select(x => x.TeacherId)
        .Distinct()
        .CountAsync();


  var completedClasses = await _context.ScheduledClasses
    .Where(x =>
        x.TeachingSchedule
         .Enrollment
         .StudentId == id
        &&
        x.Status == ScheduledClassStatus.Completed)
    .CountAsync();


var presentClasses = await _context.Attendances
    .Where(x =>
        x.ScheduledClass
         .TeachingSchedule
         .Enrollment
         .StudentId == id
        &&
        x.ScheduledClass.Status == ScheduledClassStatus.Completed
        &&
        x.Status == AttendanceStatus.Present)
    .CountAsync();


var attendancePercentage = completedClasses == 0
    ? 0
    : (decimal)presentClasses / completedClasses * 100;


  var outstandingFees = await _context.FeeInvoices
    .Where(x =>
        x.Enrollment.StudentId == id)
    .SumAsync(x => x.BalanceAmount);


    return new StudentProfileSummaryResponse
    {
        ProgramsCount = programsCount,
        TeachersCount = teachersCount,
        AttendancePercentage = Math.Round(attendancePercentage, 2),
        OutstandingFees = outstandingFees
    };
}
    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        /* if (request.DateOfBirth > DateTime.Today.AddYears(-4))
         {
             throw new ArgumentException("Student must be at least 4 years old.");
         }*/

        try
        {
            var sequence = await _context.DocumentSequences
                .SingleAsync(x => x.Code == "STUDENT");

            var generatedNumber = $"{sequence.Prefix}{sequence.NextNumber}";
            sequence.NextNumber++;

           ApplicationUser? user = null;

if (!string.IsNullOrWhiteSpace(request.Email))
{
    if (string.IsNullOrWhiteSpace(request.Password))
    {
        throw new ArgumentException(
            "Password is required when an email address is provided.");
    }

    user = new ApplicationUser
    {
        UserName = request.Email.Trim(),
        Email = request.Email.Trim(),
        FullName = request.FullName,
        PhoneNumber = request.PhoneNumber,
        IsActive = true
    };

    var result = await _userManager.CreateAsync(
        user,
        request.Password);

    if (!result.Succeeded)
    {
        throw new Exception(
            "Identity Error: " +
            string.Join(", ",
                result.Errors.Select(e => e.Description)));
    }

    await _userManager.AddToRoleAsync(user, "Student");
}

var student = _mapper.Map<Student>(request);

student.ApplicationUserId = user?.Id;
student.PhoneNumber = request.PhoneNumber;
student.AdmissionNumber = generatedNumber;

            _context.Students.Add(student);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return _mapper.Map<StudentResponse>(student);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    private string GenerateAdmissionNumber(Student? lastStudent)
    {
        const string prefix = "SGO";
        const int startingNumber = 100;

        if (lastStudent == null)
        {
            return $"{prefix}{startingNumber}";
        }

        var lastNumber = int.Parse(lastStudent.AdmissionNumber.Replace(prefix, ""));

        return $"{prefix}{lastNumber + 1}";
    }
    public async Task<PagedResponse<StudentResponse>> GetAllAsync(StudentQueryRequest request)
{
    var query = _context.Students
    .Where(s => !s.IsDeleted)
    .AsQueryable();

    if (!string.IsNullOrWhiteSpace(request.Search))
    {
        query = query.Where(s =>
            s.FullName.Contains(request.Search) ||
            s.AdmissionNumber.Contains(request.Search));
    }

    if (request.CourseId.HasValue)
    {
        query = query.Where(s =>
            s.Enrollments.Any(e => e.CourseId == request.CourseId.Value));
    }

    if (!string.IsNullOrWhiteSpace(request.Gender) &&
        Enum.TryParse<Gender>(request.Gender, true, out var gender))
    {
        query = query.Where(s => s.Gender == gender);
    }

    query = request.SortBy?.ToLower() switch
    {
        "fullname" => query.OrderBy(s => s.FullName),
        "admissionnumber" => query.OrderBy(s => s.AdmissionNumber),
        "admissiondate" => query.OrderBy(s => s.AdmissionDate),
        _ => query.OrderBy(s => s.FullName)
    };

    var totalCount = await query.CountAsync();

    var students = await query
        .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToListAsync();

    return new PagedResponse<StudentResponse>
    {
        Items = _mapper.Map<List<StudentResponse>>(students),
        Page = request.Page,
        PageSize = request.PageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling(
            (double)totalCount / request.PageSize)
    };
}
    public async Task<StudentResponse?> GetByIdAsync(Guid id)
{
    var student = await _context.Students
        .Include(x => x.StudentParents)
            .ThenInclude(x => x.Parent)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (student == null)
        return null;

    var response = _mapper.Map<StudentResponse>(student);

    var user = await GetStudentUserAsync(student);

    if (user != null)
    {
        response.Email = user.Email ?? string.Empty;
        response.PhoneNumber = user.PhoneNumber ?? string.Empty;
    }

    response.Status = student.Status.ToString();

    response.Parents = student.StudentParents
        .Where(x => x.Parent != null)
        .Select(x => new StudentParentResponse
        {
            ParentId = x.ParentId,
            FullName = x.Parent!.FullName,
            PhoneNumber = x.Parent.PhoneNumber,
            Email = x.Parent.Email,
            IsPrimaryContact = x.IsPrimaryContact
        })
        .ToList();

    var parentIds = student.StudentParents
        .Select(x => x.ParentId)
        .ToList();

    if (parentIds.Any())
    {
        response.Siblings = await _context.StudentParents
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x =>
                parentIds.Contains(x.ParentId) &&
                x.StudentId != student.Id &&
                x.Student != null)
            .Select(x => new StudentSiblingResponse
            {
                StudentId = x.StudentId,
                AdmissionNumber = x.Student!.AdmissionNumber,
                FullName = x.Student.FullName,
                Gender = x.Student.Gender.ToString(),
                Status = x.Student.Status.ToString()
            })
            .Distinct()
            .ToListAsync();
    }

    return response;
}

    public async Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.Id == id);

        if (student is null)
            throw new KeyNotFoundException("Student not found.");

       student.FullName = request.FullName;
student.PhoneNumber = request.PhoneNumber;
student.AdmissionDate = request.AdmissionDate;
        student.DateOfBirth = request.DateOfBirth;
        student.Gender = request.Gender;
        student.Country = request.Country;
        student.TimeZone = request.TimeZone;
        student.Remarks = request.Remarks;

        var user = await GetStudentUserAsync(student);

if (user != null)
{
    if (!string.IsNullOrWhiteSpace(request.Email))
    {
        user.Email = request.Email.Trim();
        user.UserName = request.Email.Trim();
    }

    user.PhoneNumber = request.PhoneNumber;
    user.FullName = request.FullName;

    var result = await _userManager.UpdateAsync(user);

    if (!result.Succeeded)
    {
        throw new Exception(
            string.Join(", ",
                result.Errors.Select(x => x.Description)));
    }
}
        await _context.SaveChangesAsync();

        return _mapper.Map<StudentResponse>(student);
    }
    public async Task<bool> DeleteAsync(Guid id)
{
    var student = await _context.Students.FindAsync(id);

    if (student == null || student.IsDeleted)
        return false;

    student.IsDeleted = true;
    student.Status = UserStatus.Inactive;

    await _context.SaveChangesAsync();

    return true;
}

public async Task<bool> SetStatusAsync(Guid id, bool isActive)
{
    await using var transaction =
        await _context.Database.BeginTransactionAsync();

    try
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.Id == id);

        if (student == null)
            return false;

        // Always make the student visible again when using
        // the Active / Inactive workflow.
        student.IsDeleted = false;

        student.Status = isActive
            ? UserStatus.Active
            : UserStatus.Inactive;

        // Enable / disable student login.
        var user = await GetStudentUserAsync(student);

        if (user != null)
        {
            user.IsActive = isActive;
        }

        // Reactivating a student does NOT automatically
        // reactivate enrollments or financial generation.
        if (!isActive)
        {
            var today = DateTime.UtcNow.Date;

            var activeEnrollments = await _context.Enrollments
                .Include(x => x.FeePauses)
                .Where(x =>
                    x.StudentId == student.Id &&
                    x.Status == EnrollmentStatus.Active)
                .ToListAsync();

            foreach (var enrollment in activeEnrollments)
            {
                // -------------------------------------------------
                // 1. Suspend enrollment
                // -------------------------------------------------

                enrollment.Status = EnrollmentStatus.Suspended;

                // -------------------------------------------------
                // 2. Pause fee generation
                // -------------------------------------------------

                var feePauseStartDate =
                    enrollment.StartDate.Date > today
                        ? enrollment.StartDate.Date
                        : today;

                var hasOpenFeePause =
                    enrollment.FeePauses.Any(x =>
                        !x.ResumeDate.HasValue);

                if (!hasOpenFeePause)
                {
                    await _feeInvoiceService.PauseEnrollmentAsync(
                        enrollment.Id,
                        feePauseStartDate,
                        "Student deactivated.");
                }

                // -------------------------------------------------
                // 3. Pause teacher salary generation
                // -------------------------------------------------

                var configurations =
                    await _context.TeacherPaymentConfigurations
                        .Include(x => x.Pauses)
                        .Where(x =>
                            x.EnrollmentId == enrollment.Id)
                        .ToListAsync();

                foreach (var configuration in configurations)
                {
                    var salaryPauseStartDate =
                        configuration.EffectiveFrom.Date > today
                            ? configuration.EffectiveFrom.Date
                            : today;

                    // Configuration already has an open pause.
                    if (configuration.Pauses.Any(x =>
                            !x.ResumeDate.HasValue))
                    {
                        continue;
                    }

                    // Configuration already ended before the
                    // student was deactivated.
                    if (configuration.EffectiveTo.HasValue &&
                        configuration.EffectiveTo.Value.Date <=
                        salaryPauseStartDate)
                    {
                        continue;
                    }

                    await _teacherPaymentService
                        .PauseConfigurationAsync(
                            configuration.Id,
                            salaryPauseStartDate,
                            "Student deactivated.");
                }
            }
        }

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return true;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
    public async Task<bool> ResetPasswordAsync(Guid id, string newPassword)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return false;

        var user = await GetStudentUserAsync(student);
        if (user == null)
            return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded;
    }
private async Task<ApplicationUser?> GetStudentUserAsync(Student student)
{
    if (!student.ApplicationUserId.HasValue)
        return null;

    return await _userManager.FindByIdAsync(
        student.ApplicationUserId.Value.ToString());
}
}