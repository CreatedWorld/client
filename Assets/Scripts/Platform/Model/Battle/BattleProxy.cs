using System.Collections.Generic;
using Platform.Net;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;
using Platform.Global;
using Platform.Model.VO.BattleVO;
using Utils;
using System;
using LZR.Data.NetWork.Client;

namespace Platform.Model.Battle
{
    /// <summary>
    /// 牌局数据代理
    /// </summary>
    internal class BattleProxy : Proxy, IProxy
    {
        /// <summary>
        /// 游戏总局数
        /// </summary>
        public int innings;
        /// <summary>
        /// 当前是第几局
        /// </summary>
        public int curInnings = 1;

        /// <summary>
        /// 本局是否已开始
        /// </summary>
        public bool isStart;
        /// <summary>
        /// 是否禁止操作正在发牌
        /// </summary>
        private bool _isForbit = false;
        /// <summary>
        /// 禁止期间未播放的动作队列
        /// </summary>
        private List<ForbitActionVO> forbitActions;
        /// <summary>
        /// 是否跳过缓动
        /// </summary>
        public bool isSkipTween;
        /// <summary>
        /// 房间房主id
        /// </summary>
        public int creatorId;
        /// <summary>
        /// 玩法类型
        /// </summary>
        public List<int> playType = new List<int>();
        /// <summary>
        /// 本局结算信息
        /// </summary>
        public PushMatchResultS2C matchResultS2C;
        /// <summary>
        /// 之前的庄家id
        /// </summary>
        public int perBankerId;

        /// <summary>
        /// 当前播放的玩家动作
        /// </summary>
        private PushPlayerActS2C _playerActS2C;

        /// <summary>
        /// 推送的玩家操作提示
        /// </summary>
        private PushPlayerActTipS2C _playerActTipS2C;

        /// <summary>
        /// 玩家信息字典{LocalId:玩家信息VO}
        /// </summary>
        public Dictionary<int, PlayerInfoVOS2C> playerIdInfoDic;

        /// <summary>
        /// 玩家信息字典{Sit:玩家信息VO}
        /// </summary>
        public Dictionary<int, PlayerInfoVOS2C> playerSitInfoDic;
        
        /// <summary>
        /// 是否轮到自己出手
        /// </summary>
        public bool isSelfAction;
        /// <summary>
        /// 房间结算信息
        /// </summary>
        public PushRoomResultS2C roomResultS2C;
        /// <summary>
        /// 剩余牌数
        /// </summary>
        public int leftCard;
        /// <summary>
        /// 语音缓存队列
        /// </summary>
        public Queue<AudioPacket> speekPacket = new Queue<AudioPacket>();
        /// <summary>
        /// 胡牌类型
        /// </summary>
        public List<PlayerActType> huTypes = new List<PlayerActType>();
        /// <summary>
        /// 隐藏执行的类型
        /// </summary>
        public List<PlayerActType> hidenActTypes = new List<PlayerActType>();
        /// <summary>
        /// 当前播放的战报
        /// </summary>
        public PlayReportS2C report;
        /// <summary>
        /// 战报开始时的本地时间
        /// </summary>
        public float reportLocalTime;
        /// <summary>
        /// 当前是否播放战报
        /// </summary>
        public bool isReport = false;
        /// <summary>
        /// 战报移动记录数组
        /// </summary>
        public List<List<PlayerCardVO>> reportActions;
        /// <summary>
        /// 之前发送语言聊天的时间
        /// </summary>
        public long perSendChatTime = 0;
        /// <summary>
        /// 是否正在播放胡牌动画
        /// </summary>
        public bool isPlayHu = false;
        /// <summary>
        /// 刚刚收到的解散申请
        /// </summary>
        public int disloveApplyUserId;
        /// <summary>
        /// 本局开始时间
        /// </summary>
        public long startTime;
        /// <summary>
        /// 申请解散剩余时间
        /// </summary>
        public int disloveRemainTime;
        /// <summary>
        /// 解散剩余时间戳
        /// </summary>
        public long disloveRemainUT;
        /// <summary>
        /// 同意的玩家id数组
        /// </summary>
        public List<int> agreeIds = new List<int>();
        /// <summary>
        /// 不同意的玩家id数组
        /// </summary>
        public List<int> refuseIds = new List<int>();
        /// <summary>
        /// 是否有申请解散
        /// </summary>
        public bool hasDisloveApply;
        /// <summary>
        /// 听的牌数组
        /// </summary>
        public List<int> tingCards = new List<int>();
        /// <summary>
        /// 当前指向
        /// </summary>
        public GuideType curGuide = GuideType.NULL;
        /// <summary>
        /// 拿牌的牌堆起始序号,自己的牌堆第一张牌为0，需要将后端的起始序号转为自己的起始序号
        /// </summary>
        public int sendHeapStartIndex = 76;
        /// <summary>
        /// 牌堆内未拿的牌序号数组
        /// </summary>
        public List<int> unGetHeapCardIndexs;
        /// <summary>
        /// 色子点数（第一个表示方向，第二个表示第几摞开始拿）
        /// </summary>
        //public static List<int> dices = new List<int>();
        /// <summary>
        /// 是否进入输入框
        /// </summary>
        public bool isEnterInput = false;
        
        /// <summary>
        /// 玩家分数
        /// </summary>
        public int score;

