using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Extensions;

namespace InfiniteOdyssey.Randomization;

public class WorldGenerator
{
    private readonly RNG m_rng;
    private readonly World m_world;
    private readonly WorldParameters m_parameters;

    private readonly Region BORDER_REGION = new() { Biome = Biome.Phlogiston };
    private readonly Region BRIDGE_REGION = new() { Biome = Biome.Bridge };

    public WorldGenerator(WorldParameters parameters) : this(DateTimeOffset.UtcNow.UtcTicks, parameters) { }

    public WorldGenerator(long seed, WorldParameters parameters)
    {
        m_rng = new RNG(seed);
        m_world = new World { Seed = seed };
        m_parameters = parameters;
    }

    public World Generate()
    {
        int rCount = m_parameters.Regions.Length;
        Region[] regions = m_world.Regions = new Region[rCount];
        for (int i = 0; i < rCount; i++)
        {
            RegionParameters? rParams = m_parameters.Regions[i] ??= SuggestRegion(m_rng, m_parameters);
            regions[i] = new Region { Seed = m_rng.RandomInt64(), Biome = rParams.Biome.Value, Parameters = rParams };
        }
        
        FillRegions();
        for (int i = 0; i < rCount; i++) RegionGenerator.Generate(i, m_parameters, m_world);

        //fill in items & bosses
        //verify solvability

        return m_world;
    }

    [ConsumesRNG]
    private void FillRegions()
    {
        var regions = m_world.Regions;
        int width = m_world.Width = m_rng.IRandom(m_parameters.MinWidth, m_parameters.MaxWidth);
        int height = m_world.Height = m_rng.IRandom(m_parameters.MinHeight, m_parameters.MaxHeight);
        Region?[][] map = m_world.RegionMap = Region.GetEmptyMap(width, height);

        foreach (Region r in regions) // seed the regions wherever
        {
            int x;
            int y;
            do
            {
                x = m_rng.IRandom(0, width);
                y = m_rng.IRandom(0, height);
            } while (map[x][y] != null);
            map[x][y] = r;
            r.SeedLocation = (x, y);
            //RegionSeeds.Add(r, (x, y));
        }

        bool someCell;
        (int x, int y)? location;
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
                    else { remainingRegions.Remove(r); }// no rom to grow means this region is done
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
        List<(int x, int y)> borderCells = GetBorderCells(pitch);

        Dictionary<(int x, int y), Region> edges = new();
        while (borderCells.Count > 0)
        {
            (int bX, int bY) = borderCells.TakeRandom(m_rng);
            Region r = map[bX][bY];
            edges.Add((bX, bY), map[bX][bY]);
            map[bX][bY] = BORDER_REGION;
            borderCells = GetBorderCells(pitch);
        }

        foreach (var edge in edges)
        {
            map[edge.Key.x][edge.Key.y] = edge.Value;
            if (GetBorderCells(pitch).Count > 0) { map[edge.Key.x][edge.Key.y] = BORDER_REGION; }
        }

        //close isolates
        /*foreach (Region region in Regions)
        {
            foreach ((int x, int y) in region.GetLocalPoints())
            {
                /*bool allBorder = true;
                if ((x - 1) >= 0)
                {
                    allBorder &= _border.Equals(this[x - 1, y]);
                }
                if ((x + 1) < Width)
                {
                    allBorder &= _border.Equals(this[x + 1, y]);
                }
                if ((y - 1) >= 0)
                {
                    allBorder &= _border.Equals(this[x, y - 1]);
                }
                if ((y + 1) < Height)
                {
                    allBorder &= _border.Equals(this[x, y + 1]);
                }
                if (allBorder) { this[x, y] = _border; }*//*
                    HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
                    FloodAllPoints(region.SeedLocation, region.GetLocalPoints(), visited);
                    if(!visited.Contains((x,y))) { this[x, y] = _border; }
                }
            }*/

        //put bridges between the regions
        //var bridges = FindRegionalBridges();
        BuildSetBridges();
        //BuildRandomBridges();
    }

    private enum CellPitch
    {
        Forward = 0,
        Backward = 1,
        Always = 2,
        Never = 3
    }

    private (int x, int y)? FindRandomEdgeCell(Region region, bool ignoreNeighbors = false, bool ignoreLimits = false)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        Region?[][] map = m_world.RegionMap;

