using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions;

[JsonConverter(typeof(Converter))]
public readonly struct Range : IReadOnlyList<int>, IEquatable<Range>
{
    public static readonly Range Invalid = new(int.MaxValue, int.MinValue, true);

    public readonly int Minimum;
    public readonly int Maximum;

    public bool IsOne => (Minimum == 1) && (Maximum == 1);

    public Range()
    {
        Minimum = 1;
        Maximum = 1;
    }

    public Range(int maximum)
    {
        Minimum = 1;
        Maximum = maximum;
    }

    public Range(int minimum, int maximum)
    {
        if (minimum > maximum) throw new ArgumentException("The minimum value cannot be greater than the maximum value.");
        Minimum = minimum;
        Maximum = maximum;
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private Range(int minimum, int maximum, bool ignoreLimit)
    {
        if ((!ignoreLimit) && (minimum > maximum)) throw new ArgumentException("The minimum value cannot be greater than the maximum value.");
        Minimum = minimum;
        Maximum = maximum;
    }

    public void Deconstruct(out int minimum, out int maximum)
    {
        minimum = Minimum;
        maximum = Maximum;
    }

    public IEnumerator<int> GetEnumerator()
    {
        for (int i = Minimum; i <= Maximum; i++) yield return i;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator (int minimum, int maximum)(Range quantityRange)
        => (quantityRange.Minimum, quantityRange.Maximum);

    public static implicit operator Range((int minimum, int maximum) value)
        => new(value.minimum, value.maximum);

    public static implicit operator Range(int maximum)
        => new(maximum);

    public static implicit operator Range(System.Range value)
        => new(value.Start.Value, value.End.Value);

    public static implicit operator System.Range(Range value)
        => new(value.Minimum, value.Maximum);

    public static bool operator ==(Range a, Range b) => a.Equals(b);
    public static bool operator ==(Range a, System.Range b) => a.Equals(b);
    public static bool operator ==(System.Range a, Range b) => b.Equals(a);

    public static bool operator !=(Range a, Range b) => !a.Equals(b);
    public static bool operator !=(Range a, System.Range b) => !a.Equals(b);
    public static bool operator !=(System.Range a, Range b) => !b.Equals(a);

    public static Range operator +(Range a, int b) => new(a.Minimum + b, a.Maximum + b);
    public static Range operator +(int a, Range b) => new(b.Minimum + a, b.Maximum + a);

    public static Range operator -(Range a, int b) => new(a.Minimum - b, a.Maximum - b);
    public static Range operator -(int a, Range b) => new(b.Minimum - a, b.Maximum - a);

    public static Range operator *(Range a, int b) => new(a.Minimum * b, a.Maximum * b);
    public static Range operator *(int a, Range b) => new(b.Minimum * a, b.Maximum * a);

    public static Range operator /(Range a, int b) => new(a.Minimum / b, a.Maximum / b);
    public static Range operator /(int a, Range b) => new(b.Minimum / a, b.Maximum / a);

    public int Count => (Maximum - Minimum) + 1;

    public int this[int index]
    {
        get
        {
            int result = Minimum + index;
            if (result > Maximum) throw new IndexOutOfRangeException();
            return result;
        }
    }

    public bool Equals(Range other) => (Minimum == other.Minimum) && (Maximum == other.Maximum);
    public bool Equals(System.Range other) => (Minimum == other.Start.Value) && (Maximum == other.End.Value);

    public override bool Equals(object? obj) =>
        obj switch
        {
            Range r => Equals(r),
            System.Range r => Equals(r),
            _ => false
        };

    public override int GetHashCode() => HashCode.Combine(Minimum, Maximum);

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
                        int min = (int?)reader.Value ?? 1;
                        reader.Read();
                        int max = (int?)reader.Value ?? 1;
                        return new Range(min, max);
                    }
                    finally
                    {
                        reader.Read();
                    }
                case JsonToken.Integer:
                    return new Range((int?)reader.Value ?? 1);
                case JsonToken.Null:
                    return new Range(1);
                default:
                    throw new JsonSerializationException();
            }
        }
    }
}