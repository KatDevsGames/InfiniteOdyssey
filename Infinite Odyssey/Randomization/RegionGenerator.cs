using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens.Transitions;

namespace InfiniteOdyssey.Randomization;

public class RegionGenerator
{
    private readonly Game m_game;
    private readonly RNG m_rng;

    private readonly WorldParameters m_parameters;
    private readonly World m_world;

    private readonly RoomBank m_bank;
    private readonly DungeonGenerator m_dungeonGenerator;

    public RegionGenerator(long seed, Game game, WorldParameters parameters, World world)
    {
        m_game = game;
        m_parameters = parameters;
        m_world = world;

        m_rng = new RNG(seed);
        m_bank = new RoomBank(game);

        m_dungeonGenerator = new(game, parameters, world);
    }

    public void Generate(int index)
    {
        if (m_world.Regions.Length <= index) throw new ArgumentOutOfRangeException(nameof(index), "Not a valid region index.");
        Region region = m_world.Regions[index];
        for (int i = 0; i < region.Parameters.Dungeons.Length; i++)
            m_dungeonGenerator.Generate(index, i);

        FillKeyMaps(region);
        FillRemainingMaps(region);

        AttachAllTransitions(region);
        SealTransitions(region);
    }

    private IEnumerable<Point> UnmappedPoints(Region region)
    {
        foreach (Point point in LocalPoints(region))
            if (m_world.RoomMap[point.X][point.Y] == null) yield return point;
    }

    private readonly Dictionary<Region, IReadOnlyList<Point>> m_localPoints = new();
    private IReadOnlyList<Point> LocalPoints(Region region)
    {
        if (m_localPoints.TryGetValue(region, out var result)) return result;

        int ia = region.Bounds.X;
        int iz = ia + region.Bounds.Width;

        int ja = region.Bounds.Y;
        int jz = ja + region.Bounds.Height;

        return m_localPoints[region] = iterator().ToArray();

        IEnumerable<Point> iterator()
        {
            for (int i = ia; i < iz; i++)
                for (int j = ja; j < jz; j++)
                    if (Equals(m_world.RegionMap[i][j])) yield return new Point(i, j);
        }
    }

    /// <remarks>
    /// The point in question may not necessarily be within the region.
    /// </remarks>
    private Point GetCenter(Region region)
    {
        int x = 0;
        int y = 0;
        int count = 0;
        foreach (var point in LocalPoints(region))
        {
            x += point.X;
            y += point.Y;
            count++;
        }
        if (count == 0) throw new ArgumentException("The specified region has no mapped points.", nameof(region));
        return new Point(x / count, y / count);
    }

    private List<(Point location, Transition transition)> GetEntrancePoints(Dungeon dungeon)
    {
        List<(Point location, Transition transition)> points = new();
        int left = int.MaxValue;
        int top = int.MaxValue;
        foreach (Transition entrance in dungeon.Entrances)
        {
            if (entrance.ExitType != ExitType.Dungeon) continue;
            Point loc = entrance.Room.Location;
            points.Add((loc, entrance));
            if (left > loc.X) left = loc.X;
            if (top > loc.Y) top = loc.Y;
        }
        Point offset = new(left, top);
        for (int i = 0; i < points.Count; i++)
            points[i] = (points[i].location - offset, points[i].transition);
        return points;
    }

    private IEnumerable<IEnumerable<(Point location, Transition transition)>> SuggestEntrances(Region region, Dungeon dungeon)
    {
        HashSet<Point> cells = UnmappedPoints(region).ToHashSet();
        var entrancePoints = GetEntrancePoints(dungeon);
        foreach (int x in region.Map.Range)
        {
            var col = region.Map[x];
            foreach (int y in col.Range)
            {
                Point offset = new(x, y);
                (Point location, Transition room)[] offsetCells = entrancePoints.Select(e => (e.location += offset, e.transition)).ToArray();
                if (offsetCells.All(c => cells.Contains(c.location))) yield return offsetCells;
            }
        }
    }

