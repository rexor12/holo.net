using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Holo.Sdk.Storage;
using Holo.Sdk.Storage.Sequences;
using Holo.ServiceHost.Storage.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holo.ServiceHost.Storage.Sequences.HiLo;

/// <summary>
/// A Hi/Lo sequence generator that works with <see cref="IIdentifier{T}"/> of type <see cref="ulong"/>.
/// </summary>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
public sealed class HiLoSequenceGenerator<TIdentifier> : IHiLoSequenceGenerator<TIdentifier>, IDisposable
    where TIdentifier : IIdentifier<ulong>
{
    private static readonly Type[] IdentifierConstructorParams = new[] { typeof(ulong) };

    private ulong _currentHiValue;
    private ulong _lastLoValue;
    private readonly SemaphoreSlim _sempahore = new(1, 1);
    private readonly ulong _windowSize;
    private readonly IDbContextFactory _dbContextFactory;

    public HiLoSequenceGenerator(
        IDbContextFactory dbContextFactory,
        ILogger<HiLoSequenceGenerator<TIdentifier>> logger,
        IOptions<DatabaseOptions> options)
    {
        _dbContextFactory = dbContextFactory;

        var windowSizes = options.Value.HiLoSequenceGeneratorOptions.WindowSizes;
        _windowSize = windowSizes.TryGetValue(typeof(TIdentifier).Name, out var windowSize)
            ? windowSize
            : HiLoSequenceGeneratorOptions.DefaultWindowSize;
        _lastLoValue = _windowSize;

        logger.LogDebug(
            "Hi/Lo sequence generator for identifier '{Identifier}' will use a range of '{Range}'",
            typeof(TIdentifier).Name,
            _windowSize);
    }

    /// <inheritdoc cref="IHiLoSequenceGenerator{TIdentifier}.GetNextIdentifierAsync"/>
    public async Task<TIdentifier> GetNextIdentifierAsync()
    {
        await _sempahore.WaitAsync();
        try
        {
            var loValue = ++_lastLoValue;
            if (loValue < _windowSize)
                return CreateIdentifier(loValue);

            await using var dbContext = _dbContextFactory.Create<ServiceHostDbContext>();
            await using var dbCommand = dbContext.Database.GetDbConnection().CreateCommand();
            dbCommand.CommandText = $"SELECT \"service_host\".\"get_current_hilo_sequence_hi_value\"('{typeof(TIdentifier).Name}')";

            await dbContext.Database.OpenConnectionAsync();
            try
            {
                // PostgreSQL 'bigint' is translated as 'long', hence the cast.
                unchecked
                {
                    _currentHiValue = (ulong)(long)(await dbCommand.ExecuteScalarAsync())!;
                }

                _lastLoValue = 0;

                return CreateIdentifier(0);
            }
            finally
            {
                await dbContext.Database.CloseConnectionAsync();
            }
        }
        finally
        {
            _sempahore.Release();
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        _sempahore.Dispose();
    }

    private TIdentifier CreateIdentifier(ulong loValue)
    {
        var identifierValue = _currentHiValue * _windowSize + loValue;
        var constructor = typeof(TIdentifier).GetConstructor(
            BindingFlags.Instance | BindingFlags.Public,
            null,
            CallingConventions.HasThis,
            IdentifierConstructorParams,
            null);
        if (constructor == null)
            throw new ArgumentException(
                $"Identifier type '{typeof(TIdentifier).Name}' must have a public constructor"
                + " that takes a single ulong.",
                nameof(TIdentifier));

        var identifier = constructor.Invoke(new object?[] { identifierValue });
        if (identifier is not TIdentifier typedIdentifier)
            throw new ArgumentException(
                $"Failed to create an instance of '{typeof(TIdentifier)}'.",
                nameof(TIdentifier));

        return typedIdentifier;
    }
}