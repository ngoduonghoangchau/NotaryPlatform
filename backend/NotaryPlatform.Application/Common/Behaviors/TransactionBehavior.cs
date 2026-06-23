using MediatR;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Common.Interfaces;

namespace NotaryPlatform.Application.Common.Behaviors;

/// <summary>
/// Wraps every <see cref="ICommand"/> / <see cref="ICommand{TResponse}"/> in a
/// database transaction. On success the transaction is committed; on any exception
/// it is rolled back and the exception re-thrown.
///
/// Queries (<see cref="IQuery{TResponse}"/>) bypass this behavior entirely —
/// they never need a write transaction.
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only intercept commands — skip queries and plain MediatR requests
        if (request is not ICommand and not ICommand<TResponse>)
            return await next();

        var requestName = typeof(TRequest).Name;
        _logger.LogDebug("Beginning transaction for {RequestName}", requestName);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next();
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction committed for {RequestName}", requestName);
            return response;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogWarning("Transaction rolled back for {RequestName}", requestName);
            throw;
        }
    }
}
