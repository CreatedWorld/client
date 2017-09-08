using System.Collections.Generic;
using Platform.Model;

public class HallInfoVO
{
    /// <summary>
    /// 开房间选择的局数
    /// </summary>
    public GameMode innings;
    /// <summary>
    /// 房间号
    /// </summary>
    public string roomCode;
    /// <summary>
    /// 座位
    /// </summary>
    public int seat;
    /// <summary>
    /// 七天登陆签到天数
    /// </summary>
    public int signInDay;
    /// <summary>
    /// 七天登陆的领取状态
    /// </summary>
    public int signInState;
    /// <summary>
    /// 游戏规则
    /// </summary>
    public GameRule gameRule;
    /// <summary>
    /// 91游戏规则
    /// </summary>
    public GameRule91 gameRule91;
    /// <summary>
    /// 游戏服务器IP
    /// </summary>
    public string battleSeverIP;
    /// <summary>
    /// 游戏服务器端口
    /// </summary>
    public int battleSeverPort;
    /// <summary>
    /// 公告
    /// </summary>
    public Queue<AnnouncementData> announcementQueue = new Queue<AnnouncementData>();
    /// <summary>
    /// 比赛场状态 0未报名 1已报名
    /// </summary>
    public int arenaStatus;
    /// <summary>
    /// 当前系统时间
    /// </summary>
    public long currentTime;
    /// <summary>
    /// 报名开始时间
    /// </summary>
    public long applyStartTime;
    /// <summary>
    /// 报名结束时间
    /// </summary>
    public long applyEndTime;
    /// <summary>
    /// 比赛开始时间
    /// </summary>
    public long startTime;
    /// <summary>
    /// 比赛结束时间
    /// </summary>
    public long endTime;
    /// <summary>
    /// 规则描述
    /// </summary>
    public string ruleDesc;
    /// <summary>
    /// 奖励描述
    /// </summary>
    public string rewardDesc;
    /// <summary>
    /// 消息
    /// </summary>
    public Dictionary<HallNoticeType, NoticeConfigDataS2C> noticeList = new Dictionary<HallNoticeType, NoticeConfigDataS2C>();

}
