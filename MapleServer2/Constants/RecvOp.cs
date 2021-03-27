﻿namespace MapleServer2.Constants
{
    public enum RecvOp : ushort
    {
        NULL = 0x0000,
        RESPONSE_VERSION = 0x0001,
        RESPONSE_LOGIN = 0x0003,
        RESPONSE_KEY = 0x0004,
        PONG = 0x0006,
        CHARACTER_MANAGEMENT = 0x0009,
        RESPONSE_HEARTBEAT = 0x000A,
        REQUEST_TIME_SYNC = 0x000B,
        RESPONSE_CLIENTTICK_SYNC = 0x000C,
        REQUEST_QUIT = 0x000D,
        REQUEST_LOAD_UGC_MAP = 0x000F,
        RESPONSE_FIELD_ENTER = 0x0010,
        USER_CHAT = 0x0011,
        USER_SYNC = 0x0012,
        EMOTION = 0x0013,
        REQUEST_SERVER_LIST = 0x0014,
        REQUEST_ITEM_CREATE = 0x0015,
        ITEM_EQUIP = 0x0016,
        REQUEST_ITEM_INVENTORY = 0x0017,
        REQUEST_ITEM_STORAGE = 0x0018,
        REQUEST_ITEM_USE = 0x0019,
        REQUEST_ITEM_USE_MULTIPLE = 0x001A,
        REQUEST_ITEM_PICKUP = 0x001C,
        REQUEST_MONEY_PICKUP = 0x001D,
        CHARACTER_INFO = 0x001E,
        REVIVAL = 0x001F,
        SKILL = 0x0020,
        STATE_SKILL = 0x0021,
        NPC_TALK = 0x0022,
        BEAUTY = 0x0023,
        REQUEST_NPC_CREATE = 0x0024,
        JOB = 0x0025,
        VIBRATE = 0x0026,
        BREAKABLE = 0x0027,
        SHOP = 0x0028,
        QUEST = 0x0029,
        TOMBSTONE = 0x002A,
        ROOM_DUNGEON = 0x002B,
        PARTY = 0x002C,
        MAIL = 0x002D,
        FUNCTION_CUBE = 0x002E,
        TRIGGER = 0x002F,
        BUDDY = 0x0030,
        INTERACT_OBJECT = 0x0031,
        INTERACT_NPC = 0x0032,
        STATE_CONSUME_EP = 0x0033,
        STATE_FALL_DAMAGE = 0x0034,
        GUIDE_OBJECT_SYNC = 0x0035,
        REQUEST_SET_CRAFT_MODE = 0x0036,
        REQUEST_CUBE = 0x0037,
        REQUEST_APARTMENT = 0x0038,
        UGC = 0x0039,
        MERET_MARKET = 0x003A,
        KEY_TABLE = 0x003B,
        CHANNEL = 0x003C,
        LIFTABLE = 0x003D,
        MY_INFO = 0x003E,
        RELOCATE_WORLD = 0x003F,
        ITEM_FUSION = 0x0040,
        REQUEST_RIDE = 0x0041,
        RIDE_SYNC = 0x0042,
        FITTING_DOLL = 0x0043,
        BONUS_GAME = 0x0044,
        RESOLVE_PENALTY = 0x0045,
        TAKE_BOAT = 0x0046,
        TROPHY = 0x0047,
        REQUEST_GEM_EQUIPMENT = 0x0048,
        REQUEST_TAXI = 0x0049,
        TRADE_EX = 0x004A,
        REQUEST_WORLD_MAP = 0x004B,
        GUILD = 0x004C,
        GROUP_CHAT = 0x004D,
        REQUEST_HOME_BANK = 0x004E,
        REQUEST_HOME_DOCTOR = 0x004F,
        AH = 0x0050,
        SET_DEBUG_MODE = 0x0051,
        REQUEST_HOME = 0x0052,
        REQUEST_REPORT = 0x0053,
        REQUEST_MARKET_INVENTORY = 0x0054,
        REQUEST_MOVE_EVENTFIELD = 0x0055,
        LOG_SEND = 0x0056,
        DPS_MODE = 0x0057,
        GUIDE_RECORD = 0x0058,
        RANK = 0x0059,
        REQUEST_SKIP_TUTORIAL = 0x005A,
        REQUEST_ITEM_BREAK = 0x005B,
        RIDE_CONSUME_EP = 0x005C,
        REQUEST_ADDITIONAL_EFFECT = 0x005D,
        RECALL_USER = 0x005E,
        REQUEST_ITEM_ENCHANT = 0x005F,
        BLACK_MARKET = 0x0060,
        GAMBLE = 0x0061,
        PVP = 0x0062,
        MAID = 0x0063,
        NEWS_NOTIFICATION = 0x0064,
        SMART_RECOMMEND_BILLING = 0x0065,
        SYSTEM_SHOP = 0x0066,
        ATTEND_GIFT = 0x0067,
        PC_BANG_BONUS = 0x0068,
        REQUEST_MANUFACTURER = 0x0069,
        REQUEST_USER_ENV = 0x006A,
        CASH = 0x006B,
        INSIGNIA = 0x006C,
        REQUEST_MOVE_FIELD = 0x006D,
        WAITING_TICKET = 0x006E,
        MATCH_PARTY = 0x006F,
        RECALL_SCROLL = 0x0070,
        POTENTIAL_ABILITY = 0x0071,
        ENCHANT_SCROLL = 0x0072,
        GLOBAL_PORTAL = 0x0073,
        FISHING = 0x0074,
        PLAY_INSTRUMENT = 0x0075,
        CHANGE_ATTRIBUTES = 0x0076,
        CHANGE_ATTRIBUTES_SCROLL = 0x0077,
        ITEM_ENCHANT_TRANSFORM = 0x0078,
        REQUEST_PET = 0x0079,
        REQUEST_PET_INVENTORY = 0x007A,
        NOTICE_DIALOG = 0x007B,
        SKILL_COMPACT_CONTROL = 0x007C,
        BANWORD = 0x007D,
        CHECK_CHAR_NAME = 0x007E,
        PLATFORM_PROTECT_PACKET = 0x007F,
        PLATFORM_ACCOUNT_SAFE = 0x0080,
        STATE = 0x0081,
        MESO_MARKET = 0x0082,
        GLOBAL_FACTOR = 0x0083,
        SMART_PUSH = 0x0084,
        PLAY_ARCADE = 0x0085,
        ENTER_EVENTFIELD = 0x0086,
        CARD_REVERSE_GAME = 0x0087,
        REQUEST_ITEM_LOCK = 0x0088,
        ITEM_SOCKET_SYSTEM = 0x0089,
        CHARACTER_ABILITY = 0x008A,
        TUTORIAL = 0x008B,
        ITEM_SOCKET_SCROLL = 0x008C,
        PREMIUM_CLUB = 0x008F,
        CONSTRUCT_RECIPE = 0x0093,
        CONSTRUCT_SHOP = 0x0094,
        CLUB = 0x0096,
        STAT_POINT = 0x009A,
        ITEM_REPEAT = 0x009B,
        ITEM_EXCHANGE = 0x009C,
        REQUEST_TUTORIAL_ITEM = 0x009D,
        PLAYER_HOST = 0x009F,
        REQUEST_SKILL_BOOK_TREE = 0x00A3,
        BUDDY_EMOTE = 0x00A4,
        SUPER_WORLDCHAT = 0x00A8,
        ITEM_LAPENSHARD = 0x00AF,
        MICROGAME = 0x00A9,
        PRESTIGE = 0x00AE,
        RESPONSE_SERVER_ENTER = 0x00B2,
        SYSTEM_INFO = 0x00B7,
        CHAT_STICKER = 0x00B9,
        FILE_HASH = 0x00BB,
        MARRIAGE = 0x00BC
    }
}
