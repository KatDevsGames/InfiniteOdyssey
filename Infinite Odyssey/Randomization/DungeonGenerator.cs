using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Randomization;

public class DungeonGenerator
{
    private readonly Game m_game;

    private readonly WorldParameters m_parameters;
    private readonly World m_world;

    private readonly RoomBank m_bank;
    private DungeonParameters? m_dungeonParameters;

    public DungeonGenerator(Game game, WorldParameters parameters, World world)
    {
        m_game = game;
        m_parameters = parameters;
        m_world = world;
        
        m_bank = new RoomBank(game);
    }
    [ConsumesRNG]
    public void Generate(int regionIndex, int dungeonIndex)
    {
        if (m_world.Regions.Length <= regionIndex) throw new ArgumentOutOfRangeException(nameof(regionIndex), "Not a valid region index.");
        Region region = m_world.Regions[regionIndex];
        Dungeon dungeon = region.Dungeons[dungeonIndex];
        m_dungeonParameters = region.Parameters.Dungeons?[dungeonIndex];
        RNG rng = new(dungeon.Seed);

        List<RoomTemplate> roomSet = m_bank.GetBy(MapType.Dungeon, dungeon.Biome).ToList();
        Floor[] floors = dungeon.Floors = new Floor[rng.IRandom(m_dungeonParameters.FloorCount)];
        int numFloors = floors.Length;
        BossPlacement bossPlacement = m_dungeonParameters.BossPlacement ?? EnumEx<BossPlacement>.TakeRandomValue(rng);
        int bossFloor;
        if (numFloors <= 2) bossFloor = numFloors - 1;
        else bossFloor = bossPlacement switch
        {
            BossPlacement.BossAtEnd => numFloors - 1,
            BossPlacement.BossKeyAtEnd => rng.IRandom(1..(numFloors - 1)),
            _ => numFloors - 1
        };
        for (int i = 0; i < numFloors; i++)
        {
            int floorTarget = rng.IRandom(m_dungeonParameters.RoomCount / numFloors);
            int entrances;
            if (i == 0)
            {
                entrances = floorTarget switch
                {
                    < 9 => 1,
                    >= 9 and < 14 => rng.IRandom(1..2, 0.5),
                    >= 14 and < 25 => rng.IRandom(1..2),
                    >= 25 => rng.IRandom(1..3, 0.9)
                };
            }
            else entrances = 0;
            GenerateFloor(rng, dungeon, roomSet, i, floorTarget, entrances, i == bossFloor);
        }
        SealTransitions(dungeon);
    }

    private bool GenerateFloor(RNG rng, Dungeon dungeon, IList<RoomTemplate> roomSet, int floorIndex, int targetRooms, int entrances, bool boss)
    {
        Floor floor = dungeon.Floors[floorIndex] = new Floor();
        var map = floor.Map;

        int totalRooms = boss ? 1 : 0;

        //if this isn't the main floor, we need to work with the existing staircases, so place those connections first
        if (floorIndex > 0)
        {
            Floor previousFloor = dungeon.Floors[floorIndex - 1] = new Floor();
            IList<Transition> staircases = previousFloor.Rooms.Values.SelectMany(r => r.Transitions.Values).ToArray().Shuffle(rng);
            TryPlaceStaircases(rng, map, roomSet, staircases);
        }

        //once we've met the requirements of existing geometry, place entrances
        RoomTemplate[]? entranceRooms = null;
        if (entrances-- > 0)
        {
            entranceRooms ??= roomSet.Where(r => r.Transitions.Values.Any(t => t.ExitType == ExitType.Dungeon)).ToArray();
            TryPlaceEntrance(rng, map, entranceRooms);
            totalRooms++;
        }

        //place the main bulk of the rooms
        targetRooms -= entrances;
        while (totalRooms++ <= targetRooms)
        {
            if (!TryPlaceRoom(rng, map, roomSet)) return false;
        }

        //place the other entrances
        while (entrances-- > 0)
        {
            entranceRooms ??= roomSet.Where(r => r.Transitions.Values.Any(t => t.ExitType == ExitType.Dungeon)).ToArray();
            if(!TryPlaceRoom(rng, map, entranceRooms)) return false;
            //totalRooms++;
        }

        //finally, we put in the boss room
        if (boss)
        {
            RoomTemplate[] bossRooms = roomSet.Where(r => r.RoomType == RoomType.Boss).ToArray();
            if(!TryPlaceRoom(rng, map, bossRooms)) return false;
            //totalRooms++;
        }
        map.Trim();
        return true;
    }

