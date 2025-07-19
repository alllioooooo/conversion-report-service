using ConversionReportService.Presentation.Http.Models;
using FluentValidation;

namespace ConversionReportService.Presentation.Http.Validators;

public class AddItemInteractionRequestValidator : AbstractValidator<AddItemInteractionRequest>
{
    public AddItemInteractionRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0)
            .WithMessage("ItemId must be greater than 0");

        RuleFor(x => x.Timestamp)
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(1))
            .WithMessage("Timestamp cannot be in the future");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid interaction type");
    }
}