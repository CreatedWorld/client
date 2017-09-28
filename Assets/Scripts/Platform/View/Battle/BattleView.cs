using Platform.Model.Battle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗UI界面
/// </summary>
public class BattleView : UIView
{
    /// <summary>
    /// 当前日期
    /// </summary>
    public Text dateTxt;
    /// <summary>
    /// 当前时间
    /// </summary>
    public Text timeTxt;
    /// <summary>
    /// 房间id
    /// </summary>
    public Text roomIdTxt;
    /// <summary>
    /// 总局数
    /// </summary>
    public Text roundTxt;
    /// <summary>
    /// 剩余牌数
    /// </summary>
    public Text leftCardNumTxt;
    /// <summary>
    /// 手机信号图标
    /// </summary>
    public Image netIcon;
   
    /// <summary>
    /// 解散按钮
    /// </summary>
    public Button dissolutionBtn;
    /// <summary>
    /// 退出房间按钮
    /// </summary>
    public Button BackBtn;

    /// <summary>
    /// 发送语音
    /// </summary>
    public Button voiceBtn;
    /// <summary>
    /// 正在录音标志
    /// </summary>
    public GameObject recordingIcon;
    /// <summary>
    /// 发送快速聊天
    /// </summary>
    public Button chatBtn;
    /// <summary>
    /// 设置按钮
    /// </summary>
    public Button settingBtn;

    /// <summary>
    /// 底部玩家头像
    /// </summary>
    public GameObject downHead;
    /// <summary>
    /// 左边玩家头像
    /// </summary>
    public GameObject leftHead;
    /// <summary>
    /// 右边玩家头像
    /// </summary>
    public GameObject rightHead;
    /// <summary>
    /// 顶部头像
    /// </summary>
    public GameObject upHead;

    /// <summary>
    /// 头像列表
    /// </summary>
    public List<GameObject> headItemList;
    /// <summary>
    /// 出牌操作区域
    /// </summary>
    public OperateView operateView;
    
    /// <summary>
    /// 邀请按钮
    /// </summary>
    public Button inviteBtn;
    /// <summary>
    /// 战报播放界面
    /// </summary>
    public GameObject reportView;
    /// <summary>
    /// 玩法
    /// </summary>
    public Text ruleText;
    /// <summary>
    /// 玩法
    /// </summary>
    public Text ruleText1;
    /// <summary>
    /// 玩法
    /// </summary>
    public Text ruleText2;
    /// <summary>
    /// 问号按钮
    /// </summary>
    public Button ruleInfoBtn;
    /// <summary>
    /// back按钮的Panel
    /// </summary>
    public GameObject BackItemPanel;
    /// <summary>
    /// 解散房间
    /// </summary>
    public Button ExitRoomBtn;
    /// <summary>
    /// 离开房间
    /// </summary>
    public Button LeftRoomBtn;
    /// <summary>
    /// 色子1
    /// </summary>
    //public GameObject saizi1;
    /// <summary>
    /// 色子2
    /// </summary>
    //public GameObject saizi2;
    /// <summary>
    /// alpha渐变容器
    /// </summary>
    public CanvasGroup canvasGroup;
    /// <summary>
    /// 投河按钮
    /// </summary>
    public Button TouHeBtn;
    /// <summary>
    /// 投河ico
    /// </summary>
    //public GameObject touheIco;
    /// <summary>
    /// 报听按钮
    /// </summary>
    //public Button BaoTingBtn;
    /// <summary>
    /// 过按钮
    /// </summary>
    public Button PassBtn;

    private BattleProxy battleProxy;

