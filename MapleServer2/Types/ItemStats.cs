﻿using Maple2Storage.Enums;
using Maple2Storage.Tools;
using Maple2Storage.Types.Metadata;
using MapleServer2.Data.Static;

namespace MapleServer2.Types;

public abstract class ItemStat
{
    public dynamic ItemAttribute;
    public dynamic Flat;
    public float Percent;
}
public class NormalStat : ItemStat
{
    public new StatId ItemAttribute;
    public new int Flat;

    public NormalStat() { }

    public NormalStat(StatId attribute, int flat, float percent)
    {
        ItemAttribute = attribute;
        Flat = flat;
        Percent = percent;
    }

    public NormalStat(ParserStat stat)
    {
        ItemAttribute = stat.Id;
        Flat = stat.Flat;
        Percent = stat.Percent;
    }
}
public class SpecialStat : ItemStat
{
    public new SpecialStatId ItemAttribute;
    public new float Flat;

    public SpecialStat() { }

    public SpecialStat(SpecialStatId attribute, float flat, float percent)
    {
        ItemAttribute = attribute;
        Flat = flat;
        Percent = percent;
    }

    public SpecialStat(ParserSpecialStat stat)
    {
        ItemAttribute = stat.Id;
        Flat = stat.Flat;
        Percent = stat.Percent;
    }
}
public class Gemstone
{
    public int Id;
    public long OwnerId = 0;
    public string OwnerName = "";
    public bool IsLocked;
    public long UnlockTime;
}
public class GemSocket
{
    public bool IsUnlocked;
    public Gemstone Gemstone;
}
public class ItemStats
{
    public List<ItemStat> BasicStats;
    public List<ItemStat> BonusStats;
    public List<GemSocket> GemSockets;

    public ItemStats() { }

    public ItemStats(Item item)
    {
        CreateNewStats(item.Id, item.Rarity, item.ItemSlot, item.Level);
    }

    public ItemStats(int itemId, int rarity, ItemSlot itemSlot, int itemLevel)
    {
        CreateNewStats(itemId, rarity, itemSlot, itemLevel);
    }

    public ItemStats(ItemStats other)
    {
        BasicStats = new(other.BasicStats);
        BonusStats = new(other.BonusStats);
        GemSockets = new();
    }

    public void CreateNewStats(int itemId, int rarity, ItemSlot itemSlot, int itemLevel)
    {
        BasicStats = new();
        BonusStats = new();
        GemSockets = new();
        if (rarity == 0)
        {
            return;
        }

        GetConstantStats(itemId, rarity, out List<NormalStat> normalStats, out List<SpecialStat> specialStats);
        GetStaticStats(itemId, rarity, normalStats, specialStats);
        GetBonusStats(itemId, rarity);
        if (itemLevel >= 50 && rarity >= 3)
        {
            GetGemSockets(itemSlot, rarity);
        }
    }

    public static void GetConstantStats(int itemId, int rarity, out List<NormalStat> normalStats, out List<SpecialStat> specialStats)
    {
        normalStats = new();
        specialStats = new();

        // Get Constant Stats
        int constantId = ItemMetadataStorage.GetOptionConstant(itemId);
        ItemOptionsConstant basicOptions = ItemOptionConstantMetadataStorage.GetMetadata(constantId, rarity);
        if (basicOptions == null)
        {
            return;
        }

        foreach (ParserStat stat in basicOptions.Stats)
        {
            normalStats.Add(new(stat.Id, stat.Flat, stat.Percent));
        }

        foreach (ParserSpecialStat stat in basicOptions.SpecialStats)
        {
            specialStats.Add(new(stat.Id, stat.Flat, stat.Percent));
        }

        if (basicOptions.HiddenDefenseAdd > 0)
        {
            AddHiddenNormalStat(normalStats, StatId.Defense, basicOptions.HiddenDefenseAdd, basicOptions.DefenseCalibrationFactor);
        }

        if (basicOptions.HiddenWeaponAtkAdd > 0)
        {
            AddHiddenNormalStat(normalStats, StatId.MinWeaponAtk, basicOptions.HiddenWeaponAtkAdd, basicOptions.WeaponAtkCalibrationFactor);
            AddHiddenNormalStat(normalStats, StatId.MaxWeaponAtk, basicOptions.HiddenWeaponAtkAdd, basicOptions.WeaponAtkCalibrationFactor);
        }
    }

