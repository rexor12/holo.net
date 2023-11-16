using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;

namespace Holo.ServiceHost.BackgroundProcessing;

/// <summary>
/// Helper methods for <see cref="Item"/>.
/// </summary>
public static class ItemHelper
{
    private static readonly JsonSerializer Serializer = new()
    {
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        TypeNameHandling = TypeNameHandling.Objects,
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// Serializes the given <paramref name="itemData"/> as a <c>string</c>.
    /// </summary>
    /// <param name="itemData">The object to serialize.</param>
    /// <returns>The serialized object.</returns>
    public static string SerializeItemData(object itemData)
    {
        using var stringWriter = new StringWriter();
        using var jsonWriter = new JsonTextWriter(stringWriter);
        Serializer.Serialize(jsonWriter, itemData);

        return stringWriter.ToString();
    }

    /// <summary>
    /// Attempts to deserialize the given <paramref name="serializedItemData"/>.
    /// </summary>
    /// <param name="serializedItemData">The serialized object.</param>
    /// <param name="itemData">The deserialized object.</param>
    /// <returns><c>true</c>, if deserialization was successful.</returns>
    public static bool TryDeserializeItemData(string serializedItemData, [NotNullWhen(true)] out object? itemData)
    {
        using var stringReader = new StringReader(serializedItemData);
        using var jsonReader = new JsonTextReader(stringReader);
        itemData = Serializer.Deserialize(jsonReader);

        return itemData != null;
    }
}