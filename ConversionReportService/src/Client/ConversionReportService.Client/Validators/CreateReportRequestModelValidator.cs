using ConversionReportService.Client.Models;
using FluentValidation;

namespace ConversionReportService.Client.Validators;

public class CreateReportRequestModelValidator : AbstractValidator<CreateReportRequestModel>
{
    public CreateReportRequestModelValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("ItemId must be greater than 0");

        RuleFor(x => x.DateTo)
            .GreaterThanOrEqualTo(x => x.DateFrom)
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateTo must be after or equal to DateFrom");
    }
}