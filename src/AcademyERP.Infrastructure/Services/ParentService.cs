using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Parents;
using AcademyERP.Application.Interfaces;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Parents;
using AcademyERP.Persistence.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AcademyERP.Infrastructure.Services;

public class ParentService : IParentService
{
    private readonly IRepository<Parent> _repository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public ParentService(
     IRepository<Parent> repository,
     IMapper mapper,
     ApplicationDbContext context,
     UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
        _userManager = userManager;

    }

    public async Task<ParentResponse> CreateAsync(CreateParentRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
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

            // Optional: Assign Parent role
            await _userManager.AddToRoleAsync(user, "Parent");

            var parent = _mapper.Map<Parent>(request);

            // Link Parent with Identity User
            parent.ApplicationUserId = user.Id;

            _context.Parents.Add(parent);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return _mapper.Map<ParentResponse>(parent);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PagedResponse<ParentResponse>> GetAllAsync(ParentQueryRequest request)
    {
        var query = _context.Parents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x =>
                x.FullName.Contains(request.Search) ||
                x.PhoneNumber.Contains(request.Search) ||
                x.Email.Contains(request.Search));
        }

        var totalCount = await query.CountAsync();

        var parents = await query
            .OrderBy(x => x.FullName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<ParentResponse>
        {
            Items = _mapper.Map<List<ParentResponse>>(parents),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<ParentResponse?> GetByIdAsync(Guid id)
    {
        var parent = await _context.Parents

            .Include(x => x.StudentParents)
                .ThenInclude(x => x.Student)

            .FirstOrDefaultAsync(x => x.Id == id);

        if (parent == null)
            return null;

        var response = _mapper.Map<ParentResponse>(parent);

        response.Status = parent.Status.ToString();

        response.ChildrenCount = parent.StudentParents.Count;

        response.Children = parent.StudentParents
            .Select(x => new ParentStudentResponse
            {
                StudentId = x.Student!.Id,
                AdmissionNumber = x.Student.AdmissionNumber,
                FullName = x.Student.FullName,
                Gender = x.Student.Gender.ToString(),
                Status = x.Student.Status.ToString()
            })
            .ToList();

        return response;
    }

    public async Task<ParentResponse?> UpdateAsync(Guid id, UpdateParentRequest request)
    {
        var parent = await _context.Parents
            .FirstOrDefaultAsync(x => x.Id == id);

        if (parent == null)
            return null;

        parent.FullName = request.FullName;
        parent.PhoneNumber = request.PhoneNumber;
        parent.Email = request.Email;
        parent.Remarks = request.Remarks;

        // Update Identity user
        var user = await _userManager.FindByIdAsync(parent.ApplicationUserId.ToString());

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

        return _mapper.Map<ParentResponse>(parent);
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var parent = await _context.Parents.FindAsync(id);

        if (parent == null)
            return false;

        // Delete Identity User first
        var user = await _userManager.FindByIdAsync(parent.ApplicationUserId.ToString());

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
        _context.Parents.Remove(parent);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> ResetPasswordAsync(Guid id, ResetParentPasswordRequest request)
    {
        var parent = await _context.Parents.FindAsync(id);

        if (parent == null)
            throw new Exception("Parent not found.");

        var user = await _userManager.FindByIdAsync(parent.ApplicationUserId.ToString());

        if (user == null)
            throw new Exception($"Identity user not found. UserId = {parent.ApplicationUserId}");

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