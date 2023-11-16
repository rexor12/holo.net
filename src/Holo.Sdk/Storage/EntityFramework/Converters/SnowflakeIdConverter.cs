using System;
using System.Linq.Expressions;
using Holo.Sdk.Storage.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Holo.Sdk.Storage.EntityFramework.Converters;

/// <summary>
/// A <see cref="ValueConverter{TModel, TProvider}"/> for <see cref="SnowflakeId"/> instances.
/// </summary>
public sealed class SnowflakeIdConverter : ValueConverter<SnowflakeId, long>
{
    private static readonly Expression<Func<SnowflakeId, long>> Serialize = i => (long)i;
    private static readonly Expression<Func<long, SnowflakeId>> Deserialize = i => (SnowflakeId)i;

    public SnowflakeIdConverter()
        : base(Serialize, Deserialize, null)
    {
    }
}