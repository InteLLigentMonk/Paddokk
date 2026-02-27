using MediatR;
using Paddokk.Core.Interfaces;

namespace Paddokk.Core.Behaviours;

public sealed class TransactionBehaviour<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        TResponse? result = default;
        await unitOfWork.ExecuteInTransactionAsync(
            async () => { result = await next(); },
            ct);
        return result!;
    }
}
