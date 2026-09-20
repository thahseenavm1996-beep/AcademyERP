using AcademyERP.Application.DTOs.Admissions;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.Admissions;
using AcademyERP.Domain.Entities.EnrollmentSchedules;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Identity;
using AcademyERP.Domain.Entities.Parents;
using AcademyERP.Domain.Entities.StudentParents;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Enums;
using AcademyERP.Application.Constants;
using AcademyERP.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        var requests =
            await _context.RegistrationRequests
                .Include(x => x.Students)
                    .ThenInclude(x => x.Program)
                .Include(x => x.Students)
                    .ThenInclude(x => x.Course)
                .OrderByDescending(x => x.SubmittedAt)
                .ToListAsync();

        var programs =
            await _context.Programs
                .ToDictionaryAsync(
                    x => x.Id,
                    x => x.ProgramName);

        return requests
            .Select(x =>
                new RegistrationRequestResponse
                {
                    Id = x.Id,

                    ParentName =
                        x.ParentName,

                    Email =
                        x.Email,

                    PhoneNumber =
                        x.PhoneNumber,

                    Country =
                        x.Country,

                    Status =
                        x.Status,

                    SubmittedAt =
                        x.SubmittedAt,

                    Students =
                        x.Students
                            .Select(s =>
                                new RegistrationRequestStudentResponse
                                {
                                    Id = s.Id,

                                    StudentName =
                                        s.StudentName,

                                    DateOfBirth =
                                        s.DateOfBirth,

                                    Gender =
                                        s.Gender,

                                    ProgramId =
                                        s.ProgramId,

                                    ProgramName =
                                        programs.TryGetValue(
                                            s.ProgramId,
                                            out var name)
                                                ? name
                                                : "Unknown Program",

                                    CourseId =
                                        s.CourseId,

                                    CourseName =
                                        s.Course?.CourseName,

                                    PreferredTime =
                                        s.PreferredTime,

                                    TeacherId =
                                        s.TeacherId,

                                    TeacherName =
                                        s.Teacher?.FullName
                                })
                            .ToList()
                })
            .ToList();
    }

    public async Task<RegistrationRequestResponse?>
        GetByIdAsync(Guid id)
    {
        var request =
            await _context.RegistrationRequests
                .Include(x => x.Students)
                    .ThenInclude(x => x.Course)
                .Include(x => x.Students)
                    .ThenInclude(x => x.Teacher)
                .Include(x => x.Students)
                    .ThenInclude(x => x.TimeSlot)
                .Include(x => x.Students)
                    .ThenInclude(x => x.ClassDuration)
                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (request == null)
        {
            return null;
        }

        var programs =
            await _context.Programs
                .ToDictionaryAsync(
                    x => x.Id,
                    x => x.ProgramName);

        return new RegistrationRequestResponse
        {
            Id = request.Id,

            ParentName =
                request.ParentName,

            Email =
                request.Email,

            PhoneNumber =
                request.PhoneNumber,

            Country =
                request.Country,

            Status =
                request.Status,

            SubmittedAt =
                request.SubmittedAt,

            Students =
                request.Students
                    .Select(x =>
                        new RegistrationRequestStudentResponse
                        {
                            Id = x.Id,

                            StudentName =
                                x.StudentName,

                            DateOfBirth =
                                x.DateOfBirth,

                            Gender =
                                x.Gender,

                            ProgramId =
                                x.ProgramId,

                            ProgramName =
                                programs.TryGetValue(
                                    x.ProgramId,
                                    out var name)
                                        ? name
                                        : "Unknown Program",

                            PreferredTime =
                                x.PreferredTime,

                            CourseId =
                                x.CourseId,

                            CourseName =
                                x.Course?.CourseName,

                            TeacherId =
                                x.TeacherId,

                            TeacherName =
                                x.Teacher?.FullName,

                            TimeSlotId =
                                x.TimeSlotId,

                            TimeSlotName =
                                x.TimeSlot?.Name,

                            ClassDurationId =
                                x.ClassDurationId,

                            ClassDurationName =
                                x.ClassDuration?.Name
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
                ParentName =
                    request.ParentName,

                Email =
                    request.Email,

                PhoneNumber =
                    request.PhoneNumber,

                Country =
                    request.Country,

                Address =
                    request.Address
            };

        foreach (var student in request.Students)
        {
            registrationRequest.Students.Add(
                new RegistrationRequestStudent
                {
                    StudentName =
                        student.StudentName,

                    DateOfBirth =
                        student.DateOfBirth,

                    Gender =
                        student.Gender,

                    ProgramId =
                        student.ProgramId,

                    CourseId =
                        student.CourseId,

                    PreferredTime =
                        student.PreferredTime,

                    Remarks =
                        student.Remarks
                });
        }

        _context.RegistrationRequests.Add(
            registrationRequest);

        await _context.SaveChangesAsync();

        return new RegistrationRequestResponse
        {
            Id =
                registrationRequest.Id,

            ParentName =
                registrationRequest.ParentName,

            Email =
                registrationRequest.Email,

            PhoneNumber =
                registrationRequest.PhoneNumber,

            Country =
                registrationRequest.Country,

            Status =
                registrationRequest.Status,

            SubmittedAt =
                registrationRequest.SubmittedAt,

            Students =
                registrationRequest.Students
                    .Select(x =>
                        new RegistrationRequestStudentResponse
                        {
                            Id =
                                x.Id,

                            StudentName =
                                x.StudentName,

                            DateOfBirth =
                                x.DateOfBirth,

                            Gender =
                                x.Gender,

                            ProgramId =
                                x.ProgramId,

                            PreferredTime =
                                x.PreferredTime,

                            CourseId =
                                x.CourseId
                        })
                    .ToList()
        };
    }

    public async Task<bool> ApproveAsync(
        ApproveRegistrationRequest request)
    {
        await using var transaction =
            await _context.Database
                .BeginTransactionAsync();

        try
        {
            // ==========================================
            // REGISTRATION REQUEST
            // ==========================================

            var registrationRequest =
                await _context.RegistrationRequests
                    .Include(x => x.Students)
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                            request.RegistrationRequestId);

            if (registrationRequest == null)
            {
                return false;
            }

            if (registrationRequest.Status !=
                RegistrationStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Request status is {registrationRequest.Status}. Only pending requests can be approved.");
            }

            // ==========================================
            // VALIDATE APPROVAL DATA
            // ==========================================

            if (request.Students == null ||
                request.Students.Count == 0)
            {
                throw new InvalidOperationException(
                    "At least one student is required.");
            }

            // ==========================================
            // PARENT NUMBER
            // ==========================================

            var parentSequence =
                await _context.DocumentSequences
                    .SingleAsync(
                        x => x.Code == "PARENT");

            var parentNumber =
                $"{parentSequence.Prefix}{parentSequence.NextNumber}";

            parentSequence.NextNumber++;

            // ==========================================
            // CREATE OR GET PARENT USER
            // ==========================================

            var existingUser =
                await _userManager
                    .FindByEmailAsync(
                        registrationRequest.Email);

            ApplicationUser parentUser;

            if (existingUser != null)
            {
                parentUser = existingUser;
            }
            else
            {
                parentUser =
                    new ApplicationUser
                    {
                        UserName =
                            registrationRequest.Email,

                        Email =
                            registrationRequest.Email,

                        FullName =
                            registrationRequest.ParentName,

                        PhoneNumber =
                            registrationRequest.PhoneNumber,

                        IsActive = true
                    };

                var parentUserResult =
                    await _userManager.CreateAsync(
                        parentUser,
                        "Parent@123");

                if (!parentUserResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(
                            ", ",
                            parentUserResult.Errors
                                .Select(
                                    x => x.Description)));
                }

                await _userManager.AddToRoleAsync(
                    parentUser,
                    Roles.Parent);
            }

            // ==========================================
            // CREATE OR GET PARENT
            // ==========================================

            var existingParent =
                await _context.Parents
                    .FirstOrDefaultAsync(
                        p =>
                            p.Email ==
                                registrationRequest.Email
                            ||
                            p.PhoneNumber ==
                                registrationRequest.PhoneNumber);

            Parent parent;

            if (existingParent != null)
            {
                parent = existingParent;

                parent.ApplicationUserId =
                    parentUser.Id;
            }
            else
            {
                parent =
                    new Parent
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

            // ==========================================
            // STUDENTS + ENROLLMENTS
            // ==========================================

            foreach (var approvalStudent
                in request.Students)
            {
                var registrationStudent =
                    registrationRequest.Students
                        .FirstOrDefault(
                            x =>
                                x.Id ==
                                approvalStudent
                                    .RegistrationStudentId);

                if (registrationStudent == null)
                {
                    throw new InvalidOperationException(
                        "Registration student not found.");
                }

                // ------------------------------------------
                // REQUIRED APPROVAL VALUES
                // ------------------------------------------

                if (approvalStudent.CourseId ==
                    Guid.Empty)
                {
                    throw new InvalidOperationException(
                        $"Course is required for {registrationStudent.StudentName}.");
                }

                if (!approvalStudent.TeacherId.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Teacher is required for {registrationStudent.StudentName}.");
                }

                if (!approvalStudent.TimeSlotId.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Time slot is required for {registrationStudent.StudentName}.");
                }

                if (!approvalStudent.ClassDurationId.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Class duration is required for {registrationStudent.StudentName}.");
                }

                if (approvalStudent.DaysOfWeek == null ||
                    approvalStudent.DaysOfWeek.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"At least one class day is required for {registrationStudent.StudentName}.");
                }

                var selectedDays =
                    approvalStudent.DaysOfWeek
                        .Distinct()
                        .ToList();

                // ------------------------------------------
                // COURSE
                // ------------------------------------------

                var course =
                    await _context.Courses
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                approvalStudent.CourseId);

                if (course == null)
                {
                    throw new InvalidOperationException(
                        $"Course not found for {registrationStudent.StudentName}.");
                }

                // Course must belong to the student's program.
                if (course.ProgramId !=
                    registrationStudent.ProgramId)
                {
                    throw new InvalidOperationException(
                        $"The selected course does not belong to the selected program for {registrationStudent.StudentName}.");
                }

                // ------------------------------------------
                // TIME SLOT
                // ------------------------------------------

                var timeSlot =
                    await _context.TimeSlots
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                approvalStudent.TimeSlotId.Value
                                &&
                                x.IsActive);

                if (timeSlot == null)
                {
                    throw new InvalidOperationException(
                        $"Selected time slot is invalid or inactive for {registrationStudent.StudentName}.");
                }

                // ------------------------------------------
                // CLASS DURATION
                // ------------------------------------------

                var classDuration =
                    await _context.ClassDurations
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                approvalStudent
                                    .ClassDurationId.Value
                                &&
                                x.IsActive);

                if (classDuration == null)
                {
                    throw new InvalidOperationException(
                        $"Selected class duration is invalid or inactive for {registrationStudent.StudentName}.");
                }

                // ------------------------------------------
                // TEACHER
                // ------------------------------------------

                var teacher =
    await _context.Teachers
        .FirstOrDefaultAsync(
            x =>
                x.Id ==
                approvalStudent.TeacherId.Value);

                if (teacher == null)
                {
                    throw new InvalidOperationException(
                        $"Selected teacher was not found for {registrationStudent.StudentName}.");
                }

                // Student gender must match teacher gender.
                

                // ------------------------------------------
                // TEACHER AVAILABILITY
                // ------------------------------------------

                var teacherAvailability =
                    await _context.TeacherAvailabilities
                        .Where(
                            x =>
                                x.TeacherId ==
                                    teacher.Id
                                &&
                                x.TimeSlotId ==
                                    timeSlot.Id
                                &&
                                x.IsAvailable)
                        .ToListAsync();

                foreach (var day in selectedDays)
                {
                    var availableOnDay =
                        teacherAvailability.Any(
                            x =>
                                x.DayOfWeek ==
                                day);

                    if (!availableOnDay)
                    {
                        throw new InvalidOperationException(
                            $"The selected teacher is not available on {day} at {timeSlot.Name} for {registrationStudent.StudentName}.");
                    }
                }

                // ------------------------------------------
                // SAVE APPROVAL VALUES
                // ------------------------------------------

                registrationStudent.CourseId =
                    approvalStudent.CourseId;

                registrationStudent.TeacherId =
                    approvalStudent.TeacherId;

                registrationStudent.TimeSlotId =
                    approvalStudent.TimeSlotId;

                registrationStudent.ClassDurationId =
                    approvalStudent.ClassDurationId;

                // ------------------------------------------
                // STUDENT NUMBER
                // ------------------------------------------

                var studentSequence =
                    await _context.DocumentSequences
                        .SingleAsync(
                            x =>
                                x.Code ==
                                "STUDENT");

                var admissionNumber =
                    $"{studentSequence.Prefix}{studentSequence.NextNumber}";

                studentSequence.NextNumber++;

                // ------------------------------------------
                // STUDENT USER
                // ------------------------------------------

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
                    throw new InvalidOperationException(
                        string.Join(
                            ", ",
                            studentUserResult.Errors
                                .Select(
                                    x => x.Description)));
                }

                await _userManager.AddToRoleAsync(
                    studentUser,
                    Roles.Student);

                // ------------------------------------------
                // STUDENT
                // ------------------------------------------

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

                // ------------------------------------------
                // STUDENT-PARENT
                // ------------------------------------------

                var studentParent =
                    new StudentParent
                    {
                        Student =
                            student,

                        Parent =
                            parent,

                        Relationship =
                            ParentRelationship.Father,

                        IsPrimaryContact =
                            true
                    };

                _context.StudentParents.Add(
                    studentParent);

                // ==========================================
                // ONE ENROLLMENT
                // ==========================================

                var enrollment =
                    new Enrollment
                    {
                        Student =
                            student,

                        CourseId =
                            course.Id,

                        TeacherId =
                            teacher.Id,

                        StartDate =
                            DateTime.UtcNow,

                        MonthlyFee =
                            course.StandardMonthlyFee,

                        ScholarshipAmount =
                            0,

                        DiscountAmount =
                            0,

                        FinalMonthlyFee =
                            course.StandardMonthlyFee,

                        NextBillingDate =
                            DateTime.UtcNow.AddMonths(1),

                        Status =
                            EnrollmentStatus.Active
                    };

                // ==========================================
                // ONE ENROLLMENT SCHEDULE PER SELECTED DAY
                // ==========================================

                foreach (var day in selectedDays)
                {
                    enrollment.EnrollmentSchedules.Add(
                        new EnrollmentSchedule
                        {
                            DayOfWeek =
                                day,

                            TimeSlotId =
                                timeSlot.Id,

                            ClassDurationId =
                                classDuration.Id,

                            IsActive =
                                true
                        });
                }

                // ==========================================
                // ADD ENROLLMENT ONLY ONCE
                // ==========================================

                _context.Enrollments.Add(
                    enrollment);

                // IMPORTANT:
                // Do NOT call EnrollmentService here.
                // Do NOT create TeachingSchedule here.
                //
                // Registration approval creates:
                // Student
                // StudentParent
                // Enrollment
                // EnrollmentSchedules
                //
                // TeachingSchedule is created separately
                // from the Teaching Schedule screen.
            }

            // ==========================================
            // APPROVE REQUEST
            // ==========================================

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
        var request =
            await _context.RegistrationRequests
                .FirstOrDefaultAsync(
                    x => x.Id == id);

        if (request == null)
        {
            return false;
        }

        if (request.Status !=
            RegistrationStatus.Pending)
        {
            throw new InvalidOperationException(
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

    public async Task<bool> MarkAsViewedAsync(Guid id)
{
    var request = await _context.RegistrationRequests
        .FirstOrDefaultAsync(x =>
            x.Id == id &&
            !x.IsDeleted);

    if (request == null)
        return false;

    if (request.ViewedByAdminAt == null)
    {
        request.ViewedByAdminAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    return true;
}
}