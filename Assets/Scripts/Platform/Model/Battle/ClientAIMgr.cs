using System.Collections;
using Platform.Net;
using UnityEngine;
using System.Collections.Generic;
using MahjongMethod;
using LZR.Data.LtiJson;
using UnityEngine.SceneManagement;
using Platform.Global;
using Platform.Utils;
using System;
using PureMVC.Patterns;

namespace Platform.Model.Battle
{
    /// <summary>
    /// 客户端数据模拟类
    /// </summary>
    public class ClientAIMgr:MonoBehaviour
    {
        /// <summary>
        /// 自己默认摸到的牌
        /// </summary>
        private int[] defaultCardIndex = new int[13];
        /// <summary>
        /// 模拟的玩家名称
        /// </summary>
        private string[] playNames = new[] {"达康书记","猴子","沙书记","郑快进"};
        /// <summary>
        /// 战斗模块数据中介
        /// </summary>
        private BattleProxy battleProxy;
        /// <summary>
        /// 当前是第几局
        /// </summary>
        private int curInnings = 1;
        /// <summary>
        /// 所有卡牌池
        /// </summary>
        private List<int> cardPool = new List<int>(GlobalData.CardWare);
        /// <summary>
        /// 当前出牌的玩家id
        /// </summary>
        private int curSit;

        /// <summary>
        /// 游戏数据中介
        /// </summary>
        private GameMgrProxy gameMgrProxy;
        /// <summary>
        /// 玩家信息中介
        /// </summary>
        private PlayerInfoProxy playerInfoProxy;

        private static ClientAIMgr instance;
        public static ClientAIMgr Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
            battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            gameMgrProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
            playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            Application.logMessageReceived += HandleLog;
            for (int i = 0;i < GlobalData.CardWare.Length;i++)
            {
                GlobalData.Test127Queue.Enqueue(GlobalData.CardWare[i]); 
            }
            for (int i = 0; i < defaultCardIndex.Length; i++)
            {
                defaultCardIndex[i] = GlobalData.Test127Queue.Dequeue();
            }
        }

        /// <summary>  
        /// Records a log from the log callback.  
        /// </summary>  
        /// <param name="message">Message.</param>  
        /// <param name="stackTrace">Trace of where the message came from.</param>  
        /// <param name="type">Type of message (error, exception, warning, assert).</param>  
        void HandleLog(string message, string stackTrace, LogType type)
        {
            GlobalData.logs.Add(new LogVO
            {
                message = message,
                stackTrace = stackTrace,
                type = type,
            });

            if (GlobalData.logs.Count > GlobalData.maxLogs)
            {
                GlobalData.logs.RemoveAt(0);
            }
        }

        /// <summary>
        /// 打印接收消息日志
        /// </summary>
        /// <param name="bytes"></param>
        public void ShowReciveMsgLog<T>(T tbuff)
        {
            if (tbuff is HallBeatS2C || tbuff is BattleBeatS2C)
            {
                return;
            }
            var logVO = new LogVO();
            logVO.stackTrace = "";
            logVO.type = LogType.Log;
            try
            {
                logVO.message = gameMgrProxy.systemDateTime.ToString("yyyy-MM-dd HH:mm:ss:ffff") + " 接到消息 " + JsonMapper.ToJson(tbuff);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
            }
            GlobalData.logs.Add(logVO);

            if (GlobalData.logs.Count > GlobalData.maxLogs)
            {
                GlobalData.logs.RemoveAt(0);
            }
        }

        /// <summary>
        /// 打印接收消息日志
        /// </summary>
        /// <param name="bytes"></param>
        public void ShowMsgLog(string msg)
        {
            GlobalData.logs.Add(new LogVO
            {
                message = gameMgrProxy.systemDateTime.ToString("yyyy-MM-dd HH:mm:ss:ffff") + " 接到消息 " + msg,
                stackTrace = "",
                type = LogType.Log,
            });

            if (GlobalData.logs.Count > GlobalData.maxLogs)
            {
                GlobalData.logs.RemoveAt(0);
            }
        }

        /// <summary>
        /// 打印发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbuff"></param>
        public void ShowSendMsgLog<T>(MsgNoC2S msgNo,T tbuff,int clientIndex=0)
        {
            var logVO = new LogVO();
            logVO.stackTrace = "";
            logVO.type = LogType.Log;
            try
            {
                logVO.message = gameMgrProxy.systemDateTime.ToString("yyyy-MM-dd HH:mm:ss:ffff") + string.Format("发送消息 消息:{0} 消息号:{1} 消息体:{2} 客户端：{3}", msgNo.ToString(), msgNo.GetHashCode(), JsonMapper.ToJson(tbuff), clientIndex);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
            }
            GlobalData.logs.Add(logVO);

            if (GlobalData.logs.Count > GlobalData.maxLogs)
            {
                GlobalData.logs.RemoveAt(0);
            }
        }

