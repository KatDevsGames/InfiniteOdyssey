using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Randomization;

public class WorldGenerator
{
    private readonly Game m_game;
    private readonly RNG m_rng;
    private readonly World m_world;
    private readonly WorldParameters m_parameters;

    private readonly Region BORDER_REGION = new() { Biome = Biome.Phlogiston };
    private readonly Region BRIDGE_REGION = new() { Biome = Biome.Bridge };

    public WorldGenerator(Game game, WorldParameters parameters) : this(game, parameters.Seed ?? DateTimeOffset.UtcNow.UtcTicks, parameters) { }

    public WorldGenerator(Game game, long seed, WorldParameters parameters)
    {
        m_game = game;
        m_rng = new RNG(seed);
        m_world = new World { Seed = seed };
        parameters.Seed = seed;
        m_parameters = parameters;//todo .Clone();
    }

    public World Generate()
    {
        int rCount = m_parameters.Regions.Length;
        Region[] regions = m_world.Regions = new Region[rCount];
        for (int i = 0; i < rCount; i++)
        {
            RegionParameters rParams = m_parameters.Regions[i] ??= RegionParameters.GetPreset(m_rng, m_parameters);
            regions[i] = new Region
            {
                Seed = m_rng.RandomInt64(),
                Name = rParams.Name ?? $"{Enum.GetName(rParams.Biome.Value)}{Guid.NewGuid().ToString()}",
                Biome = rParams.Biome.Value,
                Parameters = rParams
            };
        }
        
        FillRegions();

        RegionGenerator rGen = new(m_rng.RandomInt64(), m_game, m_parameters, m_world);
        for (int i = 0; i < rCount; i++) rGen.Generate(i);

        //fill in items & bosses
        //verify solvability

        return m_world;
    }

    [ConsumesRNG]
    private void FillRegions()
    {
        var regions = m_world.Regions;
        int width = m_world.Width = m_rng.IRandom(m_parameters.Width);
        int height = m_world.Height = m_rng.IRandom(m_parameters.Height);
        var map = m_world.RegionMap;

        foreach (Region r in regions) // seed the regions wherever
        {
            int x;
            int y;
            do
            {
                x = m_rng.IRandom(0, width - 1);
                y = m_rng.IRandom(0, height - 1);
            } while (map[x][y] != null);
            map[x][y] = r;
            r.SeedLocation = new Point(x, y);
            //RegionSeeds.Add(r, (x, y));
        }

        bool someCell;
        Point? location;
        List<Region> remainingRegions = new(regions);
        do
        {
            someCell = false;
            List<Region> currentRegions = new(remainingRegions);
            foreach (Region r in currentRegions)
            {
                location = FindRandomEdgeCell(r); // look for a cell to grow into
                if (location == null) // no room to grow
                {
                    if (r == regions[0]) { FindRandomEdgeCell(r, true); } // home region can grow into other regions if required
                    else { remainingRegions.Remove(r); }// no room to grow means this region is done
                    continue;
                }
                someCell = true;

                PlaceCell(location.Value, r); // claim this cell for region r
            }
        } while (someCell);

        //fill in holes in the home region regardless of size limits
        location = FindRandomEdgeCell(regions[0], false, true);
        while (location != null)
        {
            PlaceCell(location.Value, regions[0]);
            location = FindRandomEdgeCell(regions[0], false, true);
        }

        //draw the borders
        CellPitch pitch = (CellPitch)m_rng.IRandom(0, 3);
        List<Point> borderCells = GetBorderCells(pitch);

        Dictionary<Point, Region> edges = new();
        while (borderCells.Count > 0)
        {
            (int bX, int bY) = borderCells.TakeRandom(m_rng);
            Region r = map[bX][bY];
            edges.Add(new Point(bX, bY), map[bX][bY]);
            map[bX][bY] = BORDER_REGION;
            borderCells = GetBorderCells(pitch);
        }

        foreach (var edge in edges)
        {
            map[edge.Key.X][edge.Key.Y] = edge.Value;
            if (GetBorderCells(pitch).Count > 0) { map[edge.Key.X][edge.Key.Y] = BORDER_REGION; }
        }

        CloseIsolates();

        //put bridges between the regions
        //var bridges = FindRegionalBridges();
        BuildSetBridges();
        ReclaimBorders();
        //BuildRandomBridges();
    }

