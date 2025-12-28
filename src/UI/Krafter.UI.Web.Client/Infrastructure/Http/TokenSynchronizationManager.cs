namespace Krafter.UI.Web.Client.Infrastructure.Http;

/// <summary>
/// Manages synchronization for token refresh operations to prevent multiple concurrent refresh attempts.
/// When multiple API calls detect an expired token simultaneously, only one will perform the refresh
/// while others wait and then use the refreshed token. If the first attempt fails, waiting callers
/// can retry the operation.
/// </summary>
public static class TokenSynchronizationManager
{
    private static readonly SemaphoreSlim _synchronizationSemaphore = new(1, 1);
    private static volatile bool _isSynchronizing;
    private static DateTime _lastSyncTime = DateTime.MinValue;
    private static readonly TimeSpan RecentSyncThreshold = TimeSpan.FromSeconds(5);
    private const int MaxRetryAttempts = 2;

    public static bool IsSynchronizing => _isSynchronizing;

    /// <summary>
    /// Executes an operation with synchronization. If another operation is already in progress,
    /// waits for it to complete. If the previous operation failed, the waiting caller will retry.
    /// </summary>
    /// <param name="operation">The async operation to execute (e.g., token refresh API call)</param>
    /// <param name="isSuccessful">Predicate to determine if the operation succeeded</param>
    /// <param name="logger">Logger for diagnostics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if operation succeeded or if we waited for another successful operation</returns>
    public static async Task<bool> TryExecuteWithSynchronizationAsync<T>(
        Func<Task<T>> operation,
        Func<T, bool> isSuccessful,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        // Fast path: if a sync just completed recently, skip the operation
        if (HasRecentSync())
        {
            logger.LogInformation("Recent sync detected, skipping refresh operation");
            return true;
        }

        // Try to acquire the semaphore without blocking first
        bool acquired = await _synchronizationSemaphore.WaitAsync(0, cancellationToken);

        if (!acquired)
        {
            // Another operation is in progress, wait for it
            logger.LogInformation("Token refresh already in progress, waiting for completion...");
            await _synchronizationSemaphore.WaitAsync(cancellationToken);
            _synchronizationSemaphore.Release();

            // After waiting, check if the sync was recent (meaning it likely succeeded)
            if (HasRecentSync())
            {
                logger.LogInformation("Waited for refresh, recent sync detected - assuming success");
                return true;
            }

            // Previous attempt failed, this waiter should retry
            logger.LogInformation("Previous refresh attempt failed, retrying...");
            return await ExecuteWithRetryAsync(operation, isSuccessful, logger, cancellationToken);
        }

        // We acquired the lock, execute the operation with retry support
        return await ExecuteOperationAsync(operation, isSuccessful, logger, cancellationToken);
    }

    /// <summary>
    /// Executes the operation when the caller holds the lock.
    /// </summary>
    private static async Task<bool> ExecuteOperationAsync<T>(
        Func<Task<T>> operation,
        Func<T, bool> isSuccessful,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        try
        {
            _isSynchronizing = true;
            logger.LogInformation("Starting synchronized token refresh operation");

            T result = await operation();
            bool success = isSuccessful(result);

            if (success)
            {
                _lastSyncTime = DateTime.UtcNow;
                logger.LogInformation("Token refresh completed successfully");
            }
            else
            {
                logger.LogWarning("Token refresh operation failed");
            }

            return success;
        }
        finally
        {
            _isSynchronizing = false;
            _synchronizationSemaphore.Release();
        }
    }

    /// <summary>
    /// Retry logic for waiters when the previous operation failed.
    /// </summary>
    private static async Task<bool> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        Func<T, bool> isSuccessful,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            // Check again if someone else succeeded while we were preparing to retry
            if (HasRecentSync())
            {
                logger.LogInformation("Recent sync detected during retry preparation, skipping");
                return true;
            }

            // Try to acquire the lock for retry
            bool acquired = await _synchronizationSemaphore.WaitAsync(0, cancellationToken);

            if (!acquired)
            {
                // Another retry is in progress, wait for it
                logger.LogInformation("Another retry in progress, waiting (attempt {Attempt}/{MaxAttempts})", attempt, MaxRetryAttempts);
                await _synchronizationSemaphore.WaitAsync(cancellationToken);
                _synchronizationSemaphore.Release();

                if (HasRecentSync())
                {
                    logger.LogInformation("Retry succeeded by another caller");
                    return true;
                }

                continue; // Try again
            }

            // We got the lock, execute the retry
            logger.LogInformation("Executing retry attempt {Attempt}/{MaxAttempts}", attempt, MaxRetryAttempts);
            bool success = await ExecuteOperationAsync(operation, isSuccessful, logger, cancellationToken);

            if (success)
            {
                return true;
            }

            // Small delay before next retry attempt
            if (attempt < MaxRetryAttempts)
            {
                await Task.Delay(100 * attempt, cancellationToken);
            }
        }

        logger.LogWarning("All retry attempts exhausted");
        return false;
    }

    /// <summary>
    /// Checks if a successful sync occurred within the specified threshold.
    /// </summary>
    public static bool HasRecentSync(TimeSpan threshold) =>
        DateTime.UtcNow - _lastSyncTime < threshold;

    /// <summary>
    /// Checks if a successful sync occurred within the default threshold (5 seconds).
    /// </summary>
    public static bool HasRecentSync() => HasRecentSync(RecentSyncThreshold);
}
