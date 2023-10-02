using System;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public enum RoomType
{
    Normal = 0,
    Save,
    Shop,
    Boss,
    Joiner,
    Interior,
    Cutscene
}