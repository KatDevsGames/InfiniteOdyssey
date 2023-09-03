using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace InfiniteOdyssey.Extensions.Converters;

public class AlwaysSerializeAttribute : Attribute { }

public static class JsonInitializer
{
    public static void Init() { }

    public static readonly JsonSerializerSettings DefaultSettings = new()
    {
        Converters = new List<JsonConverter>
        {
            VectorConverter.Instance,
            TimeSpanConverter.Instance,
            CamelCaseStringEnumConverter.Instance,
            HexColorConverter.Instance
        },
        ContractResolver = ContractResolver.Instance
    };

    public static readonly JsonSerializer DefaultSerializer = JsonSerializer.Create(DefaultSettings);

    public class ContractResolver : DefaultContractResolver
    {
        public static ContractResolver Instance { get; } = new();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            bool alwaysSerialize = member.CustomAttributes.Any(d => d.AttributeType == typeof(AlwaysSerializeAttribute));
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (alwaysSerialize) property.ShouldSerialize = o => true;
            return property;
        }
    }

    static JsonInitializer()
    {
        JsonConvert.DefaultSettings = () => DefaultSettings;
    }
}