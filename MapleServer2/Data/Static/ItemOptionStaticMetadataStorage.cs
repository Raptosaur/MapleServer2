﻿using Maple2Storage.Types;
using Maple2Storage.Types.Metadata;
using ProtoBuf;

namespace MapleServer2.Data.Static;

public static class ItemOptionStaticMetadataStorage
{
    private static readonly Dictionary<int, ItemOptionStaticMetadata> ItemOptionStatic = new();

    public static void Init()
    {
        using FileStream stream = File.OpenRead($"{Paths.RESOURCES_DIR}/ms2-item-option-static-metadata");
        List<ItemOptionStaticMetadata> items = Serializer.Deserialize<List<ItemOptionStaticMetadata>>(stream);
        foreach (ItemOptionStaticMetadata item in items)
        {
            ItemOptionStatic[item.Id] = item;
        }
    }

    public static bool IsValid(int id)
    {
        return ItemOptionStatic.ContainsKey(id);
    }

    public static ItemOptionsStatic GetMetadata(int id, int rarity)
    {
        ItemOptionStaticMetadata metadata = ItemOptionStatic.Values.FirstOrDefault(x => x.Id == id);
        if (metadata == null)
        {
            return null;
        }
        return metadata.ItemOptions.FirstOrDefault(x => x.Rarity == rarity);
    }
}
