using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfiniteOdyssey;

[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
public static class Settings
{
    public static bool NoFlashing = false;
    public static bool NoColors = false;

    public static float MusicVolume = 0.7f;
    public static float SFXVolume = 0.7f;
    public static float DialogVolume = 0.7f;

    public static float TextSpeed = 1;

    public static string? LanguageLocale;

    public static JObject? InputMap;

#if DESKTOP
    public static int DisplayWidth = 1920;
    public static int DisplayHeight = 1080;
    public static bool FullScreen = false;
#else
    public const int DisplayWidth = 1920;
    public const int DisplayHeight = 1080;
    public const bool FullScreen = true;
#endif

    private static readonly string BASE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Watercolor Games", "Peacenet");

    static Settings() => Load(0);

    public static void Load(int saveNum)
    {
        string saveFile = Path.Combine(BASE_PATH, $"save_{saveNum}.db");
        if (!File.Exists(saveFile)) { return; }
        try
        {
            JObject j = JObject.Parse(File.ReadAllText(BASE_PATH));
            if (j.TryGetValue("noFlashing", out JToken? noFlashing)) NoFlashing = noFlashing.Value<bool>();
            if (j.TryGetValue("noColors", out JToken? noColors)) NoColors = noColors.Value<bool>();

            if (j.TryGetValue("musicVolume", out JToken? musicVolume)) MusicVolume = musicVolume.Value<float>();
            if (j.TryGetValue("sfxVolume", out JToken? sfxVolume)) SFXVolume = sfxVolume.Value<float>();
            if (j.TryGetValue("dialogVolume", out JToken? dialogVolume)) DialogVolume = dialogVolume.Value<float>();

            if (j.TryGetValue("textSpeed", out JToken? textSpeed)) TextSpeed = textSpeed.Value<float>();

            if (j.TryGetValue("languageLocale", out JToken? languageLocale)) LanguageLocale = languageLocale.Value<string>();

            if (j.TryGetValue("inputMap", out JToken? inputMap)) InputMap = (JObject?)inputMap;

#if DESKTOP
            if (j.TryGetValue("displayWidth", out JToken? displayWidth)) DisplayWidth = displayWidth.Value<int>();
            if (j.TryGetValue("displayHeight", out JToken? displayHeight)) DisplayHeight = displayHeight.Value<int>();
            if (j.TryGetValue("fullScreen", out JToken? fullScreen)) FullScreen = fullScreen.Value<bool>();
#endif
        }
        catch { /**/ }
    }
    
    public static void Save(int saveNum)
    {
        string saveFile = Path.Combine(BASE_PATH, $"save_{saveNum}.db");
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
        File.WriteAllText(saveFile, j.ToString(Formatting.None));
    }
}