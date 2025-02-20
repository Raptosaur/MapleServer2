﻿using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Types;

namespace MapleServer2.Packets;

public static class ChatStickerPacket
{
    private enum ChatStickerMode : byte
    {
        LoadChatSticker = 0x0,
        ExpiredStickerNotification = 0x1,
        AddSticker = 0x2,
        UseSticker = 0x3,
        GroupChatSticker = 0x4,
        Favorite = 0x5,
        Unfavorite = 0x6
    }

    public static PacketWriter LoadChatSticker(Player player)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.CHAT_STICKER);
        pWriter.Write(ChatStickerMode.LoadChatSticker);
        pWriter.WriteShort((short) player.FavoriteStickers.Count);
        foreach (int favorite in player.FavoriteStickers)
        {
            pWriter.WriteInt(favorite);
        }
        pWriter.WriteShort((short) player.ChatSticker.Count);
        foreach (ChatSticker stickerGroup in player.ChatSticker)
        {
            pWriter.WriteInt(stickerGroup.GroupId);
            pWriter.WriteLong(stickerGroup.Expiration);
        }
        return pWriter;
    }

    public static PacketWriter ExpiredStickerNotification()
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.CHAT_STICKER);
        pWriter.Write(ChatStickerMode.ExpiredStickerNotification);
        pWriter.WriteInt();
        pWriter.WriteInt(1);
        return pWriter;
    }

    public static PacketWriter AddSticker(int itemId, int stickerGroupId, long expiration = 9223372036854775807)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.CHAT_STICKER);
        pWriter.Write(ChatStickerMode.AddSticker);
        pWriter.WriteInt(itemId);
        pWriter.WriteInt(1);
        pWriter.WriteInt(stickerGroupId);
        pWriter.WriteLong(expiration);
        return pWriter;
    }

    public static PacketWriter UseSticker(int stickerId, string script)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.CHAT_STICKER);
        pWriter.Write(ChatStickerMode.UseSticker);
        pWriter.WriteInt(stickerId);
        pWriter.WriteUnicodeString(script);
        pWriter.WriteByte();
        return pWriter;
    }

    public static PacketWriter GroupChatSticker(int stickerId, string groupChatName)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.CHAT_STICKER);
        pWriter.Write(ChatStickerMode.GroupChatSticker);
        pWriter.WriteInt(stickerId);
        pWriter.WriteUnicodeString(groupChatName);
        return pWriter;
    }

    public static PacketWriter Favorite(int stickerId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.CHAT_STICKER);
        pWriter.Write(ChatStickerMode.Favorite);
        pWriter.WriteInt(stickerId);
        return pWriter;
    }

    public static PacketWriter Unfavorite(int stickerId)
    {
        PacketWriter pWriter = PacketWriter.Of(SendOp.CHAT_STICKER);
        pWriter.Write(ChatStickerMode.Unfavorite);
        pWriter.WriteInt(stickerId);
        return pWriter;
    }
}
