using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfiniteOdyssey;

[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public class Settings : ICloneable
{
    public bool NoFlashing = false;
    public bool NoColors = false;

    public float MusicVolume = 0.7f;
    public float SFXVolume = 0.7f;
    public float DialogVolume = 0.7f;

    public float TextSpeed = 1;
    public string? LanguageLocale;

    public JObject? InputMap;

#if DESKTOP
    public int DisplayWidth = 1920;
    public int DisplayHeight = 1080;
    public bool FullScreen = false;
#else
    public const int DisplayWidth = 1920;
    public const int DisplayHeight = 1080;
    public const bool FullScreen = true;
#endif

    public bool IsDirty { get; set; } = false;

    private static readonly string BASE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Watercolor Games", "Peacenet");

    public Settings() => Load(0);

    public Settings(string json) => TryParseJSON(json);

    public static bool TryLoad(int saveNum, out Settings value)
    {
        value = new();
        return value.Load(saveNum);
    }

    public bool Load(int saveNum)
    {
        string saveFile = Path.Combine(BASE_PATH, $"save_{saveNum}.db");
        if (!File.Exists(saveFile)) { return false; }
        return TryParseJSON(File.ReadAllText(BASE_PATH));
    }

    public bool TryParseJSON(string json)
    {
        try
        {
            JObject j = JObject.Parse(json);
            if (j.TryGetValue("noFlashing", out JToken? noFlashing)) NoFlashing = noFlashing.Value<bool>();
            if (j.TryGetValue("noColors", out JToken? noColors)) NoColors = noColors.Value<bool>();

            if (j.TryGetValue("musicVolume", out JToken? musicVolume)) MusicVolume = musicVolume.Value<float>();
            if (j.TryGetValue("sfxVolume", out JToken? sfxVolume)) SFXVolume = sfxVolume.Value<float>();
            if (j.TryGetValue("dialogVolume", out JToken? dialogVolume)) DialogVolume = dialogVolume.Value<float>();

            if (j.TryGetValue("textSpeed", out JToken? textSpeed)) TextSpeed = textSpeed.Value<float>();
            if (j.TryGetValue("languageLocale", out JToken? languageLocale))
                LanguageLocale = languageLocale.Value<string>();

            if (j.TryGetValue("inputMap", out JToken? inputMap)) InputMap = (JObject?)inputMap;

#if DESKTOP
            if (j.TryGetValue("displayWidth", out JToken? displayWidth)) DisplayWidth = displayWidth.Value<int>();
            if (j.TryGetValue("displayHeight", out JToken? displayHeight)) DisplayHeight = displayHeight.Value<int>();
            if (j.TryGetValue("fullScreen", out JToken? fullScreen)) FullScreen = fullScreen.Value<bool>();

            IsDirty = false;
            return true;
#endif
        }
        catch { return false; }
    }

    public void CopyFrom(Settings source) => TryParseJSON(GetJSON());
    
    public void Save(int saveNum)
    {
        string saveFile = Path.Combine(BASE_PATH, $"save_{saveNum}.db");
        File.WriteAllText(saveFile, GetJSON());
        IsDirty = false;
    }

    public string GetJSON()
    {
        JObject j = new()
        {
            {"noFlashing", NoFlashing},
            {"noColors", NoColors},

            {"musicVolume", MusicVolume},
            {"sfxVolume", SFXVolume},
            {"dialogVolume", DialogVolume},

            {"textSpeed", TextSpeed},
            {"languageLocale", LanguageLocale},

            {"inputMap", InputMap},

#if DESKTOP
            {"displayWidth", DisplayWidth},
            {"displayHeight", DisplayHeight},
            {"fullScreen", FullScreen}
#endif
        };
        return j.ToString(Formatting.None);
    }

    public Settings Clone() => new(GetJSON());

    object ICloneable.Clone() => Clone();
}