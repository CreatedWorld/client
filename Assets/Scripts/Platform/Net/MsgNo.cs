namespace Platform.Net
{
    public enum MsgNoS2C
    {
        S2C_LOGIN = 1001,//用户登录
        S2C_Hall_CREATE_ROOM = 2002,//大厅接收到客户端创建房间接口
        S2C_Hall_GET_CHECKIN_INFO = 2004,//获取签到信息
        S2C_Hall_CHECKIN = 2005,//点击签到接口
        S2C_Hall_GET_USERINFO = 2006,//获取用户信息
        S2C_Hall_GET_CHEKCIN_REWARD = 2008,//领取签到奖励接口
        S2C_Hall_JOIN_IN_ROOM = 2009,//大厅处理到客户端加入房间接口
        S2C_Hall_GET_BATTLE_HISTORY = 2010,//获取对战成绩
        S2C_Hall_GET_BATTLE_DETAIL = 2011,//获取战绩详细信息
        S2C_Hall_CHECK_APPLY_STATUS = 2012,//客户端检查报名赛信息
        S2C_Hall_APPLY_COMPETITION = 2013,//申请报名比赛
        S2C_Hall_JOIN_COMPETITION = 2014,//参加报名比赛
        S2C_Hall_Check_Invitation_Code = 2015,//商城验证
        S2C_Hall_PUSH_ANNOUNCE = 2016,//推送公告给前端
        S2C_Hall_Match = 2017,//返回玩家匹配操作
        S2C_Hall_Push_Match_Suc = 2018,//推送匹配成功操作给玩家
        S2C_Hall_Cancel_Match = 2019,//玩家取消匹配操作
        S2C_Hall_Get_Ranking_List = 2020,//服务器返回排行榜消息
        
        S2C_Hall_Push_Notice = 2021,//登录成功后，推送玩家notice、不变的公告、客服配置到前端
        S2C_Hall_Get_UserInfo_By_Id = 2022,//获取指定玩家id信息
        S2C_Hall_Get_Replay_Data = 2023,//获取回放数据
        S2C_Hall_Reconnect = 2024,//返回玩家重连消息
        S2C_Hall_Push_Item = 2025,//道具发生变化，推送消息给客户端
        S2C_HALL_BATTLEBEAT = 2026,//返回大厅心跳
        S2C_ENTER_ROOM = 4000,//响应登录
        S2C_ROOM_JOIN_ROOM = 4001,//返回加入房间
        S2C_ROOM_READY_BROADCAST = 4002,//广播玩家点击准备按钮事件
        S2C_ROOM_JOIN_ROOM_BROADCAST = 4003,//加入房间广播 
        S2C_ROOM_DISSOLVE_BROADCAST = 4004,//广播解散房间
        S2C_ROOM_EXIT_BROADCAST = 4005,//EXIT room
        S2C_ROOM_APPLY_DISSOLVE = 4006,//申请取消解散房间
        S2C_ROOM_CANCEL_APPLY_DISSOLVE = 4007,//取消申请解散房间
        S2C_ROOM_Comfirm_Dissolve = 4008,//
        S2C_ROOM_GAME_START_BROADCAST = 4010,//游戏开始广播
        S2C_ROOM_PLAY_A_MAHJONG = 4011,//广播玩家在房间出牌的操作
        S2C_ROOM_PASS = 4012,//
        S2C_ROOM_PENG = 4013,//
        S2C_ROOM_CHI = 4014,//
        S2C_ROOM_PLAYER_ACT_BROADCAST = 4015,//推送动作响应给玩家
        S2C_ROOM_PLAYER_ACT_TIP_BROADCAST = 4016,//玩家操作提示广播
        S2C_ROOM_COMMON_AN_GANG = 4020,//
        S2C_ROOM_BACK_AN_GANG = 4021,//
        S2C_ROOM_ZHI_GANG = 4022,//
        S2C_ROOM_COMMON_PENG_GANG = 4023,//
        S2C_ROOM_BACK_PENG_GANG = 4024,//
        S2C_ROOM_ZI_MO_HU = 4030,//
        S2C_ROOM_QIANG_ZHI_GANG_HU = 4032,//
        S2C_ROOM_QIANG_AN_GANG_HU = 4033,//
        S2C_ROOM_QIANG_PENG_GANG_HU = 4034,//
        S2C_ROOM_CHI_HU = 4035,//
        S2C_ROOM_SINGLE_SCORE_BROADCAST = 4040,//单局结算广播 
        S2C_ROOM_TOTAL_SCORE_BROADCAST = 4041,//总结算广播 
        S2C_ROOM_ActErrorS2C = 4050,//出错的动作类型错误码
        S2C_ROOM_BATTLEBEAT = 4051,//返回房间心跳
        S2C_ROOM_ONLINESETTING_BROADCAST = 4052,//在线/离线状态设置广播
        S2C_ROOM_TEXT_CHAT = 4060,//发送文字聊天
        S2C_ROOM_VOICE_CHAT = 4061,//发送语音
        CORE_SSSGAMEEXERCISE_S2C = 11000,//游戏开始,练习场发牌
        CORE_SSSGAMESTART_S2C = 11001,//游戏开始,发牌
        CORE_SSSPUTOUTCARD_S2C = 11002,//客户端出牌
        CORE_PUSHSSSMATCHRESULT = 11003,//服务端推送单局成绩
        CORE_PUSHSSSROOMRESULT_S2C = 11004,//房间总结算
        S2C_Hall_Remove_AGENT = 92001,//返回解除绑定是否成功信息
        S2C_CoreHall_OrederInfoC2S = 92002,//返回购买商品
        S2C_SetCardS2C = 93001,//设置牌面返回(麻将)
        S2C_GetAllCardS2C = 93002,//返回当前牌桌内所有牌的集合，包括了每个玩家的手牌，剩余牌堆中的牌
        S2C_CoreBattle_GameStart = 93003,//游戏开始

        S2C_Hall_Get_HallRoomList = 2027,//服务器返回大厅显示的房间列表
    }


    public enum MsgNoC2S
    {
        C2S_LOGIN = 1001,//用户登录
        C2S_Hall_CREATE_ROOM = 2002,//大厅接收到客户端创建房间接口
        C2S_Hall_GET_CHECKIN_INFO = 2004,//获取签到信息
        C2S_Hall_CHECKIN = 2005,//点击签到接口
        C2S_Hall_GET_USERINFO = 2006,//获取用户信息
        C2S_Hall_GET_CHECKIN_REWARD = 2008,//领取签到奖励接口
        C2S_Hall_JOIN_IN_ROOM = 2009,//大厅接收到客户端加入房间接口
        C2S_Hall_GET_BATTLE_HISTORY = 2010,//获取对战成绩
        C2S_Hall_GET_BATTLE_DETAIL = 2011,//获取战绩详细信息
        C2S_Hall_CHECK_APPLY_STATUS = 2012,//客户端检查报名赛信息
        C2S_Hall_APPLY_COMPETITION = 2013,//申请报名比赛
        C2S_Hall_JOIN_COMPETITION = 2014,//参加报名比赛
        C2S_Hall_Check_Invitation_Code = 2015,//商城验证
        C2S_Hall_Match = 2017,//玩家匹配操作
        C2S_Hall_Cancel_Match = 2019,//玩家取消匹配操作
        C2S_Hall_Get_Ranking_List = 2020,//客户端获取排行榜信息
        C2S_Hall_Get_UserInfo_By_Id = 2022,//获取指定玩家id信息
        C2S_Hall_Get_Replay_Data = 2023,//获取回放数据
        C2S_Hall_Reconnect = 2024,//处理玩家重连消息
        C2S_HALL_BATTLEBEAT = 2026,//大厅心跳
        C2S_ENTER_ROOM = 4000,//客户端登录到房间
        C2S_ROOM_JOIN_ROOM = 4001,//加入房间
        C2S_ROOM_READY = 4002,//玩家点击准备按钮
        C2S_ROOM_DISSOLVE = 4004,//解散房间
        C2S_ROOM_EXIT = 4005,//EXIT room
        C2S_ROOM_APPLY_DISSOLVE = 4006,//申请取消解散房间
        C2S_ROOM_CANCEL_APPLY_DISSOLVE = 4007,//取消申请解散房间
        C2S_ROOM_Comfirm_Dissolve = 4008,//玩家确认解散房间
        C2S_ROOM_PLAY_A_MAHJONG = 4011,//玩家打出一张牌
        C2S_ROOM_PASS = 4012,//guo
        C2S_ROOM_PENG = 4013,//peng
        C2S_ROOM_CHI = 4014,//chi
        C2S_ROOM_COMMON_AN_GANG = 4020,//普通暗杠
        C2S_ROOM_BACK_AN_GANG = 4021,//回头暗杠
        C2S_ROOM_ZHI_GANG = 4022,//点击直杠
        C2S_ROOM_COMMON_PENG_GANG = 4023,//普通碰杠
        C2S_ROOM_BACK_PENG_GANG = 4024,//回头碰杠
        C2S_ROOM_ZI_MO_HU = 4030,//自摸
        C2S_ROOM_QIANG_ZHI_GANG_HU = 4032,//抢直杠胡
        C2S_ROOM_QIANG_AN_GANG_HU = 4033,//抢暗胡
        C2S_ROOM_QIANG_PENG_GANG_HU = 4034,//抢碰杠胡
        C2S_ROOM_CHI_HU = 4035,//吃胡
        C2S_ROOM_BATTLEBEAT = 4051,//房间心跳
        C2S_ROOM_ONLINESETTING = 4052,//在线/离线状态设置
        C2S_ROOM_TEXT_CHAT = 4060,//发送文字聊天
        C2S_ROOM_VOICE_CHAT = 4061,//发送语音
        CORE_SSSPUTOUTCARD_C2S = 11002,//客户端出牌
        CORE_SSSEXERCISEPUTOUTCARD_C2S = 11005,//客户端练习场出牌
        C2S_Hall_Remove_AGENT = 92001,//点击解除绑定按钮
        C2S_CoreHall_OrederInfoC2S = 92002,//购买商品
        C2S_SetCardC2S = 93001,//设置牌面(麻将)
        C2S_GetAllCardC2S = 93002,//获取当前牌桌内所有牌的集合，包括了每个玩家的手牌，剩余牌堆中的牌

        //C2S_Hall_Get_HallRoomList = 2047,//服务器返回大厅显示的房间列表

    }
}