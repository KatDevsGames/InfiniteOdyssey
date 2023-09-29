using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Randomization;

public class DungeonGenerator
{
    private readonly Game m_game;
    private readonly RNG m_rng;

    private readonly WorldParameters m_parameters;
    private readonly World m_world;

    private readonly RoomBank m_bank;
    private DungeonParameters? m_dungeonParameters;

    public DungeonGenerator(long seed, Game game, WorldParameters parameters, World world)
    {
        m_game = game;
        m_parameters = parameters;
        m_world = world;

        m_rng = new RNG(seed);
        m_bank = new RoomBank(game);
    }

    public void Generate(int regionIndex, int dungeonIndex)
    {
        if (m_world.Regions.Length <= regionIndex) throw new ArgumentOutOfRangeException(nameof(regionIndex), "Not a valid region index.");
        Region region = m_world.Regions[regionIndex];
        Dungeon dungeon = region.Dungeons[dungeonIndex];
        m_dungeonParameters = region.Parameters.Dungeons?[dungeonIndex];

        List<RoomTemplate> roomSet = m_bank.GetBy(MapType.Dungeon, dungeon.Biome).ToList();
        Floor[] floors = dungeon.Floors = new Floor[m_rng.IRandom(m_dungeonParameters.FloorCount)];
        int numFloors = floors.Length;
        BossPlacement bossPlacement = m_dungeonParameters.BossPlacement ?? EnumEx<BossPlacement>.TakeRandomValue(m_rng);
        int bossFloor;
        if (numFloors <= 2) bossFloor = numFloors - 1;
        else bossFloor = bossPlacement switch
        {
            BossPlacement.BossAtEnd => numFloors - 1,
            BossPlacement.BossKeyAtEnd => m_rng.IRandom(1..(numFloors - 1)),
            _ => numFloors - 1
        };
        for (int i = 0; i < numFloors; i++)
        {
            int floorTarget = m_rng.IRandom(m_dungeonParameters.RoomCount / numFloors);
            int entrances;
            if (i == 0)
            {
                entrances = floorTarget switch
                {
                    < 9 => 1,
                    >= 9 and < 14 => m_rng.IRandom(1..2, 0.5),
                    >= 14 and < 25 => m_rng.IRandom(1..2),
                    >= 25 => m_rng.IRandom(1..3, 0.9)
                };
            }
            else entrances = 0;
            GenerateFloor(dungeon, roomSet, i, floorTarget, entrances, i == bossFloor);
        }
    }

    private void GenerateFloor(Dungeon dungeon, IList<RoomTemplate> roomSet, int floorIndex, int targetRooms, int entrances, bool boss)
    {
        Floor floor = dungeon.Floors[floorIndex];
        List<List<Room>> map = floor.Map;

        int totalRooms = boss ? 1 : 0;

        RoomTemplate[]? entranceRooms = null;
        for (; entrances > 0; entrances--)
        {
            entranceRooms ??= roomSet.Where(r => r.Transitions.Values.Any(t => t.ExitType == ExitType.Dungeon)).ToArray();

        }
        for (; totalRooms <= targetRooms; totalRooms++)
        {
            //fill in common rooms
        }
        if (boss)
        {
            //place boss room
        }
    }

    private void PutRoom(List<List<Room>> map, int x, int y, Room room)
    {
        int height = map.FirstOrDefault()?.Count ?? 0;
        int newHeight = Math.Max(height, y + 1);

        if (x >= map.Count) map.Resize(x + 1, () => (new List<Room>()).Resize(newHeight));
        if (y >= height) foreach (List<Room> rooms in map) rooms.Resize(newHeight);

        map[x][y] = room;
    }

    private IEnumerable<Point> UnmappedPoints(Dungeon dungeon, int floor)
    {
        List<List<Room?>> rooms = dungeon.Floors[floor].Map;
        for (int x = 0; x < rooms.Count; x++)
            for (int y = 0; y < rooms[x].Count; y++)
            {
                if (rooms[x][y] == null) yield return new(x, y);
            }
    }
}