        /// <summary>
        /// 本地消息发送回调处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="type"></param>
        /// <param name="tbuff"></param>
        public void SendBuff<T>(int channel, int type, T tbuff)
        {
            if (channel.GetHashCode() == MsgNoC2S.C2S_Hall_JOIN_IN_ROOM.GetHashCode())
            {
                StartCoroutine(ClientAIMgr.Instance.ClientJoinRoom());
            }
            else if (channel.GetHashCode() == MsgNoC2S.C2S_ROOM_APPLY_DISSOLVE.GetHashCode())
            {
                var loadInfo = new LoadSceneInfo(ESceneID.SCENE_HALL, LoadSceneType.ASYNC, LoadSceneMode.Additive);
                ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
            }
            else if (channel.GetHashCode() == MsgNoC2S.C2S_ROOM_READY.GetHashCode())
            {
                curSit = 1;
                cardPool = new List<int>(GlobalData.CardWare);
                GameMgr.Instance.StartCoroutine(ClientReady());
            }
            else if (channel.GetHashCode() == MsgNoC2S.C2S_ROOM_PLAY_A_MAHJONG.GetHashCode())
            {
                var putCard = tbuff as PlayAMahjongC2S;
                GameMgr.Instance.StartCoroutine(ClientPlayAct(putCard.mahjongCode, playerInfoProxy.userID,
                    PlayerActType.PUT_CARD));
            }
            else if (channel.GetHashCode() == MsgNoC2S.C2S_ROOM_EXIT.GetHashCode())
            {
                var exitS2C = new ExitRoomS2C();
                exitS2C.clientCode = ErrorCode.SUCCESS;
                exitS2C.userId = 9527;
                NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_EXIT_BROADCAST.GetHashCode(), 0, exitS2C);
            }
            else if (channel.GetHashCode() == MsgNoC2S.C2S_ROOM_DISSOLVE.GetHashCode())
            {
                var disloveS2C = new DissolveRoomS2C();
                disloveS2C.clientCode = ErrorCode.SUCCESS;
                NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_DISSOLVE_BROADCAST.GetHashCode(), 0, disloveS2C);
            }
            else if (channel.GetHashCode() == MsgNoC2S.C2S_ROOM_ZI_MO_HU.GetHashCode())
            {
                StartCoroutine(HuHandler());
            }
            else if (channel.GetHashCode() == MsgNoC2S.C2S_ROOM_TEXT_CHAT.GetHashCode())
            {
                SendChatHandler(tbuff as SendChatC2S);
            }
            else if (channel.GetHashCode() == MsgNoC2S.C2S_ROOM_VOICE_CHAT.GetHashCode())
            {
                SendVoiceHandler(tbuff as SendVoiceC2S);
            }
        }

        public IEnumerator HuHandler()
        {
            var pushPlayerActS2C = new PushPlayerActS2C();
            pushPlayerActS2C.actCard = 17;
            pushPlayerActS2C.userId = 9527;
            pushPlayerActS2C.act = PlayerActType.SELF_HU;
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_PLAYER_ACT_BROADCAST.GetHashCode(), 0, pushPlayerActS2C);
            yield return new WaitForSeconds(0.1f);

            PushMatchResultS2C matchResultS2C = new PushMatchResultS2C();
            for (int i = 0; i < GlobalData.SIT_NUM; i++)
            {
                var matchResultPlayerVO = new PlayerMatchResultVOS2C();
                var playerVO = battleProxy.playerSitInfoDic[i + 1];
                matchResultPlayerVO.userId = playerVO.userId;
                matchResultPlayerVO.addScore = i == 0 ? 24 : -8;
                matchResultPlayerVO.handCards.AddRange(playerVO.handCards.ToArray());
                matchResultS2C.resultInfos.Add(matchResultPlayerVO);
            }
            matchResultS2C.huedCard = 17;
            matchResultS2C.huUserId.Add(playerInfoProxy.userID);
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_SINGLE_SCORE_BROADCAST.GetHashCode(), 0, matchResultS2C);

            PushRoomResultS2C roomResultS2C = new PushRoomResultS2C();
            for (int i = 0; i < GlobalData.SIT_NUM; i++)
            {
                var roomResultPlayerVO = new PlayerRoomResultVOS2C();
                var playerVO = battleProxy.playerSitInfoDic[i + 1];
                roomResultPlayerVO.userId = playerVO.userId;
                roomResultPlayerVO.addScore = i == 0 ? 24 : -8;
                roomResultPlayerVO.anGangCount = 0;
                roomResultPlayerVO.mingGangCount = 0;
                roomResultPlayerVO.selfHuCount = i == 0 ? 1 : 0;
                roomResultPlayerVO.otherHuCount = 0;
                roomResultPlayerVO.sendPaoCount = 0;
                roomResultS2C.resultInfos.Add(roomResultPlayerVO);
            }
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_TOTAL_SCORE_BROADCAST.GetHashCode(), 0, roomResultS2C);
        }

        /// <summary>
        /// 模拟进入房间返回
        /// </summary>
        public IEnumerator ClientJoinRoom()
        {
            playerInfoProxy.userID = 9527;
            yield return new WaitForSeconds(1);
            JoinRoomS2C joinRoomS2C = new JoinRoomS2C();
            joinRoomS2C.roomCode = UnityEngine.Random.Range(1111,9999).ToString();
            joinRoomS2C.curInnings = curInnings;
            joinRoomS2C.innings = 1;
            joinRoomS2C.isStart = false;
            joinRoomS2C.createId = playerInfoProxy.userID;
            for (int i = 0; i < GlobalData.SIT_NUM; i++)
            {
                var playerInfoVO = new PlayerInfoVOS2C();
                playerInfoVO.headIcon = "https://gss0.bdstatic.com/6LZ1dD3d1sgCo2Kml5_Y_D3/sys/portrait/item/2f98e788b7e5908de9a39ee736";
                playerInfoVO.isBanker = false;
                playerInfoVO.isMaster = false;
                playerInfoVO.userId = 9527 + i;
                playerInfoVO.isOnline = true;
                playerInfoVO.isReady = false;
                playerInfoVO.name = playNames[i];
                playerInfoVO.score = UnityEngine.Random.Range(1111, 9999);
                playerInfoVO.sex = 1;
                playerInfoVO.sit = i + 1;
                joinRoomS2C.playInfoArr.Add(playerInfoVO);
            }
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_JOIN_ROOM.GetHashCode(),0, joinRoomS2C);
            yield return new WaitForSeconds(1);
            GameMgr.Instance.StartCoroutine(ClientReady());
        }

        /// <summary>
        /// 本地模拟准备和发牌
        /// </summary>
        private IEnumerator ClientReady()
        {
            for (int i = 0; i < GlobalData.SIT_NUM; i++)
            {
                PushReadyS2C pushReadyS2C = new PushReadyS2C();
                pushReadyS2C.userId = 9527 + i;
                NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_READY_BROADCAST.GetHashCode(), 0, pushReadyS2C);
                //Debug.Log("pushReadyS2C.userId = " + pushReadyS2C.userId);
                yield return new WaitForSeconds(0.5f); 
            }

            var pushSendCardS2C = new GameStart_S2C();
            pushSendCardS2C.leftCardCount = GlobalData.CardWare.Length - GlobalData.SENDCARD_NUM;
            pushSendCardS2C.bankerUserId = 9527;
            pushSendCardS2C.touchMahjongCode = GlobalData.Test127Queue.Dequeue();
            pushSendCardS2C.currentTimes = battleProxy.curInnings;
            for (int j = 0; j < 13; j++)
            {
                pushSendCardS2C.handCards.Add(GlobalData.Test127Queue.Dequeue());
            }
            //for (int i = 0; i < GlobalData.SIT_NUM; i++)
            //{
            //    //推送发牌
            //    var pushSendCardS2C = new GameStart_S2C();
            //    pushSendCardS2C.leftCardCount = GlobalData.CardWare.Length - GlobalData.SENDCARD_NUM;
            //    pushSendCardS2C.bankerUserId = 9527;
            //    pushSendCardS2C.touchMahjongCode = GlobalData.Test127Queue.Dequeue();
            //    pushSendCardS2C.currentTimes = battleProxy.curInnings;
            //    for (int j = 0; j < 13; j++)
            //    {
            //        pushSendCardS2C.handCards.Add(GlobalData.Test127Queue.Dequeue());
            //    }
            //    NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_GAME_START_BROADCAST.GetHashCode(), 0, pushSendCardS2C);
            //}

            var pushActTip = new PushPlayerActTipS2C();
            pushActTip.actCards.Add(GlobalData.Test127Queue.Dequeue());
            pushActTip.acts.Add(PlayerActType.PUT_CARD);
            pushActTip.optUserId = 9527;
            pushActTip.tipRemainTime = 15;
            pushActTip.tipRemainUT = gameMgrProxy.systemTime;
            //var pushSendCardS2C1 = new GameStart_S2C();
            pushSendCardS2C.bankerUserId = 9527;
            pushSendCardS2C.pushPlayerActTipS2C = pushActTip;
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_GAME_START_BROADCAST.GetHashCode(), 0, pushSendCardS2C);
            
        }

        /// <summary>
        /// 模拟操作提示推送
        /// </summary>
        private IEnumerator ClientPlayerTip()
        {
            var playerInfoVO = battleProxy.playerSitInfoDic[curSit];
            var playerTipS2C = new PushPlayerActTipS2C();
            playerTipS2C.optUserId = playerInfoVO.userId;
            playerTipS2C.acts.Add(PlayerActType.PUT_CARD);//
            playerTipS2C.actCards.Add(playerInfoVO.getCard);//

            
            playerTipS2C.tipRemainTime = 15;
            playerTipS2C.tipRemainUT = gameMgrProxy.systemTime;
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_PLAYER_ACT_TIP_BROADCAST.GetHashCode(), 0, playerTipS2C);
            //if (playerInfoVO.userId != playerInfoProxy.UserID)//非自己的提示自动操作
            //{
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(ClientPlayAct(playerInfoVO.getCard, playerInfoVO.userId, PlayerActType.PUT_CARD));
            //}

            //StartCoroutine(ClientPlayAct(playerInfoVO.getCard, playerInfoVO.userId, PlayerActType.CHI));

        }

        /// <summary>
        /// 模拟本地动作
        /// </summary>
        private IEnumerator ClientPlayAct(int actCard,int userId,PlayerActType act)
        {
            var pushPlayerActS2C = new PushPlayerActS2C();
            pushPlayerActS2C.actCard = actCard;
            pushPlayerActS2C.userId = userId;
            pushPlayerActS2C.act = act;
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_PLAYER_ACT_BROADCAST.GetHashCode(),0,pushPlayerActS2C);
            yield return new WaitForSeconds(0.5f);
            curSit = GlobalData.GetNextSit(curSit,1);

            if (act == PlayerActType.PUT_CARD)//PUT_CARD
            {
                //int getCard = getRangeCard(false);
                //var getCardS2C = new PushPlayerActS2C();
                //getCardS2C.actCard = getCard;
                //getCardS2C.userId = battleProxy.playerSitInfoDic[curSit].userId;
                //getCardS2C.act = PlayerActType.GET_CARD;
                int getCard = getRangeCard(false);
                var getCardS2C = new PushPlayerActS2C();
                getCardS2C.actCard = getCard;
                getCardS2C.userId = battleProxy.playerSitInfoDic[curSit].userId;
                getCardS2C.act = PlayerActType.GET_CARD;
                NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_PLAYER_ACT_BROADCAST.GetHashCode(), 0, getCardS2C);
                yield return new WaitForSeconds(0.5f);
                GameMgr.Instance.StartCoroutine(ClientPlayerTip());
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                GameMgr.Instance.StartCoroutine(ClientPlayerTip());
            }
        }

        /// <summary>
        /// 模拟客户端发送聊天
        /// </summary>
        /// <param name="chat"></param>
        private void SendChatHandler(SendChatC2S chat)
        {
            var pushChat = new PushSendChatS2C();
            pushChat.senderUserId = playerInfoProxy.userID;
            pushChat.content = chat.content;
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_TEXT_CHAT.GetHashCode(), 0, pushChat);
        }

        private void SendVoiceHandler(SendVoiceC2S chat)
        {
            var pushChat = new PushVoiceS2C();
            pushChat.senderUserId = playerInfoProxy.userID;
            pushChat.content = chat.content;
            pushChat.flag = chat.flag;
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_VOICE_CHAT.GetHashCode(), 0, pushChat);
        }

        /// <summary>
        /// 随机出一张牌
        /// </summary>
        /// <returns></returns>
        private int getRangeCard(bool isSendCard)
        {
            if (isSendCard)
            {
                int randomIndex = UnityEngine.Random.Range(0, cardPool.Count - 1);
                int getCard = cardPool[randomIndex];
                while (getCard == 17)
                {
                    randomIndex = UnityEngine.Random.Range(0, cardPool.Count - 1);
                    getCard = cardPool[randomIndex];
                }
                cardPool.RemoveAt(randomIndex);
                return getCard;
            }
            else
            {
                if (curSit == 1)
                {
                    float randomValue = UnityEngine.Random.Range(0f, 1f);
                    if (randomValue > 0)
                    {
                        cardPool.Remove(17);
                        return 17;
                    }
                    else
                    {
                        int randomIndex = UnityEngine.Random.Range(0, cardPool.Count - 1);
                        int getCard = cardPool[randomIndex];
                        cardPool.RemoveAt(randomIndex);
                        return getCard;
                    }
                }
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, cardPool.Count - 1);
                    int getCard = cardPool[randomIndex];
                    while (getCard == 17)
                    {
                        randomIndex = UnityEngine.Random.Range(0, cardPool.Count - 1);
                        getCard = cardPool[randomIndex];
                    }
                    cardPool.RemoveAt(randomIndex);
                    return 11;
                }
            }
        }

        /// <summary>
        /// 客户端模拟播放战报
        /// </summary>
        public void PlayReport()
        {
            playerInfoProxy.userID = 100763;

            var joinInfo = new JoinRoomS2C();
            joinInfo.createId = 100780;
            joinInfo.curInnings = 1;
            joinInfo.innings = 8;
            joinInfo.isStart = true;
            joinInfo.roomCode = "319851";

            var playerInfoVO = new PlayerInfoVOS2C();
            playerInfoVO.headIcon = "http://picture.youth.cn/xwjx/201705/W020170506391821287688.png";
            playerInfoVO.userId = 100763;
            playerInfoVO.name = "时间";
            playerInfoVO.score = 0;
            playerInfoVO.sex = 1;
            playerInfoVO.sit = 3;
            playerInfoVO.isBanker = false;
            playerInfoVO.isMaster = false;
            playerInfoVO.isReady = true;
            int[] handCards = { 13, 15, 21, 23, 23, 25, 26, 26, 27, 29, 33, 35, 36 };
            playerInfoVO.handCards.AddRange(handCards);
            PengGangCardVO penggangVO = new PengGangCardVO();
            penggangVO.pengGangCards.Add(11);
            penggangVO.pengGangCards.Add(11);
            penggangVO.pengGangCards.Add(11);
            penggangVO.targetUserId = 100780;
            playerInfoVO.pengGangCards.Add(penggangVO);
            penggangVO = new PengGangCardVO();
            penggangVO.pengGangCards.Add(12);
            penggangVO.pengGangCards.Add(12);
            penggangVO.pengGangCards.Add(12);
            penggangVO.targetUserId = 100780;
            playerInfoVO.pengGangCards.Add(penggangVO);
            joinInfo.playInfoArr.Add(playerInfoVO);

            playerInfoVO = new PlayerInfoVOS2C();
            playerInfoVO.headIcon = "http://picture.youth.cn/xwjx/201705/W020170506391821287688.png";
            playerInfoVO.userId = 100764;
            playerInfoVO.name = "电话";
            playerInfoVO.score = UnityEngine.Random.Range(1111, 9999);
            playerInfoVO.sex = 1;
            playerInfoVO.sit = 2;
            playerInfoVO.isBanker = false;
            playerInfoVO.isMaster = false;
            playerInfoVO.isReady = true;
            int[] handCards2 = { 12, 14, 16, 16, 17, 26, 26, 28, 29, 31, 34, 38, 39 };
            playerInfoVO.handCards.AddRange(handCards2);
            penggangVO = new PengGangCardVO();
            penggangVO.pengGangCards.Add(11);
            penggangVO.pengGangCards.Add(11);
            penggangVO.pengGangCards.Add(11);
            penggangVO.targetUserId = 100780;
            playerInfoVO.pengGangCards.Add(penggangVO);
            penggangVO = new PengGangCardVO();
            penggangVO.pengGangCards.Add(12);
            penggangVO.pengGangCards.Add(12);
            penggangVO.pengGangCards.Add(12);
            penggangVO.targetUserId = 100780;
            playerInfoVO.pengGangCards.Add(penggangVO);
            joinInfo.playInfoArr.Add(playerInfoVO);

            playerInfoVO = new PlayerInfoVOS2C();
            playerInfoVO.headIcon = "http://picture.youth.cn/xwjx/201705/W020170506391821287688.png";
            playerInfoVO.userId = 100773;
            playerInfoVO.name = "多少";
            playerInfoVO.score = UnityEngine.Random.Range(1111, 9999);
            playerInfoVO.sex = 1;
            playerInfoVO.sit = 4;
            playerInfoVO.isBanker = false;
            playerInfoVO.isMaster = false;
            playerInfoVO.isReady = true;
            int[] handCards3 = { 11, 12, 16, 19, 22, 23, 24, 25, 33, 34, 35, 36, 39 };
            playerInfoVO.handCards.AddRange(handCards3);
            penggangVO = new PengGangCardVO();
            penggangVO.pengGangCards.Add(11);
            penggangVO.pengGangCards.Add(11);
            penggangVO.pengGangCards.Add(11);
            penggangVO.targetUserId = 100780;
            playerInfoVO.pengGangCards.Add(penggangVO);
            penggangVO = new PengGangCardVO();
            penggangVO.pengGangCards.Add(12);
            penggangVO.pengGangCards.Add(12);
            penggangVO.pengGangCards.Add(12);
            penggangVO.targetUserId = 100780;
            playerInfoVO.pengGangCards.Add(penggangVO);
            joinInfo.playInfoArr.Add(playerInfoVO);

            playerInfoVO = new PlayerInfoVOS2C();
            playerInfoVO.headIcon = "http://picture.youth.cn/xwjx/201705/W020170506391821287688.png";
            playerInfoVO.userId = 100780;
            playerInfoVO.name = "学姐2";
            playerInfoVO.score = UnityEngine.Random.Range(1111, 9999);
            playerInfoVO.sex = 1;
            playerInfoVO.sit = 1;
            playerInfoVO.isBanker = true;
            playerInfoVO.isMaster = true;
            playerInfoVO.isReady = true;
            int[] handCards4 = { 11, 13, 14, 25, 27, 28, 33, 34, 36, 37, 37, 37, 38 };
            playerInfoVO.handCards.AddRange(handCards4);
            penggangVO = new PengGangCardVO();
            penggangVO.pengGangCards.Add(11);
            penggangVO.pengGangCards.Add(11);
            penggangVO.pengGangCards.Add(11);
            penggangVO.targetUserId = 100780;
            playerInfoVO.pengGangCards.Add(penggangVO);
            penggangVO = new PengGangCardVO();
            penggangVO.pengGangCards.Add(12);
            penggangVO.pengGangCards.Add(12);
            penggangVO.pengGangCards.Add(12);
            penggangVO.targetUserId = 100780;
            playerInfoVO.pengGangCards.Add(penggangVO);
            joinInfo.playInfoArr.Add(playerInfoVO);

            joinInfo.playerTipAct = new PushPlayerActTipS2C();
            joinInfo.playerTipAct.optUserId = 100780;
            joinInfo.playerTipAct.acts.Add(PlayerActType.PUT_CARD);
            joinInfo.playerTipAct.actCards.Add(14);
            joinInfo.playerTipAct.tipRemainTime = 15;
            joinInfo.playerTipAct.tipRemainUT = 1495873615422;

            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_JOIN_ROOM.GetHashCode(), 0, joinInfo);
        }

        /// <summary>
        /// AI出牌
        /// </summary>
        public void AIPutCard()
        {
            if (!GlobalData.isDebugModel || !battleProxy.isSelfAction || battleProxy.isReport)
            {
                return;
            }
            if (battleProxy.GetPlayerActTipS2C().acts.Contains(PlayerActType.PASS))
            {
                SpecialCardHandler();
            }
            else if (battleProxy.GetPlayerActTipS2C().acts.Contains(PlayerActType.PUT_CARD))
            {
                PutCardHandler();
            }
        }

        /// <summary>
        /// 可选的吃牌操作
        /// </summary>
        List<ChiPowerVO> chiPowerArr;
        /// <summary>
        /// 特殊出牌操作
        /// </summary>
        private void SpecialCardHandler()
        {
            List<ActPowerVO> actPowers = new List<ActPowerVO>();
            List<int> canPutCards = new List<int>();
            PlayerInfoVOS2C selfPlayerInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
            foreach (int card in selfPlayerInfoVO.handCards)
            {
                canPutCards.Add(card);
            }
            if (selfPlayerInfoVO.getCard > 0)
            {
                canPutCards.Add(selfPlayerInfoVO.getCard);
            }
            for (int i = 0; i < battleProxy.GetPlayerActTipS2C().acts.Count; i++)
            {
                actPowers.Add(new ActPowerVO(battleProxy.GetPlayerActTipS2C().acts[i], battleProxy.GetPlayerActTipS2C().actCards[i], 0));
            }

            foreach (ActPowerVO actPowerVO in actPowers)
            {
                bool passAddPower = false;//过操作是否加权
                if (battleProxy.huTypes.Contains(actPowerVO.act))//胡牌权重最高
                {
                    actPowerVO.power += 10000;
                }
                else if (actPowerVO.act == PlayerActType.CHI)//吃牌判断
                {
                    CalculateChiPower(actPowerVO, canPutCards, out passAddPower);
                }
                else if (actPowerVO.act != PlayerActType.PASS)//碰杠牌操作
                {
                    CalculatePengGangPower(actPowerVO, canPutCards, out passAddPower);
                }
                if (passAddPower)
                {
                    foreach (ActPowerVO actPowerVO2 in actPowers)
                    {
                        if (actPowerVO2.act == PlayerActType.PASS)
                        {
                            actPowerVO2.power += 100;
                        }
                    }
                }
            }
            actPowers.Sort((ActPowerVO actPowerVO1, ActPowerVO actPowerVO2) => {
                if (actPowerVO1.power > actPowerVO2.power)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            FirstActHandler(actPowers[0]);
        }

        /// <summary>
        /// 计算吃操作的权重
        /// </summary>
        /// <param name="actPowerVO">权重VO</param>
        /// <param name="canPutCards">可出的牌数组</param>
        /// <param name="passAddPower">是否增加过操作权重</param>
        private void CalculateChiPower(ActPowerVO actPowerVO,List<int> canPutCards,out bool passAddPower)
        {
            List<List<int>> chiSelectArr = BattleAreaUtil.GetCanChiArr(actPowerVO.actCard);
            chiPowerArr = new List<ChiPowerVO>();
            for (int i = 0; i < chiSelectArr.Count; i++)
            {
                var copyCanPutCards = new List<int>(canPutCards);
                List<int> forbitCards = new List<int>();

                if (chiSelectArr[i][0] == actPowerVO.actCard + 1)
                {
                    if (Array.IndexOf(GlobalData.CardValues, actPowerVO.actCard + 3) != -1)
                    {
                        forbitCards.Add(actPowerVO.actCard + 3);
                    }
                }
                else if (chiSelectArr[i][0] == actPowerVO.actCard - 2)
                {
                    if (Array.IndexOf(GlobalData.CardValues, actPowerVO.actCard - 3) != -1)
                    {
                        forbitCards.Add(actPowerVO.actCard - 3);
                    }
                }
                ChiPowerVO addChiPowerVO = new ChiPowerVO(chiSelectArr[i], actPowerVO.actCard, forbitCards, 0);
                chiPowerArr.Add(addChiPowerVO);
                bool isBreakCard = false;
                for (int j = 0; j < chiSelectArr[i].Count; j++)
                {
                    List<int> listFind = copyCanPutCards.FindAll(delegate (int s) {
                        return s == chiSelectArr[i][j];
                    });
                    if (listFind.Count > 1)//需要拆对
                    {
                        isBreakCard = true;
                    }
                }
                if (!isBreakCard)
                {
                    addChiPowerVO.power += 1000;
                }

                for (int j = 0; j < chiSelectArr[i].Count; j++)
                {
                    copyCanPutCards.Remove(chiSelectArr[i][j]);
                }
                bool hasDouble = false;
                for (int j = 0; j < copyCanPutCards.Count; j++)
                {
                    if (j > 0 && copyCanPutCards[j] == copyCanPutCards[j - 1])
                    {
                        hasDouble = true;
                        break;
                    }
                }
                if (hasDouble)
                {
                    addChiPowerVO.power += 1000;
                }
            }
            chiPowerArr.Sort((ChiPowerVO chiPowerVO1, ChiPowerVO chiPowerVO2) =>
            {
                if (chiPowerVO1.power > chiPowerVO2.power)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            if (chiPowerArr[0].power <= 0)
            {
                passAddPower = true;
            }
            else
            {
                passAddPower = false;
                actPowerVO.power += 50;
            }
        }

        /// <summary>
        /// 计算碰杠牌操作权重
        /// </summary>
        /// <param name="actPowerVO">权重VO</param>
        /// <param name="copyCanPutCards">可出的牌数组</param>
        /// <param name="passAddPower">是否增加过操作权重</param>
        private void CalculatePengGangPower(ActPowerVO actPowerVO,List<int> canPutCards,out bool passAddPower)
        {
            passAddPower = false;
            List<int> copyCanPutCards = new List<int>(canPutCards);
            copyCanPutCards.Remove(actPowerVO.actCard);
            copyCanPutCards.Remove(actPowerVO.actCard);
            copyCanPutCards.Remove(actPowerVO.actCard);
            copyCanPutCards.Remove(actPowerVO.actCard);
            copyCanPutCards.Sort();
            bool hasDouble = false;
            for (int i = 0; i < copyCanPutCards.Count; i++)
            {
                if (i > 0 && copyCanPutCards[i] == copyCanPutCards[i - 1])
                {
                    hasDouble = true;
                    break;
                }
            }
            if (hasDouble)
            {
                actPowerVO.power += 1000;
            }
            else
            {
                passAddPower = true;
            }
        }

        /// <summary>
        /// 第一操作响应
        /// </summary>
        /// <param name="actPowerVO"></param>
        private void FirstActHandler(ActPowerVO actPowerVO)
        {
            if (actPowerVO.act == PlayerActType.PASS)
            {
                var actC2S = new GuoC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PASS.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.PENG)
            {
                var actC2S = new PengC2S();
                actC2S.mahjongCode = actPowerVO.actCard;
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PENG.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.CHI)
            {
                var actC2S = new ChiC2S();
                actC2S.mahjongCodes.AddRange(chiPowerArr[0].chiCards);
                actC2S.mahjongCodes.Add(chiPowerArr[0].chiCard);
                actC2S.forbitCards.AddRange(chiPowerArr[0].forbitCards);
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PENG.GetHashCode(), 0, actC2S);
            }
            else if (actPowerVO.act == PlayerActType.SELF_HU)
            {
                var actC2S = new ZiMoHuC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_ZI_MO_HU.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.QIANG_AN_GANG_HU)
            {
                var actC2S = new QiangAnGangHuC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_AN_GANG_HU.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.QIANG_PENG_GANG_HU)
            {
                var actC2S = new QiangPengGangHuC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_PENG_GANG_HU.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.QIANG_ZHI_GANG_HU)
            {
                var actC2S = new QiangZhiGangHuC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_ZHI_GANG_HU.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.CHI_HU)
            {
                var actC2S = new ChiHuC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_CHI_HU.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.COMMON_AN_GANG)
            {
                var actC2S = new CommonAnGangC2S();
                actC2S.mahjongCode = actPowerVO.actCard;
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_COMMON_AN_GANG.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.BACK_AN_GANG)
            {
                var actC2S = new BackAnGangC2S();
                actC2S.mahjongCode = actPowerVO.actCard;
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_BACK_AN_GANG.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.ZHI_GANG)
            {
                var actC2S = new ZhiGangC2S();
                actC2S.mahjongCode = actPowerVO.actCard;
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_ZHI_GANG.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.COMMON_PENG_GANG)
            {
                var actC2S = new CommonPengGangC2S();
                actC2S.mahjongCode = actPowerVO.actCard;
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_COMMON_PENG_GANG.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
            else if (actPowerVO.act == PlayerActType.BACK_PENG_GANG)
            {
                var actC2S = new BackPengGangC2S();
                actC2S.mahjongCode = actPowerVO.actCard;
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_BACK_PENG_GANG.GetHashCode(), 0, actC2S);
                chiPowerArr = null;
            }
        }

        /// <summary>
        /// 出牌
        /// </summary>
        private void PutCardHandler()
        {
            List<CardPowerVO> cardPowerList = new List<CardPowerVO>();
            PlayerInfoVOS2C selfPlayerInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
            List<int> canPutCards = new List<int>();
            foreach (int card in selfPlayerInfoVO.handCards)
            {
                cardPowerList.Add(new CardPowerVO(card, 0));
                canPutCards.Add(card);
            }
            if (selfPlayerInfoVO.getCard > 0)
            {
                cardPowerList.Add(new CardPowerVO(selfPlayerInfoVO.getCard, 0));
                canPutCards.Add(selfPlayerInfoVO.getCard);
            }
            for (int i = 0; i < cardPowerList.Count; i++)
            {
                var cardPowerVO = cardPowerList[i];
                List<int> listFind = canPutCards.FindAll(delegate (int s) {
                    return s == cardPowerVO.cardValue;
                });
                if (chiPowerArr != null && chiPowerArr[0].forbitCards.Contains(cardPowerVO.cardValue))//有禁止出的牌
                {
                    cardPowerVO.power += 10000;
                }
                if (listFind.Count > 1)//有相同牌
                {
                    cardPowerVO.power += 100;
                }
                if (canPutCards.Contains(cardPowerVO.cardValue - 1))//有头牌
                {
                    cardPowerVO.power += 10;
                }
                if (canPutCards.Contains(cardPowerVO.cardValue + 1))//有尾牌
                {
                    cardPowerVO.power += 10;
                }
                var modValue = cardPowerVO.cardValue % 10;
                if (modValue > 2 && modValue < 8)//判断是否存在间隔的牌
                {
                    if (canPutCards.Contains(cardPowerVO.cardValue - 2))
                    {
                        cardPowerVO.power += 1;
                    }
                    if (canPutCards.Contains(cardPowerVO.cardValue + 2))
                    {
                        cardPowerVO.power += 1;
                    }
                }
                //统计同类型牌数量
                List<int> sampleTypeList = canPutCards.FindAll(delegate (int s) {
                    return s % 10 == modValue;
                });
                cardPowerVO.power += (float)sampleTypeList.Count / 100;
            }
            cardPowerList.Sort((CardPowerVO cardPower1, CardPowerVO cardPower2) => {
                if (cardPower1.power < cardPower2.power)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            var putC2S = new PlayAMahjongC2S();
            putC2S.mahjongCode = cardPowerList[0].cardValue;
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PLAY_A_MAHJONG.GetHashCode(), 0, putC2S);
            chiPowerArr = null;
        }
    }
}