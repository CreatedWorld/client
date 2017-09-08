using LZR.Data.NetWork.Client;
using Platform.Global;
using Platform.Model;
using Platform.Model.Battle;
using Platform.Net;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Utils;
/// <summary>
/// 压力测试单个客户端
/// </summary>
public class PressDebugClient
{
    /// <summary>
    /// 网络模块
    /// </summary>
    public Dictionary<SocketType, NSocket> ConnentionDic = new Dictionary<SocketType, NSocket>();
    /// <summary>
    /// 客户端序号
    /// </summary>
    public int clientIndex;
    /// <summary>
    /// 房间号
    /// </summary>
    public string roomCode;
    /// <summary>
    /// 玩家id
    /// </summary>
    public int userId;
    /// <summary>
    /// 玩家mac
    /// </summary>
    public string mac;
    /// <summary>
    /// 性别
    /// </summary>
    public int sex = 1;
    /// <summary>
    /// 胡牌类型
    /// </summary>
    public List<PlayerActType> huTypes = new List<PlayerActType>();
    /// <summary>
    /// 随机
    /// </summary>
    private System.Random random;

    /// <summary>
    /// 创建客户端
    /// </summary>
    /// <param name="clientIndex"></param>
    public PressDebugClient(int clientIndex)
    {
        this.clientIndex = clientIndex;
        random = new System.Random();
        huTypes.Add(PlayerActType.CHI_HU);
        huTypes.Add(PlayerActType.QIANG_AN_GANG_HU);
        huTypes.Add(PlayerActType.QIANG_PENG_GANG_HU);
        huTypes.Add(PlayerActType.QIANG_ZHI_GANG_HU);
        huTypes.Add(PlayerActType.SELF_HU);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_LOGIN, LoginResponse);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_Hall_CREATE_ROOM, CreateRoomResponse);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_ROOM_JOIN_ROOM_BROADCAST, PushJoinHandler);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_ENTER_ROOM, CreateRoomHandler);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_Hall_JOIN_IN_ROOM, JoinRoomResponse);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_ROOM_JOIN_ROOM, JoinInRoomHandler);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_ROOM_GAME_START_BROADCAST, GameStartHandler);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_ROOM_PLAYER_ACT_TIP_BROADCAST, PushPlayerActTipHandler);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_ROOM_PLAYER_ACT_BROADCAST, PushPlayerActHandler);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_ROOM_SINGLE_SCORE_BROADCAST, PushMatchEndHandler);
        GameMgr.Instance.AddMsgWithIndex(MsgNoS2C.S2C_ROOM_TOTAL_SCORE_BROADCAST, PushRoomEndHandler);
        var timerId = Timer.Instance.AddTimer(30, 0, 0, SendHallBeat);
        Timer.Instance.SetFixTimer(timerId);
        timerId = Timer.Instance.AddTimer(20, 0, 0, SendBattleBeat);
        Timer.Instance.SetFixTimer(timerId);
        NSocket.UpdateUserID(0,clientIndex);
        NSocket.UpdateRoomID(0,clientIndex);
        LoginView loginView = UIManager.Instance.GetUIView(UIViewID.LOGIN_VIEW) as LoginView;
        CreateConnect(SocketType.LOGIN, loginView.serverTxt.text, int.Parse(loginView.portTxt.text), LoginConnectHandler);
    }

    /// <summary>
    /// 发送心跳包
    /// </summary>
    void SendHallBeat()
    {
        if (GlobalData.LoginServer == "127.0.0.1")
        {
            return;
        }
        if (ConnentionDic.ContainsKey(SocketType.HALL))
        {
            SendBuff(SocketType.HALL, MsgNoC2S.C2S_HALL_BATTLEBEAT.GetHashCode(), 0, new HallBeatC2S());
        }
    }

    /// <summary>
    /// 发送心跳包
    /// </summary>
    void SendBattleBeat()
    {
        if (GlobalData.LoginServer == "127.0.0.1")
        {
            return;
        }
        if (ConnentionDic.ContainsKey(SocketType.BATTLE))
        {
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_BATTLEBEAT.GetHashCode(), 0, new BattleBeatC2S());
        }
    }
    /// <summary>
    /// 连接完成
    /// </summary>
    private void LoginConnectHandler()
    {
        LoginC2S package = new LoginC2S();
        mac = GetRandAccount();
        package.mac = mac;
        package.name = mac;
        package.imageUrl = GlobalData.ImageUrl;
        package.sex = sex;
        package.psw = GlobalData.UserPwd;
        SendBuff<LoginC2S>(SocketType.LOGIN, MsgNoC2S.C2S_LOGIN.GetHashCode(), 0, package);
    }

    string GetRandAccount()
    {
        long curtv = TimeHandle.Instance.GetTimestamp();
        int ranval = clientIndex;
        string randacc = String.Format("{0:X}", curtv) + String.Format("{0:X}", ranval);
        return randacc;
    }

    /// <summary>
    /// 发送数据包
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="socketType">socket类型</param>
    /// <param name="channel">通信协议[PID]</param>
    /// <param name="type">内容的类型,0-Protobuf1,Json</param>
    /// <param name="tbuff">传递的内容</param>
    /// <param name="offCheckTimeOut">是否忽略超时</param>
    public void SendBuff<T>(SocketType socketType, int channel, int type, T tbuff, bool offCheckTimeOut = false)
    {
        ConnentionDic[socketType].SendBuff(channel, type, tbuff, offCheckTimeOut,clientIndex);
        ClientAIMgr.Instance.ShowSendMsgLog((MsgNoC2S)channel, tbuff,clientIndex);
    }

    /// <summary>
    /// 构建连接
    /// </summary>
    public NSocket CreateConnect(SocketType socketType, string server, int port, Action connectSucess)
    {
        var connention = new NSocket(new IPEndPoint(IPAddress.Parse(server), port), clientIndex);
        connention.MaxReConnectNums = 0;
        connention.OnConnectFailed = OnConnectFailed;
        connention.OnConnectSuccessful = delegate {
            Timer.Instance.AddTimer(0, 1, 0, () => { connectSucess(); });
            ConnentionDic.Add(socketType, connention);
        };
        connention.OnDisconnect = delegate { Timer.Instance.AddTimer(0, 1, 0, () => { OnDisconnect(connention); }); };
        connention.OnReceiveBuff = OnReceiveBuff;
        connention.OnRquestTimeOut = OnSendBuffTimeOut;
        
        connention.StartTcpConnection();
        return connention;
    }

    /// <summary>
    /// 当接收到新数据
    /// </summary>
    /// <param name="channel">协议ID</param>
    /// <param name="type">数据类型</param>
    /// <param name="buff">数据内容</param>
    /// <param name="clientIndex">客户端序号</param>
    void OnReceiveBuff(int channel, int type, byte[] buff, int clientIndex)
    {
        ReciveMsgVO msgVo = new ReciveMsgVO();
        msgVo.channel = channel;
        msgVo.type = type;
        msgVo.bytes = buff;
        msgVo.clientIndex = clientIndex;
        //Debug.Log(string.Format("消息解包 {0} 客户端 {1}", channel, clientIndex));
        GameMgr.Instance.ReciveMsgPool.Enqueue(msgVo);
    }

    /// <summary>
    /// 当连接失败
    /// 即重连后依然连接不上
    /// </summary>
    void OnConnectFailed()
    {
        //连接服务器失败了
    }

    /// <summary>
    /// 当连接断开
    /// </summary>
    void OnDisconnect(NSocket socket)
    {
        DialogMsgVO dialogMsgVO = new DialogMsgVO();
        dialogMsgVO.title = "连接提示";
        dialogMsgVO.content = "您已断开连接是否重新连接";
        dialogMsgVO.dialogType = DialogType.CONFIRM;
        DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
        dialogView.data = dialogMsgVO;
    }

    /// <summary>
    /// 当发送数据超时未回应
    /// </summary>
    public void OnSendBuffTimeOut(int channel)
    {
        GlobalData.logs.Add(new LogVO
        {
            message = string.Format("协议ID:{0} 客户端：{1}，超时未回应",channel,clientIndex),
            stackTrace = "",
            type = LogType.Log,
        });
        if (GlobalData.logs.Count > GlobalData.maxLogs)
        {
            GlobalData.logs.RemoveAt(0);
        }
        Timer.Instance.AddTimer(0, 1, 0, () => { OnRquestTimeOut(); });
    }

    /// <summary>
    /// 连接超时
    /// </summary>
    /// <param name="socket"></param>
    void OnRquestTimeOut()
    {
        DialogMsgVO dialogMsgVO = new DialogMsgVO();
        dialogMsgVO.title = "超时提示";
        dialogMsgVO.content = "消息超时是否重新连接";
        dialogMsgVO.dialogType = DialogType.CONFIRM;
        DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
        dialogView.data = dialogMsgVO;
    }

    /// <summary>
    /// 停止连接
    /// </summary>
    /// <param name="socketType"></param>
    public void StopTcpConnection(SocketType socketType)
    {
        if (!ConnentionDic.ContainsKey(socketType))
        {
            return;
        }
        var connention = ConnentionDic[socketType];
        connention.StopTcpConnection();
        ConnentionDic.Remove(socketType);
    }

    /// <summary>
    /// 断开所有服务器连接
    /// </summary>
    public void StopAllTcpConnection()
    {
        foreach (KeyValuePair<SocketType, NSocket> pair in ConnentionDic)
        {
            pair.Value.StopTcpConnection();
        }
        ConnentionDic.Clear();
    }

    /// <summary>
    /// 登录返回
    /// </summary>
    /// <param name="bytes"></param>
    private void LoginResponse(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
        LoginS2C package = NetMgr.Instance.DeSerializes<LoginS2C>(bytes);
        int loginStatus = package.status;
        if (loginStatus == 1)
        {
            Debug.Log("验证成功");
            userId = package.userId;
            string hallServerIP = package.serverIp;
            int hallServerPort = package.port;
            ((GameMgrProxy)ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY)).systemTime = package.time;
            NSocket.UpdateUserID(userId, this.clientIndex);
            StopTcpConnection(SocketType.LOGIN);
            CreateConnect(SocketType.HALL, hallServerIP, hallServerPort, HallConnectHandler);
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "登录失败";
            dialogVO.content = "登录失败,请重新连接";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }

    /// <summary>
    /// 大厅连接成功回调
    /// </summary>
    private void HallConnectHandler()
    {
        if (clientIndex % GlobalData.SIT_NUM == 0)
        {
            CheckCreateRoomC2S package = new CheckCreateRoomC2S();
            package.roomRounds = GameMode.SIXTEEN_ROUND.GetHashCode();
            package.roomRule = (package.roomRounds == (int)GameMode.EIGHT_ROUND) ? 1 : 2;//(int)this.hallProxy.HallInfo.GameRule;
            package.playType.Add(GameRule.WORD.GetHashCode());
            SendBuff<CheckCreateRoomC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_CREATE_ROOM.GetHashCode(), 0, package);
        }
    }

    /// <summary>
    /// 创建房间响应
    /// </summary>
    /// <param name="bytes"></param>
    private void CreateRoomResponse(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
        CheckCreateRoomS2C package = NetMgr.Instance.DeSerializes<CheckCreateRoomS2C>(bytes);
        if (package.clientCode == (int)ErrorCode.SUCCESS)
        {
            roomCode = package.roomCode;
            NSocket.UpdateRoomID(package.roomCode == "" ? 0 : int.Parse(package.roomCode), clientIndex);
            CreateConnect(SocketType.BATTLE, package.roomServerIp, package.roomServerPort, BattleConnectHandler);
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "创建房间失败";
            dialogVO.content = "创建房间失败,请检查网络连接";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }

    /// <summary>
    /// 战斗服务器连接回调
    /// </summary>
    private void BattleConnectHandler()
    {
        SendBuff<EnterRoomServerC2S>(SocketType.BATTLE, MsgNoC2S.C2S_ENTER_ROOM.GetHashCode(), 0, new EnterRoomServerC2S());
    }

    /// <summary>
    /// 当前是第几局
    /// </summary>
    public int curInnings = 1;
    /// <summary>
    /// 本局是否已开始
    /// </summary>
    public bool isStart;
    /// <summary>
    /// 房间房主id
    /// </summary>
    public int creatorId;
    /// <summary>
    /// 是否轮到自己出手
    /// </summary>
    public bool isSelfAction;
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
    /// 剩余牌数
    /// </summary>
    public int leftCard;
    /// <summary>
    /// 当前是否播放战报
    /// </summary>
    public bool isReport = false;
    /// <summary>
    /// 是否禁止操作正在发牌
    /// </summary>
    private bool _isForbit = false;
    /// <summary>
    /// 玩家信息字典{LocalId:玩家信息VO}
    /// </summary>
    public Dictionary<int, PlayerInfoVOS2C> playerIdInfoDic;

    /// <summary>
    /// 玩家信息字典{Sit:玩家信息VO}
    /// </summary>
    public Dictionary<int, PlayerInfoVOS2C> playerSitInfoDic;
    /// <summary>
    /// 听的牌数组
    /// </summary>
    public List<int> tingCards = new List<int>();
    /// <summary>
    /// 当前指向
    /// </summary>
    public GuideType curGuide = GuideType.NULL;
    /// <summary>
    /// 当前播放的玩家动作
    /// </summary>
    private PushPlayerActS2C _playerActS2C;

    /// <summary>
    /// 推送的玩家操作提示
    /// </summary>
    private PushPlayerActTipS2C _playerActTipS2C;
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
    /// 禁止期间未播放的动作队列
    /// </summary>
    private List<ForbitActionVO> forbitActions;
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
    /// 刚刚收到的解散申请
    /// </summary>
    public int disloveApplyUserId;
    /// <summary>
    /// 创建房间返回
    /// </summary>
    /// <param PlayerName="bytes"></param>
    private void CreateRoomHandler(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }

        SetPlayerActTipS2C(null);
        playerIdInfoDic = new Dictionary<int, PlayerInfoVOS2C>();
        var selfInfoVO = new PlayerInfoVOS2C();
        selfInfoVO.headIcon = GlobalData.ImageUrl;
        selfInfoVO.isBanker = false;
        selfInfoVO.isMaster = false;
        selfInfoVO.isReady = false;
        selfInfoVO.userId = userId;
        selfInfoVO.name = mac;
        selfInfoVO.score = 0;
        selfInfoVO.sex = sex;
        selfInfoVO.sit = 1;
        selfInfoVO.isOnline = true;
        tingCards.Clear();
        playerIdInfoDic.Add(selfInfoVO.userId, selfInfoVO);
        UpdatePlayerSitDic();
        curInnings = 0;
        creatorId = userId;
        isStart = false;
        isSelfAction = false;
        hasDisloveApply = false;
        agreeIds.Clear();
        refuseIds.Clear();
        leftCard = GlobalData.CardWare.Length;
        var readyC2S = new ReadyC2S();
        SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_READY.GetHashCode(), 0, readyC2S);
        var settingC2S = new OnlineSettingC2S();
        settingC2S.isOnline = true;
        SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_ONLINESETTING.GetHashCode(), 0, settingC2S, true);
        PressDebug.Instance.AddRoom(this);
    }

    /// <summary>
    /// 刷新座位字典
    /// </summary>
    private void UpdatePlayerSitDic()
    {
        playerSitInfoDic = new Dictionary<int, PlayerInfoVOS2C>();
        foreach (var playerInfoVO in playerIdInfoDic)
        {
            playerSitInfoDic.Add(playerInfoVO.Value.sit, playerInfoVO.Value);
        }
    }

    /// <summary>
    /// 发送房间号
    /// </summary>
    public void SendRoomIDMsg(string roomCode)
    {
        if (ConnentionDic.ContainsKey(SocketType.HALL))
        {
            this.roomCode = roomCode;
            JoinInRoomC2S package = new JoinInRoomC2S();
            package.roomCode = roomCode;
            package.seat = 0;
            SendBuff<JoinInRoomC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_JOIN_IN_ROOM.GetHashCode(), 0, package);
        }        
    }

    /// <summary>
    /// 加入房间响应
    /// </summary>
    /// <param name="bytes"></param>
    private void JoinRoomResponse(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
        JoinInRoomS2C package = NetMgr.Instance.DeSerializes<JoinInRoomS2C>(bytes);
        if (package.clientCode == (int)ErrorCode.SUCCESS)
        {
            NSocket.UpdateRoomID(this.roomCode == "" ? 0 : int.Parse(this.roomCode), clientIndex);
            CreateConnect(SocketType.BATTLE, package.roomServerIp, package.roomServerPort, ()=> {
                JoinRoomC2S joinC2S = new JoinRoomC2S();
                joinC2S.seat = package.seat;
                joinC2S.roomCode = this.roomCode;
                SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_JOIN_ROOM.GetHashCode(), 0, joinC2S);
            });            
        }
        else if (package.clientCode == (int)ErrorCode.NO_ROOM)
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "加入提示";
            dialogVO.content = "房间不存在";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
        else if (package.clientCode == (int)ErrorCode.OVERFLOW_ROOM_PLAYERS)
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "加入提示";
            dialogVO.content = "房间人数已满！";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "加入房间失败";
            dialogVO.content = "加入房间失败,请检查网络连接";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }

    /// <summary>
    /// 加入房间返回
    /// </summary>
    /// <param PlayerName="bytes">消息体</param>
    private void JoinInRoomHandler(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
        forbitActions = new List<ForbitActionVO>();
        playerIdInfoDic = new Dictionary<int, PlayerInfoVOS2C>();
        var joinRoomS2C = NetMgr.Instance.DeSerializes<JoinRoomS2C>(bytes);
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
        int putCardNum = 0;
        foreach (var voS2C in joinRoomS2C.playInfoArr)
        {
            if (isStart)//已开局牌局手中的牌自动排序
            {
                voS2C.handCards.Sort();
            }
            playerIdInfoDic.Add(voS2C.userId, voS2C);
            putCardNum += voS2C.handCards.Count;
            if (voS2C.getCard > 0)
            {
                putCardNum += 1;
            }
            foreach (PengGangCardVO pengGangCardVo in voS2C.pengGangCards)
            {
                putCardNum += pengGangCardVo.pengGangCards.Count;
            }
            putCardNum += voS2C.putCards.Count;
        }
        UpdatePlayerSitDic();
        curInnings = joinRoomS2C.curInnings;
        SetPlayerActTipS2C(joinRoomS2C.playerTipAct);
        if (GetPlayerActTipS2C() != null)
        {
            isSelfAction = GetPlayerActTipS2C().optUserId == userId;
        }
        leftCard = joinRoomS2C.leftCardCount;
        var settingC2S = new OnlineSettingC2S();
        settingC2S.isOnline = true;
        SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_ONLINESETTING.GetHashCode(), 0, settingC2S, true);
        var readyC2S = new ReadyC2S();
        SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_READY.GetHashCode(), 0, readyC2S);
        if (isSelfAction)
        {
            AIPutCard();
        }
    }

    /// <summary>
    /// 推送玩家加入房间
    /// </summary>
    /// <param PlayerName="bytes"></param>
    private void PushJoinHandler(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
        var pushJoinS2C = NetMgr.Instance.DeSerializes<PushJoinS2C>(bytes);
        if (playerIdInfoDic == null || playerIdInfoDic.ContainsKey(pushJoinS2C.playerInfo.userId))
        {
            return;
        }
        playerIdInfoDic.Add(pushJoinS2C.playerInfo.userId, pushJoinS2C.playerInfo);
        UpdatePlayerSitDic();
    }

    /// <summary>
    /// AI出牌
    /// </summary>
    public void AIPutCard()
    {
        if (!GlobalData.isDebugModel || !isSelfAction || isReport)
        {
            return;
        }
        if (GetPlayerActTipS2C().acts.Contains(PlayerActType.PASS))
        {
            //SpecialCardHandler();
            var actC2S = new GuoC2S();
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PASS.GetHashCode(), 0, actC2S);
        }
        else if (GetPlayerActTipS2C().acts.Contains(PlayerActType.PUT_CARD))
        {
            //PutCardHandler();
            var actC2S = new PlayAMahjongC2S();
            actC2S.mahjongCode = playerIdInfoDic[userId].handCards[0];
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PLAY_A_MAHJONG.GetHashCode(), 0, actC2S);
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
        PlayerInfoVOS2C selfPlayerInfoVO = playerIdInfoDic[userId];
        foreach (int card in selfPlayerInfoVO.handCards)
        {
            canPutCards.Add(card);
        }
        if (selfPlayerInfoVO.getCard > 0)
        {
            canPutCards.Add(selfPlayerInfoVO.getCard);
        }
        for (int i = 0; i < GetPlayerActTipS2C().acts.Count; i++)
        {
            actPowers.Add(new ActPowerVO(GetPlayerActTipS2C().acts[i], GetPlayerActTipS2C().actCards[i], 0));
        }

        foreach (ActPowerVO actPowerVO in actPowers)
        {
            bool passAddPower = false;//过操作是否加权
            if (huTypes.Contains(actPowerVO.act))//胡牌权重最高
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
    /// 
    /// </summary>
    /// <param name="actPowerVO">权重VO</param>
    /// <param name="canPutCards">可出的牌数组</param>
    /// <param name="passAddPower">是否增加过操作权重</param>
    private void CalculateChiPower(ActPowerVO actPowerVO, List<int> canPutCards, out bool passAddPower)
    {
        List<List<int>> chiSelectArr = GetCanChiArr(actPowerVO.actCard);
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
    /// 获取当前牌内能吃的组合数组
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public List<List<int>> GetCanChiArr(int card)
    {
        var result = new List<List<int>>();
        var handCards = playerIdInfoDic[userId].handCards;

        int[] temp = { card + 1, card + 2, card - 1, card + 1, card - 2, card - 1 };
        bool isContainAll = true;

        int count = temp.Length / 2;
        int index = 0;
        List<int> canSelectCard;
        for (int i = 0; i < count; i++)
        {
            isContainAll = true;
            canSelectCard = new List<int>();
            for (int j = 0; j < 2; j++)
            {
                index = 2 * i + j;
                canSelectCard.Add(temp[index]);
                if (!handCards.Contains(temp[index]))
                {
                    isContainAll = false;
                    break;
                }
            }
            if (isContainAll)
            {
                canSelectCard.Remove(card);
                canSelectCard.Sort();
                result.Add(canSelectCard);
            }
        }

        return result;
    }

    /// <summary>
    /// 计算碰杠牌操作权重
    /// </summary>
    /// <param name="actPowerVO">权重VO</param>
    /// <param name="copyCanPutCards">可出的牌数组</param>
    /// <param name="passAddPower">是否增加过操作权重</param>
    private void CalculatePengGangPower(ActPowerVO actPowerVO, List<int> canPutCards, out bool passAddPower)
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
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PASS.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.PENG)
        {
            var actC2S = new PengC2S();
            actC2S.mahjongCode = actPowerVO.actCard;
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PENG.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.CHI)
        {
            var actC2S = new ChiC2S();
            actC2S.mahjongCodes.AddRange(chiPowerArr[0].chiCards);
            actC2S.mahjongCodes.Add(chiPowerArr[0].chiCard);
            actC2S.forbitCards.AddRange(chiPowerArr[0].forbitCards);
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PENG.GetHashCode(), 0, actC2S);
        }
        else if (actPowerVO.act == PlayerActType.SELF_HU)
        {
            var actC2S = new ZiMoHuC2S();
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_ZI_MO_HU.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.QIANG_AN_GANG_HU)
        {
            var actC2S = new QiangAnGangHuC2S();
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_AN_GANG_HU.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.QIANG_PENG_GANG_HU)
        {
            var actC2S = new QiangPengGangHuC2S();
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_PENG_GANG_HU.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.QIANG_ZHI_GANG_HU)
        {
            var actC2S = new QiangZhiGangHuC2S();
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_ZHI_GANG_HU.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.CHI_HU)
        {
            var actC2S = new ChiHuC2S();
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_CHI_HU.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.COMMON_AN_GANG)
        {
            var actC2S = new CommonAnGangC2S();
            actC2S.mahjongCode = actPowerVO.actCard;
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_COMMON_AN_GANG.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.BACK_AN_GANG)
        {
            var actC2S = new BackAnGangC2S();
            actC2S.mahjongCode = actPowerVO.actCard;
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_BACK_AN_GANG.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.ZHI_GANG)
        {
            var actC2S = new ZhiGangC2S();
            actC2S.mahjongCode = actPowerVO.actCard;
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_ZHI_GANG.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.COMMON_PENG_GANG)
        {
            var actC2S = new CommonPengGangC2S();
            actC2S.mahjongCode = actPowerVO.actCard;
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_COMMON_PENG_GANG.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
        else if (actPowerVO.act == PlayerActType.BACK_PENG_GANG)
        {
            var actC2S = new BackPengGangC2S();
            actC2S.mahjongCode = actPowerVO.actCard;
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_BACK_PENG_GANG.GetHashCode(), 0, actC2S);
            chiPowerArr = null;
        }
    }

    /// <summary>
    /// 出牌
    /// </summary>
    private void PutCardHandler()
    {
        List<CardPowerVO> cardPowerList = new List<CardPowerVO>();
        PlayerInfoVOS2C selfPlayerInfoVO = playerIdInfoDic[userId];
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
        SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PLAY_A_MAHJONG.GetHashCode(), 0, putC2S);
        chiPowerArr = null;
    }

    /// <summary>
    /// 推送发牌
    /// </summary>
    /// <param PlayerName="bytes"></param>
    private void GameStartHandler(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
        forbitActions = new List<ForbitActionVO>();
        isStart = true;
        var gameStartS2C = NetMgr.Instance.DeSerializes<GameStart_S2C>(bytes);
        startTime = gameStartS2C.startTime;
        tingCards.Clear();
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
            if (playerInfoVos2C.Value.userId == userId)
            {
                playerInfoVos2C.Value.handCards.AddRange(gameStartS2C.handCards);
                if (userId == gameStartS2C.bankerUserId)//自己是庄家,给自己加一张牌
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
        isSelfAction = gameStartS2C.pushPlayerActTipS2C.optUserId == userId;
        AIPutCard();
    }

    /// <summary>
    /// 推送玩家动作提示
    /// </summary>
    /// <param name="bytes">消息体</param>
    private void PushPlayerActTipHandler(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
        if (_isForbit)
        {
            var actionVO = new ForbitActionVO();
            actionVO.isActTip = true;
            actionVO.bytes = bytes;
            forbitActions.Add(actionVO);
            return;
        }
        var curActTips = NetMgr.Instance.DeSerializes<PushPlayerActTipS2C>(bytes);
        if (GetPlayerActTipS2C() != null && GetPlayerActTipS2C().optUserId == userId)
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
        isSelfAction = GetPlayerActTipS2C().optUserId == userId;
        AIPutCard();
    }

    /// <summary>
    /// 推送玩家动作
    /// </summary>
    /// <param PlayerName="bytes">消息体</param>
    private void PushPlayerActHandler(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
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
        }
    }

    /// <summary>
    /// 推送直杠牌消息处理
    /// </summary>
    private void ZhiGangActHandler()
    {
        var targetPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().targetUserId];
        var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
        var pengGangCardVOS2C = new PengGangCardVO();
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
        pengGangPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
        if (userId == GetPlayerActS2C().userId || isReport)//是自己找到对应的牌移除
        {
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
        }
        else//非自己随机找牌移除
        {
            var randomIndex = random.Next(0, pengGangPlayerVOS2C.handCards.Count - 3);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
        }

        pengGangPlayerVOS2C.handCards.Sort();
        targetPlayerVOS2C.putCards.RemoveAt(targetPlayerVOS2C.putCards.Count - 1); //移除最后一张出牌

    }

    /// <summary>
    /// 推送回头暗杠消息处理
    /// </summary>
    private void BackAnGangActHandler()
    {
        var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
        var pengGangCardVOS2C = new PengGangCardVO();
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
        pengGangPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
        if (userId == GetPlayerActS2C().userId || isReport)//是自己找到对应的牌移除
        {
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
        }
        else//非自己随机找牌移除
        {
            var randomIndex = random.Next(0, pengGangPlayerVOS2C.handCards.Count - 4);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
        }

        pengGangPlayerVOS2C.handCards.Sort();
    }

    /// <summary>
    /// 推送暗杠消息处理
    /// </summary>
    private void CommonAnGangActHandler()
    {
        var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
        var pengGangCardVOS2C = new PengGangCardVO();
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
        pengGangPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
        pengGangPlayerVOS2C.getCard = 0;
        if (userId == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
        {
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);

        }
        else//非自己随机找牌移除
        {
            var randomIndex = random.Next(0, pengGangPlayerVOS2C.handCards.Count - 3);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
        }

        pengGangPlayerVOS2C.handCards.Sort();
    }

    /// <summary>
    /// 推送普通碰杠消息处理
    /// </summary>
    private void CommonPengGangActHandler()
    {
        var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
        foreach (var pengGangCardVos2C in pengGangPlayerVOS2C.pengGangCards)
            if (pengGangCardVos2C.pengGangCards[0] == GetPlayerActS2C().actCard)
                pengGangCardVos2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangPlayerVOS2C.getCard = 0;
    }

    /// <summary>
    /// 推送回头碰杠消息处理
    /// </summary>
    private void BackPengGangActHandler()
    {
        var pengGangPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
        foreach (var pengGangCardVos2C in pengGangPlayerVOS2C.pengGangCards)
            if (pengGangCardVos2C.pengGangCards[0] == GetPlayerActS2C().actCard)
                pengGangCardVos2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        if (userId == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
        {
            pengGangPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
        }
        else
        {
            var randomIndex = random.Next(0, pengGangPlayerVOS2C.handCards.Count - 1);
            pengGangPlayerVOS2C.handCards.RemoveAt(randomIndex);
        }
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
        getCardPlayerVOS2C.getCard = GetPlayerActS2C().actCard;
    }

    /// <summary>
    /// 推送胡牌消息处理
    /// </summary>
    private void HuActHandler(bool isSelf)
    {
        var huCardPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
        if (isSelf) //自摸
        {
            if (huCardPlayerVOS2C.getCard > 0)
            {
                huCardPlayerVOS2C.handCards.Add(huCardPlayerVOS2C.getCard);
                huCardPlayerVOS2C.getCard = 0;
            }
        }
        else
        {
            var targetPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().targetUserId];
            huCardPlayerVOS2C.handCards.Add(GetPlayerActS2C().actCard);
            if (targetPlayerVOS2C.putCards.Count > 0 && targetPlayerVOS2C.putCards[targetPlayerVOS2C.putCards.Count - 1] == GetPlayerActS2C().actCard)
            {
                targetPlayerVOS2C.putCards.RemoveAt(targetPlayerVOS2C.putCards.Count - 1); //移除最后一张出牌
            }
        }
        SetPlayerActTipS2C(null);
    }

    /// <summary>
    /// 推送碰牌消息处理
    /// </summary>
    private void PengActHandler()
    {
        var pengPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
        var targetPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().targetUserId];
        var pengGangCardVOS2C = new PengGangCardVO();
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
        pengPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
        if (userId == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
        {
            pengPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
            pengPlayerVOS2C.handCards.Remove(GetPlayerActS2C().actCard);
        }
        else
        {
            var randomIndex = random.Next(0, pengPlayerVOS2C.handCards.Count - 2);
            pengPlayerVOS2C.handCards.RemoveAt(randomIndex);
            pengPlayerVOS2C.handCards.RemoveAt(randomIndex);
        }

        pengPlayerVOS2C.handCards.Sort();

        targetPlayerVOS2C.putCards.RemoveAt(targetPlayerVOS2C.putCards.Count - 1); //移除最后一张出牌
    }

    /// <summary>
    /// 推送过消息处理
    /// </summary>
    private void PassActHandler()
    {
        SetPlayerActTipS2C(null);
        var passPlayerVO = playerIdInfoDic[GetPlayerActS2C().userId];
    }

    /// <summary>
    /// 推送出牌消息处理
    /// </summary>
    private void PutCardActHandler()
    {
        var putCardPlayerVO = playerIdInfoDic[GetPlayerActS2C().userId];
        putCardPlayerVO.putCards.Add(GetPlayerActS2C().actCard);
        if (putCardPlayerVO.getCard != 0)
        {
            putCardPlayerVO.handCards.Add(putCardPlayerVO.getCard);
        }
        if (userId == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
        {
            putCardPlayerVO.handCards.Remove(GetPlayerActS2C().actCard);
        }
        else
        {
            var randomIndex = random.Next(0, putCardPlayerVO.handCards.Count - 1);
            putCardPlayerVO.handCards.RemoveAt(randomIndex);
        }

        putCardPlayerVO.handCards.Sort();
        putCardPlayerVO.getCard = 0;
    }

    /// <summary>
    /// 推送吃牌消息处理
    /// </summary>
    private void ChiActHandler()
    {
        var chiPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().userId];
        var targetPlayerVOS2C = playerIdInfoDic[GetPlayerActS2C().targetUserId];
        var pengGangCardVOS2C = new PengGangCardVO();
        pengGangCardVOS2C.pengGangCards.Add(GetPlayerActS2C().actCard);
        pengGangCardVOS2C.pengGangCards.AddRange(GetPlayerActS2C().chiCards);
        pengGangCardVOS2C.targetUserId = GetPlayerActS2C().targetUserId;
        chiPlayerVOS2C.pengGangCards.Add(pengGangCardVOS2C);
        if (userId == GetPlayerActS2C().userId || isReport) //是自己找到对应的牌移除
        {
            chiPlayerVOS2C.handCards.Remove(GetPlayerActS2C().chiCards[0]);
            chiPlayerVOS2C.handCards.Remove(GetPlayerActS2C().chiCards[1]);
        }
        else
        {
            var randomIndex = random.Next(0, chiPlayerVOS2C.handCards.Count - 2);
            chiPlayerVOS2C.handCards.RemoveAt(randomIndex);
            chiPlayerVOS2C.handCards.RemoveAt(randomIndex);
        }

        chiPlayerVOS2C.handCards.Sort();

        targetPlayerVOS2C.putCards.RemoveAt(targetPlayerVOS2C.putCards.Count - 1); //移除最后一张出牌
    }

    /// <summary>
    /// 本局结算信息
    /// </summary>
    public PushMatchResultS2C matchResultS2C;
    /// <summary>
    /// 之前的庄家id
    /// </summary>
    public int perBankerId;
    /// <summary>
    /// 庄家VOS2C
    /// </summary>
    public PlayerInfoVOS2C GetBankerPlayerInfoVOS2C()
    {
        foreach (var playerInfoVOS2C in playerIdInfoDic)
            if (playerInfoVOS2C.Value.isBanker)
                return playerInfoVOS2C.Value;
        return null;
    }
    /// <summary>
    /// 推送本局结束
    /// </summary>
    /// <param PlayerName="bytes"></param>
    private void PushMatchEndHandler(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
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
        Timer.Instance.AddTimer(0, 1, 1, ReadyNext);
    }

    /// <summary>
    /// 房间结算信息
    /// </summary>
    public PushRoomResultS2C roomResultS2C;
    /// <summary>
    /// 推送房间结束
    /// </summary>
    /// <param PlayerName="bytes"></param>
    private void PushRoomEndHandler(byte[] bytes, int clientIndex)
    {
        if (this.clientIndex != clientIndex)
        {
            return;
        }
        NSocket.UpdateRoomID(0,clientIndex);
        roomResultS2C = NetMgr.Instance.DeSerializes<PushRoomResultS2C>(bytes);
    }

    /// <summary>
    /// 准备下一局
    /// </summary>
    private void ReadyNext()
    {
        if (roomResultS2C != null)//最后一局
        {
            StopTcpConnection(SocketType.BATTLE);
            roomCode = null;
            CheckCreateRoomC2S package = new CheckCreateRoomC2S();
            package.roomRounds = GameMode.SIXTEEN_ROUND.GetHashCode();
            package.roomRule = (package.roomRounds == (int)GameMode.EIGHT_ROUND) ? 1 : 2;//(int)this.hallProxy.HallInfo.GameRule;
            package.playType.Add(GameRule.WORD.GetHashCode());
            SendBuff<CheckCreateRoomC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_CREATE_ROOM.GetHashCode(), 0, package);
        }
        else
        {
            AddInnings();
            var readyC2S = new ReadyC2S();
            SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_READY.GetHashCode(), 0, readyC2S);
            UIManager.Instance.HideUI(UIViewID.MATCH_RESULT_VIEW);
        }
    }

    /// <summary>
    /// 增加局数
    /// </summary>
    public void AddInnings()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
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
    }
}
