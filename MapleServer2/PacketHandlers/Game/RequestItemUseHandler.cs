﻿using Maple2Storage.Enums;
using Maple2Storage.Tools;
using Maple2Storage.Types;
using Maple2Storage.Types.Metadata;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Data.Static;
using MapleServer2.Database;
using MapleServer2.Enums;
using MapleServer2.PacketHandlers.Game.Helpers;
using MapleServer2.Packets;
using MapleServer2.Servers.Game;
using MapleServer2.Tools;
using MapleServer2.Types;

namespace MapleServer2.PacketHandlers.Game;

public class RequestItemUseHandler : GamePacketHandler
{
    public override RecvOp OpCode => RecvOp.REQUEST_ITEM_USE;

    public override void Handle(GameSession session, PacketReader packet)
    {
        long itemUid = packet.ReadLong();

        if (!session.Player.Inventory.Items.ContainsKey(itemUid))
        {
            return;
        }

        Item item = session.Player.Inventory.Items[itemUid];

        switch (item.Function.Name)
        {
            case "CallAirTaxi":
                HandleCallAirTaxi(session, packet, item);
                break;
            case "ChatEmoticonAdd":
                HandleChatEmoticonAdd(session, item);
                break;
            case "SelectItemBox": // Item box selection reward
                HandleSelectItemBox(session, packet, item);
                break;
            case "OpenItemBox": // Item box random/fixed reward
                HandleOpenItemBox(session, packet, item);
                break;
            case "OpenMassive": // Player hosted mini game
                HandleOpenMassive(session, packet, item);
                break;
            case "LevelPotion":
                HandleLevelPotion(session, item);
                break;
            case "TitleScroll":
                HandleTitleScroll(session, item);
                break;
            case "OpenInstrument":
                HandleOpenInstrument(item);
                break;
            case "VIPCoupon":
                HandleVIPCoupon(session, item);
                break;
            case "StoryBook":
                HandleStoryBook(session, item);
                break;
            case "HongBao":
                HandleHongBao(session, item);
                break;
            case "ItemRemakeScroll":
                HandleItemRemakeScroll(session, itemUid);
                break;
            case "OpenGachaBox": // Gacha capsules
                HandleOpenGachaBox(session, packet, item);
                break;
            case "OpenCoupleEffectBox": // Buddy badges
                HandleOpenCoupleEffectBox(session, packet, item);
                break;
            case "PetExtraction": // Pet skin scroll
                HandlePetExtraction(session, packet, item);
                break;
            case "InstallBillBoard": // ad balloons
                HandleInstallBillBoard(session, packet, item);
                break;
            case "ExpendCharacterSlot":
                HandleExpandCharacterSlot(session, item);
                break;
            case "ItemChangeBeauty": // special beauty vouchers
                HandleBeautyVoucher(session, item);
                break;
            case "ItemRePackingScroll":
                HandleRepackingScroll(session, item);
                break;
            case "ChangeCharName":
                HandleNameVoucher(session, packet, item);
                break;
            case "SurvivalLevelExp":
                HandleSurvivalLevelExp(session, item);
                break;
            default:
                Logger.Warn($"Unhandled item function: {item.Function.Name}");
                break;
        }
    }

    private static void HandleItemRemakeScroll(GameSession session, long itemUid)
    {
        session.Send(ChangeAttributesScrollPacket.Open(itemUid));
    }

    private static void HandleChatEmoticonAdd(GameSession session, Item item)
    {
        long expiration = TimeInfo.Now() + item.Function.ChatEmoticonAdd.Duration + Environment.TickCount;

        if (item.Function.ChatEmoticonAdd.Duration == 0) // if no duration was set, set it to not expire
        {
            expiration = long.MaxValue;
        }

        if (session.Player.ChatSticker.Any(p => p.GroupId == item.Function.ChatEmoticonAdd.Id))
        {
            // TODO: Find reject packet
            return;
        }

        session.Send(ChatStickerPacket.AddSticker(item.Id, item.Function.ChatEmoticonAdd.Id, expiration));
        session.Player.ChatSticker.Add(new((byte) item.Function.ChatEmoticonAdd.Id, expiration));
        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
    }

    private static void HandleSelectItemBox(GameSession session, PacketReader packet, Item item)
    {
        short boxType = packet.ReadShort();
        int index = packet.ReadShort() - 0x30;

        ItemBoxHelper.GiveItemFromSelectBox(session, item, index);
    }

    private static void HandleOpenItemBox(GameSession session, PacketReader packet, Item item)
    {
        short boxType = packet.ReadShort();

        ItemBoxHelper.GiveItemFromOpenBox(session, item);
    }

