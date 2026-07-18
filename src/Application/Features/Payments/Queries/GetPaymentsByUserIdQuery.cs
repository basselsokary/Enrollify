using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.ReadRepositories;
using Application.Contracts.Common;
using Application.Features.Common.Validators;
using Domain.Entities.PaymentAggregate;
using FluentValidation;

namespace Application.Features.Payments.Queries;

public sealed record GetPaymentsByUserIdQuery(
    PagingParameters Paging) : IQuery<PagedResult<UserPaymentResponse>>;

internal sealed class GetPaymentsByUserIdQueryHandler(
    IPaymentReadRepository readRepository,
    IUserContext userContext) : IQueryHandler<GetPaymentsByUserIdQuery, PagedResult<UserPaymentResponse>>
{
    public async Task<Result<PagedResult<UserPaymentResponse>>> HandleAsync(GetPaymentsByUserIdQuery query, CancellationToken cancellationToken)
    {
        var Payments = await readRepository.GetPaymentsByUserIdAsync(
            userContext.Id,
            query.Paging,
            cancellationToken);
        
        return Result.Success(Payments);
    }
}

public sealed record UserPaymentResponse(
    Guid Id,
    Guid UserId,
    string CourseTitle,
    PaymentMethod PaymentMethod,
    PaymentStatus Status,
    DateTime? PaymentAt,
    decimal PaidAmount,
    string? AmountRefunded);

internal sealed class GetPaymentsByUserIdQueryValidator : AbstractValidator<GetPaymentsByUserIdQuery>
{
    public GetPaymentsByUserIdQueryValidator()
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());
    }
}