    [ConsumesRNG]
    private void FillKeyMaps(Region region)
    {
        HashSet<RoomTemplate> history = new();
        List<(Dungeon dungeon, RoomTemplate room, TransitionTemplate transition)> toFill = new();
        IList<RoomTemplate> entrancePool = m_bank.GetBy(MapType.Overworld, region.Biome).Where(r => r.Transitions.Values.Any(t=>t.ExitType == ExitType.Dungeon)).ToArray();
        foreach (Dungeon dungeon in region.Dungeons)
        {
            IList<IEnumerable<(Point location, Transition destination)>> potentialEntrances = SuggestEntrances(region, dungeon).ToArray().Shuffle(m_rng);
            float maxScore = float.NegativeInfinity;
            List<(Point location, RoomTemplate entrance, Transition destination)>? bestSuggestion = null;
            foreach (var pE in potentialEntrances)
            {
                float middleMaxScore = float.NegativeInfinity;
                List<(Point location, RoomTemplate entrance, Transition destination)>? innerSuggestion = new();
                foreach (var (location, transition) in pE)
                {
                    float localTotal = 0;
                    entrancePool.Shuffle(m_rng);
                    float localMaxScore = float.NegativeInfinity;
                    RoomTemplate? localBestSuggestion = null;
                    foreach (RoomTemplate suggestion in entrancePool)
                    {
                        float score = ScoreSuggestion(location, suggestion, history);
                        if (score <= 0) continue;
                        if (score > localMaxScore)
                        {
                            if (localMaxScore > 0) localTotal -= localMaxScore;
                            localTotal += score;
                            localMaxScore = score;
                            localBestSuggestion = suggestion;
                        }
                    }
                    if (float.IsNegativeInfinity(localMaxScore)) goto nextCandidate;

                    if (localTotal > middleMaxScore)
                    {
                        middleMaxScore = localTotal;
                        innerSuggestion.Add((location, localBestSuggestion, transition));
                    }
                }

                if (float.IsNegativeInfinity(middleMaxScore)) goto nextCandidate;
                if (middleMaxScore > maxScore)
                {
                    maxScore = middleMaxScore;
                    bestSuggestion = innerSuggestion;
                }

                nextCandidate:;
            }
            if (bestSuggestion == null) throw new Exception($"Unable to satisfy tiling constraints for region {region.Name}. Try decreasing the number of key maps in the pool or increasing the size of the region.");
            foreach (var bSug in bestSuggestion)
            {
                Room room = FillBox(bSug.location, bSug.entrance);
                Transition dest = room.Transitions.Values.First(t => t.ExitType == ExitType.Dungeon);
                dest.Connect(bSug.destination);
            }
        }

        //if we want to add guaranteed towns/shops/whatever, put that logic here
        foreach (var map in toFill)
        {
            var cells = UnmappedPoints(region).ToArray();
            IEnumerable<Rectangle> boxes;
            int i = 0;
            do
            {
                var location = cells.TakeRandom(m_rng);
                boxes = GetFreeBoxes(region, new Rectangle(location, map.room.Size));
                if (i++ >= 64) throw new Exception($"Unable to satisfy tiling constraints for region {region.Name}. Try decreasing the number of key maps in the pool or increasing the size of the region.");
            } while (!boxes.Any());

            var box = boxes.TakeRandom(m_rng);
            FillBox(box.Location, map.room);
        }
    }

