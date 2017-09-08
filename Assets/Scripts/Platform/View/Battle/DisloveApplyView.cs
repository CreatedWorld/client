using Platform.Model;
using Platform.Model.Battle;
using Platform.Net;
using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

/// <summary>
/// 解散申请界面
/// </summary>
class DisloveApplyView : UIView
{
    /// <summary>
    /// 定时器id
    /// </summary>
    private int timeId;
    /// <summary>
    /// 战斗模块数据中介
    /// </summary>
    private BattleProxy battleProxy;
    /// <summary>
    /// 游戏数据中介
    /// </summary>
    private GameMgrProxy gameMgrProxy;
    /// <summary>
    /// 取消按钮
    /// </summary>
    public Button cancelBtn;
    /// <summary>
    /// 确定按钮
    /// </summary>
    public Button confirmBtn;
    /// <summary>
    /// 提示内容
    /// </summary>
    public Text contentTxt;
    /// <summary>
    /// 倒计时时间
    /// </summary>
    public Text remainTimeTxt;

    public override void OnInit()
    {
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        gameMgrProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;

        viewRoot = LaunchUIView("Prefab/UI/Battle/DisloveApplyView");
        cancelBtn = viewRoot.transform.Find("CancelBtn").GetComponent<Button>();
        confirmBtn = viewRoot.transform.Find("ConfirmBtn").GetComponent<Button>();
        remainTimeTxt = viewRoot.transform.Find("ConfirmBtn/RemainTimeTxt").GetComponent<Text>();
        contentTxt = viewRoot.transform.Find("ContentTxt").GetComponent<Text>();

        confirmBtn.onClick.AddListener(ConfirmDisloveHandler);
        cancelBtn.onClick.AddListener(CancelDisloveHandler);

        contentTxt.text = string.Format("你的牌友{0}正在", battleProxy.playerIdInfoDic[battleProxy.disloveApplyUserId].name);
        timeId = Timer.Instance.AddTimer(1, 0, 1, UpdateRemainTime);
        UpdateRemainTime();
    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_BATTLE;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    public override void LoadUI()
    {
        viewRootCache = Resources.Load<GameObject>("Prefab/UI/Battle/DisloveApplyView");
    }

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.DISLOVE_APPLY_VIEW);
        UIManager.Instance.ShowDOTween(viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(viewRoot.GetComponent<RectTransform>(), base.OnHide);
        Timer.Instance.CancelTimer(timeId);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Timer.Instance.CancelTimer(timeId);
    }

    /// <summary>
    /// 更新剩余时间
    /// </summary>
    private void UpdateRemainTime()
    {
        long curRemainTime = battleProxy.disloveRemainTime * 1000 - (gameMgrProxy.systemTime - battleProxy.disloveRemainUT);
        curRemainTime = curRemainTime / 1000;
        remainTimeTxt.text = string.Format("({0})", Mathf.CeilToInt(curRemainTime));
    }

    /// <summary>
    /// 同意解散请求
    /// </summary>
    private void ConfirmDisloveHandler()
    {
        var disloveC2S = new DissloveRoomConfirmC2S();
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_Comfirm_Dissolve.GetHashCode(), 0, disloveC2S,true);
        UIManager.Instance.HideUI(UIViewID.DISLOVE_APPLY_VIEW);
        Debug.Log("已发送同意解散房间。。。");
    }

    /// <summary>
    /// 取消解散请求
    /// </summary>
    private void CancelDisloveHandler()
    {
        var disloveC2S = new CancelDissolveRoomC2S();
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_CANCEL_APPLY_DISSOLVE.GetHashCode(), 0, disloveC2S,true);
        UIManager.Instance.HideUI(UIViewID.DISLOVE_APPLY_VIEW);
    }
}
