using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions;

[Serializable, JsonConverter(typeof(RangeConverter))]
public struct Range
{
    public int Min;
    public int Max;

    public bool Contains(int value) => (value >= Min) && (value <= Max);

    [JsonConstructor]
    public Range(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public class RangeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(Range).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray) { throw new JsonSerializationException(); }
            //reader.Read();
            try
            {
                int min = (int)reader.ReadAsInt32();
                int max = (int)reader.ReadAsInt32();
                return new Range(min, max);
            }
            finally
            {
                reader.Read();
            }
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            Range r = (Range)value;
            writer.WriteStartArray();
            writer.WriteValue(r.Min);
            writer.WriteValue(r.Max);
            writer.WriteEndArray();
        }
    }

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(Range))]
        public class Editor : PropertyDrawer
        {
            private static readonly GUIContent[] labels = {
                new("Min"),
                new("Max")
            };

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var min = property.FindPropertyRelative("Min");
                var max = property.FindPropertyRelative("Max");
                EditorGUI.LabelField(position, property.name);
                position.x += EditorGUIUtility.labelWidth;
                
                position.height = 18f;
                position.width = 200f;
                EditorGUI.BeginChangeCheck();
                int[] values = {min.intValue, max.intValue};
                EditorGUI.MultiIntField(position, labels, values);
                if (EditorGUI.EndChangeCheck())
                {
                    min.intValue = values[0];
                    max.intValue = values[1];
                }
            }
        }
#endif
}