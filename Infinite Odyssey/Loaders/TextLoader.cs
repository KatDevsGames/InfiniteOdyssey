using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using InfiniteOdyssey.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfiniteOdyssey.Loaders;

public class TextLoader
{
    public static TextLoader Instance { get; private set; }

    public static void SetLocale(string locale) => Instance = new(locale);

    public static LocaleManifest Manifest { get; }

    static TextLoader()
    {
        string json = File.ReadAllText("Content\\Text\\manifest.json");
        Manifest = JsonConvert.DeserializeObject<LocaleManifest>(json);
        m_defaultLocale = Manifest.Locales[Manifest.Fallback];
    }

    private const string MISSING_BANK = "MISSING DIALOG BANK {0}";
    private const string MISSING_ENTRY = "MISSING DIALOG ENTRY {1} IN BANK {0}";
    private const string MISSING_ITEM_ENTRY = "MISSING ITEM ENTRY {0}";

    public const string DEFAULT_LOCALE = "en-US";
    private static readonly Locale m_defaultLocale;

    public readonly string m_localeCode;
    public readonly string m_localeName;

    private readonly (string name, DictType type)[] LOCALE_BANKS =
    {
        ("TitleMenu", DictType.Basic),
        ("SettingsMenu", DictType.Basic),
        ("ModalDialog", DictType.Basic),
        ("LoadMenu", DictType.Template)
    };

    private enum DictType
    {
        Basic,
        Template,
        ItemSet
    }

    public Dictionary<string, Dictionary<string, string>> textData = new();

    public Dictionary<string, Dictionary<string, TemplatedString>> templateData = new();

    public Dictionary<uint, (string name, string description)> itemData = new();

    [JsonConverter(typeof(Converter))]
    public class LocaleManifest
    {
        public readonly Dictionary<string, Locale> Locales;

        public readonly string Fallback;

        [JsonConstructor]
        public LocaleManifest(Dictionary<string, Locale> locales, string? fallback)
        {
            Locales = locales;
            Fallback = fallback ?? DEFAULT_LOCALE;
        }

        private class Converter : JsonConverter<LocaleManifest>
        {
            public override void WriteJson(JsonWriter writer, LocaleManifest? value, JsonSerializer serializer) => throw new NotImplementedException();

            public override LocaleManifest? ReadJson(JsonReader reader, Type objectType, LocaleManifest? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;
                JObject j = JObject.Load(reader);
                
                JObject jLocales = (j["locales"] as JObject) ?? throw new SerializationException();
                Dictionary<string, Locale> locales = new();
                foreach (var locale in jLocales)
                {
                    string code = locale.Key;
                    JObject lj = (locale.Value as JObject) ?? throw new SerializationException();
                    string name = lj["name"]?.Value<string>() ?? throw new SerializationException();
                    string? redirect = lj["redirect"]?.Value<string>();
                    Dictionary<string, string> localeNames = lj["localeNames"]?.ToObject<Dictionary<string, string>>() ?? throw new SerializationException(); ;
                    locales.Add(code, new(code, name, redirect, localeNames));
                }

                string? fallback = j["fallback"]?.Value<string>();

                return new(locales, fallback);
            }
        }
    }

    public class Locale
    {
        [JsonProperty(PropertyName = "code")]
        public readonly string Code;

        [JsonProperty(PropertyName = "name")]
        public readonly string Name;

        [JsonProperty(PropertyName = "redirect")]
        public readonly string? Redirect;

        [JsonProperty(PropertyName = "localNames")]
        public readonly Dictionary<string, string> LocalNames;

        [JsonConstructor]
        public Locale(string code, string name, string redirect, Dictionary<string, string> localNames)
        {
            Code = code;
            Name = name;
            Redirect = redirect;
            LocalNames = localNames;
        }
    }

    public TextLoader(string? localeCode)
    {
        var languages = Manifest.Locales;
        string fallback = Manifest.Fallback ?? DEFAULT_LOCALE;

        HashSet<string> wasRedirected = new();
        loadLang:
        if ((localeCode != null) && (languages?.TryGetValue(localeCode, out Locale? locale) ?? false))
        {
            string? redirect = locale.Redirect;
            if (!string.IsNullOrWhiteSpace(redirect))
            {
                if (string.Equals(localeCode, fallback))
                    throw new("The fallback language may not contain a redirect.");

                wasRedirected.Add(localeCode);
                localeCode = redirect;

                if (wasRedirected.Contains(localeCode))
                    throw new("The locale manifest contained a redirection loop.");
                goto loadLang;
            }
            m_localeName = locale.Name;
            m_localeCode = locale.Code;
            LoadBaseStrings();
            return;
        }

        if ((!string.IsNullOrWhiteSpace(fallback)) && (!string.Equals(fallback, localeCode)))
        {
            localeCode = fallback;
            goto loadLang;
        }

        throw new("The locale was not found and the manifest did not specify a valid fallback.");
    }

    public string GetText(string bank, string entry)
    {
        if (!textData.TryGetValue(bank, out var entries)) { return string.Format(MISSING_BANK, bank); }
        if (!entries.TryGetValue(entry, out string? value)) { return string.Format(MISSING_ENTRY, bank, entry); }
        return value;
    }

    public TemplatedString GetTemplate(string bank, string entry)
    {
        if (!templateData.TryGetValue(bank, out var entries)) { return TemplatedString.INVALID_INSTANCE; }
        if (!entries.TryGetValue(entry, out TemplatedString? value)) { return TemplatedString.INVALID_INSTANCE; }
        return value;
    }

    public DialogSet GetDialogSet(string mission)
    {
        string text = File.ReadAllText($"Content\\Text\\{m_localeCode}\\Dialog\\{mission}.json");
        return JsonConvert.DeserializeObject<DialogSet>(text);
    }

    public string GetText(string bank, string entry, IDictionary<string, string> context)
    {
        TemplatedString ts = GetTemplate(bank, entry);
        ts.TryAddRange(context);
        return ts.ToString();
    }

    public (string name, string description) GetItem(uint entry)
    {
        if (!itemData.TryGetValue(entry, out (string name, string description) value))
        {
            string e = string.Format(MISSING_ITEM_ENTRY, entry);
            return (e, e);
        }
        return value;
    }

    private void LoadBaseStrings()
    {
        textData.Clear();
        foreach ((string name, DictType type) next in LOCALE_BANKS)
        {
            string path = $"Content\\Text\\{m_localeCode}\\{next.name}.json";
            if (!File.Exists(path)) path = $"Content\\Text\\{DEFAULT_LOCALE}\\{next.name}.json";
            string text = File.ReadAllText(path);
            switch (next.type)
            {
                case DictType.Basic:
                {
                    var textTable = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                    textData.Add(next.name, textTable);
                    break;
                }
                case DictType.Template:
                {
                    var textTable = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(text);
                    templateData.Add(next.name, textTable.ToDictionary(kv =>
                        new KeyValuePair<string, TemplatedString>(kv.Key, new(kv.Value))));
                    break;
                }
                case DictType.ItemSet:
                {
                    itemData = JsonConvert.DeserializeObject<Dictionary<uint, (string name, string description)>>(text);
                    break;
                }
            }
        }
    }
}