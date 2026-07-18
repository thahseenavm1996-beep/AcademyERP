using AcademyERP.Application.DTOs.Programs;
using AcademyERP.Application.Interfaces;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Programs;
using AutoMapper;

namespace AcademyERP.Infrastructure.Services;

public class ProgramService : IProgramService
{
    private readonly IRepository<Program> _repository;
    private readonly IMapper _mapper;

    public ProgramService(
        IRepository<Program> repository,
        IMapper mapper)
    {
        _repository = repository;
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

        await _repository.DeleteAsync(id);

        return true;
    }
}