    private static void HandleOpenMassive(GameSession session, PacketReader packet, Item item)
    {
        // Major WIP

        string password = packet.ReadUnicodeString();
        int duration = item.Function.OpenMassiveEvent.Duration + Environment.TickCount;
        CoordF portalCoord = session.Player.FieldPlayer.Coord;
        CoordF portalRotation = session.Player.FieldPlayer.Rotation;

        session.FieldManager.BroadcastPacket(PlayerHostPacket.StartMinigame(session.Player, item.Function.OpenMassiveEvent.FieldId));
        //  session.FieldManager.BroadcastPacket(FieldPacket.AddPortal()
        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
    }

    private static void HandleLevelPotion(GameSession session, Item item)
    {
        if (session.Player.Levels.Level >= item.Function.LevelPotion.TargetLevel)
        {
            return;
        }

        session.Player.Levels.SetLevel(item.Function.LevelPotion.TargetLevel);

        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
    }

    private static void HandleTitleScroll(GameSession session, Item item)
    {
        if (session.Player.Titles.Contains(item.Function.Id))
        {
            return;
        }

        session.Player.Titles.Add(item.Function.Id);

        session.Send(UserEnvPacket.AddTitle(item.Function.Id));

        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
    }

    private static void HandleOpenInstrument(Item item)
    {
        if (!InstrumentCategoryInfoMetadataStorage.IsValid(item.Function.Id))
        {
        }
    }

    private static void HandleVIPCoupon(GameSession session, Item item)
    {
        long vipTime = item.Function.VIPCoupon.Duration * 3600;

        PremiumClubHandler.ActivatePremium(session, vipTime);
        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
    }

    private static void HandleStoryBook(GameSession session, Item item)
    {
        session.Send(StoryBookPacket.Open(item.Function.Id));
    }

    private static void HandleHongBao(GameSession session, Item item)
    {
        HongBao newHongBao = new(session.Player, item.Function.HongBao.TotalUsers, item.Id, item.Function.HongBao.Id, item.Function.HongBao.Count, item.Function.HongBao.Duration);
        GameServer.HongBaoManager.AddHongBao(newHongBao);

        session.FieldManager.BroadcastPacket(PlayerHostPacket.OpenHongbao(session.Player, newHongBao));
        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
    }

    private static void HandleOpenGachaBox(GameSession session, PacketReader packet, Item capsule)
    {
        string amount = packet.ReadUnicodeString();
        int rollCount = 0;

        GachaMetadata gacha = GachaMetadataStorage.GetMetadata(capsule.Function.Id);

        List<Item> items = new();

        if (amount == "single")
        {
            rollCount = 1;
        }
        else if (amount == "multi")
        {
            rollCount = 10;
        }

        for (int i = 0; i < rollCount; i++)
        {
            GachaContent contents = HandleSmartGender(gacha, session.Player.Gender);

            int itemAmount = RandomProvider.Get().Next(contents.MinAmount, contents.MaxAmount);

            Item gachaItem = new(contents.ItemId)
            {
                Rarity = contents.Rarity,
                Amount = itemAmount,
                GachaDismantleId = gacha.GachaId
            };
            items.Add(gachaItem);
            session.Player.Inventory.ConsumeItem(session, capsule.Uid, 1);
        }

        session.Send(FireWorksPacket.Gacha(items));

        foreach (Item item in items)
        {
            session.Player.Inventory.AddItem(session, item, true);
        }
    }

    private static GachaContent HandleSmartGender(GachaMetadata gacha, Gender playerGender)
    {
        Random random = RandomProvider.Get();
        int index = random.Next(gacha.Contents.Count);

        GachaContent contents = gacha.Contents[index];
        if (!contents.SmartGender)
        {
            return contents;
        }

        Gender itemGender = ItemMetadataStorage.GetGender(contents.ItemId);
        if (playerGender != itemGender || itemGender != Gender.Neutral) // if it's not the same gender or unisex, roll again
        {
            bool sameGender = false;
            do
            {
                int indexReroll = random.Next(gacha.Contents.Count);

                GachaContent rerollContents = gacha.Contents[indexReroll];
                Gender rerollContentsGender = ItemMetadataStorage.GetGender(rerollContents.ItemId);

                if (rerollContentsGender == playerGender || rerollContentsGender == Gender.Neutral)
                {
                    return rerollContents;
                }
            } while (!sameGender);
        }
        return contents;
    }

