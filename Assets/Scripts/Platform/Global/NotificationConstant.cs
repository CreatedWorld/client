/// <summary>
/// Message定义
/// </summary>
public class NotificationConstant
{
    /// <summary>
    /// 游戏启动
    /// </summary>
    public const string COMM_GAMEMGR_INIT = "COMM_GAMEMGR_INIT";
    /// <summary>
    /// 检测版本
    /// </summary>
    public const string COMM_CHECK_VERSION = "COMM_CHECK_VERSION";
    /// <summary>
    /// 切换场景，body为切换的场景信息
    /// </summary>
    public const string MEDI_GAMEMGR_LOADSCENE = "MEDI_GAMEMGR_LOADSCENE";
    /// <summary>
    /// 发送连接游戏大厅
    /// </summary>
    public const string MEDI_GAMEMGR_CONNECTSERVER = "COMM_HALL_CONNECTSERVER";
    /// <summary>
    /// 创建房间跳场景
    /// </summary>
    public const string MEDI_HALL_CUTCREATESCENE = "MEDI_HALL_CUTCREATESCENE";
    /// <summary>
    /// 加入房间跳场景
    /// </summary>
    public const string MEDI_HALL_CUTJOINSCENE = "MEDI_HALL_CUTJOINSCENE";
    /// <summary>
    /// 商品信息更新
    /// </summary>
    public const string MEDI_HALL_PRODUCTUPDATE = "MEDI_HALL_PRODUCTUPDATE";
    /// <summary>
    /// 查看房间内记录
    /// </summary>
    public const string MEDI_HALL_VIEWROOM_RECORD = "MEDI_HALL_VIEWROOM_RECORD";
    /// <summary>
    /// 初始化战绩消息
    /// </summary>
    public const string MEDI_HALL_INITGRADEINFO = "MEDI_HALL_INITGRADEINFO";
    /// <summary>
    /// 从服务器获取战绩消息
    /// </summary>
    public const string MEDI_HALL_GETGRADEINFO = "MEDI_HALL_GETGRADEINFO";
    /// <summary>
    /// 获取对战消息
    /// </summary>
    public const string MEDI_HALL_GETROUNDINFO = "MEDI_HALL_GETROUNDINFO";
    /// <summary>
    /// 报名成功消息
    /// </summary>
    public const string MEDI_HALL_APPLYSUCCEED = "MEDI_HALL_APPLYSUCCEED";
    /// <summary>
    /// 刷新大厅玩家信息
    /// </summary>
    public const string MEDI_HALL_REFRESHUSERINFO = "MEDI_HALL_REFRESHUSERINFO";
    //战斗模块消息
    /// <summary>
    /// 听牌变更
    /// </summary>
    public const string TING_UPDATE = "TING_UPDATE";
    /// <summary>
    /// 显示牌面箭头，body为标识所在的BattleAreaItem
    /// </summary>
    public const string SHOW_CARD_ARROW = "SHOW_CARD_ARROW";
    /// <summary>
    /// 自己准备完成
    /// </summary>
    public const string MEDI_READY_COMPLETE = "MEDI_READY_COMPLETE";
    /// <summary>
    /// 更新所有头像信息
    /// </summary>
    public const string MEDI_BATTLEVIEW_UPDATEALLHEAD = "MEDI_BATTLEVIEW_UPDATEALLHEAD";
    /// <summary>
    /// 更新用户分数
    /// </summary>
    //public const string MEDI_BATTLEVIEW_UPDATESCORE = "MEDI_BATTLEVIEW_UPDATESCORE";
    /// <summary>
    /// 更新单个头像信息，body为要更新的PlayerInfoVO
    /// </summary>
    public const string MEDI_BATTLEVIEW_UPDATESINGLEHEAD = "MEDI_BATTLEVIEW_UPDATESINGLEHEAD";
    /// <summary>
    /// 更新麦克风音量,音量大小1-5
    /// </summary>
    public const string MEDI_BATTLEVIEW_UPDATEVOLUME = "MEDI_BATTLEVIEW_UPDATEVOLUME";
    /// <summary>
    /// 战斗场景播放色子旋转动画
    /// </summary>
    public const string MEDI_BATTLE_PLAYROTATE = "MEDI_BATTLE_PLAYROTATE";
    /// <summary>
    /// 播放玩家动作提示
    /// </summary>
    public const string MEDI_BATTLE_PLAYACTTIP = "MEDI_BATTLE_PLAYACTTIP";
    /// <summary>
    /// 播放玩家直杠
    /// </summary>
    public const string MEDI_BATTLE_PLAYZHIGANG = "MEDI_BATTLE_PLAYZHIGANG";
    /// <summary>
    /// 播放玩家普通暗杠
    /// </summary>
    public const string MEDI_BATTLE_PLAY_COMMONANGANG = "MEDI_BATTLE__PLAY_COMMONANGANG";
    /// <summary>
    /// 播放玩家回头暗杠
    /// </summary>
    public const string MEDI_BATTLE_PLAY_BACKANGANG = "MEDI_BATTLE__PLAY_BACKANGANG";
    // <summary>
    /// 播放玩家回头碰杠
    /// </summary>
    public const string MEDI_BATTLE_PLAY_BACKPENGGANG = "MEDI_BATTLE__PLAY_BACKPENGGANG";
    // <summary>
    /// 播放玩家普通股碰杠
    /// </summary>
    public const string MEDI_BATTLE_PLAY_COMMONPENGGANG = "MEDI_BATTLE__PLAY_COMMON_PENGGANG";
    /// <summary>
    /// 播放玩家摸牌
    /// </summary>
    public const string MEDI_BATTLE_PLAYGETCARD = "MEDI_BATTLE_PLAYGETCARD";
    /// <summary>
    /// 播放玩家胡牌
    /// </summary>
    public const string MEDI_BATTLE_PLAYHU = "MEDI_BATTLE_PLAYHU";
    /// <summary>
    /// 播放玩家碰牌
    /// </summary>
    public const string MEDI_BATTLE_PLAYPENG = "MEDI_BATTLE_PLAYPENG";
    /// <summary>
    /// 播放玩家吃牌
    /// </summary>
    public const string MEDI_BATTLE_PLAYCHI = "MEDI_BATTLE_PLAYCHI";
    /// <summary>
    /// 播放重置牌面
    /// </summary>
    public const string MEDI_BATTLE_RESET = "MEDI_BATTLE_RESET";
    /// <summary>
    /// 初始化牌池
    /// </summary>
    public const string MEDI_INIT_CARDPOOL = "MEDI_INIT_CARDPOOL";
    /// <summary>
    /// 播放过动画
    /// </summary>
    public const string MEDI_BATTLE_PLAYPASS = "MEDI_BATTLE_PLAYPASS";
    /// <summary>
    /// 播放出牌
    /// </summary>
    public const string MEDI_BATTLE_PLAYPUTCARD = "MEDI_BATTLE_PLAYPUTCARD";
    /// <summary>
    /// 隐藏录音中标志
    /// </summary>
    public const string MEDI_BATTLEVIEW_HIDENRECORDING = "MEDI_BATTLEVIEW_HIDENRECORDING";
    /// <summary>
    /// 显示语音播放标志
    /// </summary>
    public const string MEDI_BATTLEVIEW_SHOWPLAYINGVOICE = "MEDI_BATTLEVIEW_SHOWPLAYINGVOICE";
    /// <summary>
    /// 显示聊天文字内容,body内容PushSendChatS2C
    /// </summary>
    public const string MEDI_BATTLEVIEW_SHOWCHAT = "MEDI_BATTLEVIEW_SHOWCHAT";
    /// <summary>
    /// 显示聊天表情,body内容PushSendChatS2C
    /// </summary>
    public const string MEDI_BATTLEVIEW_SHOWFACE = "MEDI_BATTLEVIEW_SHOWFACE";
    /// <summary>
    /// 设置玩家在线状态,body内容在线变更的玩家id
    /// </summary>
    public const string MEDI_BATTLEVIEW_UPDATEONLINE = "MEDI_BATTLEVIEW_UPDATEONLINE";
    /// <summary>
    /// 隐藏语言播放标志
    /// </summary>
    public const string MEDI_BATTLEVIEW_HIDENPLAYINGVOICE = "MEDI_BATTLEVIEW_HIDENPLAYINGVOICE";
    /// <summary>
    /// 开始录音
    /// </summary>
    public const string MEDI_BATTLEREA_STARTRECORD = "MEDI_BATTLEREA_STARTRECORD";
    /// <summary>
    /// 停止录音
    /// </summary>
    public const string MEDI_BATTLEREA_STOPRECORD = "MEDI_BATTLEREA_STOPRECORD";
    /// <summary>
    /// 重置所有玩家的牌面
    /// </summary>
    public const string MEDI_BATTLEVIEW_INITPLAYERCARDS = "MEDI_BATTLEVIEW_INITPLAYERCARDS";
    /// <summary>
    /// 头像框显示庄家标志
    /// </summary>
    public const string MEDI_BATTLEVIEW_SHOWBANKERICON = "MEDI_BATTLEVIEW_SHOWBANKERICON";
    /// <summary>
    /// 显示发牌动画
    /// </summary>
    public const string MEDI_BATTLE_SENDCARD = "MEDI_BATTLE_SENDCARD";
    /// <summary>
    /// 显示单局结算界面
    /// </summary>
    public const string MEDI_BATTLEVIEW_SHOWMATCHRESULT = "MEDI_BATTLEVIEW_SHOWMATCHRESULT";
    /// <summary>
    /// 房间结束
    /// </summary>
    public const string MEDI_BATTLEVIEW_SHOWROOMRESULT = "MEDI_BATTLEVIEW_SHOWROOMRESULT";
    /// <summary>
    /// 显示战报播放UI
    /// </summary>
    public const string MEDI_BATTLEVIEW_SHOW_REPORTVIEW = "MEDI_BATTLEVIEW_SHOW_REPORTVIEW";
    /// <summary>
    /// 刷新解散统计界面
    /// </summary>
    public const string UPDATE_DISLOVE_STATISTICS = "UPDATE_DISLOVE_STATISTICS";
    /// <summary>
    /// 准备下一局
    /// </summary>
    public const string READY_NEXT = "READY_NEXT";
    /// <summary>
    /// 隐藏色子
    /// </summary>
    //public const string HIDESAIZI = "HIDESAIZI";
    /// <summary>
    /// 刷新签到界面
    /// </summary>
    public const string MEDI_SIGNIN_REFRESHSIGN = "MEDI_SIGNIN_REFRESHSIGN";
    /// <summary>
    /// 刷新玩家货币
    /// </summary>
    public const string MEDI_HALL_REFRESHITEM = "MEDI_HALL_REFRESHITEM";
    /// <summary>
    /// 刷新喇叭
    /// </summary>
    public const string MEDI_HALL_REFRESHANNOUNCEMENT = "MEDI_HALL_REFRESHANNOUNCEMENT";
    /// <summary>
    /// 喇叭播放完毕
    /// </summary>
    public const string MEDI_HALL_ANNOUNCEMENTFINISH = "MEDI_HALL_ANNOUNCEMENTFINISH";
    /// <summary>
    /// 登陆切换到大厅
    /// </summary>
    public const string MEDI_LOGIN_SWITCHHALLSCENE = "MEDI_LOGIN_SWITCHHALLSCENE";
    /// <summary>
    /// 刷新大厅公告
    /// </summary>
    public const string MEDI_HALL_REFRESHHALLNOTICE = "MEDI_HALL_REFRESHNOTICE";
    /// <summary>
    /// 重置匹配时间
    /// </summary>
    public const string MEDI_HALL_INITMATCHINGTIME = "MEDI_HALL_INITMATCHINGTIME";
    /// <summary>
    /// 清楚验证码输入
    /// </summary>
    public const string MEDI_HALL_CLEARINPUTTEXT = "MEDI_HALL_CLEARINPUTTEXT";
    /// <summary>
    /// 微信登陆成功
    /// </summary>
    public const string MEDI_LOGIN_WXLOGINSUCCEED = "MEDI_LOGIN_WXLOGINSUCCEED";
    /// <summary>
    /// 接到启动参数
    /// </summary>
    public const string MEDI_RECIVE_STARTUP = "MEDI_RECIVE_STARTUP";
    /// <summary>
    /// 大厅的房间列表
    /// </summary>
    public const string MEDI_HALL_ROOMLIST = "MEDI_HALL_ROOMLIST";
    /// <summary>
    /// 显示投河
    /// </summary>
    public const string MEDI_ROOM_TOUHE = "MEDI_ROOM_TOUHE";
    /// <summary>
    /// 隐藏投河按钮
    /// </summary>
    public const string MEDI_ROOM_HIDETOUHE = "MEDI_ROOM_HIDETOUHE";
    /// <summary>
    /// 设置报听
    /// </summary>
    public const string MEDI_ROOM_BAOTING = "MEDI_ROOM_BAOTING";
    /// <summary>
    /// 设置报吊
    /// </summary>
    public const string MEDI_ROOM_BAODIAO = "MEDI_ROOM_BAODIAO";
    /// <summary>
    /// 设置报夹
    /// </summary>
    public const string MEDI_ROOM_BAOJIA = "MEDI_ROOM_BAOJIA";
    /// <summary>
    /// 设置抻吊
    /// </summary>
    public const string MEDI_ROOM_CHENDIAO = "MEDI_ROOM_CHENDIAO";


}

