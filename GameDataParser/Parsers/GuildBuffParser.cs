﻿using System.Xml;
using GameDataParser.Files;
using Maple2.File.IO.Crypto.Common;
using Maple2Storage.Types.Metadata;

namespace GameDataParser.Parsers;

public class GuildBuffParser : Exporter<List<GuildBuffMetadata>>
{
    public GuildBuffParser(MetadataResources resources) : base(resources, "guild-buff") { }

    protected override List<GuildBuffMetadata> Parse()
    {
        List<GuildBuffMetadata> buffs = new();
        Dictionary<int, List<GuildBuffLevel>> buffLevels = new();

        foreach (PackFileEntry entry in Resources.XmlReader.Files)
        {
            if (!entry.Name.StartsWith("table/guildbuff"))
            {
                continue;
            }

            // Parse XML
            XmlDocument document = Resources.XmlReader.GetXmlDocument(entry);
            XmlNodeList contributions = document.SelectNodes("/ms2/guildBuff");

            foreach (XmlNode contribution in contributions)
            {
                int buffId = int.Parse(contribution.Attributes["id"].Value);
                byte level = byte.Parse(contribution.Attributes["level"].Value);
                int additionalEffectId = int.Parse(contribution.Attributes["additionalEffectId"].Value);
                byte additionalEffectLevel = byte.Parse(contribution.Attributes["additionalEffectLevel"].Value);
                byte levelRequirement = byte.Parse(contribution.Attributes["requireLevel"].Value);
                int upgradeCost = int.Parse(contribution.Attributes["upgradeCost"].Value);
                int cost = int.Parse(contribution.Attributes["cost"].Value);
                short duration = short.Parse(contribution.Attributes["duration"].Value);

                GuildBuffLevel buffLevel = new()
                {
                    Level = level,
                    EffectId = additionalEffectId,
                    EffectLevel = additionalEffectLevel,
                    LevelRequirement = levelRequirement,
                    UpgradeCost = upgradeCost,
                    Cost = cost,
                    Duration = duration
                };

                if (buffLevels.ContainsKey(buffId))
                {
                    buffLevels[buffId].Add(buffLevel);
                }
                else
                {
                    buffLevels[buffId] = new()
                    {
                        buffLevel
                    };
                }
            }

            foreach (KeyValuePair<int, List<GuildBuffLevel>> buffData in buffLevels)
            {
                GuildBuffMetadata metadata = new()
                {
                    BuffId = buffData.Key,
                    Levels = buffData.Value
                };
                buffs.Add(metadata);
            }
        }

        return buffs;
    }
}
