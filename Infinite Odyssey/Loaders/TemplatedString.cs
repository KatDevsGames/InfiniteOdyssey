using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace InfiniteOdyssey.Loaders;

public class TemplatedString : IDictionary<string, string>
{
    public static readonly TemplatedString INVALID_INSTANCE = new();

    private readonly JToken baseToken;
    private readonly Dictionary<string, string> stringValues = new();

    private TemplatedString() { }

    public TemplatedString(JToken baseToken) => this.baseToken = baseToken;

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => stringValues.GetEnumerator();

    public override string ToString() => Eval(baseToken, stringValues);
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) stringValues).GetEnumerator();

    private static string Eval(JToken value, Dictionary<string, string> stringValues)
    {
        switch (value.Type)
        {
            case JTokenType.Integer:
                return value.Value<long>().ToString();
            case JTokenType.String:
            {
                string result = value.Value<string>();
                if ((result.Length > 1) && (result[0] == '@')) { return stringValues[result]; }
                return result;
            }
            case JTokenType.Object:
                break;
            default:
                throw new ArgumentException(
                    $"Invalid value token type {Enum.GetName(typeof(JTokenType), value.Type)}.", nameof(value));
        }
        switch (value["type"].Value<string>())
        {
            case "template":
            {
                StringBuilder result = new(value["text"].Value<string>());
                JToken? valueTokens = value["values"];
                if (valueTokens != null)
                {
                    string[] values = valueTokens.Value<JArray>().Select(v => Eval(v, stringValues)).ToArray();
                    for (int i = 0; i < values.Length; i++) { result.Replace($"{{{i}}}", values[i]); }
                }
                return result.ToString();
            }
            case "repeat":
            {
                string text = value["text"].Value<string>();
                StringBuilder result = new();
                int times = int.Parse(Eval(value["times"], stringValues));
                for (int i = 0; i < times; i++) { result.Append(text); }
                return result.ToString();
            }
            case "modulus":
            {
                long dividend = long.Parse(Eval(value["dividend"], stringValues));
                long divisor = long.Parse(Eval(value["divisor"], stringValues));
                return (dividend % divisor).ToString();
            }
            case "empty":
            default:
                return string.Empty;
        }
    }

    public void Add(KeyValuePair<string, string> item) => ((IDictionary<string, string>)stringValues).Add(item);

    public void Clear() => stringValues.Clear();

    public bool Contains(KeyValuePair<string, string> item) => stringValues.Contains(item);

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => ((IDictionary<string, string>) stringValues).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<string, string> item) => ((IDictionary<string, string>)stringValues).Remove(item);

    public int Count => stringValues.Count;

    public bool IsReadOnly => false;

    public void Add(string key, string value) => stringValues.Add(key, value);

    public bool ContainsKey(string key) => stringValues.ContainsKey(key);

    public bool Remove(string key) => stringValues.Remove(key);

    public bool TryGetValue(string key, out string value) => stringValues.TryGetValue(key, out value);

    public string this[string key]
    {
        get => stringValues[key];
        set => stringValues[key] = value;
    }

    public ICollection<string> Keys => stringValues.Keys;

    public ICollection<string> Values => stringValues.Values;

    public static implicit operator string(TemplatedString template) => template.ToString();
}