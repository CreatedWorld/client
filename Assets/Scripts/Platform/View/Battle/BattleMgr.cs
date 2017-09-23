using Platform.Model;
using Platform.Model.Battle;
using Platform.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗区域
/// </summary>
public class BattleMgr : MonoBehaviour
{
    /// <summary>
    /// 战位区域数组
    /// </summary>
    [HideInInspector]
    public List<BattleAreaItem> battleAreaItems;
    /// <summary>
    /// 底部战位
    /// </summary>
    [HideInInspector]
    public BattleAreaItem downArea;
    /// <summary>
    /// 右侧战位
    /// </summary>
    [HideInInspector]
    public BattleAreaItem rightArea;
    /// <summary>
    /// 顶部战位
    /// </summary>
    [HideInInspector]
    public BattleAreaItem upArea;
    /// <summary>
    /// 左侧战位
    /// </summary>
    [HideInInspector]
    public BattleAreaItem leftArea;
    /// <summary>
    /// 当前牌的箭头
    /// </summary>
    [HideInInspector]
    public GameObject cardArrowIcon;
    /// <summary>
    /// 中央转圈标志
    /// </summary>
    [HideInInspector]
    public MasterView masterView;
    /// <summary>
    /// 色子1
    /// </summary>
    [HideInInspector]
    public GameObject saizi1;
    /// <summary>
    /// 色子2
    /// </summary>
    [HideInInspector]
    public GameObject saizi2;
    /// <summary>
    /// 录取器
    /// </summary>
    [HideInInspector]
    public RecorderSystem recorder;
    /// <summary>
    /// 战斗数据
    /// </summary>
    private BattleProxy battleProxy;
    private AnimationClip saizi1Clip;
    private AnimationClip saizi2Clip;
    // Use this for initialization
    void Awake()
    {
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        battleAreaItems = new List<BattleAreaItem>();
        //saizi1 = transform.FindChild("saizi1").gameObject;
        //saizi2 = transform.FindChild("saizi2").gameObject;
        downArea = transform.Find("DownArea").gameObject.GetComponent<BattleAreaItem>();
        rightArea = transform.Find("RightArea").gameObject.GetComponent<BattleAreaItem>();
        upArea = transform.Find("UpArea").gameObject.GetComponent<BattleAreaItem>();
        leftArea = transform.Find("LeftArea").gameObject.GetComponent<BattleAreaItem>();
        cardArrowIcon = transform.Find("CardArrowIcon").gameObject;
        masterView = transform.Find("MasterContainer/MasterView").GetComponent<MasterView>();
        cardArrowIcon.SetActive(false);
        battleAreaItems.Add(downArea);
        battleAreaItems.Add(rightArea);
        battleAreaItems.Add(upArea);
        battleAreaItems.Add(leftArea);
        for (int i = 0; i < battleAreaItems.Count; i++)
        {
            battleAreaItems[i].heapStartIndex = i * GlobalData.CardWare.Length / GlobalData.SIT_NUM;
            battleAreaItems[i].heapEndIndex = battleAreaItems[i].heapStartIndex + GlobalData.CardWare.Length / GlobalData.SIT_NUM - 1;
        }

        recorder = new RecorderSystem();

        saizi1Clip = Resources.Load<AnimationClip>("Animation/saizi1");
        saizi2Clip = Resources.Load<AnimationClip>("Animation/saizi2");

        UIManager.Instance.ShowUI(UIViewID.BATTLE_VIEW);
    }

    private void Start()
    {
        ApplicationFacade.Instance.RegisterMediator(new BattleAreaMediator(Mediators.BATTLE_AREA_MEDIATOR, this));
    }

    // Update is called once per frame
    void Update () {
        //录音播放
        if (battleProxy.speekPacket.Count > 0)
        {
            StartCoroutine(PlaySpeek(battleProxy.speekPacket.Dequeue()));
        }
        recorder.Update();
    }

    //播放语音
    public IEnumerator PlaySpeek(AudioPacket packet)
    {
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_BATTLEVIEW_SHOWPLAYINGVOICE, packet.LocalId);
        StartCoroutine(AudioSystem.Instance.PlayEffectAudio(packet.Clip));
        yield return new WaitForSeconds(packet.Clip.length);
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_BATTLEVIEW_HIDENPLAYINGVOICE, packet.LocalId);
    }

    private void OnDestroy()
    {
        ApplicationFacade.Instance.RemoveMediator(Mediators.BATTLE_AREA_MEDIATOR);
        (ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy).Clear();
    }

    /// <summary>
    /// 离开游戏提示重连
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool isPause)
    {
        if (battleProxy.isStart && !battleProxy.isReport && Application.isMobilePlatform)
        {
            if (isPause && GlobalData.LoginServer != "127.0.0.1")
            {
                var settingC2S = new OnlineSettingC2S();
                settingC2S.isOnline = false;
                NetMgr.Instance.ConnentionDic[SocketType.BATTLE].SendBuff(MsgNoC2S.C2S_ROOM_ONLINESETTING.GetHashCode(), 0, settingC2S, true);
                NetMgr.Instance.ShowDisconnectAlert();
            }
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && Application.isMobilePlatform)
        {
            if (battleProxy.isStart && !battleProxy.isReport && !battleProxy.isEnterInput)
            {
                if (GlobalData.LoginServer != "127.0.0.1")
                {
                    var settingC2S = new OnlineSettingC2S();
                    settingC2S.isOnline = false;
                    NetMgr.Instance.ConnentionDic[SocketType.BATTLE].SendBuff(MsgNoC2S.C2S_ROOM_ONLINESETTING.GetHashCode(), 0, settingC2S, true);
                    NetMgr.Instance.ShowDisconnectAlert();
                }
            }
        }
    }
}
