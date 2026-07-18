using AcademyERP.Application.DTOs.Programs;
using FluentValidation;

namespace AcademyERP.Application.Validators.Program;

public class CreateProgramRequestValidator
    : AbstractValidator<CreateProgramRequest>
{
    public CreateProgramRequestValidator()
    {
        RuleFor(x => x.ProgramCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.ProgramName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.DefaultDurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}