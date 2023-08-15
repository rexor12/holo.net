using System;
using System.Runtime.CompilerServices;

namespace Holo.Sdk;

/// <summary>
/// Extensions for generic types.
/// </summary>
public static class GenericExtensions
{
    /// <summary>
    /// Asserts that the specified member is not <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of the member.</typeparam>
    /// <param name="member">The member to check.</param>
    /// <param name="memberName">The name of the member, used for exceptions.</param>
    /// <returns>The value of the member, if not <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the given value is <c>null</c>.</exception>
    public static T AssertNotNull<T>(this T? member, [CallerMemberName] string? memberName = null)
        where T : class
        => member ?? throw new ArgumentNullException(memberName);
}