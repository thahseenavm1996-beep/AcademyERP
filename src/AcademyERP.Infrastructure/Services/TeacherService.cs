using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using AcademyERP.Application.DTOs.Teachers;
using AcademyERP.Domain.Entities.Teachers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AcademyERP.Application.Common;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;


namespace AcademyERP.Infrastructure.Services;

public class TeacherService : ITeacherService
{
    private readonly IRepository<Teacher> _repository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public TeacherService(
    IRepository<Teacher> repository,
    IMapper mapper,
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
        _userManager = userManager;
    }
    public async Task<TeacherResponse> CreateAsync(CreateTeacherRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        /* if (request.DateOfBirth > DateTime.Today.AddYears(-4))
         {
             throw new ArgumentException("teacher must be at least 4 years old.");
         }*/

        try
        {
            var sequence = await _context.DocumentSequences
                .SingleOrDefaultAsync(x => x.Code == "TEACHER");

            if (sequence == null)
            {
                throw new Exception("Document sequence 'TEACHER' not found.");
            }

            var generatedNumber = $"{sequence.Prefix}{sequence.NextNumber}";
            sequence.NextNumber++;

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.MobileNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new Exception("Identity Error: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "Teacher");

            var teacher = _mapper.Map<Teacher>(request);

            teacher.ApplicationUserId = user.Id;
            teacher.EmployeeCode = generatedNumber;

            _context.Teachers.Add(teacher);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return _mapper.Map<TeacherResponse>(teacher);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    private string GenerateAdmissionNumber(Teacher? lastTeacher)
    {
        const string prefix = "SGO";
        const int startingNumber = 100;

        if (lastTeacher == null)
        {
            return $"{prefix}{startingNumber}";
        }

        var lastNumber = int.Parse(lastTeacher.EmployeeCode.Replace(prefix, ""));

        return $"{prefix}{lastNumber + 1}";
    }
    public async Task<PagedResponse<TeacherResponse>> GetAllAsync(TeacherQueryRequest request)
    {
        var query = _context.Teachers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(s =>
                s.FullName.Contains(request.Search) ||
                s.EmployeeCode.Contains(request.Search));
        }


        var totalCount = await query.CountAsync();

        var teachers = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<TeacherResponse>
        {
            Items = _mapper.Map<List<TeacherResponse>>(teachers),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
    public async Task<TeacherResponse?> GetByIdAsync(Guid id)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
            return null;

        return _mapper.Map<TeacherResponse>(teacher);
    }

    public async Task<TeacherResponse> UpdateAsync(Guid id, UpdateTeacherRequest request)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(x => x.Id == id);

        if (teacher is null)
            throw new KeyNotFoundException("Teacher not found.");

        teacher.FullName = request.FullName;

        teacher.DateOfBirth = request.DateOfBirth!.Value;
        teacher.Gender = request.Gender;
        teacher.JoiningDate = request.JoiningDate!.Value;
        teacher.MobileNumber = request.MobileNumber;
        teacher.Email = request.Email;
        teacher.Qualification = request.Qualification;
        teacher.Department = request.Department;
        teacher.Address = request.Address;
        teacher.Country = request.Country;
        teacher.TimeZone = request.TimeZone;
        teacher.Remarks = request.Remarks;

        var user = await _userManager.FindByIdAsync(teacher.ApplicationUserId.ToString());

        if (user != null)
        {
            user.Email = request.Email;
            user.UserName = request.Email;
            user.PhoneNumber = request.MobileNumber;
            user.FullName = request.FullName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ",
                    result.Errors.Select(x => x.Description)));
            }
        }

        await _context.SaveChangesAsync();

        return _mapper.Map<TeacherResponse>(teacher);
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
            return false;

        // Delete Identity User first
        var user = await _userManager.FindByIdAsync(teacher.ApplicationUserId.ToString());

        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ",
                    result.Errors.Select(x => x.Description)));
            }
        }

        // Delete Parent
        _context.Teachers.Remove(teacher);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResetPasswordAsync(Guid id, ResetTeacherPasswordRequest request)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
            throw new Exception("Teacher not found.");

        var user = await _userManager.FindByIdAsync(teacher.ApplicationUserId.ToString());

        if (user == null)
            throw new Exception($"Identity user not found. UserId = {teacher.ApplicationUserId}");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            request.Password);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ",
                result.Errors.Select(e => e.Description)));
        }

        return true;
    }

}