    public static void HandleOpenCoupleEffectBox(GameSession session, PacketReader packet, Item item)
    {
        string targetUser = packet.ReadUnicodeString();

        if (targetUser == session.Player.Name)
        {
            //TODO: Find the error packet
            return;
        }

        if (!DatabaseManager.Characters.NameExists(targetUser))
        {
            session.Send(NoticePacket.Notice(SystemNotice.CharacterNotFound, NoticeType.Popup));
            return;
        }

        Player otherPlayer = GameServer.PlayerManager.GetPlayerByName(targetUser);
        if (otherPlayer == null)
        {
            otherPlayer = DatabaseManager.Characters.FindPartialPlayerByName(targetUser);
        }

        Item badge = new(item.Function.OpenCoupleEffectBox.Id)
        {
            Rarity = item.Function.OpenCoupleEffectBox.Rarity,
            PairedCharacterId = otherPlayer.CharacterId,
            PairedCharacterName = otherPlayer.Name,
            OwnerCharacterId = session.Player.CharacterId,
            OwnerCharacterName = session.Player.Name
        };

        Item otherUserBadge = new(item.Function.OpenCoupleEffectBox.Id)
        {
            Rarity = item.Function.OpenCoupleEffectBox.Rarity,
            PairedCharacterId = session.Player.CharacterId,
            PairedCharacterName = session.Player.Name,
            OwnerCharacterId = otherPlayer.CharacterId,
            OwnerCharacterName = otherPlayer.Name
        };

        List<Item> items = new()
        {
            otherUserBadge
        };

        MailHelper.SendMail(MailType.System, otherPlayer.CharacterId, session.Player.CharacterId,
                            "<ms2><v key=\"s_couple_effect_mail_sender\" /></ms2>",
                            "<ms2><v key=\"s_couple_effect_mail_title_receiver\" /></ms2>",
                            "<ms2><v key=\"s_couple_effect_mail_content_receiver\" /></ms2>",
                            "",
                            $"<ms2><v str=\"{session.Player.Name}\" ></v></ms2>",
                            items,
                            0, 0, out Mail mail);

        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
        session.Player.Inventory.AddItem(session, badge, true);
        List<string> noticeParameters = new()
        {
            otherPlayer.Name
        };

        session.Send(NoticePacket.Notice(SystemNotice.BuddyBadgeMailedToUser, NoticeType.ChatAndFastText, noticeParameters));
    }

    public static void HandlePetExtraction(GameSession session, PacketReader packet, Item item)
    {
        long petUid = long.Parse(packet.ReadUnicodeString());
        if (!session.Player.Inventory.Items.ContainsKey(petUid))
        {
            return;
        }

        Item pet = session.Player.Inventory.Items[petUid];

        Item badge = new(70100000)
        {
            PetSkinBadgeId = pet.Id,
            CreationTime = TimeInfo.Now() + Environment.TickCount
        };

        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
        session.Player.Inventory.AddItem(session, badge, true);
        session.Send(PetSkinPacket.Extract(petUid, badge));
    }

    public static void HandleCallAirTaxi(GameSession session, PacketReader packet, Item item)
    {
        int fieldID = int.Parse(packet.ReadUnicodeString());
        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
        session.Player.Warp(fieldID);
    }

    public static void HandleInstallBillBoard(GameSession session, PacketReader packet, Item item)
    {
        string[] parameters = packet.ReadUnicodeString().Split("'");
        string title = parameters[0];
        string description = parameters[1];
        bool publicHouse = parameters[2].Equals("1");

        int balloonUid = GuidGenerator.Int();
        string id = "AdBalloon_" + balloonUid;
        AdBalloon balloon = new(id, item.Function.InstallBillboard.InteractId, InteractObjectState.Default, InteractObjectType.AdBalloon, session.Player.FieldPlayer, item.Function.InstallBillboard, title, description, publicHouse);
        session.FieldManager.State.AddInteractObject(balloon);
        session.FieldManager.BroadcastPacket(InteractObjectPacket.LoadAdBallon(balloon));
        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
    }

    public static void HandleExpandCharacterSlot(GameSession session, Item item)
    {
        Account account = DatabaseManager.Accounts.FindById(session.Player.AccountId);
        int maxSlots = int.Parse(ConstantsMetadataStorage.GetConstant("MaxCharacterSlots"));
        if (account.CharacterSlots >= maxSlots)
        {
            session.Send(CouponUsePacket.MaxCharacterSlots());
            return;
        }

        account.CharacterSlots++;
        DatabaseManager.Accounts.Update(account);
        session.Send(CouponUsePacket.CharacterSlotAdded());
        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);
    }

    public static void HandleBeautyVoucher(GameSession session, Item item)
    {
        if (item.Gender != session.Player.Gender)
        {
            return;
        }

        session.Send(CouponUsePacket.BeautyCoupon(session.Player.FieldPlayer, item.Uid));
    }

    public static void HandleRepackingScroll(GameSession session, Item item)
    {
        session.Send(ItemRepackagePacket.Open(item.Uid));
    }

    public static void HandleNameVoucher(GameSession session, PacketReader packet, Item item)
    {
        string characterName = packet.ReadUnicodeString();
        session.Player.Name = characterName;

        session.Player.Inventory.ConsumeItem(session, item.Uid, 1);

        session.Send(CharacterListPacket.NameChanged(session.Player.CharacterId, characterName));
        // TODO: Needs to redirect player to character selection screen after pop-up
    }

    public static void HandleSurvivalLevelExp(GameSession session, Item item)
    {
        session.Player.Account.MushkingRoyaleStats.AddExp(session, item.Function.SurvivalLevelExp.SurvivalExp);
    }
}
