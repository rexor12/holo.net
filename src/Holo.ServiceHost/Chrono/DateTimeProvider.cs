using System;
using Holo.Sdk.Chrono;
using Holo.Sdk.DI;

namespace Holo.ServiceHost.Chrono;

/// <summary>
/// A service used to manipulate dates and times.
/// </summary>
[Service(typeof(IDateTimeProvider))]
public sealed class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc cref="IDateTimeProvider.Now"/>
    public DateTime Now()
        => DateTime.UtcNow;
}