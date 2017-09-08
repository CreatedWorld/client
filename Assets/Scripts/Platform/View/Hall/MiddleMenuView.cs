using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 加入房间中间View
/// </summary>
public class MiddleMenuView : UIView
{
    /// <summary>
    /// 创建房间按钮
    /// </summary>
    public Button createRoomButton;
    /// <summary>
    /// 加入房间按钮
    /// </summary>
    public Button joinRoomButton;
    /// <summary>
    /// 比赛场按钮
    /// </summary>
    public Button athleticsButton;
    /// <summary>
    /// 公告图片
    /// </summary>
    public RawImage noticContent;
    /// <summary>
    /// 公告文本总控件
    /// </summary>
    public Transform notice;
    /// <summary>
    /// 公告标题
    /// </summary>
    public Text noticeTitle;
    /// <summary>
    /// 公告内容
    /// </summary>
    public Text noticeText;
    /// <summary>
    /// 跑马灯文本
    /// </summary>
    public Text announcementText;
    /// <summary>
    /// 切换游戏button
    /// </summary>
    public Button switchMahjongBtn;
    /// <summary>
    /// 切换游戏类型
    /// </summary>
    public Image showMahjongTypeImg;
    /// <summary>
    /// 当前游戏类型
    /// </summary>
    public Image xiuyanMahjongImg;
    /// <summary>
    /// 当前游戏类型
    /// </summary>
    public Image kuandianMahjongImg;
    /// <summary>
    /// 宽甸麻将
    /// </summary>
    public Button kdMahjongBtn;
    /// <summary>
    /// 岫岩麻将
    /// </summary>
    public Button xyMahjongBtn;

    public override void OnInit()
    {
        viewRoot = this.LaunchUIView("Prefab/UI/Hall/MiddleMenuView", UIManager.Instance.GetUIView(UIViewID.HALL_VIEW).viewRoot.transform);
        createRoomButton = this.viewRoot.transform.FindChild("CreateRoom").FindChild("CreateRoomButton").GetComponent<Button>();
        joinRoomButton = this.viewRoot.transform.FindChild("JoinRoom").FindChild("JoinRoomButton").GetComponent<Button>();
        athleticsButton = this.viewRoot.transform.FindChild("Athletics").FindChild("AthleticsButton").GetComponent<Button>();
        noticContent = this.viewRoot.transform.FindChild("Notice").FindChild("ContentImage").GetComponent<RawImage>();
        notice = this.viewRoot.transform.FindChild("Notice").FindChild("NoticeText");
        noticeTitle = this.notice.FindChild("Title").GetComponent<Text>();
        noticeText = this.notice.FindChild("ContentText").GetComponent<Text>();
        announcementText = this.viewRoot.transform.FindChild("Announcement").FindChild("Mask").FindChild("Text").GetComponent<Text>();


        switchMahjongBtn = viewRoot.transform.FindChild("SwitchMahjongBG/switchbtn").GetComponent<Button>();
        showMahjongTypeImg = viewRoot.transform.Find("SwitchMahjongBG/ShowMahjongType").GetComponent<Image>();
        kdMahjongBtn = viewRoot.transform.Find("SwitchMahjongBG/ShowMahjongType/hdMahjong").GetComponent<Button>();
        xyMahjongBtn = viewRoot.transform.Find("SwitchMahjongBG/ShowMahjongType/xyMahjong").GetComponent<Button>();
        xiuyanMahjongImg = viewRoot.transform.Find("SwitchMahjongBG/xiuyanImg").GetComponent<Image>();
        kuandianMahjongImg = viewRoot.transform.Find("SwitchMahjongBG/kuandianImg").GetComponent<Image>();

        switchMahjongBtn.onClick.AddListener(()=> {
            showMahjongTypeImg.gameObject.SetActive(true);
        });
        kdMahjongBtn.onClick.AddListener(()=>
        {
            kuandianMahjongImg.gameObject.SetActive(true);
            xiuyanMahjongImg.gameObject.SetActive(false);
            showMahjongTypeImg.gameObject.SetActive(false);
        });
        xyMahjongBtn.onClick.AddListener(()=>
        {
            kuandianMahjongImg.gameObject.SetActive(false);
            xiuyanMahjongImg.gameObject.SetActive(true);
            showMahjongTypeImg.gameObject.SetActive(false);
        });
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

    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Hall/MiddleMenuView");
    }
}
