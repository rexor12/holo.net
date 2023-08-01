using System;

namespace Holo.Sdk;

/// <summary>
/// A disposable that does nothing on dispose.
/// </summary>
/// <remarks>This type is useful when a disposable is expected.</remarks>
public sealed class AnonymousDisposable : IDisposable
{
    /// <summary>
    /// The default instance of <see cref="AnonymousDisposable"/>.
    /// </summary>
    public static readonly AnonymousDisposable Instance = new AnonymousDisposable();

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
    }
}