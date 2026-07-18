using AcademyERP.Application.DTOs.Courses;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseResponse>> GetAllAsync()
    {
        return await _context.Courses
            .Include(x => x.Program)
            .Select(x => new CourseResponse
            {
                Id = x.Id,
                ProgramId = x.ProgramId,
                ProgramName = x.Program.ProgramName,
                CourseCode = x.CourseCode,
                CourseName = x.CourseName,
                Description = x.Description,
                StandardMonthlyFee = x.StandardMonthlyFee,
                IsGroupClassAllowed = x.IsGroupClassAllowed,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<CourseResponse?> GetByIdAsync(Guid id)
    {
        var course = await _context.Courses
            .Include(x => x.Program)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (course == null)
            return null;

        return new CourseResponse
        {
            Id = course.Id,
            ProgramId = course.ProgramId,
            ProgramName = course.Program.ProgramName,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Description = course.Description,
            StandardMonthlyFee = course.StandardMonthlyFee,
            IsGroupClassAllowed = course.IsGroupClassAllowed,
            IsActive = course.IsActive
        };
    }

    public async Task<CourseResponse> CreateAsync(CreateCourseRequest request)
    {
        var course = new Course
        {
            ProgramId = request.ProgramId,
            CourseCode = request.CourseCode,
            CourseName = request.CourseName,
            Description = request.Description,
            StandardMonthlyFee = request.StandardMonthlyFee,
            IsGroupClassAllowed = request.IsGroupClassAllowed,
            IsActive = request.IsActive
        };

        _context.Courses.Add(course);

        await _context.SaveChangesAsync();

        var program = await _context.Programs
            .FirstAsync(x => x.Id == request.ProgramId);

        return new CourseResponse
        {
            Id = course.Id,
            ProgramId = course.ProgramId,
            ProgramName = program.ProgramName,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Description = course.Description,
            StandardMonthlyFee = course.StandardMonthlyFee,
            IsGroupClassAllowed = course.IsGroupClassAllowed,
            IsActive = course.IsActive
        };
    }

    public async Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request)
    {
        var course = await _context.Courses
            .Include(x => x.Program)
            .FirstAsync(x => x.Id == id);

        course.ProgramId = request.ProgramId;
        course.CourseCode = request.CourseCode;
        course.CourseName = request.CourseName;
        course.Description = request.Description;
        course.StandardMonthlyFee = request.StandardMonthlyFee;
        course.IsGroupClassAllowed = request.IsGroupClassAllowed;
        course.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return new CourseResponse
        {
            Id = course.Id,
            ProgramId = course.ProgramId,
            ProgramName = course.Program.ProgramName,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            Description = course.Description,
            StandardMonthlyFee = course.StandardMonthlyFee,
            IsGroupClassAllowed = course.IsGroupClassAllowed,
            IsActive = course.IsActive
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await _context.Courses
            .FirstAsync(x => x.Id == id);

        _context.Courses.Remove(course);

        await _context.SaveChangesAsync();
    }
}