using System;
using System.Collections.Generic;
using InfiniteOdyssey.Randomization;
using MonoGame.Extended.Tiled;

namespace InfiniteOdyssey.Extensions;

public static class TiledMapEx
{
    private const string TRANSITION = "Transition";
    private const string TRANSITION_DIRECTION = "Direction";
    private const string TRANSITION_ENTRANCE_TYPE = "EntranceType";
    private const string TRANSITION_EXIT_TYPE = "ExitType";
    private const string TRANSITION_INDEX = "Index";

    public static List<Transition> GetTransitions(this TiledMap tiledMap)
    {
        List<Transition> result = new();
        foreach (TiledMapObject obj in tiledMap.GetVisibleObjectsByType(TRANSITION))
        {
            TiledMapProperties properties = obj.Properties;
            Transition transition = new();

            if (properties.TryGetValue(TRANSITION_DIRECTION, out string direction))
                transition.Direction = Enum.Parse<Direction4>(direction);

            if (properties.TryGetValue(TRANSITION_ENTRANCE_TYPE, out string entranceType))
                transition.EntranceType = Enum.Parse<EntranceType>(entranceType);

            if (properties.TryGetValue(TRANSITION_EXIT_TYPE, out string exitType))
                transition.ExitType = Enum.Parse<ExitType>(exitType);

            if (properties.TryGetValue(TRANSITION_INDEX, out string index))
                transition.Index = int.Parse(index);

            result.Add(transition);
        }
        return result;
    }

    public static List<Transition> GetVariations(this TiledMap tiledMap)
    {
        List<Transition> result = new();
        foreach (TiledMapObject obj in tiledMap.GetVisibleObjectsByType(TRANSITION))
        {
            TiledMapProperties properties = obj.Properties;
            Transition transition = new();

            if (properties.TryGetValue(TRANSITION_DIRECTION, out string direction))
                transition.Direction = Enum.Parse<Direction4>(direction);

            if (properties.TryGetValue(TRANSITION_ENTRANCE_TYPE, out string entranceType))
                transition.EntranceType = Enum.Parse<EntranceType>(entranceType);

            if (properties.TryGetValue(TRANSITION_EXIT_TYPE, out string exitType))
                transition.ExitType = Enum.Parse<ExitType>(exitType);

            if (properties.TryGetValue(TRANSITION_INDEX, out string index))
                transition.Index = int.Parse(index);

            result.Add(transition);
        }
        return result;
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

    public static IEnumerable<TiledMapObject> GetVisibleObjectsByType(this TiledMap tiledMap, string type)
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
            yield return layer;
        }
    }
}