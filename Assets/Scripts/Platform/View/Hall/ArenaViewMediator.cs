using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using Platform.Net;
using Platform.Model;
using Utils;

/// <summary>
/// 比赛场中介
/// </summary>
public class ArenaViewMediator : Mediator, IMediator
{
    /// <summary>
    /// 大厅数据
    /// </summary>
    private HallProxy hallProxy;
    /// <summary>
    /// 游戏自身数据
    /// </summary>
    private GameMgrProxy gameMgrProxy;
    /// <summary>
    /// 刷新定时器id
    /// </summary>
    private int updateTimeId;
    /// <summary>
    /// 是否已刷新时间
    /// </summary>
    private bool isApplyData;
    public ArenaViewMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }

    public ArenaView View
    {
        get
        {
            return (ArenaView)ViewComponent;
        }
    }
    public override void OnRegister()
    {
        base.OnRegister();
        hallProxy = Facade.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        gameMgrProxy = Facade.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
        View.ruleValueTxt.text = hallProxy.HallInfo.ruleDesc;
        View.rewardValueTxt.text = hallProxy.HallInfo.rewardDesc;
        View.closeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HideUI(UIViewID.ARENA_VIEW);
        });
        View.applyBtn.onClick.AddListener(ApplyHandler);
        View.battleBtn.onClick.AddListener(BattleHandler);
        View.rankBtn.onClick.AddListener(RankHandler);
        updateTimeId = Timer.Instance.AddTimer(1, 0, 0, UpdateBtns);
    }

    public override void OnRemove()
    {
        base.OnRemove();
        Timer.Instance.CancelTimer(updateTimeId);
    }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_HALL_APPLYSUCCEED);
        return list;
    }

    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_HALL_APPLYSUCCEED:
                ShowApplySucceed();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 显示报名成功
    /// </summary>
    private void ShowApplySucceed()
    {
        var startDate = TimeHandle.Instance.GetDateTimeByTimestamp(hallProxy.HallInfo.startTime);
        DialogMsgVO dialogMsgVO = new DialogMsgVO();
        dialogMsgVO.title = "报名成功";
        dialogMsgVO.content = string.Format("报名成功, 请于{0}准时\n参加比赛。", startDate.ToString("yyyy-MM-dd HH:mm"));
        dialogMsgVO.dialogType = DialogType.ALERT;
        DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
        dialogView.data = dialogMsgVO;
        UpdateBtns();
    }

    /// <summary>
    /// 更新界面按钮
    /// </summary>
    private void UpdateBtns()
    {
        if (gameMgrProxy.systemTime < hallProxy.HallInfo.applyStartTime)
        {
            View.applyBtn.gameObject.SetActive(false);
            View.rankBtn.gameObject.SetActive(true);
            View.battleBtn.gameObject.SetActive(false);
            View.applySucessBtn.gameObject.SetActive(false);
        }
        else if (gameMgrProxy.systemTime < hallProxy.HallInfo.applyEndTime)
        {
            View.applyBtn.gameObject.SetActive(hallProxy.HallInfo.arenaStatus == 0);
            View.rankBtn.gameObject.SetActive(false);
            View.battleBtn.gameObject.SetActive(false);
            View.applySucessBtn.gameObject.SetActive(hallProxy.HallInfo.arenaStatus == 1);
        }
        else if(gameMgrProxy.systemTime < hallProxy.HallInfo.startTime)
        {
            View.applyBtn.gameObject.SetActive(false);
            View.rankBtn.gameObject.SetActive(hallProxy.HallInfo.arenaStatus == 0);
            View.battleBtn.gameObject.SetActive(false);
            View.applySucessBtn.gameObject.SetActive(hallProxy.HallInfo.arenaStatus == 1);
        }
        else if (gameMgrProxy.systemTime < hallProxy.HallInfo.endTime + 2000)
        {
            View.applyBtn.gameObject.SetActive(false);
            View.rankBtn.gameObject.SetActive(hallProxy.HallInfo.arenaStatus == 0);
            View.battleBtn.gameObject.SetActive(hallProxy.HallInfo.arenaStatus == 1);
            View.applySucessBtn.gameObject.SetActive(false);
        }
        else
        {
            View.applyBtn.gameObject.SetActive(false);
            View.rankBtn.gameObject.SetActive(true);
            View.battleBtn.gameObject.SetActive(false);
            View.applySucessBtn.gameObject.SetActive(false);
            if (!isApplyData)
            {
                CheckApplyStatusC2S package = new CheckApplyStatusC2S();
                NetMgr.Instance.SendBuff<CheckApplyStatusC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_CHECK_APPLY_STATUS.GetHashCode(), 0, package);
                isApplyData = true;
            }            
        }
        var startDate = TimeHandle.Instance.GetDateTimeByTimestamp(hallProxy.HallInfo.startTime);
        var endDate = TimeHandle.Instance.GetDateTimeByTimestamp(hallProxy.HallInfo.endTime);
        View.battleTimeValueTxt.text = string.Format("{0}至{1}", startDate.ToString("yyyy-MM-dd HH:mm"), endDate.ToString("yyyy-MM-dd HH:mm"));
        startDate = TimeHandle.Instance.GetDateTimeByTimestamp(hallProxy.HallInfo.applyStartTime);
        endDate = TimeHandle.Instance.GetDateTimeByTimestamp(hallProxy.HallInfo.applyEndTime);
        View.applyTimeValueTxt.text = string.Format("{0}至{1}", startDate.ToString("yyyy-MM-dd HH:mm"), endDate.ToString("yyyy-MM-dd HH:mm"));
    }

    /// <summary>
    /// 申请加入比赛
    /// </summary>
    private void ApplyHandler()
    {
        ApplyCompetitionC2S package = new ApplyCompetitionC2S();
        NetMgr.Instance.SendBuff<ApplyCompetitionC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_APPLY_COMPETITION.GetHashCode(), 0, package);
    }

    /// <summary>
    /// 申请匹配
    /// </summary>
    private void BattleHandler()
    {
        ApplyMatchingC2S package = new ApplyMatchingC2S();
        hallProxy.HallInfo.gameRule = GameRule.ARENA;
        hallProxy.HallInfo.innings = GameMode.ONE;
        package.roomType = (int)hallProxy.HallInfo.gameRule;
        package.roomRounds = (int)hallProxy.HallInfo.innings;
        NetMgr.Instance.SendBuff<ApplyMatchingC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_Match.GetHashCode(), 0, package);
    }

    /// <summary>
    /// 查看排行榜
    /// </summary>
    private void RankHandler()
    {
        NetMgr.Instance.SendBuff(SocketType.HALL, MsgNoC2S.C2S_Hall_Get_Ranking_List.GetHashCode(), 0, new GetRankC2S());
    }
}