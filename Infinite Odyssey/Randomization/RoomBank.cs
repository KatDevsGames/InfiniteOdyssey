using System;
using System.Collections;
using System.Collections.Generic;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled;

namespace InfiniteOdyssey.Randomization;

public class RoomBank : IReadOnlyDictionary<string, RoomTemplate>
{
    private readonly Game m_game;

    private readonly Dictionary<MapType, Dictionary<Biome, List<RoomTemplate>>> m_byTypeBiome = new();

    private readonly Dictionary<string, RoomTemplate> m_byName = new();

    public RoomBank(Game game)
    {
        m_game = game;

        ContentManager content = m_game.Content;
        loadMapAssets("Maps\\Overworld", MapType.Overworld);
        loadMapAssets("Maps\\Dungeons", MapType.Dungeon);

        void loadMapAssets(string path, MapType mapType)
        {
            m_byTypeBiome[mapType] = Enum.GetValues<Biome>().ToDictionary(b => new KeyValuePair<Biome, List<RoomTemplate>>(b, new List<RoomTemplate>()));
            foreach (string assetName in content.GetAssetNames(path))
            {
                TiledMap map = content.Load<TiledMap>(assetName);
                content.UnloadAsset(assetName);

                RoomTemplate roomTemplate = map.GetRoom();
                roomTemplate.TileMap = assetName;

                m_byName.Add(roomTemplate.Name, roomTemplate);
                if (!m_byTypeBiome[mapType].TryGetValue(roomTemplate.Biome, out List<RoomTemplate>? rooms)) continue;
                rooms.Add(roomTemplate);
            }
        }
    }

    public IEnumerable<RoomTemplate> GetBy(MapType mapType, Biome biome)
    {
        if (!(m_byTypeBiome.TryGetValue(mapType, out var byBiome) &&
              byBiome.TryGetValue(biome, out var rooms))) yield break;

        foreach (RoomTemplate room in rooms) yield return room;
    }

    public IEnumerator<KeyValuePair<string, RoomTemplate>> GetEnumerator() => m_byName.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_byName).GetEnumerator();

    public int Count => m_byName.Count;

    public bool ContainsKey(string key) => m_byName.ContainsKey(key);

    public bool TryGetValue(string key, out RoomTemplate value) => m_byName.TryGetValue(key, out value);

    public RoomTemplate this[string key] => m_byName[key];

    public IEnumerable<string> Keys => m_byName.Keys;

    public IEnumerable<RoomTemplate> Values => m_byName.Values;
}