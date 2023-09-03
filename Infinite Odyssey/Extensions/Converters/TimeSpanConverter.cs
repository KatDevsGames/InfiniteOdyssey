using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions.Converters;

public class TimeSpanConverter : JsonConverter
{
    public static readonly TimeSpanConverter Instance = new();

    public override bool CanConvert(Type objectType) => typeof(TimeSpan).IsAssignableFrom(objectType);

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        => TimeSpan.FromMilliseconds((long)reader.Value);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        => writer.WriteValue((long)((TimeSpan)value).TotalMilliseconds);
}