using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Holo.Sdk.DI;
using Holo.Sdk.Interactions;
using Holo.ServiceHost.Modules;
using Microsoft.EntityFrameworkCore;

namespace Holo.ServiceHost.Reflection;

/// <summary>
/// Extension methods for <see cref="Assembly"/>.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Gets the service descriptors from the specified assembly.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to inspect.</param>
    /// <returns>An enumerable of <see cref="ServiceDescriptor"/>.</returns>
    public static IEnumerable<ServiceDescriptor> GetServiceDescriptors(this Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            var serviceAttribute = type.GetCustomAttributes<ServiceAttribute>().SingleOrDefault();
            if (serviceAttribute == null)
                continue;

            yield return new ServiceDescriptor(
                type,
                serviceAttribute.ContractTypes,
                serviceAttribute.Lifetime);
        }
    }

    /// <summary>
    /// Gets the <see cref="DbContext"/> types from the specified assembly.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to inspect.</param>
    /// <returns>An enumerable of <see cref="Type"/>.</returns>
    public static IEnumerable<Type> GetDbContexts(this Assembly assembly) => assembly
        .GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(DbContext)));

    /// <summary>
    /// Gets the <see cref="IInteractionGroup"/> implementors from the specified assembly.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to inspect.</param>
    /// <returns>An enumerable of <see cref="Type"/>.</returns>
    public static IEnumerable<Type> GetInteractionGroups(this Assembly assembly) => assembly
        .GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(IInteractionGroup)));
}