        List<(int x, int y)> cells = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (region.Equals(map[x][y])) { cells.Add((x, y)); }
            }
        }
        if ((!ignoreLimits) && (cells.Count >= region.Parameters.MaxCells)) { return null; }
        while (cells.Count > 0)
        {
            int index = m_rng.IRandom(0, cells.Count);
            (int x, int y) location = cells[index];

            if (ignoreNeighbors)
            {
                if (((location.x >= (width - 1)) || (map[location.x + 1][location.y] == region)) &&
                    ((location.x == 0) || (map[location.x - 1][location.y] == region)) &&
                    ((location.y >= (height - 1)) || (map[location.x][location.y + 1] == region)) &&
                    ((location.y == 0) || (map[location.x][location.y - 1] == region)))
                {
                    cells.RemoveAt(index);
                    continue;
                }
            }
            else if (((location.x >= (width - 1)) || (map[location.x + 1][location.y] != null)) &&
                     ((location.x == 0) || (map[location.x - 1][location.y] != null)) &&
                     ((location.y >= (height - 1)) || (map[location.x][location.y + 1] != null)) &&
                     ((location.y == 0) || (map[location.x][location.y - 1] != null)))
            {
                cells.RemoveAt(index);
                continue;
            }
            return location;
        }
        return null;
    }

    [ConsumesRNG(1)]
    private void PlaceCell((long x, long y) location, Region region)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        Region?[][] map = m_world.RegionMap;

        while (true)
        {
            switch (m_rng.IRandom(0, 3))
            {
                case 0:
                    if (location.x >= (width - 1)) { continue; }
                    if (map[location.x + 1][location.y] == null) { map[location.x + 1][location.y] = region; break; }
                    continue;
                case 1:
                    if (location.x == 0) { continue; }
                    if (map[location.x - 1][location.y] == null) { map[location.x - 1][location.y] = region; break; }
                    continue;
                case 2:
                    if (location.y >= (height - 1)) { continue; }
                    if (map[location.x][location.y + 1] == null) { map[location.x][location.y + 1] = region; break; }
                    continue;
                case 3:
                    if (location.y == 0) { continue; }
                    if (map[location.x][location.y - 1] == null) { map[location.x][location.y - 1] = region; break; }
                    continue;
            }
            break;
        }
    }
    private List<(int x, int y)> GetBorderCells(CellPitch pitch)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        Region?[][] map = m_world.RegionMap;

        List<(int x, int y)> result = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Region cell = map[x][y];
                if (cell == null) { continue; }
                if (cell.Equals(BORDER_REGION)) { continue; }
                if (cell?.SeedLocation == (x, y)) { continue; }

                if ((pitch == CellPitch.Backward) || (pitch == CellPitch.Always))
                {
                    if (!BorderCompare((x - 1, y - 1), cell)) { result.Add((x, y)); continue; }
                    if (!BorderCompare((x - 1, y + 1), cell)) { result.Add((x, y)); continue; }
                }

                if (!BorderCompare((x - 1, y), cell)) { result.Add((x, y)); continue; }

                if (!BorderCompare((x, y - 1), cell)) { result.Add((x, y)); continue; }
                if (!BorderCompare((x, y + 1), cell)) { result.Add((x, y)); continue; }

                if (!BorderCompare((x + 1, y), cell)) { result.Add((x, y)); continue; }

                if ((pitch == CellPitch.Forward) || (pitch == CellPitch.Always))
                {
                    if (!BorderCompare((x + 1, y - 1), cell)) { result.Add((x, y)); continue; }
                    if (!BorderCompare((x + 1, y + 1), cell)) { result.Add((x, y)); continue; }
                }
            }
        }
        return result;
    }

    private bool BorderCompare((long x, long y) location, Region r)
    {
        int width = m_world.Width;
        int height = m_world.Height;
        Region?[][] map = m_world.RegionMap;

        if (location.x < 0) { return true; }
        if (location.x >= width) { return true; }
        if (location.y < 0) { return true; }
        if (location.y >= height) { return true; }

        Region target = map[location.x][location.y];
        if (BORDER_REGION.Equals(target)) { return true; }
        return r.Equals((target ?? r));
    }
    private void BuildSetBridges()
    {
        Region?[][] map = m_world.RegionMap;

        List<List<(int x, int y, bool horiz)>> bridgeSets = FindRegionalBridges();
        HashSet<(HashSet<(int x, int y)>, HashSet<(int x, int y)>)> taken = new();
        List<HashSet<(int x, int y)>> segments = GetWorldSegments();

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

                int bridgeIndex = m_rng.IRandom(0, bridges.Count);

                var (x, y, h) = bridges[bridgeIndex];
                var touching = TouchingSegments((x, y), segments).Distinct();
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

                map[x][y] = BORDER_REGION;
                bridges.RemoveAt(bridgeIndex);
            }
        }
    }

    public List<HashSet<(int x, int y)>> TouchingSegments((int x, int y) location, List<HashSet<(int x, int y)>>? segments = null)
    {
        segments ??= new List<HashSet<(int x, int y)>>();
        var (x, y) = location;
        List<HashSet<(int x, int y)>> result = new();
        foreach (var segment in segments)
        {
            if (segment.Contains(location) || segment.Contains((x - 1, y)) || segment.Contains((x + 1, y)) || segment.Contains((x, y - 1)) || segment.Contains((x, y + 1)))
            {
                result.Add(segment);
                //continue;
            }
        }

        return result;
    }

    private List<HashSet<(int x, int y)>> GetWorldSegments()
    {
        List<HashSet<(int x, int y)>> result = new();

        foreach (var (x, y) in GetNormalCells())
        {
            HashSet<(int x, int y)> reached = new();
            FloodAllPoints((x, y), GetNormalCells(), reached);
            if (result.All(set => (!(set?.SetEquals(reached) ?? true)))) { result.Add(reached); }
        }

        return result;
    }

    private bool FloodAllPoints((int x, int y) location, List<(int x, int y)> cells, HashSet<(int x, int y)>? reached = null)
    {
        int width = m_world.Width;
        int height = m_world.Height;

        var (x, y) = location;

        if (reached == null) { reached = new HashSet<(int x, int y)>(); }
        if (!reached.Contains(location))
        {
            reached.Add(location);

            if ((x - 1) >= 0)
            {
                if (IsNormal((x - 1, y)))
                {
                    FloodAllPoints(((x - 1), y), cells, reached);
                }
            }
            if ((x + 1) < width)
            {
                if (IsNormal((x + 1, y)))
                {
                    FloodAllPoints(((x + 1), y), cells, reached);
                }
            }
            if ((y - 1) >= 0)
            {
                if (IsNormal((x, y - 1)))
                {
                    FloodAllPoints((x, (y - 1)), cells, reached);
                }
            }
            if ((y + 1) < height)
            {
                if (IsNormal((x, y + 1)))
                {
                    FloodAllPoints((x, (y + 1)), cells, reached);
                }
            }
        }

        return reached.Count >= cells.Count;
    }

    private List<(int x, int y)> GetNormalCells()
    {
        int width = m_world.Width;
        int height = m_world.Height;

        List<(int x, int y)> result = new();

        for (int x = 0; x < (width); x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsNormal((x, y))) { result.Add((x, y)); }
            }
        }

        return result;
    }

    private bool IsNormal((int x, int y) location)
    {
        Region?[][] map = m_world.RegionMap;

        var (x, y) = location;
        if (map[x][y] is Region) { return true; }

        return (map[x][y]?.Biome ?? Biome.Phlogiston) != Biome.Phlogiston;
    }

    private List<List<(int x, int y, bool horiz)>> FindRegionalBridges()
    {
        var result = new List<List<(int x, int y, bool horiz)>>();

        List<(int x, int y, bool horiz)> candidates = FindBridgeCandidates();
        List<HashSet<(int x, int y)>> segments = GetWorldSegments();

        foreach (var segment in segments)
        {
            var set = new List<(int x, int y, bool horiz)>();
            foreach (var candidate in candidates)
            {
                var (x, y, h) = candidate;
                if (ContainsAdjacent(segment, (x, y))) { set.Add(candidate); }
            }

            result.Add(set);
        }

        return result;
    }

    public bool ContainsAdjacent(HashSet<(int x, int y)> set, (int x, int y) location)
    {
        var (x, y) = location;
        return set.Contains(location) || set.Contains((x - 1, y)) || set.Contains((x + 1, y)) || set.Contains((x, y - 1)) || set.Contains((x, y + 1));
    }

    private List<(int x, int y, bool horiz)> FindBridgeCandidates()
    {
        int width = m_world.Width;
        int height = m_world.Height;
        Region?[][] map = m_world.RegionMap;

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

    private static RegionParameters SuggestRegion(RNG rng, WorldParameters parameters)
    {
        RegionParameters result = new();
        result.Biome ??= SuggestBiome(rng, parameters);
        return result;
    }

    private static Biome SuggestBiome(RNG rng, WorldParameters parameters)
    {
        switch (parameters.BiomeDistribution)
        {
            case BiomeDistribution.RandomNoRepeats:
                Biome remainingBiomes = Biome.Normal;
                foreach (RegionParameters? region in parameters.Regions)
                {
                    if (region?.Biome != null) remainingBiomes &= (~region.Biome.Value);
                }
                if (remainingBiomes == 0) goto case BiomeDistribution.RandomAllowRepeats;
                return (Biome)((uint)remainingBiomes).GetRandomBit(rng);
            case BiomeDistribution.RandomAllowRepeats:
                return (Biome)((uint)Biome.Normal).GetRandomBit(rng);
            default:
                throw new ArgumentOutOfRangeException(nameof(parameters.BiomeDistribution), parameters.BiomeDistribution, null);
        }
    }
}