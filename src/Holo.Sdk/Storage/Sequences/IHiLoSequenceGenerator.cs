using System.Threading.Tasks;

namespace Holo.Sdk.Storage.Sequences;

/// <summary>
/// Interface for a Hi/Lo sequence generator that works with
/// <see cref="IIdentifier{T}"/> of type <see cref="ulong"/>.
/// </summary>
/// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
public interface IHiLoSequenceGenerator<TIdentifier>
    where TIdentifier : IIdentifier<ulong>
{
    /// <summary>
    /// Gets the next available identifier.
    /// </summary>
    /// <returns>The next available identifier.</returns>
    Task<TIdentifier> GetNextIdentifierAsync();
}