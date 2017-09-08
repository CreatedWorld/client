using Platform.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 排行榜界面
/// </summary>
public class RankView : UIView
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;
    /// <summary>
    /// 战绩信息滚动条,控制器组件
    /// </summary>
    private TableView rankTable;
    /// <summary>
    /// 分享按钮
    /// </summary>
    private Button shareBtn;

    public override void OnInit()
    {
        viewRoot = LaunchUIView("Prefab/UI/Arena/RankView");
        closeButton = viewRoot.transform.Find("CloseButton").GetComponent<Button>();
        rankTable = viewRoot.transform.Find("RankTable").GetComponent<TableView>();
        shareBtn = viewRoot.transform.Find("ShareBtn").GetComponent<Button>();
    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_HALL;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.RANK_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
        ArrayList rankArr = new ArrayList();
        var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        rankArr.AddRange(hallProxy.ranks);
        rankTable.DataProvider = rankArr;
        closeButton.onClick.AddListener(()=> {
            UIManager.Instance.HideUI(UIViewID.RANK_VIEW);
        });
        shareBtn.onClick.AddListener(ShareHandler);
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), base.OnHide);
    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Arena/RankView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_GRADE);
    }

    /// <summary>
    /// 分享
    /// </summary>
    private void ShareHandler()
    {
        if (GlobalData.sdkPlatform == SDKPlatform.ANDROID)
        {
            string desc = "快来全民约牌吧";
            AndroidSdkInterface.WeiXinShareScreen(desc, false);
        }
        else if (GlobalData.sdkPlatform == SDKPlatform.IOS)
        {
            UIManager.Instance.StartSaveScreen((Texture2D screenShot) => {
                byte[] screenJpg = screenShot.EncodeToJPG();
                string jpgBase64 = Convert.ToBase64String(screenJpg);
                IOSSdkInterface.shareBitmap(jpgBase64, false);
            });
        }
    }
}
