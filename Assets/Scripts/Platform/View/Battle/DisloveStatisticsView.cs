using Platform.View.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 解散申请统计界面
/// </summary>
class DisloveStatisticsView : UIView
{
    /// <summary>
    /// 玩家名称数组
    /// </summary>
    public List<Text> nameTxtArr;
    /// <summary>
    /// 状态文本数组
    /// </summary>
    public List<Text> statusTxtArr;
    /// <summary>
    /// 剩余时间数组
    /// </summary>
    public Text remainTimeTxt;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button closeBtn;
    public override void OnInit()
    {
        viewRoot = LaunchUIView("Prefab/UI/Battle/DisloveStatisticsView");
        nameTxtArr = new List<Text>();
        statusTxtArr = new List<Text>();
        remainTimeTxt = viewRoot.transform.Find("RemainTimeTxt").GetComponent<Text>();
        closeBtn = viewRoot.transform.Find("CloseBtn").GetComponent<Button>();
        var nameTxt1 = viewRoot.transform.Find("NameTxt1").GetComponent<Text>();
        var nameTxt2 = viewRoot.transform.Find("NameTxt2").GetComponent<Text>();
        var nameTxt3 = viewRoot.transform.Find("NameTxt3").GetComponent<Text>();
        var nameTxt4 = viewRoot.transform.Find("NameTxt4").GetComponent<Text>();
        nameTxtArr.Add(nameTxt1);
        nameTxtArr.Add(nameTxt2);
        nameTxtArr.Add(nameTxt3);
        nameTxtArr.Add(nameTxt4);

        var statusTxt1 = viewRoot.transform.Find("StatusTxt1").GetComponent<Text>();
        var statusTxt2 = viewRoot.transform.Find("StatusTxt2").GetComponent<Text>();
        var statusTxt3 = viewRoot.transform.Find("StatusTxt3").GetComponent<Text>();
        var statusTxt4 = viewRoot.transform.Find("StatusTxt4").GetComponent<Text>();
        statusTxtArr.Add(statusTxt1);
        statusTxtArr.Add(statusTxt2);
        statusTxtArr.Add(statusTxt3);
        statusTxtArr.Add(statusTxt4);
        ApplicationFacade.Instance.RegisterMediator(new DisloveStatisticsViewMediator(Mediators.DISLOVESTATISTICS_VIEW_MEDIATOR, this));
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
        viewRootCache = Resources.Load<GameObject>("Prefab/UI/Battle/DisloveStatisticsView");
    }

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.DISLOVE_STATISTICS_VIEW);
        UIManager.Instance.ShowDOTween(viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(viewRoot.GetComponent<RectTransform>(), base.OnHide);
        ApplicationFacade.Instance.RemoveMediator(Mediators.DISLOVESTATISTICS_VIEW_MEDIATOR);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.DISLOVESTATISTICS_VIEW_MEDIATOR);
    }

}