        public BattleProxy(string NAME) : base(NAME)
        {
            huTypes.Add(PlayerActType.CHI_HU);
            huTypes.Add(PlayerActType.QIANG_AN_GANG_HU);
            huTypes.Add(PlayerActType.QIANG_PENG_GANG_HU);
            huTypes.Add(PlayerActType.QIANG_ZHI_GANG_HU);
            huTypes.Add(PlayerActType.SELF_HU);
            hidenActTypes.Add(PlayerActType.PENG);
            hidenActTypes.Add(PlayerActType.BACK_AN_GANG);
            hidenActTypes.Add(PlayerActType.BACK_PENG_GANG);
            hidenActTypes.Add(PlayerActType.COMMON_AN_GANG);
            hidenActTypes.Add(PlayerActType.COMMON_PENG_GANG);
            hidenActTypes.Add(PlayerActType.PASS);
            hidenActTypes.Add(PlayerActType.CHI_HU);
            hidenActTypes.Add(PlayerActType.QIANG_AN_GANG_HU);
            hidenActTypes.Add(PlayerActType.QIANG_PENG_GANG_HU);
            hidenActTypes.Add(PlayerActType.QIANG_ZHI_GANG_HU);
            hidenActTypes.Add(PlayerActType.SELF_HU);
            hidenActTypes.Add(PlayerActType.ZHI_GANG);
            hidenActTypes.Add(PlayerActType.CHI);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ENTER_ROOM, CreateRoomHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_JOIN_ROOM, JoinInRoomHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_JOIN_ROOM_BROADCAST, PushJoinHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_EXIT_BROADCAST, ExitHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_DISSOLVE_BROADCAST,DissolutionHandler); 
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_READY_BROADCAST, PushReadyHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_GAME_START_BROADCAST, GameStartHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_PLAYER_ACT_BROADCAST, PushPlayerActHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_PLAYER_ACT_TIP_BROADCAST, PushPlayerActTipHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_SINGLE_SCORE_BROADCAST, PushMatchEndHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_TOTAL_SCORE_BROADCAST, PushRoomEndHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_VOICE_CHAT, PushVoiceHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_TEXT_CHAT, PushChatHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Get_UserInfo_By_Id, GetUserInfoHandler);// S2C_Hall_Get_UserInfo_By_Id
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_APPLY_DISSOLVE, DisloveApplyHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_CANCEL_APPLY_DISSOLVE, DisloveCancelHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_Comfirm_Dissolve, DisloveConfirmHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Get_Replay_Data, PlayVideo);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_ActErrorS2C, ActErrorHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_ONLINESETTING_BROADCAST, OnlineSettingHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_PLAY_A_MAHJONG, PlayAmahjongHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_SetCardS2C, SetCardHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_GetAllCardS2C, GetAllCardHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_TOU_HE_TIP, ShowTouHeTipHandler);
            GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_ROOM_TOU_HE, TouHeHandler);
            //GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_BAOTING, BaoTingHandler);
            //GameMgr.Instance.AddMsgHandler(MsgNoS2C.CORE_PUSHSSSMATCHRESULT, ？);
        }
        /// <summary>
        /// 点击投河返回
        /// </summary>
        /// <param name="data"></param>
        public void ShowTouHeTipHandler(byte[] bytes)
        {
            SendNotification(NotificationConstant.MEDI_ROOM_TOUHE);
        }
        /// <summary>
        /// 显示投河
        /// </summary>
        /// <param name="bytes"></param>
        public void TouHeHandler(byte[] bytes)
        {
            touHeS2C touheS2C = NetMgr.Instance.DeSerializes<touHeS2C>(bytes);
            //Debug.Log(touheS2C.userId + " ==  "+touheS2C.touHeCode);
            SendNotification(NotificationConstant.MEDI_ROOM_HIDETOUHE,touheS2C.userId);
        }
        /// <summary>
        /// 推送报听
        /// </summary>
        /// <param name="bytes"></param>
        public void BaoTingHandler()
        {
            SendNotification(NotificationConstant.MEDI_ROOM_BAOTING, GetPlayerActS2C().baoCards);
        }
        /// <summary>
        /// 推送报夹
        /// </summary>
        /// <param name="bytes"></param>
        public void BaoJiaHandler()
        {
            SendNotification(NotificationConstant.MEDI_ROOM_BAOJIA, GetPlayerActS2C().baoCards);
        }
        /// <summary>
        /// 推送报吊
        /// </summary>
        /// <param name="bytes"></param>
        public void BaoDiaoHandler()
        {
            SendNotification(NotificationConstant.MEDI_ROOM_BAODIAO, GetPlayerActS2C().baoCards);
        }
        /// <summary>
        /// 推送抻吊
        /// </summary>
        /// <param name="bytes"></param>
        public void ChenDiaoHandler()
        {
            SendNotification(NotificationConstant.MEDI_ROOM_CHENDIAO, GetPlayerActS2C().baoCards);
        }
        /// <summary>
        /// 玩家动作
        /// </summary>
        public PushPlayerActS2C GetPlayerActS2C()
        {
            return _playerActS2C;
        }

        /// <summary>
        /// 玩家动作
        /// </summary>
        public void SetPlayerActS2C(PushPlayerActS2C value)
        {
            _playerActS2C = value;
            if (value != null)
            {
                curGuide = GuideType.ACT;
            }
        }

        /// <summary>
        /// 玩家动作提示
        /// </summary>
        public PushPlayerActTipS2C GetPlayerActTipS2C()
        {
            return _playerActTipS2C;
        }

        /// <summary>
        /// 玩家动作提示
        /// </summary>
        public void SetPlayerActTipS2C(PushPlayerActTipS2C value)
        {
            _playerActTipS2C = value;
            if (value != null)
            {
                curGuide = GuideType.ACT_TIP;
            }
        }

        /// <summary>
        /// 庄家VOS2C
        /// </summary>
        public PlayerInfoVOS2C GetBankerPlayerInfoVOS2C()
        {
            foreach (var playerInfoVOS2C in playerIdInfoDic)
            {
                if (playerInfoVOS2C.Value.isBanker)
                {
                    return playerInfoVOS2C.Value;
                }
            }
                
            return null;
        }
        /// <summary>
        /// 东家VOS2C
        /// </summary>
        public PlayerInfoVOS2C GetMasterPlayerInfoVOS2C()
        {
            foreach (var playerInfoVOS2C in playerIdInfoDic)
            {
                if (playerInfoVOS2C.Value.isMaster)
                {
                    return playerInfoVOS2C.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 是否禁止操作
        /// </summary>
        public bool GetIsForbit()
        {
            return _isForbit;
        }

        /// <summary>
        /// 是否禁止操作
        /// </summary>
        public void SetIsForbit(bool value)
        {
            _isForbit = value;
            if (!value)
            {
                PlayActionArr();
            }
        }

        /// <summary>
        /// 播放缓存的动作
        /// </summary>
        private void PlayActionArr()
        {
            isSkipTween = true;
            for (int i = 0; i < forbitActions.Count; i++)
            {
                if (forbitActions[i].isActTip)
                {
                    PushPlayerActTipHandler(forbitActions[i].bytes);
                }
                else
                {
                    PushPlayerActHandler(forbitActions[i].bytes);
                }
            }
            forbitActions.Clear();
            isSkipTween = false;
        }

        /// <summary>
        /// 创建房间返回
        /// </summary>
        /// <param PlayerName="bytes"></param>
        private void CreateRoomHandler(byte[] bytes)
        {
            SetPlayerActTipS2C(null);
            playerIdInfoDic = new Dictionary<int, PlayerInfoVOS2C>();
            var selfInfoVO = new PlayerInfoVOS2C();
            var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            RoomInfo.RoomId = hallProxy.HallInfo.roomCode;
            selfInfoVO.headIcon = playerInfoProxy.headIconUrl;
            selfInfoVO.isBanker = false;
            selfInfoVO.isMaster = false;
            selfInfoVO.isReady = false;
            selfInfoVO.userId = playerInfoProxy.userID;
            selfInfoVO.name = playerInfoProxy.userName;
            selfInfoVO.score = playerInfoProxy.score;
            selfInfoVO.sex = playerInfoProxy.sex;
            selfInfoVO.sit = hallProxy.HallInfo.seat;
            selfInfoVO.isOnline = true;
            tingCards.Clear();
            playerIdInfoDic.Add(selfInfoVO.userId, selfInfoVO);
            UpdatePlayerSitDic();
            curInnings = 1;
            creatorId = playerInfoProxy.userID;
            isStart = false;
            isSelfAction = false;
            hasDisloveApply = false;
            agreeIds.Clear();
            refuseIds.Clear();
            //if (hallProxy.HallInfo.gameRule91 == GameRule91.)
            //{
            //    leftCard = GlobalData.CardWare.Length;
            //}
            //else
            //{
            //    leftCard = GlobalData.CardWare.Length - 16;
            //}
            UIManager.Instance.HideUI(UIViewID.CREATEROOM_VIEW,
                () =>
                {
                    var loadInfo = new LoadSceneInfo(ESceneID.SCENE_BATTLE, LoadSceneType.ASYNC, LoadSceneMode.Additive);
                    SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
                });

            var readyC2S = new ReadyC2S();
            NetMgr.Instance.SendBuff(SocketType.BATTLE,MsgNoC2S.C2S_ROOM_READY.GetHashCode(), 0, readyC2S);
            if (!isReport && GlobalData.LoginServer != "127.0.0.1")
            {
                var settingC2S = new OnlineSettingC2S();
                settingC2S.isOnline = true;
                NetMgr.Instance.ConnentionDic[SocketType.BATTLE].SendBuff(MsgNoC2S.C2S_ROOM_ONLINESETTING.GetHashCode(), 0, settingC2S, true);
            }
        }

        /// <summary>
        /// 加入房间返回
        /// </summary>
        /// <param PlayerName="bytes">消息体</param>
        private void JoinInRoomHandler(byte[] bytes)
        {
            GlobalData.ComfirSit = true;
            forbitActions = new List<ForbitActionVO>();
            var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            playerIdInfoDic = new Dictionary<int, PlayerInfoVOS2C>();
            var joinRoomS2C = NetMgr.Instance.DeSerializes<JoinRoomS2C>(bytes);
            playType = joinRoomS2C.playType;
            RoomInfo.RoomId = joinRoomS2C.roomCode;
            //RoomInfo.Rule = joinRoomS2C.playType
            if (joinRoomS2C.innings == 8)
            {
                RoomInfo.Round = (joinRoomS2C.curInnings.ToString() +"/8");
            }else RoomInfo.Round = "一锅";
            for (int i = 0; i < playType.Count; i++)
            {
                if (playType[i] == 44)
                {
                    RoomInfo.Rule1 = "【三家炮】";
                }
                if (playType[i] == 45)
                {
                    RoomInfo.Rule1 = "【一家炮】";
                }
                if (playType[i] == 46)
                {
                    RoomInfo.Rule2 = "【蛋带翻】";
                }
                if (playType[i] == 47)
                {
                    RoomInfo.Rule2 += "【蛋不翻】";
                }
                if (playType[i] == 48)
                {
                    RoomInfo.Rule3 = "【长跑3】";
                }
                if (playType[i] == 49)
                {
                    RoomInfo.Rule3 = "【长跑5】";
                }
                if (playType[i] == 50)
                {
                    RoomInfo.Rule3 = "【长跑10】";
                }
            }
            
            innings = joinRoomS2C.innings;
            curInnings = joinRoomS2C.curInnings;
            creatorId = joinRoomS2C.createId;
            isStart = joinRoomS2C.isStart;
            startTime = joinRoomS2C.startTime;
            hasDisloveApply = joinRoomS2C.hasDisloveApply;
            agreeIds = joinRoomS2C.agreeIds;
            refuseIds = joinRoomS2C.refuseIds;
            disloveRemainTime = joinRoomS2C.disloveRemainTime;
            disloveRemainUT = joinRoomS2C.disloveRemainUT;
            tingCards = joinRoomS2C.tingCards;
            if (joinRoomS2C.agreeIds.Count > 0)
            {
                disloveApplyUserId = joinRoomS2C.agreeIds[0];
            }
            foreach (var voS2C in joinRoomS2C.playInfoArr)
            {
                if (isStart)//已开局牌局手中的牌自动排序
                {
                    voS2C.handCards.Sort();
                }
                playerIdInfoDic.Add(voS2C.userId, voS2C);
            }
            UpdatePlayerSitDic();
            hallProxy.HallInfo.roomCode = joinRoomS2C.roomCode;
            hallProxy.HallInfo.innings = (GameMode)joinRoomS2C.innings;
            //hallProxy.HallInfo.gameRule = (GameRule)joinRoomS2C.gameRule;
            curInnings = joinRoomS2C.curInnings;
            SetPlayerActTipS2C(joinRoomS2C.playerTipAct);
            if (GetPlayerActTipS2C() != null)
            {
                isSelfAction = GetPlayerActTipS2C().optUserId == playerInfoProxy.userID;
            }

            if (isStart)
            {
                leftCard = joinRoomS2C.leftCardCount;
                if (GlobalData.hasHeap)
                {
                    InitHeapCardIndexs(leftCard);
                }
            }
            else
            {
                //游戏没开始就自动发准备
                ReadyC2S ready = new ReadyC2S();
                NetMgr.Instance.SendBuff<ReadyC2S>(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_READY.GetHashCode(), 0, ready);
               
            }
            if (UIManager.Instance.GetUIView(UIViewID.JOINROOM_VIEW).isShow)
            {
                UIManager.Instance.HideUI(UIViewID.JOINROOM_VIEW, EnterBattle);
            }
            else if (UIManager.Instance.GetUIView(UIViewID.MATCHING_VIEW).isShow)
            {
                UIManager.Instance.HideUI(UIViewID.MATCHING_VIEW, EnterBattle);
            }
            else
            {
                EnterBattle();
            }
            if (joinRoomS2C.optUserId != 0)//需要添加这个字段
            {

                GlobalData.sit = playerIdInfoDic[playerInfoProxy.userID].sit;
                GlobalData.optUserId = playerIdInfoDic[joinRoomS2C.optUserId].sit;
                Debug.Log(string.Format("当前玩家的座位号：{0}，轮到{1}家出牌", GlobalData.sit, GlobalData.optUserId));
            }

            if (!isReport && GlobalData.LoginServer != "127.0.0.1")
            {
                var settingC2S = new OnlineSettingC2S();
                settingC2S.isOnline = true;
                NetMgr.Instance.ConnentionDic[SocketType.BATTLE].SendBuff(MsgNoC2S.C2S_ROOM_ONLINESETTING.GetHashCode(), 0, settingC2S, true);
            }
            ClientAIMgr.Instance.AIPutCard();
        }

        /// <summary>
        /// 初始化牌堆未拿牌的序号
        /// </summary>
        private void InitHeapCardIndexs(int leftCardValue)
        {
            unGetHeapCardIndexs = new List<int>();
            int recivedCard = GlobalData.CardWare.Length - leftCardValue;
            for (int i = sendHeapStartIndex + recivedCard; i < GlobalData.CardWare.Length; i++)
            {
                unGetHeapCardIndexs.Add(i);
            }
            int addStart = 0;
            if (sendHeapStartIndex + recivedCard > GlobalData.CardWare.Length - 1)
            {
                addStart = (sendHeapStartIndex + recivedCard) % GlobalData.CardWare.Length;
            }
            for (int i = addStart; i < sendHeapStartIndex; i++)
            {
                unGetHeapCardIndexs.Add(i);
            }
        }

        /// <summary>
        /// 进入战斗场景
        /// </summary>
        private void EnterBattle()
        {
            var loadInfo = new LoadSceneInfo(ESceneID.SCENE_BATTLE, LoadSceneType.ASYNC,
                            LoadSceneMode.Additive);
            SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
        }

        /// <summary>
        /// 推送玩家加入房间
        /// </summary>
        /// <param PlayerName="bytes"></param>
        private void PushJoinHandler(byte[] bytes)
        {
            GlobalData.ShowSendCardAnimation = false;
            var pushJoinS2C = NetMgr.Instance.DeSerializes<PushJoinS2C>(bytes);
            playerIdInfoDic.Add(pushJoinS2C.playerInfo.userId, pushJoinS2C.playerInfo);
            UpdatePlayerSitDic();
            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATESINGLEHEAD, pushJoinS2C.playerInfo);
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param PlayerName="bytes"></param>
        private void ExitHandler(byte[] bytes)
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var exitS2C = NetMgr.Instance.DeSerializes<ExitRoomS2C>(bytes);
            if (exitS2C.clientCode != ErrorCode.SUCCESS)
            {
                DialogMsgVO dialogMsgVO = new DialogMsgVO();
                dialogMsgVO.title = "退出";
                dialogMsgVO.dialogType = DialogType.ALERT;
                dialogMsgVO.content = "房间开局后不可中途退出";
                DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
                dialogView.data = dialogMsgVO;
                return;
            }
            NSocket.UpdateRoomID(0);
            if (exitS2C.userId == playerInfoProxy.userID) //自己退出
            {
                PopMsg.Instance.ShowMsg("成功退出房间");
                playerIdInfoDic = null;
                playerSitInfoDic = null;
                curInnings = 0;
                var loadInfo = new LoadSceneInfo(ESceneID.SCENE_HALL, LoadSceneType.ASYNC, LoadSceneMode.Additive);
                SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
            }
            else
            {
                var exitPlayerInfoVO = playerIdInfoDic[exitS2C.userId];
                playerIdInfoDic.Remove(exitS2C.userId);
                UpdatePlayerSitDic();
                SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATESINGLEHEAD, exitPlayerInfoVO);
            }
            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_SHOWROOMRESULT);
        }

        /// <summary>
        /// 解散房间
        /// </summary>
        /// <param name="bytes"></param>
        private void DissolutionHandler(byte[] bytes)
        {
            var disloveS2C = NetMgr.Instance.DeSerializes<DissolveRoomS2C>(bytes);
            if (disloveS2C.clientCode != ErrorCode.SUCCESS)
            {
                return;
            }
            NSocket.UpdateRoomID(0);
            if (UIManager.Instance.GetUIView(UIViewID.DISLOVE_APPLY_VIEW).isShow)
            {
                UIManager.Instance.HideUI(UIViewID.DISLOVE_APPLY_VIEW);
                UIManager.Instance.ShowUI(UIViewID.DISLOVE_STATISTICS_VIEW);
            }
            if (!UIManager.Instance.GetUIView(UIViewID.DISLOVE_STATISTICS_VIEW).isShow && roomResultS2C == null)
            {
                DialogMsgVO dialogMsgVO = new DialogMsgVO();
                dialogMsgVO.title = "解散提示";
                dialogMsgVO.dialogType = DialogType.ALERT;
                dialogMsgVO.content = "房间已解散";
                dialogMsgVO.confirmCallBack = (() =>
                {
                    playerIdInfoDic = null;
                    playerSitInfoDic = null;
                    curInnings = 0;
                    var loadInfo = new LoadSceneInfo(ESceneID.SCENE_HALL, LoadSceneType.ASYNC, LoadSceneMode.Additive);
                    SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
                });
                DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
                dialogView.data = dialogMsgVO;
            }
        }

        /// <summary>
        /// 推送玩家准备
        /// </summary>
        /// <param PlayerName="bytes"></param>
        private void PushReadyHandler(byte[] bytes)
        {
            var pushReadyS2C = NetMgr.Instance.DeSerializes<PushReadyS2C>(bytes);
            var readyPlayerInfoVO = playerIdInfoDic[pushReadyS2C.userId];
            readyPlayerInfoVO.isReady = true;
            SendNotification(NotificationConstant.MEDI_READY_COMPLETE);
            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATESINGLEHEAD, readyPlayerInfoVO);
        }

        /// <summary>
        /// 推送发牌
        /// </summary>
        /// <param PlayerName="bytes"></param>
        private void GameStartHandler(byte[] bytes)
        {
            GlobalData.ShowSendCardAnimation = true;
            forbitActions = new List<ForbitActionVO>();
            _isForbit = true;
            isStart = true;
            var playerInfoProxy =
                ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var gameStartS2C = NetMgr.Instance.DeSerializes<GameStart_S2C>(bytes);
            if (RoomInfo.Round != "一锅")
            {
                RoomInfo.Round = gameStartS2C.currentTimes.ToString()+"/8";
            }
            
            startTime = gameStartS2C.startTime;
            tingCards.Clear();
            score = gameStartS2C.score;
            #region  计算牌堆起始序号
            if (GlobalData.LoginServer != "127.0.0.1")
            {
                GlobalData.dices = gameStartS2C.dices;
            }
            else
            {
                List<int> list = new List<int>() { 1, 5 };
                GlobalData.dices = list;
            }
            List<int> playerIndex = new List<int>();
            playerIndex.Add(GlobalData.dices[1] * 2);
            playerIndex.Add(34 + (GlobalData.dices[1] * 2));
            playerIndex.Add(2 * 34 + (GlobalData.dices[1] * 2));
            playerIndex.Add(3 * 34 + (GlobalData.dices[1] * 2));

            int cardIndex = (GlobalData.dices[0] + 4) % 4;
            if (playerIdInfoDic[playerInfoProxy.userID].sit == 1)
            {
                if (cardIndex == 1)
                {
                    sendHeapStartIndex = playerIndex[0];
                }
                if (cardIndex == 2)
                {
                    sendHeapStartIndex = playerIndex[1];
                }
                if (cardIndex == 3)
                {
                    sendHeapStartIndex = playerIndex[2];
                }
                if (cardIndex == 0)
                {
                    sendHeapStartIndex = playerIndex[3];
                }
            }
            else if (playerIdInfoDic[playerInfoProxy.userID].sit == 2)
            {
                if (cardIndex == 1)
                {
                    sendHeapStartIndex = playerIndex[3];
                }
                if (cardIndex == 2)
                {
                    sendHeapStartIndex = playerIndex[0];
                }
                if (cardIndex == 3)
                {
                    sendHeapStartIndex = playerIndex[1];
                }
                if (cardIndex == 0)
                {
                    sendHeapStartIndex = playerIndex[2];
                }
            }
            else if (playerIdInfoDic[playerInfoProxy.userID].sit == 3)
            {
                if (cardIndex == 1)
                {
                    sendHeapStartIndex = playerIndex[2];
                }
                if (cardIndex == 2)
                {
                    sendHeapStartIndex = playerIndex[3];
                }
                if (cardIndex == 3)
                {
                    sendHeapStartIndex = playerIndex[0];
                }
                if (cardIndex == 0)
                {
                    sendHeapStartIndex = playerIndex[1];
                }
            }
            else
            {
                if (cardIndex == 1)
                {
                    sendHeapStartIndex = playerIndex[1];
                }
                if (cardIndex == 2)
                {
                    sendHeapStartIndex = playerIndex[2];
                }
                if (cardIndex == 3)
                {
                    sendHeapStartIndex = playerIndex[3];
                }
                if (cardIndex == 0)
                {
                    sendHeapStartIndex = playerIndex[0];
                }
            }


            #endregion

            //foreach (KeyValuePair<int, PlayerInfoVOS2C> playerInfoVos2C in playerIdInfoDic)//已经有庄家
            //{
            //    if (playerInfoVos2C.Value.isBanker)
            //    {
            //        isFirstMatch = false;
            //        break;
            //    }
            //}
            var bankerPlayerInfoVO = playerIdInfoDic[gameStartS2C.bankerUserId]; //设置庄家
            leftCard = gameStartS2C.leftCardCount;
            curInnings = gameStartS2C.currentTimes;
            if (curInnings == 1)
            {
                bankerPlayerInfoVO.isMaster = true;
            }

            foreach (KeyValuePair<int, PlayerInfoVOS2C> playerInfoVos2C in playerIdInfoDic)
            {
                playerInfoVos2C.Value.pengGangCards.Clear();
                playerInfoVos2C.Value.handCards.Clear();
                playerInfoVos2C.Value.putCards.Clear();
                playerInfoVos2C.Value.isBanker = playerInfoVos2C.Value.userId == gameStartS2C.bankerUserId;
                playerInfoVos2C.Value.score = gameStartS2C.score;
                if (playerInfoVos2C.Value.userId == playerInfoProxy.userID)
                {
                    playerInfoVos2C.Value.handCards.AddRange(gameStartS2C.handCards);
                    if (playerInfoProxy.userID == gameStartS2C.bankerUserId)//自己是庄家,给自己加一张牌
                    {
                        playerInfoVos2C.Value.getCard = gameStartS2C.touchMahjongCode;
                    }
                }
                else
                {
                    for (int i = 0; i < GlobalData.PLAYER_CARD_NUM; i++)
                    {
                        playerInfoVos2C.Value.handCards.Add(GlobalData.CardValues[0]);
                    }
                    if (playerInfoVos2C.Value.userId == gameStartS2C.bankerUserId)//庄家再发一张牌
                    {
                        playerInfoVos2C.Value.getCard = GlobalData.CardValues[0];
                    }
                }
            }
            SetPlayerActTipS2C(gameStartS2C.pushPlayerActTipS2C);
            isSelfAction = gameStartS2C.pushPlayerActTipS2C.optUserId == playerInfoProxy.userID;
            if (GlobalData.hasHeap)
            {  
                InitHeapCardIndexs(GlobalData.CardWare.Length);
            }

            GlobalData.sit = playerIdInfoDic[playerInfoProxy.userID].sit;
            GlobalData.optUserId = playerIdInfoDic[gameStartS2C.bankerUserId].sit;

            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATEALLHEAD);
            //SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATESCORE);
            SendNotification(NotificationConstant.MEDI_BATTLE_SENDCARD);
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYROTATE);
        } 
        

        /// <summary>
        /// 当前是否第一局未开启
        /// </summary>
        public bool GetIsFirstMatch()
        {
            if (isStart)
            {
                return false;
            }
            foreach (KeyValuePair<int, PlayerInfoVOS2C> playerInfoVos2C in playerIdInfoDic)
            {
                if (playerInfoVos2C.Value.score != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 推送玩家动作提示
        /// </summary>
        /// <param name="bytes">消息体</param>
        private void PushPlayerActTipHandler(byte[] bytes)
        {
            if (_isForbit)
            {
                var actionVO = new ForbitActionVO();
                actionVO.isActTip = true;
                actionVO.bytes = bytes;
                forbitActions.Add(actionVO);
                return;
            }
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var curActTips = NetMgr.Instance.DeSerializes<PushPlayerActTipS2C>(bytes);
            if (GetPlayerActTipS2C() != null && GetPlayerActTipS2C().optUserId == playerInfoProxy.userID)
            {
                for (int i = 0; i < huTypes.Count; i++)
                {
                    if (GetPlayerActTipS2C().acts.IndexOf(huTypes[i]) != -1)//自己已接到胡牌推送,忽略后续的提示
                    {
                        return;
                    }
                }
            }
            SetPlayerActTipS2C(curActTips);
            isSelfAction = GetPlayerActTipS2C().optUserId == playerInfoProxy.userID;

            GlobalData.sit = playerIdInfoDic[playerInfoProxy.userID].sit;
            GlobalData.optUserId = playerIdInfoDic[GetPlayerActTipS2C().optUserId].sit;

            if (isStart)
                SendNotification(NotificationConstant.MEDI_BATTLE_PLAYACTTIP);
            ClientAIMgr.Instance.AIPutCard();
        }

        /// <summary>
        /// 推送玩家动作
        /// </summary>
        /// <param PlayerName="bytes">消息体</param>
        private void PushPlayerActHandler(byte[] bytes)
        {
            if (_isForbit)
            {
                var actionVO = new ForbitActionVO();
                actionVO.isActTip = false;
                actionVO.bytes = bytes;
                forbitActions.Add(actionVO);
                return;
            }
            SetPlayerActTipS2C(null);
            SetPlayerActS2C(NetMgr.Instance.DeSerializes<PushPlayerActS2C>(bytes));
            switch (GetPlayerActS2C().act)
            {
                case PlayerActType.ZHI_GANG:
                    ZhiGangActHandler();
                    break;
                case PlayerActType.BACK_AN_GANG:
                    BackAnGangActHandler();
                    break;
                case PlayerActType.COMMON_AN_GANG:
                    CommonAnGangActHandler();
                    break;
                case PlayerActType.BACK_PENG_GANG:
                    BackPengGangActHandler();
                    break;
                case PlayerActType.COMMON_PENG_GANG:
                    CommonPengGangActHandler();
                    break;
                case PlayerActType.GET_CARD:
                    GetCardActHandler();
                    break;
                case PlayerActType.QIANG_AN_GANG_HU:
                    HuActHandler(false);
                    break;
                case PlayerActType.QIANG_PENG_GANG_HU:
                    HuActHandler(false);
                    break;
                case PlayerActType.QIANG_ZHI_GANG_HU:
                    HuActHandler(false);
                    break;
                case PlayerActType.SELF_HU:
                    HuActHandler(true);
                    break;
                case PlayerActType.CHI_HU:
                    HuActHandler(false);
                    break;
                case PlayerActType.PENG:
                    PengActHandler();
                    break;
                case PlayerActType.PASS:
                    PassActHandler();
                    break;
                case PlayerActType.PUT_CARD:
                    PutCardActHandler();
                    break;
                case PlayerActType.CHI:
                    ChiActHandler();
                    break;
                case PlayerActType.BAO_TING:
                    BaoTingHandler();
                    break;
                case PlayerActType.BAO_JIA:
                    BaoJiaHandler();
                    break;
                case PlayerActType.BAO_DIAO:
                    BaoDiaoHandler();
                    break;
                case PlayerActType.CHENG_DIAO:
                    ChenDiaoHandler();
                    break;
            }
        }

        /// <summary>
        /// 推送直杠牌消息处理
        /// </summary>
        private void ZhiGangActHandler()
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var targetPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().targetUserId];
            var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(targetPlayerVOS2C, PlayerCardType.PUT));
                playerCards.Add(RecordCardInfo(pengGangPlayerVOS2C, PlayerCardType.HAND, PlayerCardType.PENGGANG));
                reportActions.Add(playerCards);
            }
            var pengGangCardVOS2C = new PengGangCardVO();
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
            pengGangPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
            if (playerInfoProxy.userID == GetPlayerActS2C().userId || isReport)//是自己找到对应的牌移除
            {
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            }
            else//非自己随机找牌移除
            {
                var randomIndex = UnityEngine.Random.Range(0, pengGangPlayerVOS2C.handCards.Count - 3);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            }
            
            pengGangPlayerVOS2C.handCards.Sort();
            targetPlayerVOS2C.putCards.RemoveAt(targetPlayerVOS2C.putCards.Count - 1); //移除最后一张出牌
            
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYZHIGANG);
        }

        /// <summary>
        /// 推送回头暗杠消息处理
        /// </summary>
        private void BackAnGangActHandler()
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(pengGangPlayerVOS2C, PlayerCardType.HAND, PlayerCardType.PENGGANG));
                reportActions.Add(playerCards);
            }
            var pengGangCardVOS2C = new PengGangCardVO();
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
            pengGangPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
            if (playerInfoProxy.userID == GetPlayerActS2C().userId || isReport)//是自己找到对应的牌移除
            {
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            }
            else//非自己随机找牌移除
            {
                var randomIndex = UnityEngine.Random.Range(0, pengGangPlayerVOS2C.handCards.Count - 4);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            }
            
            pengGangPlayerVOS2C.handCards.Sort();
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAY_BACKANGANG);
        }

        /// <summary>
        /// 推送暗杠消息处理
        /// </summary>
        private void CommonAnGangActHandler()
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(pengGangPlayerVOS2C, PlayerCardType.HAND, PlayerCardType.PENGGANG, PlayerCardType.GET));
                reportActions.Add(playerCards);
            }
            var pengGangCardVOS2C = new PengGangCardVO();
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
            pengGangPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
            pengGangPlayerVOS2C.getCard = 0;
            if (playerInfoProxy.userID == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
            {
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
               
            }
            else//非自己随机找牌移除
            {
                var randomIndex = UnityEngine.Random.Range(0, pengGangPlayerVOS2C.handCards.Count - 3);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            }
            
            pengGangPlayerVOS2C.handCards.Sort();
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAY_COMMONANGANG);
        }

        /// <summary>
        /// 推送普通碰杠消息处理
        /// </summary>
        private void CommonPengGangActHandler()
        {
            var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(pengGangPlayerVOS2C, PlayerCardType.GET, PlayerCardType.PENGGANG));
                reportActions.Add(playerCards);
            }
            foreach (var pengGangCardVos2C in pengGangPlayerVOS2C.pengGangCards)
                if (pengGangCardVos2C.pengGangCards[0] == GetPlayerActS2C().actCard)
                    pengGangCardVos2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangPlayerVOS2C.getCard = 0;
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAY_COMMONPENGGANG);
        }

        /// <summary>
        /// 推送回头碰杠消息处理
        /// </summary>
        private void BackPengGangActHandler()
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(pengGangPlayerVOS2C, PlayerCardType.HAND, PlayerCardType.PENGGANG));
                reportActions.Add(playerCards);
            }
            foreach (var pengGangCardVos2C in pengGangPlayerVOS2C.pengGangCards)
                if (pengGangCardVos2C.pengGangCards[0] == GetPlayerActS2C().actCard)
                    pengGangCardVos2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            if (playerInfoProxy.userID == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
            {
                pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            }
            else
            {
                var randomIndex = UnityEngine.Random.Range(0, pengGangPlayerVOS2C.handCards.Count - 1);
                pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            }
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAY_BACKPENGGANG);
        }


        /// <summary>
        /// 推送摸牌消息处理
        /// </summary>
        private void GetCardActHandler()
        {
            leftCard -= 1;
            var getCardPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            if (getCardPlayerVOS2C.getCard != 0)
            {
                getCardPlayerVOS2C.handCards.Add(getCardPlayerVOS2C.getCard);
            }
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(getCardPlayerVOS2C, PlayerCardType.GET));
                reportActions.Add(playerCards);
            }
            getCardPlayerVOS2C.getCard = GetPlayerActS2C().actCard;
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYGETCARD);
        }

        /// <summary>
        /// 推送胡牌消息处理
        /// </summary>
        private void HuActHandler(bool isSelf)
        {
            var huCardPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            if (isSelf) //自摸
            {
                if (isReport)
                {
                    var playerCards = new List<PlayerCardVO>();
                    playerCards.Add(RecordCardInfo(huCardPlayerVOS2C, PlayerCardType.GET, PlayerCardType.HAND));
                    reportActions.Add(playerCards);
                }
                if (huCardPlayerVOS2C.getCard > 0)
                {
                    huCardPlayerVOS2C.handCards.Add(huCardPlayerVOS2C.getCard);
                    huCardPlayerVOS2C.getCard = 0;
                }
            }
            else
            {
                var targetPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().targetUserId];
                if (isReport)
                {
                    var playerCards = new List<PlayerCardVO>();
                    playerCards.Add(RecordCardInfo(targetPlayerVOS2C, PlayerCardType.PUT));
                    playerCards.Add(RecordCardInfo(huCardPlayerVOS2C, PlayerCardType.HAND));
                    reportActions.Add(playerCards);
                }
                huCardPlayerVOS2C.handCards.Add(GetPlayerActS2C().actCard);
                if (targetPlayerVOS2C.putCards.Count > 0 && targetPlayerVOS2C.putCards[targetPlayerVOS2C.putCards.Count - 1] == GetPlayerActS2C().actCard)
                {
                    targetPlayerVOS2C.putCards.RemoveAt(targetPlayerVOS2C.putCards.Count - 1); //移除最后一张出牌
                }
            }
            SetPlayerActTipS2C(null);
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYHU, isSelf);
        }

        /// <summary>
        /// 推送碰牌消息处理
        /// </summary>
        private void PengActHandler()
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var pengPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            var targetPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().targetUserId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(targetPlayerVOS2C, PlayerCardType.PUT));
                playerCards.Add(RecordCardInfo(pengPlayerVOS2C, PlayerCardType.HAND, PlayerCardType.PENGGANG));
                reportActions.Add(playerCards);
            }
            var pengGangCardVOS2C = new PengGangCardVO();
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
            pengPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
            if (playerInfoProxy.userID == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
            {
                pengPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
                pengPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            }
            else
            {
                var randomIndex = UnityEngine.Random.Range(0, pengPlayerVOS2C.handCards.Count - 2);
                pengPlayerVOS2C.handCards.RemoveAt(randomIndex);
                pengPlayerVOS2C.handCards.RemoveAt(randomIndex);
            }
            
            pengPlayerVOS2C.handCards.Sort();

            targetPlayerVOS2C.putCards.RemoveAt(targetPlayerVOS2C.putCards.Count - 1); //移除最后一张出牌
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYPENG);
        }

        /// <summary>
        /// 推送过消息处理
        /// </summary>
        private void PassActHandler()
        {
            SetPlayerActTipS2C(null);
            var passPlayerVO = playerIdInfoDic[GetPlayerActS2C().userId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                reportActions.Add(playerCards);
            }
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYPASS);
        }

        /// <summary>
        /// 推送出牌消息处理
        /// </summary>
        private void PutCardActHandler()
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var putCardPlayerVO = playerIdInfoDic[GetPlayerActS2C().userId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(putCardPlayerVO, PlayerCardType.GET, PlayerCardType.HAND, PlayerCardType.PUT));
                reportActions.Add(playerCards);
            }
            putCardPlayerVO.putCards.Add(GetPlayerActS2C().actCard);
            if (putCardPlayerVO.getCard != 0)
            {
                putCardPlayerVO.handCards.Add(putCardPlayerVO.getCard);
            }
            if (playerInfoProxy.userID == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
            {
                putCardPlayerVO.handCards.Remove(GetPlayerActS2C().actCard);
            }
            else
            {
                var randomIndex = UnityEngine.Random.Range(0, putCardPlayerVO.handCards.Count - 1);
                putCardPlayerVO.handCards.RemoveAt(randomIndex);
            }
            
            putCardPlayerVO.handCards.Sort();
            putCardPlayerVO.getCard = 0;
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYPUTCARD);
        }

        /// <summary>
        /// 推送吃牌消息处理
        /// </summary>
        private void ChiActHandler()
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var chiPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
            var targetPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().targetUserId];
            if (isReport)
            {
                var playerCards = new List<PlayerCardVO>();
                playerCards.Add(RecordCardInfo(targetPlayerVOS2C, PlayerCardType.PUT));
                playerCards.Add(RecordCardInfo(chiPlayerVOS2C, PlayerCardType.HAND, PlayerCardType.PENGGANG));
                reportActions.Add(playerCards);
            }
            var pengGangCardVOS2C = new PengGangCardVO();
            pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
            pengGangCardVOS2C.pengGangCards.AddRange(GetPlayerActS2C().chiCards);
            pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
            chiPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
            if (playerInfoProxy.userID == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
            {
                chiPlayerVOS2C.handCards.Remove(GetPlayerActS2C().chiCards[0]);
                chiPlayerVOS2C.handCards.Remove(GetPlayerActS2C().chiCards[1]);
            }
            else
            {
                var randomIndex = UnityEngine.Random.Range(0, chiPlayerVOS2C.handCards.Count - 2);
                chiPlayerVOS2C.handCards.RemoveAt(randomIndex);
                chiPlayerVOS2C.handCards.RemoveAt(randomIndex);
            }

            chiPlayerVOS2C.handCards.Sort();

            targetPlayerVOS2C.putCards.RemoveAt(targetPlayerVOS2C.putCards.Count - 1); //移除最后一张出牌
            SendNotification(NotificationConstant.MEDI_BATTLE_PLAYCHI);
        }

        /// <summary>
        /// 重置牌面响应
        /// </summary>
        /// <param name="bytes"></param>
        private void SetCardHandler(byte[] bytes)
        {
            var setCardS2C = NetMgr.Instance.DeSerializes<SetCardS2C>(bytes);
            foreach (PlayerCardSet cardSet in setCardS2C.cardSets)
            {
                cardSet.handCards.Sort();
                if (playerSitInfoDic == null || playerSitInfoDic[cardSet.sit] == null)
                {
                    continue;
                }
                playerSitInfoDic[cardSet.sit].handCards.Clear();
                playerSitInfoDic[cardSet.sit].handCards.AddRange(cardSet.handCards);
                playerSitInfoDic[cardSet.sit].getCard = cardSet.getCard;
            }
            SendNotification(NotificationConstant.MEDI_BATTLE_RESET);
        }

        /// <summary>
        /// 可设置的牌池
        /// </summary>
        public List<int> cardValuePool;
        /// <summary>
        /// 获取所有玩家的牌面
        /// </summary>
        private void GetAllCardHandler(byte[] bytes)
        {
            var getAllCardS2C = NetMgr.Instance.DeSerializes<GetAllCardS2C>(bytes);
            cardValuePool = getAllCardS2C.remainder;
            foreach (PlayerCardSet cardSet in getAllCardS2C.cardSets)
            {
                if (playerSitInfoDic == null || playerSitInfoDic[cardSet.sit] == null)
                {
                    continue;
                }
                cardSet.handCards.Sort();
                playerSitInfoDic[cardSet.sit].handCards.Clear();
                playerSitInfoDic[cardSet.sit].handCards.AddRange(cardSet.handCards);
                playerSitInfoDic[cardSet.sit].getCard = cardSet.getCard;
            }
            SendNotification(NotificationConstant.MEDI_BATTLE_RESET);
            SendNotification(NotificationConstant.MEDI_INIT_CARDPOOL);
        }

        /// <summary>
        /// 出牌返回
        /// </summary>
        /// <param name="bytes"></param>
        private void PlayAmahjongHandler(byte[] bytes)
        {
            var playAmahjongS2C = NetMgr.Instance.DeSerializes<PlayAMahjongS2C>(bytes);
            tingCards = playAmahjongS2C.tingCards;
        }

        /// <summary>
        /// 推送本局结束
        /// </summary>
        /// <param PlayerName="bytes"></param>
        private void PushMatchEndHandler(byte[] bytes)
        {
            isStart = false;
            matchResultS2C = NetMgr.Instance.DeSerializes<PushMatchResultS2C>(bytes);
            perBankerId = GetBankerPlayerInfoVOS2C().userId;
            foreach (PlayerMatchResultVOS2C playerMatchResultVO in matchResultS2C.resultInfos)
            {
                playerMatchResultVO.handCards.Sort();
                playerIdInfoDic[playerMatchResultVO.userId].score += playerMatchResultVO.addScore;
                playerIdInfoDic[playerMatchResultVO.userId].isReady = false;
            }
            tingCards.Clear();
            SendNotification(NotificationConstant.TING_UPDATE);
            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATEALLHEAD);
            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_SHOWMATCHRESULT);
        }


        /// <summary>
        /// 推送房间结束
        /// </summary>
        /// <param PlayerName="bytes"></param>
        private void PushRoomEndHandler(byte[] bytes)
        {
            NSocket.UpdateRoomID(0);
            roomResultS2C = NetMgr.Instance.DeSerializes<PushRoomResultS2C>(bytes);
            if (matchResultS2C == null)
            {
                if (UIManager.Instance.GetUIView(UIViewID.DISLOVE_APPLY_VIEW).isShow)
                {
                    UIManager.Instance.HideUI(UIViewID.DISLOVE_APPLY_VIEW);
                    UIManager.Instance.ShowUI(UIViewID.DISLOVE_STATISTICS_VIEW);
                    Timer.Instance.AddTimer(0, 1, 2, DelayShowRoomResult);
                }
                else
                {
                    if (UIManager.Instance.GetUIView(UIViewID.DISLOVE_STATISTICS_VIEW).isShow)
                    {
                        Timer.Instance.AddTimer(0, 1, 2, DelayShowRoomResult);
                    }
                    else
                    {
                        DelayShowRoomResult();
                    }
                }               
            }
            
        }

        /// <summary>
        /// 延迟显示房间结算
        /// </summary>
        private void DelayShowRoomResult()
        {
            UIManager.Instance.ShowUI(UIViewID.ROOM_RESULT_VIEW);
            UIManager.Instance.HideUI(UIViewID.DISLOVE_STATISTICS_VIEW);
        }

        /// <summary>
        /// 刷新座位字典
        /// </summary>
        private void UpdatePlayerSitDic()
        {
            playerSitInfoDic = new Dictionary<int, PlayerInfoVOS2C>();
            foreach (var playerInfoVO in playerIdInfoDic)
                playerSitInfoDic.Add(playerInfoVO.Value.sit, playerInfoVO.Value);
        }

        /// <summary>
        /// 增加局数
        /// </summary>
        public void AddInnings()
        {
            Resources.UnloadUnusedAssets();
            forbitActions.Clear();
            matchResultS2C = null;
            roomResultS2C = null;
            SetPlayerActTipS2C(null);
            SetPlayerActS2C(null);
            _isForbit = false;
            isSelfAction = false;
            isStart = false;
            curGuide = GuideType.NULL;
            tingCards.Clear();
            SendNotification(NotificationConstant.TING_UPDATE);
        }

        /// <summary>
        /// 推送语音消息处理
        /// </summary>
        public void PushVoiceHandler(byte[] bytes)
        {
            var pushVoiceS2C = NetMgr.Instance.DeSerializes<PushVoiceS2C>(bytes);
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            if (pushVoiceS2C.senderUserId == playerInfoProxy.userID)
            {
                return;
            }
            RecorderSystem.GetAudioPacket(pushVoiceS2C.senderUserId, pushVoiceS2C.flag, pushVoiceS2C.content);
        }

        /// <summary>
        /// 推送聊天消息处理
        /// </summary>
        /// <param name="bytes"></param>
        private void PushChatHandler(byte[] bytes)
        {
            var pushChatS2C = NetMgr.Instance.DeSerializes<PushSendChatS2C>(bytes);
            if (pushChatS2C.content.Contains(GlobalData.FACE_PREFIX))
            {
                SendNotification(NotificationConstant.MEDI_BATTLEVIEW_SHOWFACE, pushChatS2C); 
            }
            else
            {
                SendNotification(NotificationConstant.MEDI_BATTLEVIEW_SHOWCHAT, pushChatS2C);
            }
        }

        /// <summary>
        /// 获取战报内容
        /// </summary>
        /// <param name="bytes"></param>
        private void PlayVideo(byte[] bytes)
        {
            var gameMgrProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
            var playVideoS2C = NetMgr.Instance.DeSerializes<PlayVideoS2C>(bytes);
            report = PlayReportS2C.Paser(playVideoS2C.report);
            reportActions = new List<List<PlayerCardVO>>();
            var addTime = gameMgrProxy.systemTime - report.startTime;
            foreach (ActionVO action in report.actions)//将时间置为当前服务器时间
            {
                action.actionTime += addTime;
                if (action.isActionTip)
                {
                    action.actTip.tipRemainUT += addTime;
                }
            }
            reportLocalTime = Time.time;
            gameMgrProxy.ReviseScaleSystemTime();
            isReport = true;
            NetMgr.Instance.OnClientReceiveBuff(MsgNoS2C.S2C_ROOM_JOIN_ROOM.GetHashCode(), 0, report.joinInfo);
            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_SHOW_REPORTVIEW);
        }

        /// <summary>
        /// 获取玩家信息返回
        /// </summary>
        /// <param name="bytes"></param>
        private void GetUserInfoHandler(byte[] bytes)
        {
            var getPlayerInfoS2C = NetMgr.Instance.DeSerializes<GetUserInfoByIdS2C>(bytes);
            var playerInfoView = UIManager.Instance.ShowUI(UIViewID.PLATER_INFO_VIEW) as PlayerInfoView;
            playerInfoView.data = getPlayerInfoS2C;
        }

        /// <summary>
        /// 获取申请解散消息处理
        /// </summary>
        /// <param name="bytes"></param>
        private void DisloveApplyHandler(byte[] bytes)
        {
            var disloveApplyS2C = NetMgr.Instance.DeSerializes<ApplyDissolveRoomS2C>(bytes);
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var gameMgrProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
            if (disloveApplyS2C.userId == playerInfoProxy.userID)//自己的申请忽略
            {
                UIManager.Instance.ShowUI(UIViewID.DISLOVE_STATISTICS_VIEW);
                return;
            }
            disloveApplyUserId = disloveApplyS2C.userId;
            agreeIds.Add(disloveApplyS2C.userId);
            hasDisloveApply = true;
            disloveRemainTime = GlobalData.DISLOVE_APPLY_TIMEOUT;
            disloveRemainUT = gameMgrProxy.systemTime;
            
            UIManager.Instance.ShowUI(UIViewID.DISLOVE_APPLY_VIEW);
        }

        /// <summary>
        /// 同意解散
        /// </summary>
        /// <param name="bytes"></param>
        private void DisloveConfirmHandler(byte[] bytes)
        {
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var disloveConfirmS2C = NetMgr.Instance.DeSerializes<DissloveRoomConfirmS2C>(bytes);
            agreeIds.Add(disloveConfirmS2C.userId);
            SendNotification(NotificationConstant.UPDATE_DISLOVE_STATISTICS);
            if (disloveConfirmS2C.userId == playerInfoProxy.userID)
            {
                UIManager.Instance.ShowUI(UIViewID.DISLOVE_STATISTICS_VIEW);
            }
        }

        /// <summary>
        /// 拒绝解散
        /// </summary>
        /// <param name="bytes"></param>
        private void DisloveCancelHandler(byte[] bytes)
        {
            var disloveCancelS2C = NetMgr.Instance.DeSerializes<CancelDissolveRoomS2C>(bytes);
            SendNotification(NotificationConstant.UPDATE_DISLOVE_STATISTICS);
            UIManager.Instance.HideUI(UIViewID.DISLOVE_STATISTICS_VIEW);
            UIManager.Instance.HideUI(UIViewID.DISLOVE_APPLY_VIEW);
            DialogMsgVO dialogMsgVO = new DialogMsgVO();
            dialogMsgVO.title = "解散提示";
            dialogMsgVO.dialogType = DialogType.ALERT;
            dialogMsgVO.content = string.Format("{0}拒绝解散房间",playerIdInfoDic[disloveCancelS2C.userId].name);
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogMsgVO;
            hasDisloveApply = false;
            agreeIds.Clear();
            refuseIds.Clear();
        }


        /// <summary>
        /// 出牌出错返回
        /// </summary>
        /// <param name="bytes"></param>
        private void ActErrorHandler(byte[] bytes)
        {
            var errorS2C = NetMgr.Instance.DeSerializes<ActErrorS2C>(bytes);
            if (errorS2C.errorCode == ErrorCode.FORBIDDEN_CARD)
            {
                PopMsg.Instance.ShowMsg("当前牌不允许出");
            }
            isSelfAction = true;
        }

        /// <summary>
        /// 在线状态设置
        /// </summary>
        /// <param name="bytes"></param>
        private void OnlineSettingHandler(byte[] bytes)
        {
            var onlineSettingS2C = NetMgr.Instance.DeSerializes<PushOnlineSettingS2C>(bytes);
            playerIdInfoDic[onlineSettingS2C.userId].isOnline = onlineSettingS2C.isOnline;
            SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATEONLINE, onlineSettingS2C.userId);
        }

        /// <summary>
        /// 记录玩家牌面数据
        /// </summary>
        /// <param name="playerInfoVO"></param>
        /// <param name="recordCardType"></param>
        private PlayerCardVO RecordCardInfo(PlayerInfoVOS2C playerInfoVO,params PlayerCardType[] recordCardType)
        {
            var result = new PlayerCardVO();
            result.userId = playerInfoVO.userId;
            result.recordCardType = recordCardType;
            foreach (PlayerCardType cardType in recordCardType)
            {
                if (cardType == PlayerCardType.GET)
                {
                    result.getCard = playerInfoVO.getCard;
                }
                else if (cardType == PlayerCardType.HAND)
                {
                    result.handCards.AddRange(playerInfoVO.handCards);
                }
                else if (cardType == PlayerCardType.PUT)
                {
                    result.putCards.AddRange(playerInfoVO.putCards);
                }
                if (cardType == PlayerCardType.PENGGANG)
                {
                    for (int i = 0; i < playerInfoVO.pengGangCards.Count; i++)
                    {
                        var pengGangVO = new PengGangCardVO();
                        pengGangVO.pengGangCards.AddRange(playerInfoVO.pengGangCards[i].pengGangCards);
                        pengGangVO.targetUserId = playerInfoVO.pengGangCards[i].targetUserId;
                        result.pengGangCards.Add(pengGangVO);
                    }
                }
            }
            return result;
        }
        
        /// <summary>
        /// 重置单个玩家的牌面数据
        /// </summary>
        public void BackSinglePlayerCard()
        {
            List<PlayerCardVO> playersCardVO = reportActions[reportActions.Count - 1];
            reportActions.RemoveAt(reportActions.Count - 1);
            foreach (PlayerCardVO playerCardVO in playersCardVO)
            {
                var backPlayerVO = playerIdInfoDic[playerCardVO.userId];
                if (Array.IndexOf(playerCardVO.recordCardType,PlayerCardType.GET) != -1)
                {
                    backPlayerVO.getCard = playerCardVO.getCard;
                }
                if (Array.IndexOf(playerCardVO.recordCardType, PlayerCardType.HAND) != -1)
                {
                    backPlayerVO.handCards.Clear();
                    backPlayerVO.handCards.AddRange(playerCardVO.handCards);
                }
                if (Array.IndexOf(playerCardVO.recordCardType, PlayerCardType.PUT) != -1)
                {
                    backPlayerVO.putCards.Clear();
                    backPlayerVO.putCards.AddRange(playerCardVO.putCards);
                }
                if (Array.IndexOf(playerCardVO.recordCardType, PlayerCardType.PENGGANG) != -1)
                {
                    backPlayerVO.pengGangCards.Clear();
                    backPlayerVO.pengGangCards.AddRange(playerCardVO.pengGangCards);
                }
            }
        }

        /// <summary>
        /// 清理数据
        /// </summary>
        public void Clear()
        {
            curInnings = 1;
            isStart = false;
            _isForbit = false;
            creatorId = 0;
            matchResultS2C = null;
            roomResultS2C = null;
            SetPlayerActS2C(null);
            SetPlayerActTipS2C(null);
            playerIdInfoDic = null;
            playerSitInfoDic = null;
            isSelfAction = false;
            roomResultS2C = null;
            leftCard = GlobalData.CardWare.Length; ;
            speekPacket = new Queue<AudioPacket>();
            report = null;
            reportLocalTime = 0;
            isReport = false;
            perSendChatTime = 0;
            isPlayHu = false;
            if (reportActions != null)
            {
                reportActions.Clear();
            }
        }
    }

    /// <summary>
    /// 动作指向类型
    /// </summary>
    public enum GuideType
    {
        ACT,
        ACT_TIP,
        NULL
    }
}