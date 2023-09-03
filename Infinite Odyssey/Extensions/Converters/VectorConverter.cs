using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions.Converters;

public class VectorConverter : JsonConverter
{
    public static readonly VectorConverter Instance = new();

    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(Vector2)) return true;
        //if (objectType == typeof(Vector2Int)) return true;
        if (objectType == typeof(Vector3)) return true;
        //if (objectType == typeof(Vector3Int)) return true;
        if (objectType == typeof(Vector4)) return true;
        return false;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartArray) { throw new JsonSerializationException(); }
        //reader.Read();
        try
        {
            if (objectType == typeof(Vector2))
            {
                return new Vector2((float)reader.ReadAsDouble(), (float)reader.ReadAsDouble());
            }
            /*if (objectType == typeof(Vector2Int))
                {
                    return new Vector2Int((int)reader.ReadAsInt32(), (int)reader.ReadAsInt32());
                }*/
            if (objectType == typeof(Vector3))
            {
                return new Vector3((float)reader.ReadAsDouble(), (float)reader.ReadAsDouble(), (float)reader.ReadAsDouble());
            }
            /*if (objectType == typeof(Vector3Int))
                {
                    return new Vector3Int((int)reader.ReadAsInt32(), (int)reader.ReadAsInt32(), (int)reader.ReadAsInt32());
                }*/
            if (objectType == typeof(Vector4))
            {
                return new Vector4((float)reader.ReadAsDouble(), (float)reader.ReadAsDouble(), (float)reader.ReadAsDouble(), (float)reader.ReadAsDouble());
            }
            return null;
        }
        finally
        {
            reader.Read();
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        switch (value)
        {
            case Vector2 v2:
                writer.WriteValue(v2.X);
                writer.WriteValue(v2.Y);
                break;
            /*case Vector2Int v2i:
                    writer.WriteValue(v2i.x);
                    writer.WriteValue(v2i.y);
                    break;*/
            case Vector3 v3:
                writer.WriteValue(v3.X);
                writer.WriteValue(v3.Y);
                writer.WriteValue(v3.Z);
                break;
            /*case Vector3Int v3i:
                    writer.WriteValue(v3i.x);
                    writer.WriteValue(v3i.y);
                    writer.WriteValue(v3i.z);
                    break;*/
            case Vector4 v4:
                writer.WriteValue(v4.X);
                writer.WriteValue(v4.Y);
                writer.WriteValue(v4.Z);
                writer.WriteValue(v4.W);
                break;
        }
        writer.WriteEndArray();
    }
}