    private void ReclaimBorders()
    {
        int width = m_world.Width;
        int height = m_world.Height;
        var map = m_world.RegionMap;

        List<Point> borderCells = GetRegionCells(BORDER_REGION);
        int reclaims;
        do
        {
            reclaims = 0;
            foreach (Point cell in borderCells.Shuffle(m_rng).ToArray())
            {
                Region? claimant = null;
                if ((cell.X - 1) >= 0)
                {
                    Region candidate = (map[cell.X - 1][cell.Y]);
                    if ((candidate != BORDER_REGION) && (claimant != candidate))
                    {
                        if (claimant != null) continue;
                        claimant = candidate;
                    }
                }
                if ((cell.X + 1) < width)
                {
                    Region candidate = (map[cell.X + 1][cell.Y]);
                    if ((candidate != BORDER_REGION) && (claimant != candidate))
                    {
                        if (claimant != null) continue;
                        claimant = candidate;
                    }
                }
                if ((cell.Y - 1) >= 0)
                {
                    Region candidate = (map[cell.X][cell.Y - 1]);
                    if ((candidate != BORDER_REGION) && (claimant != candidate))
                    {
                        if (claimant != null) continue;
                        claimant = candidate;
                    }
                }
                if ((cell.Y + 1) < height)
                {
                    Region candidate = (map[cell.X][cell.Y + 1]);
                    if ((candidate != BORDER_REGION) && (claimant != candidate))
                    {
                        if (claimant != null) continue;
                        claimant = candidate;
                    }
                }

                if (claimant != null)
                {
                    map[cell.X][cell.Y] = claimant;
                    borderCells.Remove(cell);
                    reclaims += 1;
                }
            }
        } while (reclaims > 0);
    }

    private void CloseIsolates()
    {
        int width = m_world.Width;
        int height = m_world.Height;
        var map = m_world.RegionMap;

        foreach (Region region in m_world.Regions)
        {
            List<Point> regionCells = GetRegionCells(region);


            HashSet<Point> visited = new HashSet<Point>();
            FloodAllPoints(region.SeedLocation, regionCells, visited);
            foreach (Point cell in regionCells)
            {
                if (!visited.Contains(cell)) map[cell.X][cell.Y] = BORDER_REGION;
            }

            /*foreach (Point in regionCells)
            {
                bool allBorder = true;
                if ((x - 1) >= 0)
                {
                    allBorder &= BORDER_REGION.Equals(map[x - 1][y]);
                }
                if ((x + 1) < width)
                {
                    allBorder &= BORDER_REGION.Equals(map[x + 1][y]);
                }
                if ((y - 1) >= 0)
                {
                    allBorder &= BORDER_REGION.Equals(map[x][y - 1]);
                }
                if ((y + 1) < height)
                {
                    allBorder &= BORDER_REGION.Equals(map[x][y + 1]);
                }
                if (allBorder) { map[x][y] = BORDER_REGION; }
            }*/
        }
    }

    private enum CellPitch
    {
        Forward = 0,
        Backward = 1,
        Always = 2,
        Never = 3
    }

    private Point? FindRandomEdgeCell(Region region, bool ignoreNeighbors = false, bool ignoreLimits = false)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        var map = m_world.RegionMap;

