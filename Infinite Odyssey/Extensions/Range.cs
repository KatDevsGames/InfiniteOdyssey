using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions;

[JsonConverter(typeof(Converter))]
public readonly struct Range
{
    public readonly uint Minimum;
    public readonly uint Maximum;

    public bool IsOne => (Minimum == 1) && (Maximum == 1);

    public Range()
    {
        Minimum = 1;
        Maximum = 1;
    }

    public Range(uint maximum)
    {
        Minimum = 1;
        Maximum = maximum;
    }

    public Range(uint minimum, uint maximum)
    {
        if (minimum > maximum) throw new ArgumentException("The minimum value cannot be greater than the maximum value.");
        Minimum = minimum;
        Maximum = maximum;
    }

    public void Deconstruct(out uint minimum, out uint maximum)
    {
        minimum = Minimum;
        maximum = Maximum;
    }

    public static implicit operator (uint minimum, uint maximum)(Range quantityRange)
        => (quantityRange.Minimum, quantityRange.Maximum);

    public static implicit operator Range((uint minimum, uint maximum) value)
        => new(value.minimum, value.maximum);

    public static implicit operator Range(uint maximum)
        => new(maximum);

    public static implicit operator Range(int maximum)
        => new((uint)maximum);

#if NETCOREAPP
    public static implicit operator Range(System.Range value)
        => new((uint)value.Start.Value, (uint)value.End.Value);

    public static implicit operator System.Range(Range value)
        => new((int)value.Minimum, (int)value.Maximum);
#endif

    private class Converter : JsonConverter<Range>
    {
        public override void WriteJson(JsonWriter writer, Range value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.Minimum);
            writer.WriteValue(value.Maximum);
            writer.WriteEndArray();
        }

        public override Range ReadJson(JsonReader reader, Type objectType, Range existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartArray:
                    try
                    {
                        reader.Read();
                        uint min = (uint?)reader.Value ?? 1;
                        reader.Read();
                        uint max = (uint?)reader.Value ?? 1;
                        return new Range(min, max);
                    }
                    finally
                    {
                        reader.Read();
                    }
                case JsonToken.Integer:
                    return new Range((uint?)reader.Value ?? 1);
                case JsonToken.Null:
                    return new Range(1);
                default:
                    throw new JsonSerializationException();
            }
        }
    }
}