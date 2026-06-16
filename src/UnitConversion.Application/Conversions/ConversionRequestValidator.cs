using FluentValidation;

namespace UnitConversion.Application.Conversions;

public sealed class ConversionRequestValidator : AbstractValidator<ConversionRequest>
{
    public ConversionRequestValidator()
    {
        RuleFor(x => x.Value)
            .Must(v => !double.IsNaN(v) && !double.IsInfinity(v))
            .WithMessage("Value must be a finite number.");

        RuleFor(x => x.FromUnit)
            .NotEmpty()
            .WithMessage("'fromUnit' is required.")
            .MaximumLength(64);

        RuleFor(x => x.ToUnit)
            .NotEmpty()
            .WithMessage("'toUnit' is required.")
            .MaximumLength(64);
    }
}