    // Use this for initialization
    public override void OnInit()
    {
        viewRoot = this.LaunchUIView("Prefab/UI/Battle/BattleView");
        //saizi1 = viewRoot.transform.FindChild("saizi1").gameObject;
        //saizi2 = viewRoot.transform.FindChild("saizi2").gameObject;
        dateTxt = viewRoot.transform.Find("RoomInfoBg/DateTxt").GetComponent<Text>();
        timeTxt = viewRoot.transform.Find("RoomInfoBg/TimeTxt").GetComponent<Text>();
        roomIdTxt = viewRoot.transform.Find("RoomInfoBg/RoomIdTxt").GetComponent<Text>();
        roundTxt = viewRoot.transform.Find("RoomInfoBg/RoundTxt").GetComponent<Text>();
        leftCardNumTxt = viewRoot.transform.Find("RoomInfoBg/LeftCardNumTxt").GetComponent<Text>();
        netIcon = viewRoot.transform.Find("RoomInfoBg/NetIcon").GetComponent<Image>();
        ruleText = viewRoot.transform.Find("RoomInfoBg/Rule/Text").GetComponent<Text>();
        ruleText1 = viewRoot.transform.Find("RoomInfoBg/Rule/Text1").GetComponent<Text>();
        ruleText2 = viewRoot.transform.Find("RoomInfoBg/Rule/Text2").GetComponent<Text>();
        //touheIco = viewRoot.transform.FindChild("DownHead/touhe").gameObject;
        BackBtn = viewRoot.transform.Find("ChatView/BackBtn").GetComponent<Button>();
        BackItemPanel = viewRoot.transform.Find("ChatView/BackBtn/BackItemPanel").gameObject;
        ExitRoomBtn = viewRoot.transform.Find("ChatView/BackBtn/BackItemPanel/ExitRoomBtn").GetComponent<Button>();
        LeftRoomBtn = viewRoot.transform.Find("ChatView/BackBtn/BackItemPanel/LeftRoomBtn").GetComponent<Button>();

        ruleInfoBtn = viewRoot.transform.Find("ChatView/RuleInfoBtn").GetComponent<Button>();
        voiceBtn = viewRoot.transform.Find("ChatView/VoiceBtn").GetComponent<Button>();
        chatBtn = viewRoot.transform.Find("ChatView/ChatBtn").GetComponent<Button>();
        settingBtn = viewRoot.transform.Find("ChatView/SettingBtn").GetComponent<Button>();
        recordingIcon = viewRoot.transform.Find("ChatView/RecordingIcon").gameObject;

        downHead = viewRoot.transform.Find("DownHead").gameObject;
        leftHead = viewRoot.transform.Find("LeftHead").gameObject;
        rightHead = viewRoot.transform.Find("RightHead").gameObject;
        upHead = viewRoot.transform.Find("UpHead").gameObject;
        operateView = viewRoot.transform.Find("OperateView").GetComponent<OperateView>();
        canvasGroup = viewRoot.transform.GetComponent<CanvasGroup>();
        headItemList = new List<GameObject>();
        headItemList.Add(downHead);
        headItemList.Add(rightHead);
        headItemList.Add(upHead);
        headItemList.Add(leftHead);

        //readyBtn = viewRoot.transform.Find("ReadyBtn").gameObject.GetComponent<Button>();
        inviteBtn = viewRoot.transform.Find("InviteBtn").gameObject.GetComponent<Button>();
        reportView = viewRoot.transform.Find("ReportView").gameObject;

        TouHeBtn = viewRoot.transform.FindChild("OperateView/touhe").GetComponent<Button>();
        //BaoTingBtn = viewRoot.transform.FindChild("OperateView/baoting").GetComponent<Button>();
        PassBtn = viewRoot.transform.FindChild("OperateView/pass").GetComponent<Button>();
        BackBtn.onClick.AddListener(
            () => { if (BackItemPanel.activeSelf) BackItemPanel.SetActive(false); else BackItemPanel.SetActive(true); }
            );

        ApplicationFacade.Instance.RegisterMediator(new BattleViewMediator(Mediators.BATTLE_VIEW_MEDIATOR, this));
        
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
    }
    public override void OnShow()
    {
        base.OnShow();
        
        if (!battleProxy.isStart)
        {
            if (PlayerPrefs.GetString(PrefsKey.ROUND) != null &&
               PlayerPrefs.GetString(PrefsKey.RULE1) != null &&
               PlayerPrefs.GetString(PrefsKey.RULE2) != null &&
               PlayerPrefs.GetString(PrefsKey.RULE3) != null)
            {
                if (PlayerPrefs.GetString(PrefsKey.ROUND) == "8")
               {
                    roundTxt.text = "1/8";
                }
                else
                {
                    roundTxt.text = "一锅";
                }
                
                ruleText.text = string.Format("【{0}】", PlayerPrefs.GetString(PrefsKey.RULE1));
                ruleText1.text = string.Format("【{0}】", PlayerPrefs.GetString(PrefsKey.RULE2));
                ruleText2.text = string.Format("【{0}】", PlayerPrefs.GetString(PrefsKey.RULE3));
                RoomInfo.Rule1 = string.Format("【{0}】", PlayerPrefs.GetString(PrefsKey.RULE1));
                RoomInfo.Rule2 = string.Format("【{0}】", PlayerPrefs.GetString(PrefsKey.RULE2));
                RoomInfo.Rule3 = string.Format("【{0}】", PlayerPrefs.GetString(PrefsKey.RULE3));
            } 
        }

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
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Battle/BattleView");
    }

    public override void OnDestroy()
    {
        ApplicationFacade.Instance.RemoveMediator(Mediators.BATTLE_VIEW_MEDIATOR);
        base.OnDestroy();
    }

}
