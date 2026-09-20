using AcademyERP.Application.DTOs.Programs;
using AcademyERP.Application.Interfaces;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Programs;
using AutoMapper;
using AcademyERP.Domain.Entities.Courses;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class ProgramService : IProgramService
{
    private readonly IRepository<Program> _repository;
private readonly IRepository<Course> _courseRepository;
private readonly IMapper _mapper;

    public ProgramService(
    IRepository<Program> repository,
    IRepository<Course> courseRepository,
    IMapper mapper)
{
    _repository = repository;
    _courseRepository = courseRepository;
    _mapper = mapper;
}

    public async Task<List<ProgramResponse>> GetAllAsync()
    {
        var programs = await _repository.GetAllAsync();

        return _mapper.Map<List<ProgramResponse>>(programs);
    }

    public async Task<ProgramResponse?> GetByIdAsync(Guid id)
    {
        var program = await _repository.GetByIdAsync(id);

        if (program == null)
            return null;

        return _mapper.Map<ProgramResponse>(program);
    }

    public async Task<ProgramResponse> CreateAsync(CreateProgramRequest request)
    {
        var program = _mapper.Map<Program>(request);

        await _repository.AddAsync(program);

        return _mapper.Map<ProgramResponse>(program);
    }

    public async Task<ProgramResponse?> UpdateAsync(Guid id, UpdateProgramRequest request)
    {
        var program = await _repository.GetByIdAsync(id);

        if (program == null)
            return null;

        _mapper.Map(request, program);

        await _repository.UpdateAsync(program);

        return _mapper.Map<ProgramResponse>(program);
    }

    public async Task<bool> DeleteAsync(Guid id)
{
    var program = await _repository.GetByIdAsync(id);

    if (program == null)
        return false;

    var courses = await _courseRepository.Query()
        .Where(x => x.ProgramId == id && !x.IsDeleted)
        .ToListAsync();

    foreach (var course in courses)
    {
        await _courseRepository.DeleteAsync(course.Id);
    }

    await _repository.DeleteAsync(id);

    return true;
}
}