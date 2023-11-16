using System;

namespace Holo.Sdk.Chrono;

/// <summary>
/// Interface for a service used to manipulate dates and times.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    /// <returns>The current date an time in UTC.</returns>
    DateTime Now();
}