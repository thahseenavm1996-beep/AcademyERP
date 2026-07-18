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

namespace AcademyERP.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly IRepository<Student> _repository;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public StudentService(
        IRepository<Student> repository,
        IMapper mapper,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
        _userManager = userManager;
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

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new Exception("Identity Error: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "Student");

            var student = _mapper.Map<Student>(request);

            student.ApplicationUserId = user.Id;
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
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(s =>
                s.FullName.Contains(request.Search) ||
                s.AdmissionNumber.Contains(request.Search));
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
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
    public async Task<StudentResponse?> GetByIdAsync(Guid id)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.Id == id);

        if (student == null)
            return null;

        var response = _mapper.Map<StudentResponse>(student);

        var user = await _userManager.FindByIdAsync(student.ApplicationUserId.ToString());

        if (user != null)
        {
            response.Email = user.Email ?? string.Empty;
            response.PhoneNumber = user.PhoneNumber ?? string.Empty;
        }

        response.Status = student.Status.ToString();

        return response;
    }

    public async Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.Id == id);

        if (student is null)
            throw new KeyNotFoundException("Student not found.");

        student.FullName = request.FullName;
        student.AdmissionDate = request.AdmissionDate;
        student.DateOfBirth = request.DateOfBirth;
        student.Gender = request.Gender;
        student.Country = request.Country;
        student.TimeZone = request.TimeZone;
        student.Remarks = request.Remarks;

        var user = await _userManager.FindByIdAsync(student.ApplicationUserId.ToString());

        if (user != null)
        {
            user.Email = request.Email;
            user.UserName = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.FullName = request.FullName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ",
                    result.Errors.Select(x => x.Description)));
            }
        }

        await _context.SaveChangesAsync();

        return _mapper.Map<StudentResponse>(student);
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return false;

        // Delete Identity User first
        var user = await _userManager.FindByIdAsync(student.ApplicationUserId.ToString());

        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ",
                    result.Errors.Select(x => x.Description)));
            }
        }

        // Delete 
        _context.Students.Remove(student);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> ResetPasswordAsync(Guid id, string newPassword)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return false;

        var user = await _userManager.FindByIdAsync(student.ApplicationUserId.ToString());

        if (user == null)
            return false;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded;
    }

}