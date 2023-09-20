using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Randomization;
using MonoGame.Extended.Tiled;

namespace InfiniteOdyssey.Extensions;

public static class TiledMapEx
{
    private const string TRANSITION = "Transition";
    private const string TRANSITION_DIRECTION = "Direction";
    private const string TRANSITION_ENTRANCE_TYPE = "EntranceType";
    private const string TRANSITION_EXIT_TYPE = "ExitType";

    private const string VARIATION = "Variation";
    private const string VARIATION_PROBABILITY = "Probability";
    private const string VARIATION_LEVEL_MIN = "LevelMin";
    private const string VARIATION_LEVEL_MAX = "LevelMax";

    private const string REQUIREMENT = "Requirement";
    private const string REQUIREMENT_FROM_TRANSITION = "FromTransition";
    private const string REQUIREMENT_ITEM = "Item";

    public static List<Transition> GetTransitions(this TiledMap tiledMap)
    {
        List<Transition> result = new();
        foreach (TiledMapObject obj in tiledMap.GetVisibleObjectsOfType(TRANSITION))
        {
            TiledMapProperties properties = obj.Properties;
            Transition transition = new(obj.Name);

            if (properties.TryGetValue(TRANSITION_DIRECTION, out string direction))
                transition.Direction = Enum.Parse<Direction4>(direction);

            if (properties.TryGetValue(TRANSITION_ENTRANCE_TYPE, out string entranceType))
                transition.EntranceType = Enum.Parse<EntranceType>(entranceType);

            if (properties.TryGetValue(TRANSITION_EXIT_TYPE, out string exitType))
                transition.ExitType = Enum.Parse<ExitType>(exitType);

            foreach (TiledMapProperties prop in obj.GetPropertiesOfType(REQUIREMENT))
            {
                transition.Requirements.Add(new Requirement
                {
                    FromTransition = prop.TryGetValue(REQUIREMENT_FROM_TRANSITION, out string fromTransition) ? fromTransition : string.Empty,
                    Items = prop.TryGetValue(REQUIREMENT_ITEM, out string item) ?
                        item.Split(',').Select(Enum.Parse<Item>).ToList() :
                        new()
                });
            }

            result.Add(transition);
        }
        return result;
    }

    public static List<Variation> GetVariations(this TiledMap tiledMap)
    {
        List<Variation> result = new();
        foreach (TiledMapLayer layer in tiledMap.GetVisibleLayersByType(VARIATION))
        {
            TiledMapProperties properties = layer.Properties;
            Variation variation = new() { Name = layer.Name, Layer = layer };

            int min = properties.TryGetValue(VARIATION_LEVEL_MIN, out string levelMin) ? int.Parse(levelMin) : 0;
            int max = properties.TryGetValue(VARIATION_LEVEL_MAX, out string levelMax) ? int.Parse(levelMax) : int.MaxValue;
            variation.Level = new(min, max);

            if (properties.TryGetValue(VARIATION_PROBABILITY, out string exitType))
                variation.Probability = float.Parse(exitType);

            result.Add(variation);
        }
        return result;
    }

    public static IEnumerable<TiledMapProperties> GetPropertiesOfType(this TiledMap tiledMap, string type)
    {
        foreach (var prop in tiledMap.Properties)
        {
            TiledMapPropertyValue p = prop.Value;
            if (p.PropertyType == type) yield return p;
        }
    }

    public static IEnumerable<TiledMapProperties> GetPropertiesOfType(this TiledMapLayer layer, string type)
    {
        foreach (var prop in layer.Properties)
        {
            TiledMapPropertyValue p = prop.Value;
            if (p.PropertyType == type) yield return p;
        }
    }

    public static IEnumerable<TiledMapProperties> GetPropertiesOfType(this TiledMapObject obj, string type)
    {
        foreach (var prop in obj.Properties)
        {
            TiledMapPropertyValue p = prop.Value;
            if (p.PropertyType == type) yield return p;
        }
    }

    public static IEnumerable<TiledMapProperties> GetPropertiesOfType(this TiledMapTileset tileset, string type)
    {
        foreach (var prop in tileset.Properties)
        {
            TiledMapPropertyValue p = prop.Value;
            if (p.PropertyType == type) yield return p;
        }
    }

    public static IEnumerable<TiledMapObject> GetVisibleObjects(this TiledMap tiledMap)
    {
        foreach (TiledMapObjectLayer layer in tiledMap.ObjectLayers)
        {
            if (!layer.IsVisible) continue;
            foreach (TiledMapObject obj in layer.Objects)
            {
                if (!obj.IsVisible) continue;
                yield return obj;
            }
        }
    }

    public static IEnumerable<TiledMapObject> GetVisibleObjectsOfType(this TiledMap tiledMap, string type)
    {
        foreach (TiledMapObject obj in tiledMap.GetVisibleObjects())
        {
            if (obj.Type == type) yield return obj;
        }
    }

    public static IEnumerable<TiledMapLayer> GetVisibleLayersByType(this TiledMap tiledMap, string type)
    {
        foreach (TiledMapLayer layer in tiledMap.Layers)
        {
            if (!layer.IsVisible) continue;
            if (layer.Type == type) yield return layer;
        }
    }
}