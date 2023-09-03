using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions.Converters;

public class FlagsEnumConverter<T> : JsonConverter where T : struct, Enum
{
    // ReSharper disable once StaticMemberInGenericType
    public static readonly VectorConverter Instance = new();

    private static readonly Func<ulong, T> GetEnumValue;

    private static readonly Func<T, ulong> GetNumericValue;

    static FlagsEnumConverter()
    {
        if (typeof(byte).IsAssignableFrom(Enum.GetUnderlyingType(typeof(T))))
        {
            GetNumericValue = t => (byte)(object)t;
            GetEnumValue = t => unchecked((T)(object)(byte)t);
            return;
        }

        if (typeof(sbyte).IsAssignableFrom(Enum.GetUnderlyingType(typeof(T))))
        {
            GetNumericValue = t => unchecked((ulong)(sbyte)(object)t);
            GetEnumValue = t => unchecked((T)(object)(sbyte)t);
            return;
        }

        if (typeof(short).IsAssignableFrom(Enum.GetUnderlyingType(typeof(T))))
        {
            GetNumericValue = t => unchecked((ulong)(short)(object)t);
            GetEnumValue = t => unchecked((T)(object)(short)t);
            return;
        }

        if (typeof(ushort).IsAssignableFrom(Enum.GetUnderlyingType(typeof(T))))
        {
            GetNumericValue = t => (ushort)(object)t;
            GetEnumValue = t => unchecked((T)(object)(ushort)t);
            return;
        }

        if (typeof(int).IsAssignableFrom(Enum.GetUnderlyingType(typeof(T))))
        {
            GetNumericValue = t => unchecked((ulong)(int)(object)t);
            GetEnumValue = t => unchecked((T)(object)(int)t);
            return;
        }

        if (typeof(uint).IsAssignableFrom(Enum.GetUnderlyingType(typeof(T))))
        {
            GetNumericValue = t => (uint)(object)t;
            GetEnumValue = t => unchecked((T)(object)(uint)t);
            return;
        }

        if (typeof(long).IsAssignableFrom(Enum.GetUnderlyingType(typeof(T))))
        {
            GetNumericValue = t => unchecked((ulong)(long)(object)t);
            GetEnumValue = t => unchecked((T)(object)(long)t);
            return;
        }

        if (typeof(ulong).IsAssignableFrom(Enum.GetUnderlyingType(typeof(T))))
        {
            GetNumericValue = t => (ulong)(object)t;
            GetEnumValue = t => (T)(object)t;
            return;
        }

        throw new NotSupportedException("FlagsEnumConverter requires an integral type enum.");
    }

    public override bool CanConvert(Type objectType) => typeof(T).IsAssignableFrom(objectType);

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        => GetEnumValue(serializer.Deserialize<IEnumerable<string>>(reader)
            .Select(v => (T)Enum.Parse(typeof(T), (string)v)).Aggregate(0UL, (acc, val) => acc | GetNumericValue(val)));

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        => serializer.Serialize(writer, Enum.GetValues(typeof(T)).OfType<T>().Where(v => ((T)value).HasFlag(v)).Select(v => Enum.GetName(typeof(T), v)));
}