    public void GetStaticStats(int itemId, int rarity, List<NormalStat> normalStats, List<SpecialStat> specialStats)
    {
        //Get Static Stats
        int staticId = ItemMetadataStorage.GetOptionStatic(itemId);

        ItemOptionsStatic staticOptions = ItemOptionStaticMetadataStorage.GetMetadata(staticId, rarity);
        if (staticOptions == null)
        {
            BasicStats.AddRange(normalStats);
            BasicStats.AddRange(specialStats);
            return;
        }

        foreach (ParserStat stat in staticOptions.Stats)
        {
            NormalStat normalStat = normalStats.FirstOrDefault(x => x.ItemAttribute == stat.Id);
            if (normalStat == null)
            {
                normalStats.Add(new(stat.Id, stat.Flat, stat.Percent));
                continue;
            }
            int index = normalStats.FindIndex(x => x.ItemAttribute == stat.Id);
            int summedFlat = normalStat.Flat + stat.Flat;
            float summedPercent = normalStat.Percent + stat.Percent;

            normalStats[index] = new(stat.Id, summedFlat, summedPercent);
        }

        foreach (ParserSpecialStat stat in staticOptions.SpecialStats)
        {
            SpecialStat normalStat = specialStats.FirstOrDefault(x => x.ItemAttribute == stat.Id);
            if (normalStat == null)
            {
                specialStats.Add(new(stat.Id, stat.Flat, stat.Percent));
                continue;
            }

            int index = specialStats.FindIndex(x => x.ItemAttribute == stat.Id);
            float summedFlat = normalStat.Flat + stat.Flat;
            float summedPercent = normalStat.Percent + stat.Percent;

            specialStats[index] = new(stat.Id, summedFlat, summedPercent);
        }

        if (staticOptions.HiddenDefenseAdd > 0)
        {
            AddHiddenNormalStat(normalStats, StatId.Defense, staticOptions.HiddenDefenseAdd, staticOptions.DefenseCalibrationFactor);
        }

        if (staticOptions.HiddenWeaponAtkAdd > 0)
        {
            AddHiddenNormalStat(normalStats, StatId.MinWeaponAtk, staticOptions.HiddenWeaponAtkAdd, staticOptions.WeaponAtkCalibrationFactor);
            AddHiddenNormalStat(normalStats, StatId.MaxWeaponAtk, staticOptions.HiddenWeaponAtkAdd, staticOptions.WeaponAtkCalibrationFactor);
        }

        BasicStats.AddRange(normalStats);
        BasicStats.AddRange(specialStats);
    }

    private static void AddHiddenNormalStat(List<NormalStat> normalStats, StatId attribute, int value, float calibrationFactor)
    {
        NormalStat normalStat = normalStats.FirstOrDefault(x => x.ItemAttribute == attribute);
        if (normalStat == null)
        {
            return;
        }
        int calibratedValue = (int) (value * calibrationFactor);

        int index = normalStats.FindIndex(x => x.ItemAttribute == attribute);
        int biggerValue = Math.Max(value, calibratedValue);
        int smallerValue = Math.Min(value, calibratedValue);
        int summedFlat = normalStat.Flat + RandomProvider.Get().Next(smallerValue, biggerValue);
        normalStats[index] = new(normalStat.ItemAttribute, summedFlat, normalStat.Percent);
    }

    public void GetBonusStats(int itemId, int rarity)
    {
        int randomId = ItemMetadataStorage.GetOptionRandom(itemId);
        ItemOptionRandom randomOptions = ItemOptionRandomMetadataStorage.GetMetadata(randomId, rarity);
        if (randomOptions == null)
        {
            return;
        }

        // get amount of slots
        Random random = RandomProvider.Get();
        int slots = random.Next(randomOptions.Slots[0], randomOptions.Slots[1]);

        List<ItemStat> itemStats = RollStats(randomOptions, randomId, itemId);
        List<ItemStat> selectedStats = itemStats.OrderBy(x => random.Next()).Take(slots).ToList();

        BonusStats.AddRange(selectedStats);
    }

