using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public abstract class MapContainer
{
    [JsonProperty(PropertyName = "map")]
    public List<List<Room?>> Map;

    [JsonIgnore]
    public Dictionary<Guid, Room> m_byName = new();

    [JsonIgnore]
    public Rectangle Bounds;

    public bool TryFindRoom(Guid id, out Room room) => m_byName.TryGetValue(id, out room);

    [OnDeserialized]
    public void OnDeserialize(StreamingContext context) => Populate();

    public void Populate()
    {
        int maxY = 0;
        int lastWidth = 0;
        int lastHeight = 0;
        int lenRow = Map.Count;
        int lastRowCell = lenRow - 1;
        for (int x = 0; x < lenRow; x++)
        {
            List<Room> column = Map[x];
            int lenCol = column.Count;
            int lastColCell = lenCol - 1;
            for (int y = 0; y < lenCol; y++)
            {
                Room cell = column[y];
                m_byName[cell.ID] = cell;
                cell.Location.X = x;
                cell.Location.Y = y;
                if (x == lastRowCell) lastWidth = Math.Max(lastWidth, cell.Bounds.Width);
                if (y == lastColCell) lastHeight = Math.Max(lastHeight, cell.Bounds.Height);
            }
            maxY = Math.Max(maxY, lenCol);
        }
        Bounds.Width = Map.Count + (lastWidth - 1);
        Bounds.Height = Map[0].Count + (lastHeight - 1);
    }
}