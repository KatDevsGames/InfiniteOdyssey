using System.Collections.Generic;

namespace InfiniteOdyssey.Randomization;

public class Solver
{
    public static Room GetFurthest(Floor floor, Room origin)
    {
        Dictionary<Room, int> distances = new();
        distances[origin] = 0;
        return GetFurthest_Impl(floor, origin, distances);
    }

    private static Room GetFurthest_Impl(Floor floor, Room origin, Dictionary<Room, int> distances)
    {
        GetFurthest_Impl(floor, origin, 0, distances);

        Room furthestRoom = null!;
        int furthestDistance = -1;
        foreach (var roomDistance in distances)
        {
            if (roomDistance.Value > furthestDistance)
            {
                furthestDistance = roomDistance.Value;
                furthestRoom = roomDistance.Key;
            }
        }
        return furthestRoom;
    }

    private static void GetFurthest_Impl(Floor floor, Room origin, int distance, Dictionary<Room, int> distances)
    {
        int newDist = distance + 1;
        foreach (Room neighbor in floor.GetNeighbors(origin))
        {
            if (!distances.ContainsKey(neighbor))
            {
                distances[neighbor] = newDist;
                GetFurthest_Impl(floor, neighbor, newDist, distances);
            }
            else if (distances[neighbor] > newDist)
            {
                distances[neighbor] = newDist;
                GetFurthest_Impl(floor, neighbor, newDist, distances);
            }
        }
    }
}