    public static List<ItemStat> RollStats(ItemOptionRandom randomOptions, int randomId, int itemId)
    {
        List<ItemStat> itemStats = new();

        foreach (ParserStat stat in randomOptions.Stats)
        {
            Dictionary<StatId, List<ParserStat>> rangeDictionary = GetRange(randomId);
            if (!rangeDictionary.ContainsKey(stat.Id))
            {
                continue;
            }

            NormalStat normalStat = new(rangeDictionary[stat.Id][Roll(itemId)]);
            if (randomOptions.MultiplyFactor > 0)
            {
                normalStat.Flat *= (int) Math.Ceiling(randomOptions.MultiplyFactor);
                normalStat.Percent *= randomOptions.MultiplyFactor;
            }
            itemStats.Add(normalStat);
        }

        foreach (ParserSpecialStat stat in randomOptions.SpecialStats)
        {
            Dictionary<SpecialStatId, List<ParserSpecialStat>> rangeDictionary = GetSpecialRange(randomId);
            if (!rangeDictionary.ContainsKey(stat.Id))
            {
                continue;
            }

            SpecialStat specialStat = new(rangeDictionary[stat.Id][Roll(itemId)]);
            if (randomOptions.MultiplyFactor > 0)
            {
                specialStat.Flat *= (int) Math.Ceiling(randomOptions.MultiplyFactor);
                specialStat.Percent *= randomOptions.MultiplyFactor;
            }
            itemStats.Add(specialStat);
        }

        return itemStats;
    }

    // Roll new bonus stats and values except the locked stat
    public static List<ItemStat> RollBonusStatsWithStatLocked(Item item, short ignoreStat, bool isSpecialStat)
    {
        int id = item.Id;

        int randomId = ItemMetadataStorage.GetOptionRandom(id);
        ItemOptionRandom randomOptions = ItemOptionRandomMetadataStorage.GetMetadata(randomId, item.Rarity);
        if (randomOptions == null)
        {
            return null;
        }

        List<ItemStat> itemStats = new();

        List<ParserStat> attributes = isSpecialStat ? randomOptions.Stats : randomOptions.Stats.Where(x => (short) x.Id != ignoreStat).ToList();
        List<ParserSpecialStat> specialAttributes = isSpecialStat ? randomOptions.SpecialStats.Where(x => (short) x.Id != ignoreStat).ToList() : randomOptions.SpecialStats;

        foreach (ParserStat attribute in attributes)
        {
            Dictionary<StatId, List<ParserStat>> dictionary = GetRange(randomId);
            if (!dictionary.ContainsKey(attribute.Id))
            {
                continue;
            }

            NormalStat normalStat = new(dictionary[attribute.Id][Roll(id)]);
            if (randomOptions.MultiplyFactor > 0)
            {
                normalStat.Flat *= (int) Math.Ceiling(randomOptions.MultiplyFactor);
                normalStat.Percent *= randomOptions.MultiplyFactor;
            }
            itemStats.Add(normalStat);
        }

        foreach (ParserSpecialStat attribute in specialAttributes)
        {
            Dictionary<SpecialStatId, List<ParserSpecialStat>> dictionary = GetSpecialRange(randomId);
            if (!dictionary.ContainsKey(attribute.Id))
            {
                continue;
            }

            SpecialStat specialStat = new(dictionary[attribute.Id][Roll(id)]);
            if (randomOptions.MultiplyFactor > 0)
            {
                specialStat.Flat *= (int) Math.Ceiling(randomOptions.MultiplyFactor);
                specialStat.Percent *= randomOptions.MultiplyFactor;
            }
            itemStats.Add(specialStat);
        }

        return itemStats.OrderBy(x => RandomProvider.Get().Next()).Take(item.Stats.BonusStats.Count).ToList();
    }

