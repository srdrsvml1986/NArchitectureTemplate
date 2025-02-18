using System.Transactions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NArchitecture.Core.Application.Pipelines.Transaction;

public class TransactionScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    private readonly ILogger<TransactionScopeBehavior<TRequest, TResponse>> _logger;

    public TransactionScopeBehavior(ILogger<TransactionScopeBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        using TransactionScope transactionScope = new(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = new TimeSpan(0, 5, 0)
            },
            TransactionScopeAsyncFlowOption.Enabled
        );

        TResponse response;
        try
        {
            _logger.LogInformation("Transaction started for {RequestName}", typeof(TRequest).Name);
            response = await next();
            transactionScope.Complete();
            _logger.LogInformation("Transaction completed for {RequestName}", typeof(TRequest).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}", typeof(TRequest).Name);
            transactionScope.Dispose();
            throw;
        }

        return response;
    }
}
