using AcademyERP.Application.DTOs.Fees;
using AcademyERP.Domain.Entities.Finance;
using AutoMapper;

namespace AcademyERP.Application.Mappings;

public class FeeInvoiceMappingProfile : Profile
{
    public FeeInvoiceMappingProfile()
    {
        CreateMap<CreateFeeInvoiceRequest, FeeInvoice>();

        CreateMap<UpdateFeeInvoiceRequest, FeeInvoice>();

        CreateMap<FeeInvoice, FeeInvoiceResponse>()
            .ForMember(
                dest => dest.StudentName,
                opt => opt.MapFrom(
                    src => src.Enrollment.Student.FullName))

            .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom(
                    src => src.Enrollment.Course.CourseName));
    }
}