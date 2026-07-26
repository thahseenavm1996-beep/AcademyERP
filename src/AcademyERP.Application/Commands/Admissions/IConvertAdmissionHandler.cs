namespace AcademyERP.Application.Commands.Admissions;

public interface IConvertAdmissionHandler
{
    Task<ConvertAdmissionResult> HandleAsync(
        ConvertAdmissionCommand command);
}