        List<Point> cells = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (region.Equals(map[x][y])) { cells.Add(new Point(x, y)); }
            }
        }
        if ((!ignoreLimits) && (cells.Count >= (region.Parameters.CellCount?.Maximum ?? int.MaxValue))) { return null; }
        while (cells.Count > 0)
        {
            int index = m_rng.IRandom(0, cells.Count - 1);
            Point location = cells[index];

            if (ignoreNeighbors)
            {
                if (((location.X >= (width - 1)) || (map[location.X + 1][location.Y] == region)) &&
                    ((location.X == 0) || (map[location.X - 1][location.Y] == region)) &&
                    ((location.Y >= (height - 1)) || (map[location.X][location.Y + 1] == region)) &&
                    ((location.Y == 0) || (map[location.X][location.Y - 1] == region)))
                {
                    cells.RemoveAt(index);
                    continue;
                }
            }
            else if (((location.X >= (width - 1)) || (map[location.X + 1][location.Y] != null)) &&
                     ((location.X == 0) || (map[location.X - 1][location.Y] != null)) &&
                     ((location.Y >= (height - 1)) || (map[location.X][location.Y + 1] != null)) &&
                     ((location.Y == 0) || (map[location.X][location.Y - 1] != null)))
            {
                cells.RemoveAt(index);
                continue;
            }
            return location;
        }
        return null;
    }

    [ConsumesRNG(1)]
    private void PlaceCell(Point location, Region region)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        var map = m_world.RegionMap;

        while (true)
        {
            switch (m_rng.IRandom(0, 3))
            {
                case 0:
                    if (location.X >= (width - 1)) { continue; }
                    if (map[location.X + 1][location.Y] == null) { map[location.X + 1][location.Y] = region; break; }
                    continue;
                case 1:
                    if (location.X == 0) { continue; }
                    if (map[location.X - 1][location.Y] == null) { map[location.X - 1][location.Y] = region; break; }
                    continue;
                case 2:
                    if (location.Y >= (height - 1)) { continue; }
                    if (map[location.X][location.Y + 1] == null) { map[location.X][location.Y + 1] = region; break; }
                    continue;
                case 3:
                    if (location.Y == 0) { continue; }
                    if (map[location.X][location.Y - 1] == null) { map[location.X][location.Y - 1] = region; break; }
                    continue;
            }
            break;
        }
    }
    private List<Point> GetBorderCells(CellPitch pitch)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        var map = m_world.RegionMap;

        List<Point> result = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Region cell = map[x][y];
                if (cell == null) { continue; }
                if (cell.Equals(BORDER_REGION)) { continue; }
                if (cell?.SeedLocation == new Point(x, y)) { continue; }

                if ((pitch == CellPitch.Backward) || (pitch == CellPitch.Always))
                {
                    if (!BorderCompare(new(x - 1, y - 1), cell)) { result.Add(new Point(x, y)); continue; }
                    if (!BorderCompare(new(x - 1, y + 1), cell)) { result.Add(new Point(x, y)); continue; }
                }

                if (!BorderCompare(new(x - 1, y), cell)) { result.Add(new Point(x, y)); continue; }

                if (!BorderCompare(new(x, y - 1), cell)) { result.Add(new Point(x, y)); continue; }
                if (!BorderCompare(new(x, y + 1), cell)) { result.Add(new Point(x, y)); continue; }

                if (!BorderCompare(new(x + 1, y), cell)) { result.Add(new Point(x, y)); continue; }

                if ((pitch == CellPitch.Forward) || (pitch == CellPitch.Always))
                {
                    if (!BorderCompare(new(x + 1, y - 1), cell)) { result.Add(new Point(x, y)); continue; }
                    if (!BorderCompare(new(x + 1, y + 1), cell)) { result.Add(new Point(x, y)); continue; }
                }
            }
        }
        return result;
    }

    private bool BorderCompare(Point location, Region r)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        var map = m_world.RegionMap;

        if (location.X < 0) { return true; }
        if (location.X >= width) { return true; }
        if (location.Y < 0) { return true; }
        if (location.Y >= height) { return true; }

        Region target = map[location.X][location.Y];
        if (BORDER_REGION.Equals(target)) { return true; }
        return r.Equals((target ?? r));
    }
    private void BuildSetBridges()
    {
        var map = m_world.RegionMap;

        List<List<(int x, int y, bool horiz)>> bridgeSets = FindRegionalBridges();
        HashSet<(HashSet<Point>, HashSet<Point>)> taken = new();
        List<HashSet<Point>> segments = GetWorldSegments();

        int lastTaken = -1;
        while (lastTaken < taken.Count)
        {
            lastTaken = taken.Count;
            foreach (var bridges in bridgeSets.Shuffle(m_rng))
            {
                loop_redo:
                if (bridges.Count == 0)
                {
                    continue;
                }

                int bridgeIndex = m_rng.IRandom(0, bridges.Count - 1);

                var (x, y, h) = bridges[bridgeIndex];
                var touching = TouchingSegments(new Point(x, y), segments).Distinct();
                var combinations = touching.SelectMany(i => touching, (i, j) => (i, j)).Where(tuple => ((!tuple.i.Equals(tuple.j)) && (tuple.i.GetHashCode() > tuple.j.GetHashCode())));
                foreach (var c in combinations.ToList().Shuffle(m_rng))
                {
                    if (taken.Contains((c.i, c.j)))
                    {
                        bridges.RemoveAt(bridgeIndex);
                        goto loop_redo;
                    }
                }

                foreach (var c in combinations)
                {
                    taken.TryAdd((c.i, c.j));
                    //taken.TryAdd((c.j, c.i));
                }

                map[x][y] = BRIDGE_REGION;
                bridges.RemoveAt(bridgeIndex);
            }
        }
    }

    private List<HashSet<Point>> TouchingSegments(Point location, List<HashSet<Point>>? segments = null)
    {
        segments ??= new List<HashSet<Point>>();
        var (x, y) = location;
        List<HashSet<Point>> result = new();
        foreach (var segment in segments)
        {
            if (segment.Contains(location) || segment.Contains(new Point(x - 1, y)) || segment.Contains(new Point(x + 1, y)) || segment.Contains(new Point(x, y - 1)) || segment.Contains(new Point(x, y + 1)))
            {
                result.Add(segment);
                //continue;
            }
        }

        return result;
    }

    private List<HashSet<Point>> GetWorldSegments()
    {
        List<HashSet<Point>> result = new();

        foreach (var (x, y) in GetNormalCells())
        {
            HashSet<Point> reached = new();
            FloodAllPoints(new Point(x, y), GetNormalCells(), reached);
            if (result.All(set => (!(set?.SetEquals(reached) ?? true)))) { result.Add(reached); }
        }

        return result;
    }

    private bool FloodAllPoints(Point location, List<Point> cells, HashSet<Point>? reached = null)
    {
        int width = m_world.Width;
        int height = m_world.Height;

        var (x, y) = location;

        if (reached == null) { reached = new HashSet<Point>(); }
        if (!reached.Contains(location))
        {
            reached.Add(location);

            if ((x - 1) >= 0)
            {
                if (IsNormal(new Point(x - 1, y)))
                {
                    FloodAllPoints(new Point((x - 1), y), cells, reached);
                }
            }
            if ((x + 1) < width)
            {
                if (IsNormal(new Point(x + 1, y)))
                {
                    FloodAllPoints(new Point((x + 1), y), cells, reached);
                }
            }
            if ((y - 1) >= 0)
            {
                if (IsNormal(new Point(x, y - 1)))
                {
                    FloodAllPoints(new Point(x, (y - 1)), cells, reached);
                }
            }
            if ((y + 1) < height)
            {
                if (IsNormal(new Point(x, y + 1)))
                {
                    FloodAllPoints(new Point(x, (y + 1)), cells, reached);
                }
            }
        }

        return reached.Count >= cells.Count;
    }

    private List<Point> GetNormalCells()
    {
        int width = m_world.Width;
        int height = m_world.Height;

        List<Point> result = new();

        for (int x = 0; x < (width); x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsNormal(new Point(x, y))) result.Add(new Point(x, y));
            }
        }

        return result;
    }

    private List<Point> GetRegionCells(Region region)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        var map = m_world.RegionMap;

        List<Point> result = new();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (region.Equals(map[x][y])) result.Add(new Point(x, y));
            }
        }

        return result;
    }

    private bool IsNormal(Point location)
    {
        var map = m_world.RegionMap;

        var (x, y) = location;
        if (map[x][y] is { } r) return r.Biome != Biome.Phlogiston;

        return (map[x][y]?.Biome ?? Biome.Phlogiston) != Biome.Phlogiston;
    }

    private List<List<(int x, int y, bool horiz)>> FindRegionalBridges()
    {
        var result = new List<List<(int x, int y, bool horiz)>>();

        List<(int x, int y, bool horiz)> candidates = FindBridgeCandidates();
        List<HashSet<Point>> segments = GetWorldSegments();

        foreach (var segment in segments)
        {
            var set = new List<(int x, int y, bool horiz)>();
            foreach (var candidate in candidates)
            {
                var (x, y, h) = candidate;
                if (ContainsAdjacent(segment, new Point(x, y))) { set.Add(candidate); }
            }

            result.Add(set);
        }

        return result;
    }

    public bool ContainsAdjacent(HashSet<Point> set, Point location)
    {
        var (x, y) = location;
        return set.Contains(location) || set.Contains(new Point(x - 1, y)) || set.Contains(new Point(x + 1, y)) || set.Contains(new Point(x, y - 1)) || set.Contains(new Point(x, y + 1));
    }

    private List<(int x, int y, bool horiz)> FindBridgeCandidates()
    {
        int width = m_world.Width;
        int height = m_world.Height;
        var map = m_world.RegionMap;

        List<(int x, int y, bool horiz)> result = new();

        for (int x = 0; x < (width - 2); x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x][y].Equals(null)) { continue; }
                if (!(map[x + 1][y].Equals(BORDER_REGION))) { continue; }
                if (map[x + 2][y].Equals(BORDER_REGION)) { continue; }
                if ((map[x][y] == null) || (map[x + 2][y] == null)) { continue; }
                if ((map[x][y].Equals(BORDER_REGION)) || (map[x + 2][y].Equals(BORDER_REGION))) { continue; }
                if (!map[x][y].Equals(map[x + 2][y]))
                {
                    result.Add((x + 1, y, true));
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < (height - 2); y++)
            {
                if (map[x][y].Equals(BORDER_REGION)) { continue; }
                if (!(map[x][y + 1].Equals(BORDER_REGION))) { continue; }
                if (map[x][y + 2].Equals(BORDER_REGION)) { continue; }
                if ((map[x][y] == null) || (map[x][y + 2] == null)) { continue; }
                if (!map[x][y].Equals(map[x][y + 2]))
                {
                    result.Add((x, y + 1, false));
                }
            }
        }

        return result;
    }
}