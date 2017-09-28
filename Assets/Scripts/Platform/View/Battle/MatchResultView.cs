using System.Collections.Generic;
using Platform.View.Battle;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
///     单局结算界面
/// </summary>
public class MatchResultView : UIView
{
    /// <summary>
    ///     节点间距
    /// </summary>
    public float itemGap;

    /// <summary>
    ///     玩家节点数组
    /// </summary>
    public List<MatchResultPlayerItem> playerItems;

    /// <summary>
    ///     开始下一局按钮
    /// </summary>
    public Button startNextBtn;
    /// <summary>
    /// 房间号
    /// </summary>
    public Text RoomCode;
    /// <summary>
    /// 局数
    /// </summary>
    public Text CurrentInn;
    /// <summary>
    /// 玩法1
    /// </summary>
    public Text Rule1;
    /// <summary>
    ///     开始下一局按钮文本
    /// </summary>
    //public Text startNextBtnTxt;
    /// <summary>
    ///     分享按钮
    /// </summary>
    //public Button shareBtn;

    /// <summary>
    ///     单局胜败图标
    /// </summary>
    //public Image titleIcon;
    /// <summary>
    /// 动作特效
    /// </summary>
    public GameObject actEffect;

    public override void OnInit()
    {
        viewRoot = LaunchUIView("Prefab/UI/Battle/MatchResultView");
        startNextBtn = viewRoot.transform.Find("WinBg/StartNextBtn").GetComponent<Button>();
        RoomCode = viewRoot.transform.Find("WinBg/RoomId/Text").GetComponent<Text>();
        CurrentInn = viewRoot.transform.Find("WinBg/Round/Text").GetComponent<Text>();
        Rule1 = viewRoot.transform.Find("WinBg/Rule/Text").GetComponent<Text>();
        //shareBtn = viewRoot.transform.Find("WinBg/ShareBtn").GetComponent<Button>();
        //startNextBtnTxt = viewRoot.transform.Find("WinBg/StartNextBtn/Text").GetComponent<Text>();
        playerItems = new List<MatchResultPlayerItem>();
        for (var i = 0; i < GlobalData.SIT_NUM; i++)
        {
            var playerItem = viewRoot.transform.Find("WinBg/PlayerItem" + (i + 1)).GetComponent<MatchResultPlayerItem>();
            playerItems.Add(playerItem);
        }
        itemGap = playerItems[1].gameObject.GetComponent<RectTransform>().localPosition.y -
                  playerItems[0].gameObject.GetComponent<RectTransform>().localPosition.y;
        //titleIcon = viewRoot.transform.Find("TitleIcon").GetComponent<Image>();

        RoomCode.text = RoomInfo.RoomId;
        CurrentInn.text = RoomInfo.Round;
        Rule1.text = RoomInfo.Rule1 + RoomInfo.Rule2 + RoomInfo.Rule3;
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

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.MATCH_RESULT_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
        ApplicationFacade.Instance.RegisterMediator(new MatchResultViewMediator(Mediators.MATCH_RESULT_VIEW_MEDIATOR,
            this));
    }

    public override void LoadUI()
    {
        viewRootCache = Resources.Load<GameObject>("Prefab/UI/Battle/MatchResultView");
    }

    public override void OnHide()
    {
        base.OnHide();
        ApplicationFacade.Instance.RemoveMediator(Mediators.MATCH_RESULT_VIEW_MEDIATOR);
    }

    public override void OnHide(Action callBack)
    {
        base.OnHide(callBack);
        ApplicationFacade.Instance.RemoveMediator(Mediators.MATCH_RESULT_VIEW_MEDIATOR);
    }

    public override void OnDestroy()
    {
        ApplicationFacade.Instance.RemoveMediator(Mediators.MATCH_RESULT_VIEW_MEDIATOR);
        base.OnDestroy();
    }
}