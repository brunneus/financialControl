using FinanceControl.Domain;
using FinanceControl.Infra;
using MediatR;

namespace FinanceControl.Application;

public record CreateTransactionCategoryRequest(
    string Name,
    string Description,
    TransactionType CategoryType) : IRequest<ResultResponse<CreateTransactionCategoryResponse>>;

public record CreateTransactionCategoryResponse(
    string Id,
    string Name,
    string Description,
    TransactionType CategoryType);

public class CreateTransactionCategoryCommandHandler(FinanceControlDbContext context)
    : IRequestHandler<CreateTransactionCategoryRequest, ResultResponse<CreateTransactionCategoryResponse>>
{
    public async Task<ResultResponse<CreateTransactionCategoryResponse>> Handle(
        CreateTransactionCategoryRequest request, 
        CancellationToken cancellationToken)
    {
        var transactionCategory = new TransactionCategory(
            request.Name,
            request.Description,
            request.CategoryType
        );

        context.TransactionCategories.Add(transactionCategory);

        await context.SaveChangesAsync(cancellationToken);

        return new CreateTransactionCategoryResponse(
            transactionCategory.Id,
            transactionCategory.Name,
            transactionCategory.Description,
            transactionCategory.Type
        );
    }
}
