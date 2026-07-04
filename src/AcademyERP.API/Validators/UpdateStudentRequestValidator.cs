using AcademyERP.Application.DTOs.Students;
using FluentValidation;

namespace AcademyERP.API.Validators;

public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
{
    public UpdateStudentRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.AdmissionDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Admission date cannot be in the future.");

        RuleFor(x => x.DateOfBirth)
            .LessThanOrEqualTo(DateTime.Today.AddYears(-4))
            .WithMessage("Student must be at least 4 years old.");

        RuleFor(x => x.Remarks)
            .MaximumLength(500);
    }
}