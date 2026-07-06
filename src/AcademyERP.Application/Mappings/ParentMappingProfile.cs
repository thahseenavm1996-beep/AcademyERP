using AutoMapper;
using AcademyERP.Domain.Entities.Parents;
using AcademyERP.Application.DTOs.Parents;

namespace AcademyERP.Application.Mappings;

public class ParentMappingProfile : Profile
{
    public ParentMappingProfile()
    {
        CreateMap<CreateParentRequest, Parent>();

        CreateMap<UpdateParentRequest, Parent>();

        CreateMap<Parent, ParentResponse>();
    }
}