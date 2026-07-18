using FluentValidation;
using Domain.Common;
using Application.Contracts.Common;
using Domain.Common.Constants;

namespace Application.Features.Common.Validators;

internal sealed class PagingParametersValidator : AbstractValidator<PagingParameters>
{
    public PagingParametersValidator()
    {
        var maxPageSize = DomainConstants.Paging.MaxPageSize;
        
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage(ValidationErrors.NumberMustBeGreaterThanZero.Message);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage(ValidationErrors.RangeInvalid(1, maxPageSize).Message)
            .LessThanOrEqualTo(maxPageSize)
            .WithMessage(ValidationErrors.RangeInvalid(1, maxPageSize).Message);
    }
}
