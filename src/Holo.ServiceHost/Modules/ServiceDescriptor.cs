using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Holo.ServiceHost.Modules;

/// <summary>
/// Descriptor of a service registered via dependency injection.
/// </summary>
/// <param name="ServiceType">The type of the service.</param>
/// <param name="ContractTypes">The types of the contracts by which the service is exposed.</param>
/// <param name="Lifetime">The lifetime of the service.</param>
public record ServiceDescriptor(
    Type ServiceType,
    IReadOnlyList<Type> ContractTypes,
    ServiceLifetime Lifetime);