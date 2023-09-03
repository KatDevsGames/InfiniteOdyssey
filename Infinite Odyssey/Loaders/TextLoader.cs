using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using InfiniteOdyssey.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfiniteOdyssey.Loaders;

public class TextLoader
{
    public static TextLoader Instance { get; private set; } = new(
        Settings.LanguageLocale ??
        CultureInfo.CurrentCulture.Name);

    public static void SetLocale(string locale) => Instance = new TextLoader(locale);

    private const string MISSING_BANK = "MISSING DIALOG BANK {0}";
    private const string MISSING_ENTRY = "MISSING DIALOG ENTRY {1} IN BANK {0}";
    private const string MISSING_ITEM_ENTRY = "MISSING ITEM ENTRY {0}";

    private const string GENERAL_SPECIES_NAME = "General";

    public readonly string localeCode;
    public readonly string localeName;

    private readonly (string name, DictType type)[] LOCALE_BANKS =
    {
        ("TitleMenu", DictType.Basic),
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
    public TextLoader(string? locale)
    {
        string text = File.ReadAllText("Content\\Text\\manifest.json");
        JObject mObj = JObject.Parse(text);

        var languages = (IDictionary<string, JToken>)mObj["languages"];

        loadLang:
        if ((locale != null) && (languages?.TryGetValue(locale, out JToken? lToken) ?? false))
        {
            var lDict = ((IDictionary<string, JToken>)lToken);
            if (lDict.TryGetValue("redirect", out JToken? fbToken))
            {
                locale = fbToken.Value<string>();
                goto loadLang;
            }
            localeName = lDict["name"].Value<string>();
            localeCode = locale;
            LoadBaseStrings();
            return;
        }

        foreach (KeyValuePair<string, JToken> lang in languages)
        {
            IDictionary<string, JToken> lVal = (IDictionary<string, JToken>)lang.Value;
            if (lVal.TryGetValue("fallback", out JToken fbVal) && fbVal.Value<bool>())
            {
                locale = lang.Key;
                goto loadLang;
            }
        }
        throw new Exception($"The locale was not found and the manifest did not specify a fallback.");
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
        string text = File.ReadAllText($"Content\\Text\\{localeCode}\\Dialog\\{mission}.json");
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
            string text = File.ReadAllText($"Content\\Text\\{localeCode}\\{next.name}.json");
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
                        new KeyValuePair<string, TemplatedString>(kv.Key, new TemplatedString(kv.Value))));
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