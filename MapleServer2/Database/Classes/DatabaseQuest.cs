﻿using Maple2Storage.Enums;
using MapleServer2.Types;
using Newtonsoft.Json;
using SqlKata.Execution;

namespace MapleServer2.Database.Classes;

using NLog;

public class DatabaseQuest : DatabaseTable
{
    public DatabaseQuest() : base("quests") { }

    public long Insert(QuestStatus questStatus)
    {
        return QueryFactory.Query(TableName).InsertGetId<long>(new
        {
            questStatus.Id,
            questStatus.State,
            start_timestamp = questStatus.StartTimestamp,
            complete_timestamp = questStatus.CompleteTimestamp,
            questStatus.Tracked,
            condition = JsonConvert.SerializeObject(questStatus.Condition),
            character_id = questStatus.CharacterId
        });
    }

    public Dictionary<int, QuestStatus> FindAllByCharacterId(long characterId)
    {
        IEnumerable<dynamic> results = QueryFactory.Query(TableName).Where("character_id", characterId).Get();
        Dictionary<int, QuestStatus> questStatusList = new();
        foreach (dynamic data in results)
        {
            try
            {
                QuestStatus questStatus = (QuestStatus) ReadQuest(data);
                questStatusList.Add(questStatus.Id, questStatus);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
        }

        return questStatusList;
    }

    public void Update(QuestStatus questStatus)
    {
        QueryFactory.Query(TableName).Where("id", questStatus.Id).Update(new
        {
            questStatus.Id,
            questStatus.State,
            start_timestamp = questStatus.StartTimestamp,
            complete_timestamp = questStatus.CompleteTimestamp,
            questStatus.Tracked,
            condition = JsonConvert.SerializeObject(questStatus.Condition),
            character_id = questStatus.CharacterId
        });
    }

    public bool Delete(long uid)
    {
        return QueryFactory.Query(TableName).Where("uid", uid).Delete() == 1;
    }

    private static QuestStatus ReadQuest(dynamic data)
    {
        return new QuestStatus(data.uid, data.id, data.character_id, data.tracked, data.start_timestamp, data.complete_timestamp, JsonConvert.DeserializeObject<List<Condition>>(data.condition), (QuestState) data.state);
    }
}
