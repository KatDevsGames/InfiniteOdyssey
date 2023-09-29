using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
public static class ItemEx
{
    private const Item TYPE_MASK = unchecked((Item)0xFFFF_0000);
    private const Item VALUE_MASK = unchecked((Item)0x0000_FFFF);

    public static bool IsItemType(this Item item, ItemType type) => (int)(item & TYPE_MASK) == (int)type;
    public static ItemType GetItemType(this Item item) => (ItemType)(item & TYPE_MASK);

    public static int GetValue(this Item item) => (int)(item & VALUE_MASK);
}

[Serializable]
public enum ItemType
{
    Nothing = 0,
    Money = 0x0001_0000,
    Health = 0x0002_0000,
    MaxHealth = 0x0003_0000,
    Magic = 0x0004_0000,
    Armor = 0x0005_0000,
    Shield = 0x0006_0000,
    Sword = 0x0007_0000,
    Bomb = 0x0008_0000,
    MaxBombs = 0x0009_0000,
    Arrow = 0x000A_0000,
    MaxArrows = 0x000B_0000,
    MajorItem = 0x000C_0000,
    Potion = 0x000D_0000,
    Trap = 0x000E_0000,
    Key = 0x000F_0000,
    TreasureKey = 0x0010_0000,
    BossKey = 0x0011_0000,
    Rune = 0x0012_0000
}

[Serializable, JsonConverter(typeof(ItemConverter))]
public enum Item
{
    Nothing = ItemType.Nothing,

    Money1 = ItemType.Money + 1,
    Money5 = ItemType.Money + 5,
    Money10 = ItemType.Money + 10,
    Money20 = ItemType.Money + 20,
    Money50 = ItemType.Money + 50,
    Money100 = ItemType.Money + 100,
    Money200 = ItemType.Money + 200,
    Money300 = ItemType.Money + 300,
    Money500 = ItemType.Money + 500,
    Money1000 = ItemType.Money + 1000,
    Money5000 = ItemType.Money + 5000,
    
    Health1 = ItemType.Health + 8,
    Health2 = ItemType.Health + 16,
    Health5 = ItemType.Health + 40,
    Health1_2 = ItemType.Health + 4,
    HealthFull = ItemType.Health + 0xFFFF,
    
    MaxHealth1_4 = ItemType.MaxHealth + 2,
    MaxHealth1_2 = ItemType.MaxHealth + 4,
    MaxHealth1 = ItemType.MaxHealth + 8,
    
    Magic1_8 = ItemType.Magic + 16,
    Magic1_4 = ItemType.Magic + 32,
    Magic1_2 = ItemType.Magic + 64,
    Magic1 = ItemType.Magic + 128,
    MagicFull = ItemType.Magic + 256,
    
    Armor1 = ItemType.Armor + 1,
    Armor2 = ItemType.Armor + 2,
    Armor3 = ItemType.Armor + 3,
    
    Shield1 = ItemType.Shield + 1,
    Shield2 = ItemType.Shield + 2,
    Shield3 = ItemType.Shield + 3,
    
    Sword1 = ItemType.Sword + 1,
    Sword2 = ItemType.Sword + 2,
    Sword3 = ItemType.Sword + 3,
    Sword4 = ItemType.Sword + 4,
    
    Bomb1 = ItemType.Bomb + 1,
    Bomb5 = ItemType.Bomb + 5,
    Bomb10 = ItemType.Bomb + 10,
    
    MaxBombs1 = ItemType.MaxBombs + 1,
    MaxBombs5 = ItemType.MaxBombs + 5,
    MaxBombs10 = ItemType.MaxBombs + 10,
    
    Arrow1 = ItemType.Arrow + 1,
    Arrow5 = ItemType.Arrow + 5,
    Arrow10 = ItemType.Arrow + 10,
    Arrow20 = ItemType.Arrow + 20,
    
