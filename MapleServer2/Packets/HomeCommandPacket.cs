﻿using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Types;

namespace MapleServer2.Packets;

public class HomeCommandPacket
{
    private enum HomeCommandMode : byte
    {
        Load = 0x00,
        UpdateArchitectScore = 0x01
    }

    public static PacketWriter LoadHome(Player player)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.HOME_COMMAND);
        pWriter.Write(HomeCommandMode.Load);
        pWriter.WriteLong(player.AccountId);
        pWriter.WriteLong(); // last time player nominated home

        return pWriter;
    }

    public static PacketWriter UpdateArchitectScore(int ownerObjectId, int architectScoreCurrent, int architectScoreTotal)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.HOME_COMMAND);
        pWriter.Write(HomeCommandMode.UpdateArchitectScore);
        pWriter.WriteInt(ownerObjectId);
        pWriter.WriteLong(TimeInfo.Now());
        pWriter.WriteInt(architectScoreCurrent);
        pWriter.WriteInt(architectScoreTotal);

        return pWriter;
    }
}
