using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.ClassReports;
using AcademyERP.Persistence;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Persistence.Context;
using AcademyERP.Application.Common;


namespace AcademyERP.Infrastructure.Services;

public class ClassReportService : IClassReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ClassReportService(
        ApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ClassReportResponse>> GetAllAsync(ClassReportQueryRequest request)
    {
        var query = _context.ClassReports
            .Include(x => x.Student)
            .Include(x => x.Teacher)
           .Include(x => x.Enrollment)
    .ThenInclude(e => e.Course)
            .AsQueryable();

        if (request.StudentId.HasValue)
            query = query.Where(x => x.StudentId == request.StudentId);

        if (request.TeacherId.HasValue)
            query = query.Where(x => x.TeacherId == request.TeacherId);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.ReportDate >= request.FromDate);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.ReportDate <= request.ToDate);

        var totalCount = await query.CountAsync();

        var reports = await query
            .OrderByDescending(x => x.ReportDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<ClassReportResponse>
        {
            Items = _mapper.Map<List<ClassReportResponse>>(reports),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<ClassReportResponse?> GetByIdAsync(Guid id)
    {
        var report = await _context.ClassReports
            .Include(x => x.Student)
            .Include(x => x.Teacher)
           .Include(x => x.Enrollment)
    .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (report == null)
            return null;

        return _mapper.Map<ClassReportResponse>(report);
    }

    public async Task<ClassReportResponse> CreateAsync(CreateClassReportRequest request)
    {
        var report = _mapper.Map<ClassReport>(request);

        report.SubmittedAt = DateTime.UtcNow;

        _context.ClassReports.Add(report);

        await _context.SaveChangesAsync();

        report = await _context.ClassReports
            .Include(x => x.Student)
            .Include(x => x.Teacher)
           .Include(x => x.Enrollment)
    .ThenInclude(e => e.Course)
            .FirstAsync(x => x.Id == report.Id);

        return _mapper.Map<ClassReportResponse>(report);
    }

    public async Task<ClassReportResponse?> UpdateAsync(Guid id, UpdateClassReportRequest request)
    {
        var report = await _context.ClassReports
            .Include(x => x.Student)
            .Include(x => x.Teacher)
           .Include(x => x.Enrollment)
    .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (report == null)
            throw new Exception("Class report not found.");

        _mapper.Map(request, report);

        await _context.SaveChangesAsync();

        return _mapper.Map<ClassReportResponse>(report);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var report = await _context.ClassReports
            .FirstOrDefaultAsync(x => x.Id == id);

        if (report == null)
            return false;

        _context.ClassReports.Remove(report);

        await _context.SaveChangesAsync();

        return true;
    }
}