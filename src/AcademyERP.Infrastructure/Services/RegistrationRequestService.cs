using AcademyERP.Application.DTOs.Admissions;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.Admissions;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.Parents;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.StudentParents;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Identity;
using AcademyERP.Application.Constants;
using Microsoft.AspNetCore.Identity;
using AcademyERP.Domain.Entities.EnrollmentSchedules;
namespace AcademyERP.Infrastructure.Services;

public class RegistrationRequestService
    : IRegistrationRequestService
{
    private readonly ApplicationDbContext _context;
     private readonly UserManager<ApplicationUser> _userManager;

    public RegistrationRequestService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager)
{
    _context = context;
    _userManager = userManager;
}
  public async Task<List<RegistrationRequestResponse>>
    GetAllAsync()
{
    var requests = await _context
        .RegistrationRequests
        .Include(x => x.Students)
        .ThenInclude(x => x.Program)
        .Include(x => x.Students)
    .ThenInclude(x => x.Course)
        .OrderByDescending(x => x.SubmittedAt)
        .ToListAsync();
var programs = await _context.Programs
    .ToDictionaryAsync(
        x => x.Id,
        x => x.ProgramName);
    return requests
        .Select(x =>
            new RegistrationRequestResponse
            {
                Id = x.Id,
                ParentName = x.ParentName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Country = x.Country,
                Status = x.Status,
                SubmittedAt = x.SubmittedAt,

                Students = x.Students
    .Select(s =>
        new RegistrationRequestStudentResponse
        {
            Id = s.Id,
            StudentName = s.StudentName,
            DateOfBirth = s.DateOfBirth,
            Gender = s.Gender,

            ProgramId = s.ProgramId,

            ProgramName =
                programs.TryGetValue(
                    s.ProgramId,
                    out var name)
                        ? name
                        : "Unknown Program",

            CourseId = s.CourseId,
            CourseName = s.Course != null
                ? s.Course.CourseName
                : null,

            PreferredTime = s.PreferredTime
        })
    .ToList()
            })
        .ToList();
}

