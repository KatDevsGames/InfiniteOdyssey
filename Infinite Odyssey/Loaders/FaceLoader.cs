namespace InfiniteOdyssey.Loaders;

public static class FaceLoader
{
    private const int MAX_EXPRESSIONS = 15;

    /*public static Sprite LoadSprite(string name, Expression expression, int index = 0)
    {
        string path = $"Images/Faces/{name}";
        Sprite[] set = Resources.LoadAll<Sprite>(path);
        int i = ((int) expression) + (index * MAX_EXPRESSIONS);
        return (set?.Length > i) ? set[i] : LoadSprite("Fallback", expression);
    }*/

    public enum Expression
    {
        Neutral = 0,
        Happy = 1,
        Sad = 2,
        Angry = 3,
        Injured = 4,
        Optimistic = 5,
        Flirty = 6,
        Surprised = 7,
        Smug = 8,
        Embarrassed = 9,
        Distant = 10,
        Charged = 11,
        Curious = 12,
        Stressed = 13,
        Insane = 14
    }
}