    // this needs to also do the other way where it picks blocks first and then places them
    [ConsumesRNG]
    private void FillRemainingMaps(Region region, int searchSize = 32)
    {
        var cells = UnmappedPoints(region);
        List<RoomTemplate> candidates = m_bank.GetBy(MapType.Overworld, region.Biome).ToList();
        HashSet<RoomTemplate> history = new();
        while (cells.Any())
        {
            var location = cells.TakeRandom(m_rng);

            RoomTemplate selection = null;
            float selectionScore = -1;
            Rectangle maxRect = PotentialRectangle(location);
            location = maxRect.Location;
            RoomTemplate[] sizedCandidates = candidates.Where(r => (maxRect.Size.X >= r.Size.X) && (maxRect.Size.Y >= r.Size.Y)).ToArray();
            for (int i = (searchSize - 1); i >= 0; i++)
            {
                if (i == 0) sizedCandidates = sizedCandidates.Where(FindFiller).ToArray();
                RoomTemplate candidate = sizedCandidates.TakeRandom(m_rng);
                float candidateScore = ScoreSuggestion(location, candidate, history);
                if (candidateScore > selectionScore)
                {
                    selection = candidate;
                    selectionScore = candidateScore;
                }
            }
            if (selection == null) throw new Exception("Search exceeded maximum size and no filler map was found.");
            FillBox(location, selection);
            cells = UnmappedPoints(region);
        }
    }

    private bool FindFiller(RoomTemplate toCompare)
    {
        if (toCompare.Size != new Point(1, 1)) return false;
        Direction4 exits = 0;
        foreach (TransitionTemplate t in toCompare.Transitions.Values)
        {
            if (t.ExitType != ExitType.Open) return false;
            Direction4 thisDir = t.Direction;
            if (exits.HasFlag(thisDir)) return false;
            exits &= thisDir;
        }
        return exits == (Direction4.North | Direction4.South | Direction4.East | Direction4.West);
    }

    /// <remarks>
    /// we could do a lot more here, this serves up basic box candidates but
    /// it favors strictly biggest boxes (long) and not necessarily optimal ones
    /// it also only goes down-right instead of all potential directions
    /// todo: improve
    /// </remarks>
    private Rectangle PotentialRectangle(Point contains)
    {
        Rectangle result = new();
        var map = m_world.RoomMap;
        int limX = m_world.Width - contains.X;
        int limY = m_world.Height - contains.Y;
        for (int x = contains.X; x < limX; x++)
        for (int y = contains.Y; y < limY; y++)
        {
            int width = x - contains.X;
            int height = y - contains.Y;
            Rectangle candidate = new(contains.X, contains.Y, width, height);

            float currentSize = result.Size.ToVector2().Length();
            float candidatetSize = candidate.Size.ToVector2().Length();
            if (candidatetSize > currentSize) result = candidate;
        }
        return result;
    }

    private float ScoreSuggestion(Point location, RoomTemplate template, HashSet<RoomTemplate> history)
    {
        var rooms = m_world.RoomMap;

        int ax = location.X;
        int zx = ax + template.Size.X;
        int ay = location.Y;
        int zy = ax + template.Size.Y;

        for (int x = ax; x < zx; x++)
        {
            for (int y = ay; y < zy; y++)
            {
                if (rooms[x][y] != null) return -1;
            }
        }

        int i = 0;
        float score = 0;
        foreach (Room room in SurroundingRooms(location, template))
        {
            switch (template.IsExitMatch(room))
            {
                case RoomTemplate.ExitMatch.Partial:
                    score += 0.5f;
                    break;
                case RoomTemplate.ExitMatch.Full:
                    score += 1f;
                    break;
                default:
                    return -1;
            }
            i++;
        }
        if (i == 0) return 0.9f;
        score /= i;
        if (history.Contains(template)) score *= 0.7f;
        return score;
    }

    private IEnumerable<Room> SurroundingRooms(Room room)
        => SurroundingRooms(room.Location, room.Template);