    [ConsumesRNG]
    private void TryPlaceStaircases(RNG rng, Map2D<Room> map, IList<RoomTemplate> roomSet, IList<Transition> transitions)
    {
        var candidates = roomSet
            .Where(r => r.Transitions.Values.Any(t => t.ExitType is (ExitType.Door or ExitType.Staircase)))
            .ToArray();
        foreach (Transition transition in transitions)
        {
            candidates.Shuffle(rng);
            foreach (RoomTemplate room in candidates)
            {
                var destTransitions = room.Transitions.Values
                    .Where(t => t.ExitType is (ExitType.Door or ExitType.Staircase))
                    .ToArray()
                    .Shuffle(rng);
                foreach (TransitionTemplate destTransition in destTransitions)
                {
                    if (TryConnect(rng, map, transition, destTransition)) goto nextTransition;
                }
            }
            throw new ArgumentException("Could not satisfy transition requirements.", nameof(map));
            nextTransition:;
        }
    }

    private void TryPlaceEntrance(RNG rng, Map2D<Room> map, IList<RoomTemplate> roomSet)
    {
        if (roomSet.Count == 0) throw new ArgumentException("The supplied room set was empty.", nameof(roomSet));

        var rooms = map.Values.ToArray();
        if (rooms.Length != 0)
            if (roomSet.Count == 0) throw new ArgumentException("The supplied map was not empty.", nameof(map));
        PlaceAt(rng, map, roomSet.TakeRandom(rng), Point.Zero);
    }


    [ConsumesRNG]
    private bool TryPlaceRoom(RNG rng, Map2D<Room> map, IList<RoomTemplate> roomSet)
    {
        if (roomSet.Count == 0) throw new ArgumentException("The supplied room set was empty.", nameof(roomSet));

        IList<Transition> transitions = FindUnboundTransitions(map).ToArray().Shuffle(rng);
        if (transitions.Count == 0) throw new ArgumentException("Cannot place room because the supplied map contained no unbound transitions to grow from. Place an entrance room first.", nameof(map));
        foreach (var transition in transitions)
        {
            if (transition.ExitType is not (ExitType.Door or ExitType.Open)) continue;
            var rooms = roomSet.Where(r => r.Transitions.Values.Any(transition.Template.IsMatch)).ToArray().Shuffle(rng);
            foreach (var room in rooms)
            {
                var otherTransitions = room.Transitions.Values.Where(transition.Template.IsMatch).ToArray().Shuffle(rng);
                foreach (var otherTransition in otherTransitions)
                {
                    if (TryConnect(rng, map, transition, otherTransition)) return true;
                }
            }
        }

        return false;
    }

    [ConsumesRNG]
    private Room PlaceAt(RNG rng, Map2D<Room> map, RoomTemplate room, Point location)
    {
        Room r = new(room)
        {
            Location = location,
            Seed = rng.RandomInt64()
        };

        for (int x = 0; x < room.Size.X; x++)
        for (int y = 0; y < room.Size.Y; y++)
            map[x, y] = r;

        return r;
    }

    private IEnumerable<Transition> FindUnboundTransitions(Map2D<Room> map)
    {
        foreach (Room room in map.Values)
        foreach (Transition transition in room.Transitions.Values)
        {
            if (transition.State != TransitionState.Unbound) continue;
            yield return transition;
        }
    }

    private bool TryConnect(RNG rng, Map2D<Room> map, Transition baseTransition, TransitionTemplate newTransition)
    {
        //we don't do validation here because the only function that calls this doesn't pass bad data - kat
        //if (!baseTransition.Template.IsMatch(newTransition)) return false;
        
        Room baseRoom = baseTransition.Room;
        RoomTemplate newRoomTemplate = m_bank[newTransition.RoomTemplate];
        Point baseLoc = baseRoom.Location + baseTransition.Template.Location;
        Point newLoc = baseLoc;
        if (baseTransition.ExitType != ExitType.Staircase) newLoc += baseTransition.Template.Direction.GetPoint();
        newLoc -= newTransition.Location;
        if (!map.IsEmpty(new Rectangle(newLoc, newRoomTemplate.Size))) return false;

        Room newRoom = PlaceAt(rng, map, newRoomTemplate, newLoc);
        baseTransition.Connect(newRoom.Transitions[newTransition.Name]);
        return true;
    }

    private void SealTransitions(Dungeon dungeon)
    {
        foreach (Floor floor in dungeon.Floors)
        foreach (Room room in floor.Map.Values)
        foreach (Transition transition in room.Transitions.Values)
        {
            if (transition.State == TransitionState.Unbound)
                transition.State = TransitionState.Sealed;
        }
    }
}