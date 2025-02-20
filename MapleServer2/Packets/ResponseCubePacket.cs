﻿using Maple2Storage.Types;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Enums;
using MapleServer2.Tools;
using MapleServer2.Types;

namespace MapleServer2.Packets;

public static class ResponseCubePacket
{
    private enum ResponseCubePacketMode : byte
    {
        LoadFurnishingItem = 0x1,
        EnablePlotFurnishing = 0x2,
        LoadPurchasedLand = 0x3,
        CompletePurchase = 0x4,
        ForfeitPlot = 0x5,
        ForfeitPlot2 = 0x7,
        PlaceFurnishing = 0xA,
        RemoveCube = 0xC,
        RotateCube = 0xE,
        ReplaceCube = 0xF,
        Pickup = 0x11,
        Drop = 0x12,
        LoadHome = 0x14,
        HomeName = 0x15,
        PurchasePlot = 0x16,
        ChangePassword = 0x18,
        ArchitectScoreExpiration = 0x19,
        KickEveryone = 0x1A,
        UpdateArchitectScore = 0x1C,
        HomeDescription = 0x1D,
        ReturnMap = 0x22,
        LoadLayout = 0x24,
        IncreaseSize = 0x25,
        DecreaseSize = 0x26,
        Rewards = 0x27,
        EnablePermission = 0x2A,
        SetPermission = 0x2B,
        IncreaseHeight = 0x2C,
        DecreaseHeight = 0x2D,
        SaveLayout = 0x2E,
        ChangeBackground = 0x33,
        ChangeLighting = 0x34,
        GiveBuildingPermission = 0x35,
        ChangeCamera = 0x36,
        LoadWarehouseItems = 0x37,
        UpdateBudget = 0x38,
        AddBuildingPermission = 0x39,
        RemoveBuildingPermission = 0x3A,
        UpdateSizeHeight = 0x3E
    }

