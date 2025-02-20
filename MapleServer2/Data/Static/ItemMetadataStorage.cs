﻿using Maple2Storage.Enums;
using Maple2Storage.Tools;
using Maple2Storage.Types;
using Maple2Storage.Types.Metadata;
using MapleServer2.Enums;
using ProtoBuf;

namespace MapleServer2.Data.Static;

// This is an in-memory storage to help with determining some metadata of items
public static class ItemMetadataStorage
{
    private static readonly Dictionary<int, ItemMetadata> ItemMetadatas = new();

    public static void Init()
    {
        using FileStream stream = File.OpenRead($"{Paths.RESOURCES_DIR}/ms2-item-metadata");
        List<ItemMetadata> items = Serializer.Deserialize<List<ItemMetadata>>(stream);
        foreach (ItemMetadata item in items)
        {
            ItemMetadatas[item.Id] = item;
        }
    }

    public static bool IsValid(int itemId) => ItemMetadatas.ContainsKey(itemId);

    public static ItemMetadata GetMetadata(int itemId) => ItemMetadatas.GetValueOrDefault(itemId);

    public static string GetName(int itemId) => GetMetadata(itemId).Name;

    public static ItemSlot GetSlot(int itemId) => GetMetadata(itemId).Slot;

    public static GemSlot GetGem(int itemId) => GetMetadata(itemId).Gem;

    public static MedalSlot GetMedalSlot(int itemId) => GetMetadata(itemId).Medal;

    public static InventoryTab GetTab(int itemId) => GetMetadata(itemId).Tab;

    public static int GetRarity(int itemId) => GetMetadata(itemId).Rarity;

    public static int GetStackLimit(int itemId) => GetMetadata(itemId).StackLimit;

    public static bool GetEnableBreak(int itemId) => GetMetadata(itemId).EnableBreak;

    public static bool GetIsTwoHand(int itemId) => GetMetadata(itemId).IsTwoHand;

    public static bool GetIsDress(int itemId) => GetMetadata(itemId).IsDress;

    public static bool GetIsTemplate(int itemId) => GetMetadata(itemId).IsTemplate;

    public static bool GetIsCustomScore(int itemId) => GetMetadata(itemId).IsCustomScore;

    public static Gender GetGender(int itemId) => GetMetadata(itemId).Gender;

    public static int GetPlayCount(int itemId) => GetMetadata(itemId).PlayCount;

    public static string GetFileName(int itemId) => GetMetadata(itemId).FileName;

    public static int GetSkillID(int itemId) => GetMetadata(itemId).SkillID;

    public static int GetShopID(int itemId) => GetMetadata(itemId).ShopID;

    public static bool IsSellablle(int itemId) => GetMetadata(itemId).Sellable;

    public static TransferType GetTransferType(int itemId) => GetMetadata(itemId).TransferType;

    public static int GetTradeableCount(int itemId) => GetMetadata(itemId).TradeableCount;

    public static int GetRepackageCount(int itemId) => GetMetadata(itemId).RepackageCount;

    public static int GetRepackageConsumeCount(int itemId) => GetMetadata(itemId).RepackageItemConsumeCount;

    public static string GetCategory(int itemId) => GetMetadata(itemId).Category;

    public static List<Job> GetRecommendJobs(int itemId)
    {
        Converter<int, Job> converter = integer => (Job) integer;

        return GetMetadata(itemId).RecommendJobs.ConvertAll(converter);
    }

    public static int GetSellPrice(int itemId)
    {
        // get random selling price from price points
        List<int> pricePoints = GetMetadata(itemId)?.SellPrice;
        if (pricePoints == null || !pricePoints.Any())
        {
            return 0;
        }

        int rand = RandomProvider.Get().Next(0, pricePoints.Count);

        return pricePoints.ElementAt(rand);
    }

    public static int GetCustomSellPrice(int itemId)
    {
        // get random selling price from price points
        List<int> pricePoints = GetMetadata(itemId)?.SellPriceCustom;
        if (pricePoints == null || !pricePoints.Any())
        {
            return 0;
        }

        int rand = RandomProvider.Get().Next(0, pricePoints.Count);

        return pricePoints.ElementAt(rand);
    }

    public static ItemFunction GetFunction(int itemId) => GetMetadata(itemId).FunctionData;

    public static string GetTag(int itemId) => GetMetadata(itemId).Tag;

    public static int GetOptionStatic(int itemId) => GetMetadata(itemId).OptionStatic;

    public static int GetOptionRandom(int itemId) => GetMetadata(itemId).OptionRandom;

    public static int GetOptionConstant(int itemId) => GetMetadata(itemId).OptionConstant;

    public static int GetOptionLevelFactor(int itemId) => GetMetadata(itemId)?.OptionLevelFactor ?? 0;

    public static EquipColor GetEquipColor(int itemId)
    {
        ItemMetadata itemMetadata = GetMetadata(itemId);
        int colorPalette = itemMetadata.ColorPalette;
        int colorIndex = itemMetadata.ColorIndex;

        if (colorPalette == 0) // item has no color
        {
            return EquipColor.Custom(MixedColor.Custom(Color.Argb(0, 0, 0, 0), Color.Argb(0, 0, 0, 0), Color.Argb(0, 0, 0, 0)), colorIndex, colorPalette);
        }

        ColorPaletteMetadata palette = ColorPaletteMetadataStorage.GetMetadata(colorPalette);

        if (colorPalette > 0 && colorIndex == -1) // random color from color palette
        {
            Random random = RandomProvider.Get();

            int index = random.Next(palette.DefaultColors.Count);

            return EquipColor.Argb(palette.DefaultColors[index], colorIndex, colorPalette);
        }

        return EquipColor.Argb(palette.DefaultColors[colorIndex], colorIndex, colorPalette);
    }

    public static List<ItemBreakReward> GetBreakRewards(int itemId) => GetMetadata(itemId).BreakRewards;

    public static int GetLevel(int itemId) => GetMetadata(itemId).Level;

    public static bool GetIsCubeSolid(int itemId) => GetMetadata(itemId).IsCubeSolid;

    public static ItemHousingCategory GetHousingCategory(int itemId) => GetMetadata(itemId).HousingCategory;

    public static int GetObjectId(int itemId) => GetMetadata(itemId).ObjectId;

    public static string GetBlackMarketCategory(int itemId) => GetMetadata(itemId).BlackMarketCategory;

    public static IEnumerable<ItemMetadata> GetAll() => ItemMetadatas.Values;
}
