using System;
using System.Collections.Generic;
using InfiniteOdyssey.Extensions;
using InfiniteOdyssey.Randomization;
using InfiniteOdyssey.Scenes.Action;
using Newtonsoft.Json;

namespace InfiniteOdyssey;

[Serializable]
public class PlayerState
{
    private const int MONEY_MAX = 999_999_999;
    private const int MAX_MAX_HEALTH = 240;
    private const int MAX_MAX_MAGIC = 512;
    private const int MAX_MAX_BOMBS = 50;
    private const int MAX_MAX_ARROWS = 99;
    private const int LOCAL_DUNGEON = 0;
    private const int GLOBAL_DUNGEON = 0xFFFF;

    [JsonProperty(PropertyName = "items")]
    private readonly HashSet<Item> m_majorItems = new();

    [JsonProperty(PropertyName = "money")]
    public int Money;

    [JsonProperty(PropertyName = "health")]
    public int Health;

    [JsonProperty(PropertyName = "maxHealth")]
    public int MaxHealth;

    [JsonProperty(PropertyName = "magic")]
    public int Magic;

    [JsonProperty(PropertyName = "maxMagic")]
    public int MaxMagic;

    [JsonProperty(PropertyName = "armorLevel")]
    public int ArmorLevel;

    [JsonProperty(PropertyName = "shieldLevel")]
    public int ShieldLevel;

    [JsonProperty(PropertyName = "bombs")]
    public int Bombs;

    [JsonProperty(PropertyName = "maxBombs")]
    public int MaxBombs;

    [JsonProperty(PropertyName = "arrows")]
    public int Arrows;

    [JsonProperty(PropertyName = "maxArrows")]
    public int MaxArrows;

    [JsonProperty(PropertyName = "keys")]
    private Dictionary<int, int> m_keys = new();

    [JsonProperty(PropertyName = "treasureKeys")]
    private HashSet<int> m_treasureKeys = new();

    [JsonProperty(PropertyName = "bossKeys")]
    private HashSet<int> m_bossKeys = new();

    [JsonProperty(PropertyName = "runes")]
    private HashSet<int> m_runes = new();

    [JsonProperty(PropertyName = "keyUsageStrategy")]
    public KeyUsageStrategy KeyUsageStrategy = KeyUsageStrategy.PreferLocal;

    [JsonProperty(PropertyName = "currentDungeon")]
    public int CurrentDungeon;

    [JsonProperty(PropertyName = "currentRoom")]
    public RoomData CurrentRoom;

    public bool TryUseKey(int dungeon) =>
        KeyUsageStrategy switch
        {
            KeyUsageStrategy.PreferLocal => m_keys.TryDecrement(dungeon) || m_keys.TryDecrement(GLOBAL_DUNGEON),
            _ => m_keys.TryDecrement(GLOBAL_DUNGEON) || m_keys.TryDecrement(dungeon)
        };

    public int KeyCount(int dungeon)
    {
        if (!m_keys.TryGetValue(dungeon, out int keys)) return 0;
        return keys;
    }

    public bool HasTreasureKey(int dungeon) => m_treasureKeys.Contains(dungeon);

    public bool HasBosseKey(int dungeon) => m_bossKeys.Contains(dungeon);

    public bool HasMajorItem(Item item) => m_majorItems.Contains(item);

    public bool TryAddItem(Item item)
    {
        switch (item.GetItemType())
        {
            case ItemType.Money:
                Money += Money.AddClamped(item.GetValue(), 0, MONEY_MAX);
                break;
            case ItemType.Health:
                Health = Health.AddClamped(item.GetValue(), 0, MaxHealth);
                break;
            case ItemType.MaxHealth:
                MaxHealth = MaxHealth.AddClamped(item.GetValue(), 0, MAX_MAX_HEALTH);
                break;
            case ItemType.Magic:
                Magic = Magic.AddClamped(item.GetValue(), 0, MaxMagic);
                break;
            case ItemType.Armor:
                ArmorLevel = ArmorLevel.AddClamped(item.GetValue());
                break;
            case ItemType.Shield:
                break;
            case ItemType.Sword:
                break;
            case ItemType.Bomb:
                Bombs = Bombs.AddClamped(item.GetValue(), 0, MaxBombs);
                break;
            case ItemType.MaxBombs:
                MaxBombs = MaxBombs.AddClamped(item.GetValue(), 0, MAX_MAX_BOMBS);
                break;
            case ItemType.Arrow:
                Arrows = Arrows.AddClamped(item.GetValue(), 0, MaxArrows);
                break;
            case ItemType.MaxArrows:
                MaxArrows = MaxArrows.AddClamped(item.GetValue(), 0, MAX_MAX_ARROWS);
                break;
            case ItemType.MajorItem:
                m_majorItems.Add(item);
                break;
            case ItemType.Potion:
                break;
            case ItemType.Trap:
                break;
            case ItemType.Key:
                m_keys.TryIncrement(item.GetValue().IfValue(LOCAL_DUNGEON, CurrentDungeon));
                break;
            case ItemType.TreasureKey:
                m_treasureKeys.Add(item.GetValue().IfValue(LOCAL_DUNGEON, CurrentDungeon));
                break;
            case ItemType.BossKey:
                m_bossKeys.Add(item.GetValue().IfValue(LOCAL_DUNGEON, CurrentDungeon));
                break;
            case ItemType.Rune:
                m_runes.Add(item.GetValue());
                break;
            default:
                return false;
        }
        return true;
    }

    public bool TryRemoveItem(Item item) => m_majorItems.Remove(item);
}