    public static PacketWriter LoadFurnishingItem(IFieldObject<Player> player, int itemId, long itemUid)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.LoadFurnishingItem);
        pWriter.WriteInt(player.ObjectId);
        pWriter.WriteInt(itemId);
        pWriter.WriteLong(itemUid);
        pWriter.WriteLong();
        pWriter.WriteByte();

        return pWriter;
    }

    public static PacketWriter EnablePlotFurnishing(Player player)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.EnablePlotFurnishing);
        pWriter.WriteByte(); // disable bool
        pWriter.WriteInt(player.Account.Home.PlotNumber);
        pWriter.WriteInt(player.Account.Home.ApartmentNumber);
        pWriter.WriteUnicodeString(player.Name);
        pWriter.WriteLong(player.Account.Home.Expiration);
        pWriter.WriteLong(player.CharacterId);

        return pWriter;
    }

    public static PacketWriter CompletePurchase()
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.CompletePurchase);

        return pWriter;
    }

    public static PacketWriter RemovePlot(int plotNumber, int apartmentNumber)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ForfeitPlot);
        pWriter.WriteByte();
        pWriter.WriteInt(plotNumber);
        pWriter.WriteInt(apartmentNumber);
        pWriter.WriteByte();

        return pWriter;
    }

    public static PacketWriter RemovePlot2(int plotId, int plotNumber)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ForfeitPlot2);
        pWriter.WriteByte(72); // unknown
        pWriter.WriteShort();
        pWriter.WriteInt(plotId);
        pWriter.WriteInt(plotNumber);

        return pWriter;
    }

    public static PacketWriter PlaceFurnishing(IFieldObject<Cube> cube, int ownerObjectId, int fieldPlayerObjectId, bool sendOnlyObjectId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.PlaceFurnishing);
        pWriter.WriteBool(sendOnlyObjectId);
        pWriter.WriteInt(ownerObjectId);
        pWriter.WriteInt(fieldPlayerObjectId);

        if (sendOnlyObjectId)
        {
            return pWriter;
        }

        pWriter.WriteInt(cube.Value.PlotNumber);
        pWriter.WriteInt();
        pWriter.Write(cube.Coord.ToByte());
        pWriter.WriteByte();
        pWriter.WriteLong(cube.Value.Uid);
        pWriter.WriteInt(cube.Value.Item.Id);
        pWriter.WriteLong(cube.Value.Item.Uid);
        pWriter.WriteLong();
        pWriter.WriteByte();
        pWriter.WriteByte();
        pWriter.Write(cube.Rotation.Z);
        pWriter.WriteInt();
        pWriter.WriteByte();

        return pWriter;
    }

    public static PacketWriter PlaceLiftable(LiftableObject liftable, int ownerObjectId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.PlaceFurnishing);
        pWriter.WriteBool(false);
        pWriter.WriteInt(ownerObjectId);
        pWriter.WriteInt(ownerObjectId);
        pWriter.WriteInt();
        pWriter.WriteInt();
        pWriter.Write(liftable.Position.ToByte());
        pWriter.WriteByte();
        pWriter.WriteLong();
        pWriter.WriteInt(liftable.ItemId);
        pWriter.WriteLong();
        pWriter.WriteLong();
        pWriter.WriteByte();
        pWriter.WriteByte(1);
        pWriter.Write(liftable.Rotation.Z);
        pWriter.WriteInt();
        pWriter.WriteByte();

        return pWriter;
    }

    public static PacketWriter CantPlaceHere(int fieldPlayerObjectId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.PlaceFurnishing);
        pWriter.WriteByte(44);
        pWriter.WriteInt(fieldPlayerObjectId);
        pWriter.WriteInt(fieldPlayerObjectId);

        return pWriter;
    }

    public static PacketWriter RemoveCube(int ownerObjectId, int fieldPlayerObjectId, CoordB coord)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.RemoveCube);
        pWriter.WriteByte();
        pWriter.WriteInt(ownerObjectId);
        pWriter.WriteInt(fieldPlayerObjectId);
        pWriter.Write(coord);
        pWriter.WriteByte();
        pWriter.WriteByte();

        return pWriter;
    }

    public static PacketWriter RotateCube(IFieldObject<Player> player, IFieldObject<Cube> cube)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.RotateCube);
        pWriter.WriteByte();
        pWriter.WriteInt(player.ObjectId);
        pWriter.WriteInt(player.ObjectId);
        pWriter.Write(cube.Coord.ToByte());
        pWriter.WriteByte();
        pWriter.WriteFloat(cube.Rotation.Z);

        return pWriter;
    }

    public static PacketWriter ReplaceCube(int homeOwnerObjectId, int fieldPlayerObjectId, IFieldObject<Cube> newCube, bool sendOnlyObjectId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ReplaceCube);
        pWriter.WriteBool(sendOnlyObjectId);
        pWriter.WriteInt(homeOwnerObjectId);
        pWriter.WriteInt(fieldPlayerObjectId);

        if (sendOnlyObjectId)
        {
            return pWriter;
        }

        pWriter.Write(newCube.Coord.ToByte());
        pWriter.WriteByte();
        pWriter.WriteInt(newCube.Value.Item.Id);
        pWriter.WriteLong(newCube.Value.Item.Uid);
        pWriter.WriteLong(newCube.Value.Uid);
        pWriter.WriteLong();
        pWriter.WriteByte();
        pWriter.WriteByte();
        pWriter.WriteFloat(newCube.Rotation.Z);
        pWriter.WriteInt();

        return pWriter;
    }

    public static PacketWriter Pickup(IFieldActor<Player> fieldPlayer, int weaponId, CoordB coords)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.Pickup);
        pWriter.WriteZero(1);
        pWriter.WriteInt(fieldPlayer.ObjectId);
        pWriter.Write(coords);
        pWriter.WriteZero(1);
        pWriter.WriteInt(weaponId);
        pWriter.WriteInt(GuidGenerator.Int()); // Item uid

        return pWriter;
    }

    public static PacketWriter Drop(IFieldObject<Player> player)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.Drop);
        pWriter.WriteZero(1);
        pWriter.WriteInt(player.ObjectId);

        return pWriter;
    }

    public static PacketWriter LoadHome(int playerObjectId, Home home)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.LoadHome);
        pWriter.WriteInt(playerObjectId);
        pWriter.WriteInt(home?.MapId ?? 0);
        pWriter.WriteInt(home?.PlotMapId ?? 0);
        pWriter.WriteInt(home?.PlotNumber ?? 0);
        pWriter.WriteInt(home?.ApartmentNumber ?? 0);
        pWriter.WriteUnicodeString(home?.Name ?? "");
        pWriter.WriteLong(home?.Expiration ?? 0);
        pWriter.WriteLong();
        pWriter.WriteByte(1);

        return pWriter;
    }

    public static PacketWriter HomeName(Player player)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.HomeName);
        pWriter.WriteByte();
        pWriter.WriteLong(player.AccountId);
        pWriter.WriteInt(player.Account.Home.PlotNumber);
        pWriter.WriteInt(player.Account.Home.ApartmentNumber);
        pWriter.WriteUnicodeString(player.Account.Home.Name);

        return pWriter;
    }

    public static PacketWriter PurchasePlot(int plotNumber, int apartmentNumber, long expiration)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.PurchasePlot);
        pWriter.WriteInt(plotNumber);
        pWriter.WriteInt(apartmentNumber);
        pWriter.WriteByte(1);
        pWriter.WriteLong(expiration);

        return pWriter;
    }

    public static PacketWriter ForfeitPlot(int plotNumber, int apartmentNumber, long expiration)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.PurchasePlot);
        pWriter.WriteInt(plotNumber);
        pWriter.WriteInt(apartmentNumber);
        pWriter.WriteByte(4);
        pWriter.WriteLong(expiration);

        return pWriter;
    }

    public static PacketWriter ChangePassword()
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ChangePassword);
        pWriter.WriteByte();

        return pWriter;
    }

    public static PacketWriter ArchitectScoreExpiration(long accountId, long now)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ArchitectScoreExpiration);
        pWriter.WriteByte();
        pWriter.WriteLong(accountId);
        pWriter.WriteLong(now);

        return pWriter;
    }

    public static PacketWriter KickEveryone()
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.KickEveryone);

        return pWriter;
    }

    public static PacketWriter UpdateArchitectScore(int current, int total)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.UpdateArchitectScore);
        pWriter.WriteInt(current);
        pWriter.WriteInt(total);

        return pWriter;
    }

    public static PacketWriter HomeDescription(string description)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.HomeDescription);
        pWriter.WriteByte();
        pWriter.WriteUnicodeString(description);

        return pWriter;
    }

    public static PacketWriter ReturnMap(int mapId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ReturnMap);
        pWriter.WriteInt(mapId);

        return pWriter;
    }

    public static PacketWriter BillPopup(Dictionary<byte, long> cubeCosts, int cubeCount)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.LoadLayout);
        pWriter.WriteByte((byte) cubeCosts.Keys.Count);
        pWriter.WriteInt(cubeCount);
        foreach (KeyValuePair<byte, long> kvp in cubeCosts)
        {
            pWriter.WriteByte(kvp.Key);
            pWriter.WriteLong(kvp.Value);
        }

        return pWriter;
    }

    public static PacketWriter IncreaseSize(byte size)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.IncreaseSize);
        pWriter.WriteByte();
        pWriter.WriteByte(size);

        return pWriter;
    }

    public static PacketWriter DecreaseSize(byte size)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.DecreaseSize);
        pWriter.WriteByte();
        pWriter.WriteByte(size);

        return pWriter;
    }

    public static PacketWriter DecorationScore(Home home)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.Rewards);
        pWriter.WriteLong(home?.AccountId ?? 0);
        pWriter.WriteLong(home?.DecorationRewardTimestamp ?? 0);
        pWriter.WriteLong(home?.DecorationLevel ?? 1);
        pWriter.WriteLong(home?.DecorationExp ?? 0);
        pWriter.WriteInt(home?.InteriorRewardsClaimed.Count ?? 0);
        if (home != null)
        {
            foreach (int rewardId in home.InteriorRewardsClaimed)
            {
                pWriter.WriteInt(rewardId);
            }
        }

        return pWriter;
    }

    public static PacketWriter EnablePermission(HomePermission permission, bool enabled)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.EnablePermission);
        pWriter.Write(permission);
        pWriter.WriteBool(enabled);

        return pWriter;
    }

    public static PacketWriter SetPermission(HomePermission permission, byte setting)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.SetPermission);
        pWriter.Write(permission);
        pWriter.WriteByte(setting);

        return pWriter;
    }

    public static PacketWriter IncreaseHeight(byte height)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.IncreaseHeight);
        pWriter.WriteByte();
        pWriter.WriteByte(height);

        return pWriter;
    }

    public static PacketWriter DecreaseHeight(byte height)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.DecreaseHeight);
        pWriter.WriteByte();
        pWriter.WriteByte(height);

        return pWriter;
    }

    public static PacketWriter SaveLayout(long accountId, int layoutId, string layoutName, long timestamp)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.SaveLayout);
        pWriter.WriteByte();
        pWriter.WriteLong(accountId);
        pWriter.WriteInt(layoutId);
        pWriter.WriteUnicodeString(layoutName);
        pWriter.WriteLong(timestamp);

        return pWriter;
    }

    public static PacketWriter ChangeBackground(byte lighthing)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ChangeLighting);
        pWriter.WriteByte();
        pWriter.WriteByte(lighthing);

        return pWriter;
    }

    public static PacketWriter ChangeLighting(byte background)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ChangeBackground);
        pWriter.WriteByte();
        pWriter.WriteByte(background);

        return pWriter;
    }

    public static PacketWriter UpdateBuildingPermissions(long targetAccountId, long ownerAccountId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.GiveBuildingPermission);
        pWriter.WriteLong(targetAccountId);
        pWriter.WriteLong(ownerAccountId);
        pWriter.WriteLong();
        pWriter.WriteLong();

        return pWriter;
    }

    public static PacketWriter ChangeCamera(byte camera)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.ChangeCamera);
        pWriter.WriteByte();
        pWriter.WriteByte(camera);

        return pWriter;
    }

    public static PacketWriter SendWarehouseItems(List<Item> items)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.LoadWarehouseItems);
        pWriter.WriteShort(3);
        pWriter.WriteInt(items.Count);
        foreach (Item item in items)
        {
            pWriter.WriteInt(item.Id);
            pWriter.WriteInt(item.Amount);
        }

        return pWriter;
    }

    public static PacketWriter UpdateBudget(Home home)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.UpdateBudget);
        pWriter.WriteByte();
        pWriter.WriteLong(home.Mesos);
        pWriter.WriteLong(home.Merets);

        return pWriter;
    }

    public static PacketWriter AddBuildingPermission(long accountId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.AddBuildingPermission);
        pWriter.WriteByte();
        pWriter.WriteLong(accountId);

        return pWriter;
    }

    public static PacketWriter RemoveBuildingPermission(long accountId, string characterName)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.RemoveBuildingPermission);
        pWriter.WriteByte();
        pWriter.WriteLong(accountId);
        pWriter.WriteUnicodeString(characterName);

        return pWriter;
    }

    public static PacketWriter UpdateHomeSizeAndHeight(byte size, byte height)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.RESPONSE_CUBE);
        pWriter.Write(ResponseCubePacketMode.UpdateSizeHeight);
        pWriter.WriteByte();
        pWriter.WriteByte(size);
        pWriter.WriteByte(height);

        return pWriter;
    }
}
