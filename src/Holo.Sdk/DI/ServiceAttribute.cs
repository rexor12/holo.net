using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Holo.Sdk.DI;

/// <summary>
/// An attribute for marking classes that should be automatically registered
/// in the dependency injection container.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ServiceAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the lifetime of the service.
    /// </summary>
    /// <remarks>The default value is <see cref="ServiceLifetime.Singleton"/></remarks>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

    /// <summary>
    /// Gets the types serving as contracts to the service.
    /// </summary>
    public IReadOnlyList<Type> ContractTypes { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ServiceAttribute"/>.
    /// </summary>
    /// <param name="contractType">The type serving as a contract to the service.</param>
    /// <param name="additionalContractTypes">Additional types serving as contracts to the service.</param>
    public ServiceAttribute(Type contractType, params Type[] additionalContractTypes)
    {
        ContractTypes = additionalContractTypes == null || additionalContractTypes.Length == 0
            ? new[] { contractType }
            : additionalContractTypes.Append(contractType).ToArray();
    }
}