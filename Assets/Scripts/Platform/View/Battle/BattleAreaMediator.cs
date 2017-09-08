﻿using Platform.Model.Battle;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using Platform.Model;
using UnityEngine;
using Platform.Net;
using System;
using Utils;
using Platform.Utils;
using DG.Tweening;

/// <summary>
/// 战斗区域中介
/// </summary>
public class BattleAreaMediator : Mediator, IMediator
{
    /// <summary>
    /// 游戏数据中介
    /// </summary>
    private GameMgrProxy gameMgrProxy;
    /// <summary>
    /// 用户信息数据中介
    /// </summary>
    private PlayerInfoProxy playerInfoProxy;
    /// <summary>
    /// 战斗模块数据中介
    /// </summary>
    private BattleProxy battleProxy;
    
    public BattleAreaMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }

    public BattleMgr View
    {
        get
        {
            return (BattleMgr)ViewComponent;
        }
    }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_BATTLE_SENDCARD);
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
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_SHOWMATCHRESULT);
        list.Add(NotificationConstant.MEDI_BATTLEREA_STARTRECORD);
        list.Add(NotificationConstant.MEDI_BATTLEREA_STOPRECORD);
        list.Add(NotificationConstant.MEDI_BATTLEVIEW_INITPLAYERCARDS);
        list.Add(NotificationConstant.READY_NEXT);
        list.Add(NotificationConstant.SHOW_CARD_ARROW);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYACTTIP);
        list.Add(NotificationConstant.MEDI_BATTLE_PLAYROTATE);
        list.Add(NotificationConstant.MEDI_BATTLE_RESET);
        return list;
    }


    public override void OnRegister()
    {
        base.OnRegister();
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        gameMgrProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
        playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
       
        View.recorder.OnReComplete = SendAudioToServer;
        View.recorder.OnGetComplete = PlayAudio;
        AudioSystem.Instance.PlayBgm("Voices/Bgm/91bgmusic");

        InitView();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_BATTLE_SENDCARD:
                GameMgr.Instance.StartCoroutine(PlaySendCardAnimator()); 
                break;
            case NotificationConstant.MEDI_BATTLE_PLAY_COMMONANGANG:
                PlayCommonAnGang();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAY_BACKANGANG:
                PlayBackAnGang();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYGETCARD:
                PlayGetCard();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYPASS:
                PlayPass();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYPENG:
                PlayPeng();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYCHI:
                PlayChi();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAY_COMMONPENGGANG:
                PlayCommonPengGang();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAY_BACKPENGGANG:
                PlayBackPengGang();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYPUTCARD:
                PlayPutCard();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYZHIGANG:
                PlayZhiGang();
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_RESET:
                PlayRestCard();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYHU:
                PlayHu((bool)notification.Body);
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_SHOWMATCHRESULT:
                SaveAllCard();
                View.cardArrowIcon.SetActive(false);
                break;
            case NotificationConstant.MEDI_BATTLEREA_STARTRECORD:
                View.recorder.Recording();
                break;
            case NotificationConstant.MEDI_BATTLEREA_STOPRECORD:
                View.recorder.StopRecording();
                break;
            case NotificationConstant.MEDI_BATTLEVIEW_INITPLAYERCARDS:
                BackPlayerCard();
                break;
            case NotificationConstant.READY_NEXT:
                foreach (BattleAreaItem areaItem in View.battleAreaItems)
                {
                    areaItem.SaveAllCard();
                }
                View.cardArrowIcon.SetActive(false);
                ResourcesMgr.Instance.RecoveryAll();
                break;
            case NotificationConstant.SHOW_CARD_ARROW:
                UpdateCardArrow(notification.Body as BattleAreaItem);
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYACTTIP:
                //View.masterView.ShowPlayActTip();
                break;
            case NotificationConstant.MEDI_BATTLE_PLAYROTATE:
                PlayRotate();
                
                break;
        }
    }

    /// <summary>
    /// 初始化界面显示
    /// </summary>
    private void InitView()
    {
        if (battleProxy.isStart)
        {
            var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
            for (int i = 0; i < View.battleAreaItems.Count; i++)
            {
                View.battleAreaItems[i].data = battleProxy.playerSitInfoDic[GlobalData.GetNextSit(selfInfoVO.sit, i)];
            }
            //View.masterView.UpdateMasterInfo(true);
            //View.masterView.ShowPlayActTip();
        }
        //View.masterView.gameObject.SetActive(true);//battleProxy.isStart
    }

    /// <summary>
    /// 显示庄家旋转标志
    /// </summary>
    private void PlayRotate()
    {
        //isFirstMatch = false;
        //if (false)//第一局需要转庄家
        //{
        //   // View.masterView.PlayRotate();
        //}
        //else
        //{
            
        //}

        //View.saizi1.GetComponent<Animator>().Play("saizi1");
        //View.saizi2.GetComponent<Animator>().Play("saizi2");
        
        //View.masterView.UpdateMasterInfo(true);
        //View.masterView.ShowPlayActTip();
        //SendNotification(NotificationConstant.MEDI_BATTLE_SENDCARD);
    }


    /// <summary>
    /// 播放发牌动画
    /// </summary>
    private IEnumerator PlaySendCardAnimator()
    {
        //发牌之前先回收之前的牌
        foreach (BattleAreaItem areaItem in View.battleAreaItems)
        {
            areaItem.SaveAllCard();
        }
        View.cardArrowIcon.SetActive(false);
        ResourcesMgr.Instance.RecoveryAll();
        if (GlobalData.hasHeap)
        {
            foreach (BattleAreaItem areaItem in View.battleAreaItems)
            {
                BattleAreaUtil.InitHeapCard(areaItem, GlobalData.CardWare.Length);
            }
        }
        //Debug.Log("色子动画...");
        //View.saizi1.GetComponent<Animation>().Play();
        //View.saizi2.GetComponent<Animation>().Play("saizi2");
        //yield return new WaitForSeconds(2);
        //yield break;

        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var sendStartIndex = (battleProxy.GetBankerPlayerInfoVOS2C().sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        for (int i = 0; i < View.battleAreaItems.Count; i++)
        {
            View.battleAreaItems[i].SetData(battleProxy.playerSitInfoDic[GlobalData.GetNextSit(selfInfoVO.sit, i)]);
        }
        int sendCount = Mathf.CeilToInt((float)GlobalData.PLAYER_CARD_NUM / GlobalData.SEND_SINGLE) *GlobalData.SIT_NUM;
        for (int i = 0; i < sendCount; i++)
        {
            GameMgr.Instance.StartCoroutine(View.battleAreaItems[(i + sendStartIndex) % GlobalData.SIT_NUM].PlaySendCardAnimator());
            yield return new WaitForSeconds(0.4f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < View.battleAreaItems.Count; i++)
        {
            GameMgr.Instance.StartCoroutine(View.battleAreaItems[i].PlayCloseCardAction());
        }
        yield return new WaitForSeconds(0.83f);
        ClientAIMgr.Instance.AIPutCard();
        battleProxy.SetIsForbit(false);
        SendNotification(NotificationConstant.MEDI_BATTLE_PLAYACTTIP);
    }

    /// <summary>
    /// 播放直接暗杠动画
    /// </summary>
    private void PlayCommonAnGang()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.battleAreaItems[actIndex].PlayCommonAnGang();
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/AnGang", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        View.cardArrowIcon.SetActive(false);
    }

    /// <summary>
    /// 播放回头暗杠动画
    /// </summary>
    private void PlayBackAnGang()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.battleAreaItems[actIndex].PlayBackAnGang();
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/AnGang", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        View.cardArrowIcon.SetActive(false);
    }

    /// <summary>
    /// 播放摸牌动作
    /// </summary>
    private void PlayGetCard()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        GameMgr.Instance.StartCoroutine(View.battleAreaItems[actIndex].PlayGetCard());
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio("Voices/Effect/GetCard"));
    }

    /// <summary>
    /// 播放过动作
    /// </summary>
    private void PlayPass()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.battleAreaItems[actIndex].PlayPass();
    }
    
    /// <summary>
    /// 播放碰牌动作
    /// </summary>
    private void PlayPeng()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        var targetPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().targetUserId];
        var targetActIndex = (targetPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        var pengedCard = View.battleAreaItems[targetActIndex].PlayPenged();
        View.battleAreaItems[actIndex].PlayPeng(pengedCard);
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/Peng", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        View.cardArrowIcon.SetActive(false);
    }

    /// <summary>
    /// 播放吃牌动作
    /// </summary>
    private void PlayChi()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        var targetPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().targetUserId];
        var targetActIndex = (targetPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        var chiedCard = View.battleAreaItems[targetActIndex].PlayChied();
        View.battleAreaItems[actIndex].PlayChi(chiedCard);
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/Chi", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        View.cardArrowIcon.SetActive(false);
    }

    /// <summary>
    /// 播放直接碰杠动作
    /// </summary>
    private void PlayCommonPengGang()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.battleAreaItems[actIndex].PlayCommonPengGang();
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/Gang", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        View.cardArrowIcon.SetActive(false);
    }

    /// <summary>
    /// 播放回头碰杠动作
    /// </summary>
    private void PlayBackPengGang()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        View.battleAreaItems[actIndex].PlayBackPengGang();
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/Gang", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        View.cardArrowIcon.SetActive(false);
    }

    /// <summary>
    /// 播放出牌动作
    /// </summary>
    private void PlayPutCard()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        GameMgr.Instance.StartCoroutine(View.battleAreaItems[actIndex].PlayPutCard());        
    }

    /// <summary>
    /// 播放直杠动作
    /// </summary>
    private void PlayZhiGang()
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        var targetPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().targetUserId];
        var targetActIndex = (targetPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        var gangedCard = View.battleAreaItems[targetActIndex].PlayZhiGanged();
        View.battleAreaItems[actIndex].PlayZhiGang(gangedCard);
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/Gang", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        View.cardArrowIcon.SetActive(false);
    }

    /// <summary>
    /// 播放牌面重置
    /// </summary>
    private void PlayRestCard()
    {
        for (int i = 0; i < View.battleAreaItems.Count; i++)
        {
            View.battleAreaItems[i].PlayRestCard();
        }
    }

    /// <summary>
    /// 播放胡牌动作
    /// </summary>
    private void PlayHu(bool isSelf)
    {
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var actPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
        var actIndex = (actPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        if (isSelf)
        {
            View.battleAreaItems[actIndex].PlaySelfHu();
            GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/SelfHu", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        }
        else
        {
            var targetPlayerInfoVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().targetUserId];
            var targetActIndex = (targetPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
            var huedCard = View.battleAreaItems[targetActIndex].PlayHued();
            View.battleAreaItems[actIndex].PlayHu(huedCard);
            GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio(string.Format("Voices/{0}/Card/Hu", actPlayerInfoVO.sex == 1 ? "Man" : "Woman")));
        }
        View.cardArrowIcon.SetActive(false);
    }
    /// <summary>
    /// 更新已出牌的箭头
    /// </summary>
    /// <param name="areaItem"></param>
    private void UpdateCardArrow(BattleAreaItem areaItem)
    {
        if (areaItem.putCards.Count == 0)
        {
            return;
        }
        View.cardArrowIcon.SetActive(true);
        View.cardArrowIcon.transform.parent = areaItem.putCards[areaItem.putCards.Count - 1].transform;
        View.cardArrowIcon.transform.localPosition = new Vector3(0, -0.4f, 0);
        if (areaItem.dir == AreaDir.DOWN)
        {
            View.cardArrowIcon.transform.localEulerAngles = new Vector3(90, 0f, 0);
        }
        else if (areaItem.dir == AreaDir.LEFT)
        {
            View.cardArrowIcon.transform.localEulerAngles = new Vector3(120, 90, 0);
        }
        else if (areaItem.dir == AreaDir.RIGHT)
        {
            View.cardArrowIcon.transform.localEulerAngles = new Vector3(120, 270, 0);
        }
        else if (areaItem.dir == AreaDir.UP)
        {
            View.cardArrowIcon.transform.localEulerAngles = new Vector3(118, 0, 0);
        }
        View.cardArrowIcon.transform.localScale = Vector3.one;
        View.cardArrowIcon.transform.DOKill();
        View.cardArrowIcon.transform.DOLocalMoveY(-0.8f, 1).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// 显示单局结算,隐藏回收桌面牌资源
    /// </summary>
    private void SaveAllCard()
    {
        foreach (BattleAreaItem areaItem in View.battleAreaItems)
        {
            areaItem.SaveAllCard();
        }
        View.cardArrowIcon.SetActive(false);
        //打开本局结算界面
        if (!battleProxy.isPlayHu)
        {
            Timer.Instance.AddTimer(0, 1, 0.5f, () => {
                UIManager.Instance.ShowUI(UIViewID.MATCH_RESULT_VIEW);
            });
        }
    }

    /// <summary>
    /// 录音完成回调
    /// </summary>
    /// <param name="flag"></param>
    /// <param name="data"></param>
    private void SendAudioToServer(int flag, byte[] data)
    {
        if (flag == 1)
        {
            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_HIDENRECORDING);
        }
        var sendVoiceC2S = new SendVoiceC2S();
        sendVoiceC2S.flag = flag;
        sendVoiceC2S.content = data;
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_VOICE_CHAT.GetHashCode(), 0, sendVoiceC2S, true);
        battleProxy.perSendChatTime = gameMgrProxy.systemTime;
        RecorderSystem.GetAudioPacket(playerInfoProxy.userID, flag, data);
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="obj"></param>
    private void PlayAudio(AudioPacket packet)
    {
        battleProxy.speekPacket.Enqueue(packet);
    }

    /// <summary>
    /// 回退重置单个玩家牌面
    /// </summary>
    private void BackPlayerCard()
    {
        foreach (BattleAreaItem areaItem in View.battleAreaItems)
        {
            areaItem.SaveAllCard();
        }
        View.cardArrowIcon.SetActive(false);
        InitView();
    }

    /// <summary>
    /// 获取当前摸到的牌堆的牌
    /// </summary>
    /// <returns></returns>
    public GameObject GetHeapCard(int cardValue)
    {
        int heapCardIndex = battleProxy.unGetHeapCardIndexs[0];
        battleProxy.unGetHeapCardIndexs.RemoveAt(0);
        foreach (BattleAreaItem areaItem in View.battleAreaItems)
        {
            if (heapCardIndex >= areaItem.heapStartIndex && heapCardIndex <= areaItem.heapEndIndex)
            {
                return areaItem.GetHeapCard(cardValue);
            }
        }
        return null;
    }
}