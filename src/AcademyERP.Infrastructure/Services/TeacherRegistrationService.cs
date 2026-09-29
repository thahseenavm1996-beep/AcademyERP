using AcademyERP.Application.DTOs.TeacherRegistrations;
using AcademyERP.Application.DTOs.Teachers;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.TeacherRegistrations;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Application.Services;

namespace AcademyERP.Infrastructure.Services;

public class TeacherRegistrationService : ITeacherRegistrationService
{
    private readonly ApplicationDbContext _context;
    private readonly ITeacherService _teacherService;

    public TeacherRegistrationService(
        ApplicationDbContext context,
        ITeacherService teacherService)
    {
        _context = context;
        _teacherService = teacherService;
    }

    public async Task<TeacherRegistrationResponse> CreateAsync(
        CreateTeacherRegistrationRequest request)
    {
        ValidateRequest(request);

        var emailExists = await _context.TeacherRegistrationRequests
            .AnyAsync(x =>
                x.Email == request.Email &&
                x.Status == TeacherRegistrationStatus.Pending &&
                !x.IsDeleted);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "A pending teacher registration already exists for this email address.");
        }

        await ValidateAvailabilityAsync(request.Availability);
        await ValidateCurrentClassesAsync(request.CurrentClasses);

        var registration = new TeacherRegistrationRequest
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            MobileNumber = request.MobileNumber.Trim(),
            Email = request.Email.Trim(),
            Country = request.Country.Trim(),
            Address = string.IsNullOrWhiteSpace(request.Address)
                ? null
                : request.Address.Trim(),

            Qualification = request.Qualification.Trim(),
            Department = request.Department.Trim(),
            JoiningDate = request.JoiningDate,
            TeachingExperience = string.IsNullOrWhiteSpace(request.TeachingExperience)
                ? null
                : request.TeachingExperience.Trim(),
            Remarks = string.IsNullOrWhiteSpace(request.Remarks)
                ? null
                : request.Remarks.Trim(),

            Status = TeacherRegistrationStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };

        foreach (var item in request.Availability
                     .GroupBy(x => new { x.DayOfWeek, x.TimeSlotId })
                     .Select(x => x.First()))
        {
            registration.Availability.Add(
                new TeacherRegistrationAvailability
                {
                    Id = Guid.NewGuid(),
                    DayOfWeek = item.DayOfWeek,
                    TimeSlotId = item.TimeSlotId
                });
        }

        foreach (var item in request.CurrentClasses)
        {
            registration.CurrentClasses.Add(
                new TeacherRegistrationClass
                {
                    Id = Guid.NewGuid(),
                    StudentId = item.StudentId,
StudentName = string.IsNullOrWhiteSpace(item.StudentName)
    ? null
    : item.StudentName.Trim(),
CourseId = item.CourseId,
                    DayOfWeek = item.DayOfWeek,
                    TimeSlotId = item.TimeSlotId,
                    ClassDurationId = item.ClassDurationId,
                    ClassType = string.IsNullOrWhiteSpace(item.ClassType)
                        ? null
                        : item.ClassType.Trim()
                });
        }

        _context.TeacherRegistrationRequests.Add(registration);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(registration.Id)
            ?? throw new InvalidOperationException(
                "Teacher registration was created but could not be loaded.");
    }

    public async Task<List<TeacherRegistrationResponse>> GetAllAsync()
    {
        var registrations = await BuildQuery()
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync();

        return registrations
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<TeacherRegistrationResponse?> GetByIdAsync(Guid id)
    {
        var registration = await BuildQuery()
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        return registration == null
            ? null
            : MapToResponse(registration);
    }

    public async Task<List<TeacherRegistrationResponse>> GetPendingAsync()
    {
        var registrations = await BuildQuery()
            .Where(x =>
                x.Status == TeacherRegistrationStatus.Pending &&
                !x.IsDeleted)
            .OrderBy(x => x.SubmittedAt)
            .ToListAsync();

        return registrations
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<bool> ApproveAsync(
        Guid id,
        ApproveTeacherRegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Password) ||
            request.Password.Length < 8)
        {
            throw new ArgumentException(
                "Password must be at least 8 characters long.");
        }

        var registration = await _context.TeacherRegistrationRequests
            .Include(x => x.Availability)
            .Include(x => x.CurrentClasses)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (registration == null)
            return false;

        if (registration.Status != TeacherRegistrationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending teacher registrations can be approved.");
        }

        var existingTeacher = await _context.Teachers
            .AnyAsync(x =>
                x.Email == registration.Email &&
                !x.IsDeleted);

        if (existingTeacher)
        {
            throw new InvalidOperationException(
                "A teacher already exists with this email address.");
        }

        var existingUser = await _context.Users
            .AnyAsync(x =>
                x.Email == registration.Email);

        if (existingUser)
        {
            throw new InvalidOperationException(
                "An account already exists with this email address.");
        }

        var teacherRequest = new CreateTeacherRequest
        {
            FullName = registration.FullName,
            DateOfBirth = registration.DateOfBirth,
            JoiningDate = registration.JoiningDate,
            Gender = registration.Gender,
            MobileNumber = registration.MobileNumber,
            Email = registration.Email,
            Qualification = registration.Qualification,
            Department = registration.Department,
            Country = registration.Country,
            TimeZone = GetDefaultTimeZone(registration.Country),
            Address = registration.Address,
            Remarks = registration.Remarks,
            Password = request.Password,
            Availability = registration.Availability
                .Select(x => new TeacherAvailabilityRequest
                {
                    DayOfWeek = x.DayOfWeek,
                    TimeSlotId = x.TimeSlotId
                })
                .ToList()
        };

        await _teacherService.CreateAsync(teacherRequest);

        registration.Status = TeacherRegistrationStatus.Approved;
        registration.ReviewedAt = DateTime.UtcNow;
        registration.ReviewRemarks = string.IsNullOrWhiteSpace(request.ReviewRemarks)
            ? null
            : request.ReviewRemarks.Trim();

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RejectAsync(
        Guid id,
        string? reviewRemarks = null)
    {
        var registration = await _context.TeacherRegistrationRequests
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (registration == null)
            return false;

        if (registration.Status != TeacherRegistrationStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending teacher registrations can be rejected.");
        }

        registration.Status = TeacherRegistrationStatus.Rejected;
        registration.ReviewedAt = DateTime.UtcNow;
        registration.ReviewRemarks = string.IsNullOrWhiteSpace(reviewRemarks)
            ? null
            : reviewRemarks.Trim();

        await _context.SaveChangesAsync();

        return true;
    }

    private IQueryable<TeacherRegistrationRequest> BuildQuery()
    {
        return _context.TeacherRegistrationRequests
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Availability)
                .ThenInclude(x => x.TimeSlot)
            .Include(x => x.CurrentClasses)
                .ThenInclude(x => x.Student)
            .Include(x => x.CurrentClasses)
                .ThenInclude(x => x.Course)
            .Include(x => x.CurrentClasses)
                .ThenInclude(x => x.TimeSlot)
            .Include(x => x.CurrentClasses)
                .ThenInclude(x => x.ClassDuration);
    }

    private static TeacherRegistrationResponse MapToResponse(
        TeacherRegistrationRequest registration)
    {
        return new TeacherRegistrationResponse
        {
            Id = registration.Id,
            FullName = registration.FullName,
            DateOfBirth = registration.DateOfBirth,
            Gender = registration.Gender,
            MobileNumber = registration.MobileNumber,
            Email = registration.Email,
            Country = registration.Country,
            Address = registration.Address,

            Qualification = registration.Qualification,
            Department = registration.Department,
            JoiningDate = registration.JoiningDate,
            TeachingExperience = registration.TeachingExperience,
            Remarks = registration.Remarks,

            Status = registration.Status,
            SubmittedAt = registration.SubmittedAt,
            ReviewedAt = registration.ReviewedAt,
            ReviewRemarks = registration.ReviewRemarks,

            Availability = registration.Availability
                .Select(x => new TeacherRegistrationAvailabilityResponse
                {
                    Id = x.Id,
                    DayOfWeek = x.DayOfWeek,
                    TimeSlotId = x.TimeSlotId,
                    TimeSlotName = x.TimeSlot?.Name ?? string.Empty
                })
                .ToList(),

            CurrentClasses = registration.CurrentClasses
                .Select(x => new TeacherRegistrationClassResponse
{
    Id=x.Id,
    StudentId = x.StudentId,
    StudentName = x.Student?.FullName
        ?? x.StudentName
        ?? string.Empty,
                    AdmissionNumber = x.Student?.AdmissionNumber ?? string.Empty,

                    CourseId = x.CourseId,
                    CourseName = x.Course?.CourseName ?? string.Empty,
                    CourseCode = x.Course?.CourseCode ?? string.Empty,

                    DayOfWeek = x.DayOfWeek,

                    TimeSlotId = x.TimeSlotId,
                    TimeSlotName = x.TimeSlot?.Name ?? string.Empty,

                    ClassDurationId = x.ClassDurationId,
                    ClassDurationName = x.ClassDuration?.Name ?? string.Empty,

                    ClassType = x.ClassType
                })
                .ToList()
        };
    }

    private async Task ValidateAvailabilityAsync(
        IEnumerable<CreateTeacherRegistrationAvailability> availability)
    {
        var items = availability
            .GroupBy(x => new { x.DayOfWeek, x.TimeSlotId })
            .Select(x => x.First())
            .ToList();

        if (items.Count == 0)
            return;

        var slotIds = items
            .Select(x => x.TimeSlotId)
            .Distinct()
            .ToList();

        var activeSlotIds = await _context.TimeSlots
            .Where(x =>
                slotIds.Contains(x.Id) &&
                x.IsActive &&
                !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync();

        if (activeSlotIds.Count != slotIds.Count)
        {
            throw new InvalidOperationException(
                "One or more selected time slots are inactive or invalid.");
        }
    }

    private async Task ValidateCurrentClassesAsync(
        IEnumerable<CreateTeacherRegistrationClass> classes)
    {
        foreach (var item in classes)
        {
           if (item.StudentId.HasValue)
{
    var studentExists = await _context.Students
        .AnyAsync(x =>
            x.Id == item.StudentId.Value &&
            !x.IsDeleted);

    if (!studentExists)
        throw new InvalidOperationException(
            "One or more selected students are invalid.");
}
else if (string.IsNullOrWhiteSpace(item.StudentName))
{
    throw new InvalidOperationException(
        "Please select a student or enter the student's name.");
}

            var courseExists = await _context.Courses
                .AnyAsync(x =>
                    x.Id == item.CourseId &&
                    !x.IsDeleted &&
                    x.IsActive);

            if (!courseExists)
                throw new InvalidOperationException(
                    "One or more selected courses are invalid.");

            var timeSlotExists = await _context.TimeSlots
                .AnyAsync(x =>
                    x.Id == item.TimeSlotId &&
                    !x.IsDeleted &&
                    x.IsActive);

            if (!timeSlotExists)
                throw new InvalidOperationException(
                    "One or more selected time slots are invalid.");

            var durationExists = await _context.ClassDurations
                .AnyAsync(x =>
                    x.Id == item.ClassDurationId &&
                    !x.IsDeleted &&
                    x.IsActive);

            if (!durationExists)
                throw new InvalidOperationException(
                    "One or more selected class durations are invalid.");
        }
    }

    private static void ValidateRequest(
        CreateTeacherRegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("Full name is required.");

        if (request.DateOfBirth > DateTime.Today.AddYears(-18))
            throw new ArgumentException(
                "Teacher must be at least 18 years old.");

        if (request.JoiningDate.Date > DateTime.Today)
            throw new ArgumentException(
                "Joining date cannot be in the future.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email is required.");

        if (string.IsNullOrWhiteSpace(request.MobileNumber))
            throw new ArgumentException("Mobile number is required.");

        if (string.IsNullOrWhiteSpace(request.Qualification))
            throw new ArgumentException("Qualification is required.");

        if (string.IsNullOrWhiteSpace(request.Department))
            throw new ArgumentException("Department is required.");

        if (string.IsNullOrWhiteSpace(request.Country))
            throw new ArgumentException("Country is required.");
    }

    private static string GetDefaultTimeZone(string country)
    {
        return country.Trim().Equals(
            "India",
            StringComparison.OrdinalIgnoreCase)
            ? "Asia/Kolkata"
            : "UTC";
    }
}
