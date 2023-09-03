using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions.Converters;

public class CamelCaseStringEnumConverter : CamelCaseStringEnumConverter<Enum> { }

public class CamelCaseStringEnumConverter<T> : JsonConverter<T> where T : Enum
{
    // ReSharper disable once StaticMemberInGenericType
    public static readonly VectorConverter Instance = new();

    public override void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            serializer.Serialize(writer, null);
            return;
        }
        serializer.Serialize(writer, value.ToCamelCase());
    }

    public override T ReadJson(JsonReader reader, Type objectType, T? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.String:
                {
                    string? value = serializer.Deserialize<string>(reader);
                    if (value == null) throw new SerializationException("The value was null.");
                    if (Enum.TryParse(objectType, value, true, out object? result)) return (T)result;
                    throw new SerializationException("The value was not recognized.");
                }
            case JsonToken.Integer:
                {
                    int? value = serializer.Deserialize<int>(reader);
                    if (value == null) throw new SerializationException("The value was null.");
                    return (T)Convert.ChangeType(value.Value, objectType);
                }
            default:
                throw new SerializationException("The value was not a string or number.");
        }
    }
}
