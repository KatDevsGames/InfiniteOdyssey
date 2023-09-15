using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

public abstract class MapContainer
{
    [JsonProperty(PropertyName = "map")]
    public Room[][] Map;

    [JsonIgnore]
    public int Width;

    [JsonIgnore]
    public int Height;

    [OnDeserialized]
    public void OnDeserialize(StreamingContext context) => Populate();

    public void Populate()
    {
        int maxY = 0;
        int lastWidth = 0;
        int lastHeight = 0;
        int lenRow = Map.Length;
        int lastRowCell = lenRow - 1;
        for (int x = 0; x < lenRow; x++)
        {
            Room[] column = Map[x];
            int lenCol = column.Length;
            int lastColCell = lenCol - 1;
            for (int y = 0; y < lenCol; y++)
            {
                Room cell = column[y];
                cell.X = x;
                cell.Y = y;
                if (x == lastRowCell) lastWidth = Math.Max(lastWidth, cell.Width);
                if (y == lastColCell) lastHeight = Math.Max(lastHeight, cell.Height);
            }
            maxY = Math.Max(maxY, lenCol);
        }
        Width = Map.Length + (lastWidth - 1);
        Height = Map[0].Length + (lastHeight - 1);
    }
}