    MaxArrows1 = ItemType.MaxArrows + 1,
    MaxArrows5 = ItemType.MaxArrows + 5,
    MaxArrows10 = ItemType.MaxArrows + 10,
    MaxArrows20 = ItemType.MaxArrows + 20,
    
    Bow = ItemType.MajorItem,
    Boomerang,
    Grapple,
    Dash,
    Jump,
    Phase,
    Vision1,
    Vision2,
    Lantern,
    RubyWand,
    SapphireWand,
    CitrineWand,
    Bottle,

    HealthPotion = ItemType.Potion,
    MagicPotion,
    FullRestorePotion,

    Trap1 = ItemType.Trap + 1,
    Trap2,
    Trap3,

    KeyDungeonLocal = ItemType.Key,
    KeyDungeon1,
    KeyDungeon2,
    KeyDungeon3,
    KeyDungeon4,
    KeyDungeon5,
    KeyDungeon6,
    KeyDungeon7,
    KeyDungeon8,
    KeyDungeon9,
    KeyDungeon10,
    KeyDungeon11,
    KeyDungeon12,
    KeyDungeon13,
    KeyDungeon14,
    KeyDungeon15,
    KeyDungeon16,
    KeyGlobal = ItemType.Key + 0xFFFF,

    TreasureKeyDungeonLocal = ItemType.TreasureKey,
    TreasureKeyDungeon1,
    TreasureKeyDungeon2,
    TreasureKeyDungeon3,
    TreasureKeyDungeon4,
    TreasureKeyDungeon5,
    TreasureKeyDungeon6,
    TreasureKeyDungeon7,
    TreasureKeyDungeon8,
    TreasureKeyDungeon9,
    TreasureKeyDungeon10,
    TreasureKeyDungeon11,
    TreasureKeyDungeon12,
    TreasureKeyDungeon13,
    TreasureKeyDungeon14,
    TreasureKeyDungeon15,
    TreasureKeyDungeon16,
    TreasureKeyGlobal = ItemType.TreasureKey + 0xFFFF,

    BossKeyDungeonLocal = ItemType.BossKey,
    BossKeyDungeon1,
    BossKeyDungeon2,
    BossKeyDungeon3,
    BossKeyDungeon4,
    BossKeyDungeon5,
    BossKeyDungeon6,
    BossKeyDungeon7,
    BossKeyDungeon8,
    BossKeyDungeon9,
    BossKeyDungeon10,
    BossKeyDungeon11,
    BossKeyDungeon12,
    BossKeyDungeon13,
    BossKeyDungeon14,
    BossKeyDungeon15,
    BossKeyDungeon16,
    BossKeyGlobal = ItemType.BossKey + 0xFFFF,

    Rune1 = ItemType.Rune + 1,
    Rune2,
    Rune3,
    Rune4,
    Rune5,
    Rune6,
    Rune7,
    Rune8,
    Rune9,
    Rune10,
    Rune11,
    Rune12,
    Rune13,
    Rune14,
    Rune15,
    Rune16
}

public class ItemConverter : JsonConverter<Item>
{
    public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer)
    {
        if (Enum.IsDefined(value))
        {
            writer.WriteValue(Enum.GetName(value));
            return;
        }

        ItemType itemType = value.GetItemType();
        if (Enum.IsDefined(itemType))
        {
            writer.WriteStartArray();
            writer.WriteValue(Enum.GetName(value.GetItemType()));
            writer.WriteValue(value.GetValue());
            writer.WriteEndArray();
        }

        writer.WriteValue((int)value);
    }

    public override Item ReadJson(JsonReader reader, Type objectType, Item existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.String:
                return Enum.Parse<Item>(reader.ReadAsString());
            case JsonToken.StartArray:
                //reader.Read();
                try { return (Item)Enum.Parse<ItemType>(reader.ReadAsString()) & (Item)reader.ReadAsInt32(); }
                finally { reader.Read(); }
            case JsonToken.Integer:
                return (Item)reader.ReadAsInt32();
            default:
                throw new JsonSerializationException();
        }
    }
}