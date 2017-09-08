using System.Collections.Generic;
using Platform.Model;
using Platform.Model.Battle;
using Platform.Net;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using Utils;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using Platform.Utils;
using UnityEngine.UI;
/// <summary>
/// 战斗场景UI中介
/// </summary>
public class BattleViewMediator : Mediator, IMediator
{
    /// <summary>
    /// 战斗模块数据中介
    /// </summary>
    private BattleProxy battleProxy;

    /// <summary>
    /// 游戏数据中介
    /// </summary>
    private GameMgrProxy gameMgrProxy;
    /// <summary>
    /// 玩家信息数据
    /// </summary>
    private PlayerInfoProxy playerInfoProxy;
    /// <summary>
    /// 同步时间定时器id
    /// </summary>
    private int sysTimeId;
    /// <summary>
    /// 点击后是否展开
    /// </summary>
    private bool isExpand = false;
    public BattleViewMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {
    }

    public BattleView View
    {
        get { return (BattleView) ViewComponent; }
    }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_UPDATEALLHEAD);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_UPDATESINGLEHEAD);
        list.Add(NotificationConstant.MEDI_READY_COMPLETE);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYROTATE);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYACTTIP);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAY_COMMONANGANG);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAY_BACKANGANG);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYGETCARD);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYPASS);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYPENG);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYCHI);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAY_COMMONPENGGANG);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAY_BACKPENGGANG);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYPUTCARD);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYZHIGANG);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYHU);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_SHOWBANKERICON);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_HIDENRECORDING);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_SHOWPLAYINGVOICE);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_HIDENPLAYINGVOICE);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_SHOWCHAT);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_SHOWFACE); 
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_SHOW_REPORTVIEW);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_UPDATEONLINE);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_INITPLAYERCARDS);
        list.Add(NotificationConstant.TING_UPDATE);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_UPDATEVOLUME);
        return list;
    }


    public override void OnRegister()
    {
        base.OnRegister();
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        gameMgrProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
        playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
        //View.readyBtn.onClick.AddListener(onReadyClick);
        //View.BackBtn.onClick.AddListener(OnDissloutionClick);
        //View.dissolutionBtn.onClick.AddListener(OnDissloutionClick);
        View.chatBtn.onClick.AddListener(OnChatClick);
        View.settingBtn.onClick.AddListener(OnSettingClick);
        View.inviteBtn.onClick.AddListener(OnInviteClick);
        View.ruleInfoBtn.onClick.AddListener(OnRuleInfoClick);

        View.ExitRoomBtn.onClick.AddListener(OnDissloutionClick);
        View.LeftRoomBtn.onClick.AddListener(OnLeftRoomBtnClick);

        var voideTrigger = View.voiceBtn.GetComponent<EventTrigger>();
        voideTrigger.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry onUp = new EventTrigger.Entry();
        onUp.eventID = EventTriggerType.PointerUp;
        onUp.callback.AddListener(OnVoiceUp);
        EventTrigger.Entry onDown = new EventTrigger.Entry();
        onDown.eventID = EventTriggerType.PointerDown;
        onDown.callback.AddListener(OnVoiceDown);
        voideTrigger.triggers.Add(onDown);
        voideTrigger.triggers.Add(onUp);

        Timer.Instance.AddTimer(0, 1, 0, InitView);
    }


    public override void OnRemove()
    {
        base.OnRemove();
        //View.readyBtn.onClick.RemoveListener(onReadyClick);
        //View.BackBtn.onClick.RemoveListener(OnDissloutionClick);
        View.ExitRoomBtn.onClick.RemoveListener(OnDissloutionClick);
        View.LeftRoomBtn.onClick.AddListener(OnLeftRoomBtnClick);
        View.chatBtn.onClick.RemoveListener(OnChatClick);
        View.settingBtn.onClick.RemoveListener(OnSettingClick);
        View.inviteBtn.onClick.RemoveListener(OnInviteClick);
        View.ruleInfoBtn.onClick.RemoveListener(OnRuleInfoClick);
        Timer.Instance.CancelTimer(sysTimeId);
    }

    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_BATTLEVIEW_UPDATEALLHEAD:
                UpdateAllHeadItem();//(bool)notification.Body
                UpdateInviteBtn();
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_UPDATESINGLEHEAD:
                UpdateSingleHeadItem(notification.Body as PlayerInfoVOS2C);
                UpdateInviteBtn();
                break;
            case NotificationConstant.MEDI_READY_COMPLETE:
                //UpdateReadyBtn();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYROTATE:
                HidenReady();//(bool)notification.Body
                UpdateRoomInfo();
                //View.readyBtn.gameObject.SetActive(false);
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_SHOWBANKERICON:
                UpdateBankerIcon();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYACTTIP:
                ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAY_COMMONANGANG:
                PlayAct(PlayerActType.COMMON_AN_GANG);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAY_BACKANGANG:
                PlayAct(PlayerActType.BACK_AN_GANG);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYGETCARD:
                UpdateRoomInfo();
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_INITPLAYERCARDS:
                UpdateRoomInfo();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYPASS:
                PlayAct(PlayerActType.PASS);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYPENG:
                PlayAct(PlayerActType.PENG);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYCHI:
                PlayAct(PlayerActType.CHI);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAY_COMMONPENGGANG:
                PlayAct(PlayerActType.COMMON_PENG_GANG);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAY_BACKPENGGANG:
                PlayAct(PlayerActType.BACK_PENG_GANG);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYPUTCARD:
                PlayAct(PlayerActType.PUT_CARD);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYZHIGANG:
                PlayAct(PlayerActType.ZHI_GANG);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYHU:
                PlayHuAction();
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_HIDENRECORDING:
                View.recordingIcon.SetActive(false);
                AudioSystem.Instance.ResumeBgm();
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_UPDATEVOLUME:
                UpdateRecorcdVolume((float)notification.Body);
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_SHOWPLAYINGVOICE:
                ShowVoicePlayIcon((int)notification.Body);
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_HIDENPLAYINGVOICE:
                HidenVoicePlayIcon((int)notification.Body);
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_SHOWCHAT:
                ShowChatInfo(notification.Body as PushSendChatS2C);
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_SHOWFACE:
                ShowFace(notification.Body as PushSendChatS2C);
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_SHOW_REPORTVIEW:
                ShowReportView();
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_UPDATEONLINE:
                SetOnline((int)notification.Body);
                break;
            case NotificationConstant.TING_UPDATE:
                UpdateTingIcon();
                break;
        }
    }

    /// <summary>
    /// 初始化界面显示
    /// </summary>
    private void InitView()
    {
        UpdateAllHeadItem();//false
        if (battleProxy.isStart)
        {
            if (battleProxy.GetPlayerActTipS2C() != null)
            {
                ShowPlayActTip();
            }
            UpdateTingIcon();

            if (battleProxy.innings == 8)
            {
                View.roundTxt.text = string.Format("{0}/{1}", battleProxy.curInnings, battleProxy.innings);
            }
            else
            {
                View.roundTxt.text = "一锅";
            }
            for (int i = 0; i < battleProxy.playType.Count; i++)
            {
                if (battleProxy.playType[i] == 44)
                {
                    View.ruleText.text = string.Format("【三家炮】");
                }
                if (battleProxy.playType[i] == 45)
                {
                    View.ruleText.text = string.Format("【一家炮】");
                }
                if (battleProxy.playType[i] == 46)
                {
                    View.ruleText1.text = string.Format("【蛋带翻】");
                }
                if (battleProxy.playType[i] == 47)
                {
                    View.ruleText1.text = string.Format("【蛋不翻】");
                }
                if (battleProxy.playType[i] == 48)
                {
                    View.ruleText2.text = string.Format("【长跑3】");
                }
                if (battleProxy.playType[i] == 49)
                {
                    View.ruleText2.text = string.Format("【长跑5】");
                }
                if (battleProxy.playType[i] == 50)
                {
                    View.ruleText2.text = string.Format("【长跑10】");
                }
            }

            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYROTATE);
            //SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATEALLHEAD);
            SendNotification(NotificationConstant.MEDI_BATTLE_SENDCARD);
        }

        //View.readyBtn.gameObject.SetActive(!battleProxy.playerIdInfoDic[playerInfoProxy.userID].isReady);
        UpdateRoomInfo();
        if (battleProxy.isReport)
        {
            ShowReportView();
        }
        UpdateInviteBtn();
        if (battleProxy.hasDisloveApply)
        {
			if(battleProxy.agreeIds.IndexOf(playerInfoProxy.userID) == -1 && battleProxy.refuseIds.IndexOf(playerInfoProxy.userID) == -1)
			{
				UIManager.Instance.ShowUI(UIViewID.DISLOVE_APPLY_VIEW);
			}
			else
			{
				UIManager.Instance.ShowUI(UIViewID.DISLOVE_STATISTICS_VIEW);
			}            
        }
        if (battleProxy.isStart)
        {
            
        }

       
    }

    /// <summary>
    /// 上次点击准备的时间
    /// </summary>
    private float perClickTime = 0;
    /// <summary>
    /// 点击准备按钮
    /// </summary>
    private void onReadyClick()
    {
        if (Time.time - perClickTime < 1)
        {
            return;
        }
        perClickTime = Time.time;
        var readyC2S = new ReadyC2S();
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_READY.GetHashCode(), 0, readyC2S);
    }

    /// <summary>
    /// 更新所有头像
    /// </summary>
    private void UpdateAllHeadItem()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        for (var i = 0; i < View.headItemList.Count; i++)
        {
            var nextSit = GlobalData.GetNextSit(selfInfoVO.sit , i);
            if (!battleProxy.playerSitInfoDic.ContainsKey(nextSit))
            {
                View.headItemList[i].GetComponent<HeadItem>().data = null;
            }
            else
            {
                var nextPlayerInfoVOS2C = battleProxy.playerSitInfoDic[nextSit];
                View.headItemList[i].GetComponent<HeadItem>().data = nextPlayerInfoVOS2C;
                //if (isFirstMatch)
                //{
                //    View.headItemList[i].GetComponent<HeadItem>().HidemBanker();
                //}
            }
        }
    }

    /// <summary>
    /// 更新单个头像
    /// </summary>
    /// <param name="updatePlayInfoVOS2C"></param>
    private void UpdateSingleHeadItem(PlayerInfoVOS2C updatePlayInfoVOS2C)
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var updateHeadIndex = (updatePlayInfoVOS2C.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        if (battleProxy.playerIdInfoDic.ContainsKey(updatePlayInfoVOS2C.userId))
        {
            View.headItemList[updateHeadIndex].GetComponent<HeadItem>().data = updatePlayInfoVOS2C;
        }
        else
        {
            View.headItemList[updateHeadIndex].GetComponent<HeadItem>().data = null;
        }
    }

    /// <summary>
    /// 更新房间信息
    /// </summary>
    private void UpdateRoomInfo()
    {
        if (sysTimeId > 0)
        {
            Timer.Instance.CancelTimer(sysTimeId);
        }
        sysTimeId = Timer.Instance.AddDeltaTimer(1, 0, 1, UpdateSystemTime);
        UpdateSystemTime();
        var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        View.roomIdTxt.text = hallProxy.HallInfo.roomCode;
        if (hallProxy.HallInfo.innings.GetHashCode() == 8)
        {
            View.roundTxt.text = string.Format("{0}/{1}", battleProxy.curInnings, "8");
        }
        else
        {
            View.roundTxt.text =  "一锅";
        }
       
        View.leftCardNumTxt.text = string.Format("剩 {0} 张", battleProxy.leftCard); 
        GameMgr.Instance.StartCoroutine(PingIP());
    }
    /// <summary>
    /// 更新服务器时间
    /// </summary>
    private void UpdateSystemTime()
    {
        if (battleProxy.isReport)//战报显示战报发生时间
        {
            var reportDate = TimeHandle.Instance.GetDateTimeByTimestamp(battleProxy.report.startTime + (long)(Time.time - battleProxy.reportLocalTime) * 1000);
            View.dateTxt.text = reportDate.ToString("yyyy-MM-dd");
            View.timeTxt.text = reportDate.ToString("HH:mm");
        }
        else
        {
            View.dateTxt.text = gameMgrProxy.systemDateTime.ToString("yyyy-MM-dd");
            View.timeTxt.text = gameMgrProxy.systemDateTime.ToString("HH:mm");
        }
        
        if (Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
        {
            View.netIcon.sprite = Resources.Load<Sprite>("Textures/NetPoor");
        }
        else if (gameMgrProxy.pingBackMS < 100)
        {
            View.netIcon.sprite = Resources.Load<Sprite>("Textures/NetPerfect");
        }
        else if (gameMgrProxy.pingBackMS < 200)
        {

            View.netIcon.sprite = Resources.Load<Sprite>("Textures/NetGood");
        }
        else
        {
            View.netIcon.sprite = Resources.Load<Sprite>("Textures/NetOK");
        }
    }

    private IEnumerator PingIP()
    {
        var ping = new Ping(GlobalData.LoginServer);
        while (!ping.isDone)
        {
            yield return null;
        }
        gameMgrProxy.pingBackMS = ping.time;
        yield return new WaitForSeconds(10);
        GameMgr.Instance.StartCoroutine(PingIP());
    }

    /// <summary>
    /// 显示庄家旋转标志
    /// </summary>
    private void HidenReady()//bool isFirstMatch
    {
        for (var i = 0; i < View.headItemList.Count; i++)
        {
            View.headItemList[i].GetComponent<HeadItem>().HidenReady();
        }
    }

    /// <summary>
    /// 头像框显示庄家标志
    /// </summary>
    private void UpdateBankerIcon()
    {
        var bankerInfoVO = battleProxy.GetBankerPlayerInfoVOS2C();
        for (var i = 0; i < View.headItemList.Count; i++)
        {
            View.headItemList[i].GetComponent<HeadItem>().ShowBankerIcon(bankerInfoVO.userId);
        }
    }

    /// <summary>
    /// 显示玩家操作提示,其他人提示动作时隐藏自己的动作提示
    /// </summary>
    private void ShowPlayActTip()
    {
        var tipPlayVO = battleProxy.GetPlayerActTipS2C();
        if (tipPlayVO == null)
        {
            return;
        }
        if (tipPlayVO.optUserId == playerInfoProxy.userID)
        {
            View.operateView.ShowPlayActTip();
        }
    }

    /// <summary>
    /// 播放玩家操作提示
    /// </summary>
    /// <param name="playerAct"></param>
    private void PlayAct(PlayerActType playerAct)
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        if (actIndex != 0)
        {
            View.headItemList[actIndex].GetComponent<HeadItem>().PlayAct(playerAct);
        }
        if (actIndex == 0)
        {
            View.operateView.HidenPlayActTip();
        }
        else if (playerAct != PlayerActType.PASS && !battleProxy.huTypes.Contains(playerAct))
        {
            View.operateView.HidenPlayActTip();
        }
    }

    /// <summary>
    /// 播放胡牌动画
    /// </summary>
    private void PlayHuAction()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.headItemList[actIndex].GetComponent<HeadItem>().PlayHu();
        if (actIndex == 0)
        {
            View.operateView.HidenPlayActTip();
        }
        battleProxy.isPlayHu = true;
        
    }

    /// <summary>
    /// 解散房间
    /// </summary>
    private void OnDissloutionClick()
    {
        if (playerInfoProxy.userID != battleProxy.creatorId && battleProxy.GetIsFirstMatch())
        {
            DialogMsgVO dialogMsgVO = new DialogMsgVO();
            dialogMsgVO.title = "退出确认";
            dialogMsgVO.content = "是否退出房间";
            dialogMsgVO.dialogType = DialogType.CONFIRM;
            dialogMsgVO.confirmCallBack = delegate { ConfirmExit(); };
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogMsgVO;
        }
        else
        {
            DialogMsgVO dialogMsgVO = new DialogMsgVO();
            dialogMsgVO.dialogType = DialogType.CONFIRM;
            dialogMsgVO.title = "解散确认";
            dialogMsgVO.content = "是否解散房间";
            dialogMsgVO.confirmCallBack = delegate { ConfirmDissloution(); };
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogMsgVO;
        }
        Debug.Log("发送解散房间");
    }
    private void OnLeftRoomBtnClick()
    {
        var exitC2S = new ExitRoomC2S();
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_EXIT.GetHashCode(), 0, exitC2S);
        Debug.Log("发送离开房间");
    }
    /// <summary>
    /// 退出房间确认回调
    /// </summary>
    private void ConfirmExit()
    {
        var exitC2S = new ExitRoomC2S();
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_EXIT.GetHashCode(),0, exitC2S);
    }

    /// <summary>
    /// 解散房间确认回调
    /// </summary>
    private void ConfirmDissloution()
    {
        if (battleProxy.isStart)
        {
            var disloveC2S = new ApplyDissolveRoomC2S();
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_APPLY_DISSOLVE.GetHashCode(), 0, disloveC2S, false);
        }
        else
        {
            if (battleProxy.creatorId == playerInfoProxy.userID)
            {
                var disloveC2S = new DissolveRoomC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_DISSOLVE.GetHashCode(), 0, disloveC2S);
            }
            else
            {
                var disloveC2S = new ApplyDissolveRoomC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_APPLY_DISSOLVE.GetHashCode(), 0, disloveC2S);
            }
        }
    }


    /// <summary>
    /// 按下录音按钮
    /// </summary>
    /// <param name="arg0"></param>
    private void OnVoiceDown(BaseEventData arg0)
    {
        View.recordingIcon.SetActive(true);
        AudioSystem.Instance.PauseBgm();
        SendNotification(NotificationConstant.MEDI_BATTLEREA_STARTRECORD);
    }

    /// <summary>
    /// 松开录音按钮
    /// </summary>
    /// <param name="arg0"></param>
    private void OnVoiceUp(BaseEventData arg0)
    {
        View.recordingIcon.SetActive(false);
        AudioSystem.Instance.ResumeBgm();
        SendNotification(NotificationConstant.MEDI_BATTLEREA_STOPRECORD);
    }

    /// <summary>
    /// 更新录音音量
    /// </summary>
    private void UpdateRecorcdVolume(float volume)
    {
        int value = Mathf.RoundToInt(volume);
        Sprite targetSprite = Resources.Load<Sprite>(string.Format("Textures/RecordIcon/RecordIcon{0}",value));
        View.recordingIcon.GetComponent<Image>().sprite = targetSprite;
    }

    /// <summary>
    /// 显示语言播放标志
    /// </summary>
    private void ShowVoicePlayIcon(int userId)
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var voicePlayerInfo = battleProxy.playerIdInfoDic[userId];
        var updateHeadIndex = (voicePlayerInfo.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.headItemList[updateHeadIndex].GetComponent<HeadItem>().ShowVoicePlayIcon();
    }

    /// <summary>
    /// 隐藏语言播放标志
    /// </summary>
    private void HidenVoicePlayIcon(int userId)
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var voicePlayerInfo = battleProxy.playerIdInfoDic[userId];
        var updateHeadIndex = (voicePlayerInfo.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.headItemList[updateHeadIndex].GetComponent<HeadItem>().HidenVoicePlayIcon();
    }

    /// <summary>
    /// 显示聊天信息
    /// </summary>
    /// <param name="chatS2C"></param>
    private void ShowChatInfo(PushSendChatS2C chatS2C)
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var chatPlayerInfo = battleProxy.playerIdInfoDic[chatS2C.senderUserId];
        var updateHeadIndex = (chatPlayerInfo.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.headItemList[updateHeadIndex].GetComponent<HeadItem>().ShowChatInfo(chatS2C.content);
        var chatIndex = Array.IndexOf(GlobalData.Chat_Const, chatS2C.content);
        if (chatIndex != -1)
        {
            chatIndex += 1;
            string voiceUrl = string.Empty;
            if (chatPlayerInfo.sex == 0)
            {
                voiceUrl = string.Format("Voices/Woman/{0}", chatIndex);
            }
            else
            {
                voiceUrl = string.Format("Voices/Man/{0}", chatIndex);
            }
            GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(voiceUrl));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatS2C"></param>
    private void ShowFace(PushSendChatS2C chatS2C)
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var chatPlayerInfo = battleProxy.playerIdInfoDic[chatS2C.senderUserId];
        var updateHeadIndex = (chatPlayerInfo.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        int faceIndex = int.Parse(chatS2C.content.Replace(GlobalData.FACE_PREFIX, ""));
        View.headItemList[updateHeadIndex].GetComponent<HeadItem>().ShowFace(faceIndex);
    }

    /// <summary>
    /// 显示播放界面
    /// </summary>
    private void ShowReportView()
    {
        View.reportView.SetActive(true);
        View.reportView.GetComponent<ReportView>().PlayReport();
    }

    private void OnRuleInfoClick()
    {
        UIManager.Instance.ShowUI(UIViewID.RULE_VIEW);
    }
    /// <summary>
    /// 点击聊天按钮
    /// </summary>
    private void OnChatClick()
    {
        UIManager.Instance.ShowUI(UIViewID.CHAT_VIEW);
    }

    /// <summary>
    /// 打开设置界面
    /// </summary>
    private void OnSettingClick()
    {
        UIManager.Instance.ShowUI(UIViewID.SETTING_VIEW);
    }

    /// <summary>
    /// 更新准备按钮
    /// </summary>
    private void UpdateReadyBtn()
    {
        //View.readyImg.gameObject.SetActive(!battleProxy.playerIdInfoDic[playerInfoProxy.userID].isReady);
    }
    
    /// <summary>
    /// 更新邀请按钮
    /// </summary>
    private void UpdateInviteBtn()
    {
        if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].isReady && battleProxy.playerIdInfoDic.Count < GlobalData.SIT_NUM)
        {
            View.inviteBtn.gameObject.SetActive(true);
        }
        else
        {
            View.inviteBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 更新听牌图标
    /// </summary>
    private void UpdateTingIcon()
    {
        View.operateView.UpdateTingIcon();
    }

    /// <summary>
    /// 邀请好友
    /// </summary>
    private void OnInviteClick()
    {
        var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        string inviteUrl = string.Format("{0}?{1}={2}",GlobalData.ShareUrl, StartUpParam.ROOMID, hallProxy.HallInfo.roomCode);
        string title = string.Format("房间号：{0} 全民麻将",hallProxy.HallInfo.roomCode);
        string desc = string.Format("我在(全民约牌吧)开了{0}局，{1}风的4人房间，快来一起玩吧！", hallProxy.HallInfo.innings.GetHashCode(), hallProxy.HallInfo.gameRule == GameRule.WORD ? "有" : "无");
        if (GlobalData.sdkPlatform == SDKPlatform.ANDROID)
        {
            AndroidSdkInterface.WeiXinShare(inviteUrl, title, desc, false);
        }
        else if(GlobalData.sdkPlatform == SDKPlatform.IOS)
        {
            IOSSdkInterface.shareWeChat(inviteUrl, title, desc, false);
        }
    }

    /// <summary>
    /// 设置离线标志
    /// </summary>
    private void SetOnline(int userId)
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var offlinePlayerVO = battleProxy.playerIdInfoDic[userId];
        var actIndex = (offlinePlayerVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        if (actIndex != 0)
        {
            View.headItemList[actIndex].GetComponent<HeadItem>().UpdateOnline(offlinePlayerVO.isOnline);
        }
    }
}