    // Roll new values for existing bonus stats
    public static List<ItemStat> RollNewBonusValues(Item item, short ignoreStat, bool isSpecialStat)
    {
        List<ItemStat> newBonus = new();

        foreach (NormalStat stat in item.Stats.BonusStats.OfType<NormalStat>())
        {
            if (!isSpecialStat && (short) stat.ItemAttribute == ignoreStat)
            {
                newBonus.Add(stat);
                continue;
            }

            Dictionary<StatId, List<ParserStat>> dictionary = GetRange(item.Id);
            if (!dictionary.ContainsKey(stat.ItemAttribute))
            {
                continue;
            }
            newBonus.Add(new NormalStat(dictionary[stat.ItemAttribute][Roll(item.Level)]));
        }

        foreach (SpecialStat stat in item.Stats.BonusStats.OfType<SpecialStat>())
        {
            if (isSpecialStat && (short) stat.ItemAttribute == ignoreStat)
            {
                newBonus.Add(stat);
                continue;
            }

            Dictionary<SpecialStatId, List<ParserSpecialStat>> dictionary = GetSpecialRange(item.Id);
            if (!dictionary.ContainsKey(stat.ItemAttribute))
            {
                continue;
            }
            newBonus.Add(new SpecialStat(dictionary[stat.ItemAttribute][Roll(item.Level)]));
        }

        return newBonus;
    }

    private static Dictionary<StatId, List<ParserStat>> GetRange(int itemId)
    {
        ItemSlot slot = ItemMetadataStorage.GetSlot(itemId);
        if (Item.IsAccessory(slot))
        {
            return ItemOptionRangeStorage.GetAccessoryRanges();
        }

        if (Item.IsArmor(slot))
        {
            return ItemOptionRangeStorage.GetArmorRanges();
        }

        if (Item.IsWeapon(slot))
        {
            return ItemOptionRangeStorage.GetWeaponRanges();
        }

        return ItemOptionRangeStorage.GetPetRanges();
    }

    private static Dictionary<SpecialStatId, List<ParserSpecialStat>> GetSpecialRange(int itemId)
    {
        ItemSlot slot = ItemMetadataStorage.GetSlot(itemId);
        if (Item.IsAccessory(slot))
        {
            return ItemOptionRangeStorage.GetAccessorySpecialRanges();
        }

        if (Item.IsArmor(slot))
        {
            return ItemOptionRangeStorage.GetArmorSpecialRanges();
        }

        if (Item.IsWeapon(slot))
        {
            return ItemOptionRangeStorage.GetWeaponSpecialRanges();
        }

        return ItemOptionRangeStorage.GetPetSpecialRanges();
    }

    // Returns index 0~7 for equip level 70-
    // Returns index 8~15 for equip level 70+
    private static int Roll(int itemId)
    {
        int itemLevelFactor = ItemMetadataStorage.GetOptionLevelFactor(itemId);
        Random random = RandomProvider.Get();
        if (itemLevelFactor >= 70)
        {
            return random.NextDouble() switch
            {
                >= 0.0 and < 0.24 => 8,
                >= 0.24 and < 0.48 => 9,
                >= 0.48 and < 0.74 => 10,
                >= 0.74 and < 0.9 => 11,
                >= 0.9 and < 0.966 => 12,
                >= 0.966 and < 0.985 => 13,
                >= 0.985 and < 0.9975 => 14,
                _ => 15
            };
        }
        return random.NextDouble() switch
        {
            >= 0.0 and < 0.24 => 0,
            >= 0.24 and < 0.48 => 1,
            >= 0.48 and < 0.74 => 2,
            >= 0.74 and < 0.9 => 3,
            >= 0.9 and < 0.966 => 4,
            >= 0.966 and < 0.985 => 5,
            >= 0.985 and < 0.9975 => 6,
            _ => 7
        };
    }

    private void GetGemSockets(ItemSlot itemSlot, int rarity)
    {
        if (itemSlot != ItemSlot.EA &&
            itemSlot != ItemSlot.RI &&
            itemSlot != ItemSlot.PD)
        {
            return;
        }

        int rollAmount = 0;
        if (rarity == 3)
        {
            rollAmount = 1;
        }
        else if (rarity > 3)
        {
            rollAmount = 3;
        }

        // add sockets
        for (int i = 0; i < rollAmount; i++)
        {
            GemSocket socket = new();
            GemSockets.Add(socket);
        }

        // roll to unlock sockets
        for (int i = 0; i < GemSockets.Count; i++)
        {
            int successNumber = RandomProvider.Get().Next(0, 100);

            // 5% success rate to unlock a gemsocket
            if (successNumber < 95)
            {
                break;
            }
            GemSockets[i].IsUnlocked = true;
        }
    }
}