    private IEnumerable<Room> SurroundingRooms(Point location, RoomTemplate template)
    {
        return allCells().Distinct();
        IEnumerable<Room> allCells()
        {
            Rectangle toCheck = new(location - new Point(1, 1), template.Size + new Point(2, 2));
            if (toCheck.Top >= 0)
            {
                int lim = Math.Min(toCheck.Right, m_world.Width - 1) - 1;
                for (int i = toCheck.Left + 1; i < lim; i++)
                {
                    Room room = m_world.RoomMap[i][toCheck.Top];
                    if (room == null) continue;
                    yield return room;
                }
            }
            if (toCheck.Left >= 0)
            {
                int lim = Math.Min(toCheck.Bottom, m_world.Height - 1) - 1;
                for (int i = toCheck.Top + 1; i < lim; i++)
                {
                    Room room = m_world.RoomMap[toCheck.Left][i];
                    if (room == null) continue;
                    yield return room;
                }
            }
            if (toCheck.Bottom < m_world.Height)
            {
                int lim = Math.Min(toCheck.Right, m_world.Width - 1) - 1;
                for (int i = toCheck.Left + 1; i < lim; i++)
                {
                    Room room = m_world.RoomMap[i][toCheck.Bottom];
                    if (room == null) continue;
                    yield return room;
                }
            }
            if (toCheck.Right < m_world.Width)
            {
                int lim = Math.Min(toCheck.Bottom, m_world.Height - 1) - 1;
                for (int i = toCheck.Top + 1; i < lim; i++)
                {
                    Room room = m_world.RoomMap[toCheck.Right][i];
                    if (room == null) continue;
                    yield return room;
                }
            }
        }
    }

    private void AttachAllTransitions(Region region)
    {
        foreach (Room room in region.Map.Values)
        foreach (Room neighbor in SurroundingRooms(room))
        foreach ((Transition t1, Transition t2) in GetUnboundMutualTransitions(room, neighbor))
            if ((t1.State == TransitionState.Unbound) && (t2.State == TransitionState.Unbound)) t1.Connect(t2);
    }

    private void SealTransitions(Region region)
    {
        foreach (Room room in region.Map.Values)
        foreach (Transition transition in room.Transitions.Values)
        {
            if (transition.State == TransitionState.Unbound)
                transition.State = TransitionState.Sealed;
        }
    }

    private IEnumerable<(Transition t1, Transition t2)> GetUnboundMutualTransitions(Room r1, Room r2)
    {
        var remnoteTransitions = r2.Transitions.Values;
        foreach (Transition tLocal in r1.Transitions.Values)
        {
            TransitionTemplate localTemplate = tLocal.Template;
            if (tLocal.ExitType != ExitType.Open) continue;
            Point requiredLocation = r1.Location + localTemplate.Location + localTemplate.Direction.GetPoint();
            foreach (Transition tRemote in remnoteTransitions)
            {
                if (tRemote.ExitType != ExitType.Open) continue;
                TransitionTemplate remoteTemplate = tRemote.Template;
                if (localTemplate.Direction != remoteTemplate.Direction.GetOpposite()) continue;
                if ((r2.Location + remoteTemplate.Location) != requiredLocation) continue;
                yield return (tLocal, tRemote);
            }
        }
    }

    private IEnumerable<Rectangle> GetFreeBoxes(Region region, Rectangle rectangle)
    {
        for (int nX = rectangle.X - (rectangle.Width - 1); nX <= rectangle.X; nX++)
        {
            if (nX < 0) { continue; }
            for (int nY = rectangle.Y - (rectangle.Height - 1); nY <= rectangle.Y; nY++)
            {
                if (nY < 0) { continue; }
                for (int iX = nX; iX < (nX + rectangle.Width); iX++)
                {
                    if ((iX + (rectangle.Width - 1)) >= m_world.Width) { goto continue_outer; }
                    for (int iY = nY; iY < (nY + rectangle.Height); iY++)
                    {
                        if ((iY + (rectangle.Height - 1)) >= m_world.Height) { goto continue_outer; }
                        if (m_world.RegionMap[iX][iY] != region) { goto continue_outer; }
                        if (m_world.RoomMap[iX][iY] != null) { goto continue_outer; }
                    }
                }
                yield return new Rectangle(nX, nY, rectangle.Width, rectangle.Height);
            continue_outer:;
            }
        }
    }

    private Room FillBox(Point location, RoomTemplate room)
    {
        Room fill = new(room) { Location = new Point(location.X, location.Y) };

        for (int i = 0; i < room.Size.X; i++)
        for (int j = 0; j < room.Size.Y; j++)
            m_world.RoomMap[location.X + i][location.Y + j] = fill;

        return fill;
    }
}