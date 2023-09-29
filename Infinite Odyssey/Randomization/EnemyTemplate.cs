using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Randomization;

public class EnemyTemplate
{
    public string Name;
    public Rectangle Location;
    public Range Level;
    public int Pattern;
    public float Probability = 1f;

    public EnemyTemplate(string name)
    {
        Name = name;
    }
}