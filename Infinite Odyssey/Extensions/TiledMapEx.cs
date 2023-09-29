using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InfiniteOdyssey.Randomization;
using Microsoft.Xna.Framework;
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

    private const string SEAL = "Seal";
    private const string SEAL_GAP = "Gap";
    private const string SEAL_TRANSITION = "Transition";

    private const string TREASURE = "Treasure";
    private const string TREASURE_ITEM = "Item";

    private const string ENEMY = "Variation";
    private const string ENEMY_PATTERN = "Pattern";
    private const string ENEMY_PROBABILITY = "Probability";
    private const string ENEMY_LEVEL_MIN = "LevelMin";
    private const string ENEMY_LEVEL_MAX = "LevelMax";

    public static RoomTemplate GetRoom(this TiledMap tiledMap)
    {
        RoomTemplate roomTemplate = new()
        {
            Name = tiledMap.Name,
            Size = new Point(tiledMap.Width / Game.TILES_PER_SCREEN.X, tiledMap.Height / Game.TILES_PER_SCREEN.Y),
            Treasure = tiledMap.GetTreasure().ToDictionary(t => new KeyValuePair<string, TreasureTemplate>(t.Name, t)),
            Transitions = tiledMap.GetTransitions().ToDictionary(t => new KeyValuePair<string, TransitionTemplate>(t.Name, t)),
            TransitionSeals = tiledMap.GetTransitionSeals().ToDictionary(t => new KeyValuePair<string, TransitionSeal>(t.Name, t)),
            Enemies = tiledMap.GetEnemies().ToDictionary(e => new KeyValuePair<string, EnemyTemplate>(e.Name, e)),
            Variations = tiledMap.GetVariations().ToDictionary(v => new KeyValuePair<string, Variation>(v.Name, v))
        };

        return roomTemplate;
    }

    public static bool TryGetTransition(this TiledMap tiledMap, string name, [MaybeNullWhen(false)] out TransitionTemplate transition)
    {
        transition = GetTransitions(tiledMap).FirstOrDefault(t => string.Equals(t.Name, name));
        return transition != default;
    }

    public static IEnumerable<TransitionTemplate> GetTransitions(this TiledMap tiledMap)
    {
        foreach (TiledMapObject obj in tiledMap.GetVisibleObjectsOfType(TRANSITION))
        {
            TiledMapProperties properties = obj.Properties;
            TransitionTemplate transition = new(obj.Name);

            transition.RoomTemplate = tiledMap.Name;
            transition.Bounds = new Rectangle((int)obj.Position.X, (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height);

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
                        new List<Item>()
                });
            }

            yield return transition;
        }
    }

    public static IEnumerable<TransitionSeal> GetTransitionSeals(this TiledMap tiledMap)
    {
        foreach (TiledMapObject obj in tiledMap.GetVisibleObjectsOfType(SEAL))
        {
            TiledMapProperties properties = obj.Properties;
            TransitionSeal seal = new(obj.Name);
            
            seal.Gap = properties.TryGetValue(SEAL_GAP, out string gap) ? int.Parse(gap) : 0;
            seal.Transition = properties.TryGetValue(SEAL_TRANSITION, out string transition) ? transition : string.Empty;

            yield return seal;
        }
    }

    public static IEnumerable<TreasureTemplate> GetTreasure(this TiledMap tiledMap)
    {
        foreach (TiledMapObject obj in tiledMap.GetVisibleObjectsOfType(TREASURE))
        {
            TiledMapProperties properties = obj.Properties;
            TreasureTemplate treasureTemplate = new(obj.Name);
            treasureTemplate.Bounds = new Rectangle((int)obj.Position.X, (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height);

            foreach (TiledMapProperties prop in obj.GetPropertiesOfType(REQUIREMENT))
            {
                treasureTemplate.Requirements.Add(new Requirement
                {
                    FromTransition = prop.TryGetValue(REQUIREMENT_FROM_TRANSITION, out string fromTransition) ? fromTransition : string.Empty,
                    Items = prop.TryGetValue(REQUIREMENT_ITEM, out string item) ?
                        item.Split(',').Select(Enum.Parse<Item>).ToList() :
                        new List<Item>()
                });
            }

            yield return treasureTemplate;
        }
    }

    public static IEnumerable<EnemyTemplate> GetEnemies(this TiledMap tiledMap)
    {
        foreach (TiledMapObject obj in tiledMap.GetVisibleObjectsOfType(ENEMY))
        {
            TiledMapProperties properties = obj.Properties;
            EnemyTemplate enemyTemplate = new(obj.Name);

            enemyTemplate.Location = new Rectangle((int)obj.Position.X, (int)obj.Position.Y, (int)obj.Size.Width, (int)obj.Size.Height);

            int min = properties.TryGetValue(ENEMY_LEVEL_MIN, out string levelMin) ? int.Parse(levelMin) : 0;
            int max = properties.TryGetValue(ENEMY_LEVEL_MAX, out string levelMax) ? int.Parse(levelMax) : int.MaxValue;
            enemyTemplate.Level = new Range(min, max);

            if (properties.TryGetValue(ENEMY_PROBABILITY, out string exitType))
                enemyTemplate.Probability = float.Parse(exitType);

            if (properties.TryGetValue(ENEMY_PATTERN, out string pattern))
                enemyTemplate.Pattern = int.Parse(pattern);

            yield return enemyTemplate;
        }
    }

    public static IEnumerable<Variation> GetVariations(this TiledMap tiledMap)
    {
        foreach (TiledMapLayer layer in tiledMap.GetVisibleLayersByType(VARIATION))
        {
            TiledMapProperties properties = layer.Properties;
            Variation variation = new(layer.Name) { Layer = layer };

            int min = properties.TryGetValue(VARIATION_LEVEL_MIN, out string levelMin) ? int.Parse(levelMin) : 0;
            int max = properties.TryGetValue(VARIATION_LEVEL_MAX, out string levelMax) ? int.Parse(levelMax) : int.MaxValue;
            variation.Level = new Range(min, max);

            if (properties.TryGetValue(VARIATION_PROBABILITY, out string exitType))
                variation.Probability = float.Parse(exitType);

            yield return variation;
        }
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