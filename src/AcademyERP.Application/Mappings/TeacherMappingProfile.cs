using AutoMapper;
using AcademyERP.Application.DTOs.Teachers;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.TeacherAvailabilities;

namespace AcademyERP.Application.Mappings;

public class TeacherMappingProfile : Profile
{
    public TeacherMappingProfile()
    {
        // DTO -> Entity
        CreateMap<CreateTeacherRequest, Teacher>();

        CreateMap<UpdateTeacherRequest, Teacher>();

CreateMap<TeacherAvailability, TeacherAvailabilityResponse>()
    .ForMember(
        dest => dest.TimeSlotName,
        opt => opt.MapFrom(src => src.TimeSlot.Name))
    .ForMember(
        dest => dest.StartTime,
        opt => opt.MapFrom(src => src.TimeSlot.StartTime))
    .ForMember(
        dest => dest.EndTime,
        opt => opt.MapFrom(src => src.TimeSlot.EndTime));
        // Entity -> DTO

    CreateMap<Teacher, TeacherResponse>()
    .ForMember(
        dest => dest.Gender,
        opt => opt.MapFrom(src => (int)src.Gender))
    .ForMember(
        dest => dest.IsActive,
        opt => opt.MapFrom(
            src => src.Status == UserStatus.Active))
    .ForMember(
        dest => dest.Availability,
        opt => opt.MapFrom(
            src => src.TeacherAvailabilities
                .Where(x => x.IsAvailable)));
    }
}