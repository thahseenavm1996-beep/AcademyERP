using AcademyERP.Application.DTOs.Programs;
using AcademyERP.Domain.Entities.Programs;
using AutoMapper;

namespace AcademyERP.Application.Mappings;

public class ProgramMappingProfile : Profile
{
    public ProgramMappingProfile()
    {
        CreateMap<Program, ProgramResponse>();

        CreateMap<CreateProgramRequest, Program>();

        CreateMap<UpdateProgramRequest, Program>();
    }
}