public async Task<RegistrationRequestResponse?>
    GetByIdAsync(Guid id)
{
    var request = await _context
    .RegistrationRequests
    .Include(x => x.Students)
        .ThenInclude(x => x.Course)
    .Include(x => x.Students)
        .ThenInclude(x => x.Teacher)
    .Include(x => x.Students)
        .ThenInclude(x => x.TimeSlot)
    .Include(x => x.Students)
        .ThenInclude(x => x.ClassDuration)
    .FirstOrDefaultAsync(x => x.Id == id);

    if (request == null)
        return null;

    var programs = await _context.Programs
        .ToDictionaryAsync(
            x => x.Id,
            x => x.ProgramName);

    return new RegistrationRequestResponse
    {
        Id = request.Id,
        ParentName = request.ParentName,
        Email = request.Email,
        PhoneNumber = request.PhoneNumber,
        Country = request.Country,
        Status = request.Status,
        SubmittedAt = request.SubmittedAt,

        Students = request.Students
            .Select(x =>
                new RegistrationRequestStudentResponse
                {
                    Id = x.Id,
                    StudentName = x.StudentName,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    ProgramId = x.ProgramId,

                    ProgramName =
                        programs.TryGetValue(
                            x.ProgramId,
                            out var name)
                            ? name
                            : "Unknown Program",

                    PreferredTime = x.PreferredTime,
                    CourseId = x.CourseId,
CourseName = x.Course?.CourseName,

TeacherId = x.TeacherId,
TeacherName = x.Teacher?.FullName,


                })
            .ToList()
    };
}
public async Task<RegistrationRequestResponse>
    CreateAsync(
        CreateRegistrationRequest request)
{
    var registrationRequest =
        new RegistrationRequest
        {
            ParentName = request.ParentName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Country = request.Country,
            Address = request.Address
        };

    foreach (var student in request.Students)
    {
        registrationRequest.Students.Add(
    new RegistrationRequestStudent
    {
        StudentName = student.StudentName,
        DateOfBirth = student.DateOfBirth,
        Gender = student.Gender,
        ProgramId = student.ProgramId,
        CourseId = student.CourseId,
        PreferredTime = student.PreferredTime,
        Remarks = student.Remarks
    });
    }

    _context.RegistrationRequests.Add(
        registrationRequest);

    await _context.SaveChangesAsync();

    return new RegistrationRequestResponse
    {
        Id = registrationRequest.Id,
        ParentName = registrationRequest.ParentName,
        Email = registrationRequest.Email,
        PhoneNumber = registrationRequest.PhoneNumber,
        Country = registrationRequest.Country,
        Status = registrationRequest.Status,
        SubmittedAt = registrationRequest.SubmittedAt,

        Students = registrationRequest.Students
            .Select(x =>
                new RegistrationRequestStudentResponse
                {
                    Id = x.Id,
                    StudentName = x.StudentName,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    ProgramId = x.ProgramId,
                    PreferredTime = x.PreferredTime
                })
            .ToList()
    };
}
/*public async Task<bool> ApproveAsync(Guid id)
{
    var request = await _context
        .RegistrationRequests
        .FirstOrDefaultAsync(x => x.Id == id);

    if (request == null)
        return false;

    request.Status =
        RegistrationStatus.Approved;

    request.ReviewedAt =
        DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return true;
}*/
public async Task<bool> ApproveAsync(
    ApproveRegistrationRequest request)
{
    using var transaction =
        await _context.Database.BeginTransactionAsync();

    try
    {
        var registrationRequest =
            await _context.RegistrationRequests
                .Include(x => x.Students)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.RegistrationRequestId);

        if (registrationRequest == null)
            return false;

        if (registrationRequest.Status != RegistrationStatus.Pending)
        {
            throw new Exception(
                $"Request status is {registrationRequest.Status}. Only pending requests can be approved.");
        }

        // =========================
        // PARENT NUMBER
        // =========================

        var parentSequence =
            await _context.DocumentSequences
                .SingleAsync(x =>
                    x.Code == "PARENT");

        var parentNumber =
            $"{parentSequence.Prefix}{parentSequence.NextNumber}";

        parentSequence.NextNumber++;

        // =========================
        // CREATE OR GET PARENT USER
        // =========================

        var existingUser =
            await _userManager.FindByEmailAsync(
                registrationRequest.Email);

        ApplicationUser parentUser;

        if (existingUser != null)
        {
            parentUser = existingUser;
        }
        else
        {
            parentUser = new ApplicationUser
            {
                UserName = registrationRequest.Email,
                Email = registrationRequest.Email,
                FullName = registrationRequest.ParentName,
                PhoneNumber = registrationRequest.PhoneNumber,
                IsActive = true
            };

            var parentUserResult =
                await _userManager.CreateAsync(
                    parentUser,
                    "Parent@123");

            if (!parentUserResult.Succeeded)
            {
                throw new Exception(
                    string.Join(", ",
                        parentUserResult.Errors
                            .Select(x => x.Description)));
            }

            await _userManager.AddToRoleAsync(
                parentUser,
                Roles.Parent);
        }

        // =========================
        // CREATE OR GET PARENT
        // =========================

        var existingParent =
            await _context.Parents
                .FirstOrDefaultAsync(p =>
                    p.Email == registrationRequest.Email ||
                    p.PhoneNumber == registrationRequest.PhoneNumber);

        Parent parent;

        if (existingParent != null)
        {
            parent = existingParent;

            parent.ApplicationUserId =
                parentUser.Id;
        }
        else
        {
            parent = new Parent
            {
                ApplicationUserId =
                    parentUser.Id,

                ParentNumber =
                    parentNumber,

                FullName =
                    registrationRequest.ParentName,

                Email =
                    registrationRequest.Email,

                PhoneNumber =
                    registrationRequest.PhoneNumber,

                Country =
                    registrationRequest.Country,

                Address =
                    registrationRequest.Address,

                Status =
                    UserStatus.Active
            };

            _context.Parents.Add(parent);
        }

        // =========================
        // STUDENTS
        // =========================

        foreach (var approvalStudent in request.Students)
        {
            var registrationStudent =
                registrationRequest.Students
                    .FirstOrDefault(x =>
                        x.Id ==
                        approvalStudent.RegistrationStudentId);

            if (registrationStudent == null)
                continue;
                // Save selected values back to registration request student

registrationStudent.CourseId =
    approvalStudent.CourseId;

registrationStudent.TeacherId =
    approvalStudent.TeacherId;

/*registrationStudent.TimeSlotId =
    approvalStudent.TimeSlotId;

registrationStudent.ClassDurationId =
    approvalStudent.ClassDurationId;*/

            var studentSequence =
                await _context.DocumentSequences
                    .SingleAsync(x =>
                        x.Code == "STUDENT");

            var admissionNumber =
                $"{studentSequence.Prefix}{studentSequence.NextNumber}";

            studentSequence.NextNumber++;

            var studentUser =
                new ApplicationUser
                {
                    UserName =
                        admissionNumber.ToLower(),

                    FullName =
                        registrationStudent.StudentName,

                    IsActive = true
                };

            var studentUserResult =
                await _userManager.CreateAsync(
                    studentUser,
                    "Student@123");

            if (!studentUserResult.Succeeded)
            {
                throw new Exception(
                    string.Join(", ",
                        studentUserResult.Errors
                            .Select(x => x.Description)));
            }

            await _userManager.AddToRoleAsync(
                studentUser,
                Roles.Student);

            var student =
                new Student
                {
                    ApplicationUserId =
                        studentUser.Id,

                    AdmissionNumber =
                        admissionNumber,

                    FullName =
                        registrationStudent.StudentName,

                    AdmissionDate =
                        DateTime.UtcNow,

                    DateOfBirth =
                        registrationStudent.DateOfBirth,

                    Gender =
                        registrationStudent.Gender,

                    Country =
                        registrationRequest.Country,

                    Status =
                        UserStatus.Active
                };

            _context.Students.Add(student);

            var studentParent =
                new StudentParent
                {
                    Student = student,
                    Parent = parent,

                    Relationship =
                        ParentRelationship.Father,

                    IsPrimaryContact = true
                };

            _context.StudentParents.Add(
                studentParent);

            var course =
                await _context.Courses
                    .FirstOrDefaultAsync(x =>
                        x.Id ==
                        approvalStudent.CourseId);

            if (course == null)
            {
                throw new Exception(
                    "Course not found.");
            }

            var enrollment =
                new Enrollment
                {
                    Student = student,

                    CourseId =
                        approvalStudent.CourseId,

                    TeacherId =
                        approvalStudent.TeacherId,

                 

                    StartDate =
                        DateTime.UtcNow,

                    MonthlyFee =
                        course.StandardMonthlyFee,

                    ScholarshipAmount = 0,

                    DiscountAmount = 0,

                    FinalMonthlyFee =
                        course.StandardMonthlyFee,

                    NextBillingDate =
                        DateTime.UtcNow.AddMonths(1),

                    Status =
                        EnrollmentStatus.Active
                };
                 _context.Enrollments.Add(enrollment); 
        }

        registrationRequest.Status =
            RegistrationStatus.Approved;

        registrationRequest.ReviewedAt =
            DateTime.UtcNow;

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
public async Task<bool> RejectAsync(
    Guid id,
    string remarks)
{
    var request = await _context
        .RegistrationRequests
        .FirstOrDefaultAsync(x => x.Id == id);

    if (request == null)
        return false;
         if (request.Status != RegistrationStatus.Pending)
    {
        throw new Exception(
            "Only pending requests can be rejected.");
    }

    request.Status =
        RegistrationStatus.Rejected;

    request.ReviewedAt =
        DateTime.UtcNow;

    request.ReviewRemarks =
        remarks;

    await _context.SaveChangesAsync();

    return true;
}
}