/// <summary>
/// View模块定义
/// </summary>
public class Mediators
{
    /// <summary>
    /// 游戏管理
    /// </summary>
    public const string GAMEMGR_MEDIATOR = "GAMEMGR_MEDIATOR";
    /// <summary>
    /// 登陆
    /// </summary>
    public const string LOGIN_MGR_MEDIATOR = "LOGIN_MEDIATOR";
    /// <summary>
    /// 大厅
    /// </summary>
    public const string HALL_MEDIATOR = "HALL_MEDIATOR";
    public const string HALL_ACTIVITY = "HALL_ACTIVITY";
    public const string HALL_GRADE = "HALL_GRADE";
    public const string HALL_HELP = "HALL_HELP";
    public const string HALL_INVITE = "HALL_INVITE";
    public const string HALL_PLAYERINFO = "HALL_PLAYERINFO";
    public const string HALL_SETTING = "HALL_SETTING";
    public const string HALL_SHOPPING = "HALL_SHOPPING";
    public const string HALL_SHARE = "HALL_SHARE";
    public const string HALL_SIGNIN = "HALL_SIGNIN";
    public const string HALL_CREATEROOM = "HALL_CREATE_ROOM";
    public const string HALL_JOINROOM = "HALL_JOIN_ROOM";
    /// <summary>
    /// 比赛场中介
    /// </summary>
    public const string ARENA_MEDIATOR = "ARENA_MEDIATOR";
    /// <summary>
    /// 战斗UI代理
    /// </summary>
    public const string BATTLE_VIEW_MEDIATOR = "BATTLE_VIEW_MEDIATOR";
    /// <summary>
    /// 战斗区域战位
    /// </summary>
    public const string BATTLE_AREA_MEDIATOR = "BATTLE_AREA_MEDIATOR";
    /// <summary>
    /// 聊天界面中介
    /// </summary>
    public const string CHAT_VIEW_MEDIATOR = "CHAT_VIEW_MEDIATOR";
    /// <summary>
    /// 单局战斗结果界面中介
    /// </summary>
    public const string MATCH_RESULT_VIEW_MEDIATOR = "MATCH_RESULT_VIEW_MEDIATOR";
    /// <summary>
    /// 房间战斗结果界面中介
    /// </summary>
    public const string ROOM_RESULT_VIEW_MEDIATOR = "ROOM_RESULT_VIEW_MEDIATOR";
    /// <summary>
    /// 解散统计界面中介
    /// </summary>
    public const string DISLOVESTATISTICS_VIEW_MEDIATOR = "DISLOVESTATISTICS_VIEW_MEDIATOR";
    /// <summary>
    /// 匹配
    /// </summary>
    public const string HALL_MATCHING = "HALL_MATCHING";
    /// <summary>
    /// 调试中介
    /// </summary>
    public const string DEBUG_MEDIATOR = "DEBUG_MEDIATOR";
}

/// <summary>
/// Model模块定义
/// </summary>
public class Proxys
{
    public const string GAMEMGR_PROXY = "GAMEMGR_PROXY";
    public const string LOGIN_PROXY = "LOGIN_PROXY";
    /// <summary>
    /// 大厅数据代理
    /// </summary>
    public const string HALL_PROXY = "HALL_PROXY";
    public const string PLAYER_PROXY = "PLAYER_PROXY";
    /// <summary>
    /// 牌局数据代理
    /// </summary>
    public const string BATTLE_PROXY = "BATTLE_PROXY";
}