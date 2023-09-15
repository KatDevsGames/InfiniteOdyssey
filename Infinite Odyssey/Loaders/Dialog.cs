using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace InfiniteOdyssey.Loaders;

[Serializable]
public class Dialog
{
    public string line;
}

[Serializable]
public class DialogSet : Dictionary<string, DialogSet.Line>
{
    [Serializable]
    public class Line
    {
        public string faceName;
        [JsonConverter(typeof(StringEnumConverter))]
        public FaceLoader.Expression expression;
        public int? faceIndex;

        public string name;
        public string text;
        public string voice;
        public Dictionary<string, string> values;

        public string[] choices;
        public string[] nextLine;
        public string[] fireEvent;

        [JsonIgnore] public TemplatedString template;

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            template = new TemplatedString(text);
            if (values?.Count > 0)
                foreach (var value in values) template.Add(value);
        }
    }
}