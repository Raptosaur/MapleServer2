﻿using Maple2Storage.Types;
using Maple2Storage.Types.Metadata;
using ProtoBuf;

namespace MapleServer2.Data.Static;

public static class InstrumentCategoryInfoMetadataStorage
{
    private static readonly Dictionary<int, InstrumentCategoryInfoMetadata> Instruments = new();

    public static void Init()
    {
        using FileStream stream = File.OpenRead($"{Paths.RESOURCES_DIR}/ms2-instrument-category-info-metadata");
        List<InstrumentCategoryInfoMetadata> items = Serializer.Deserialize<List<InstrumentCategoryInfoMetadata>>(stream);
        foreach (InstrumentCategoryInfoMetadata item in items)
        {
            Instruments[item.CategoryId] = item;
        }
    }

    public static bool IsValid(int categoryId)
    {
        return Instruments.ContainsKey(categoryId);
    }

    public static InstrumentCategoryInfoMetadata GetMetadata(int categoryId)
    {
        return Instruments.GetValueOrDefault(categoryId);
    }

    public static int GetId(int categoryId)
    {
        return Instruments.GetValueOrDefault(categoryId).CategoryId;
    }
}
