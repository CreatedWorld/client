using System.Collections.Generic;
using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using Platform.Net;
using Platform.Model;
using Platform.Utils;
using Platform.Global;
using LZR.Data.NetWork.Client;
using Platform.Model.Hall;
using Utils;
using System.Collections;
/// <summary>
/// 大厅数据代理
/// </summary>
public class HallProxy : Proxy, IProxy
{
    /// <summary>
    /// 进入战斗的方式
    /// </summary>
    private string battleMsgNo;
    /// <summary>
    /// 商品信息数组
    /// </summary>
    public List<Productinfo> productList;
    /// <summary>
    /// 排行榜列表
    /// </summary>
    public List<RankItem> ranks;
    /// <summary>
    /// 大厅房间列表
    /// </summary>
    public List<HallRoomInfo> hallRoomListItem = null;
    /// <summary>
    /// 订单查询定时器id
    /// </summary>
    public int orderTimeId;

    public HallInfoVO HallInfo
    {
        get
        {
            return (HallInfoVO)this.Data;
        }
    }

    public HallProxy(string NAME) : base(NAME, new HallInfoVO())
    {
        this.HallInfo.gameRule = GameRule.WORD;
        this.HallInfo.innings = GameMode.EIGHT_ROUND;
    }
    public override void OnRegister()
    {
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_GET_CHECKIN_INFO, GetSignInInfoResponse);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_CHECKIN, RefreshSignInInfoResponse);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_CREATE_ROOM, CreateRoomResponse);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_JOIN_IN_ROOM,JoinRoomResponse);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_APPLY_COMPETITION,ApplySucceed);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_CHECK_APPLY_STATUS, CheckApplyState);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_GET_BATTLE_HISTORY,GetGradeInfo);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_GET_BATTLE_DETAIL,GetRoundInfo);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Match, StartMatching);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_GET_CHEKCIN_REWARD,GetCheckInreward);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Check_Invitation_Code, InviteCodeSucceed);
        //GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_PUSH_ANNOUNCE, UpdateAnnouncementContent);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Push_Notice,GetNoticeInfo);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Push_Match_Suc,MatchingSucceed);
		GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Reconnect, ReConnectHandler);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Cancel_Match,CancelMatching);
        //GameMgr.Instance.AddMsgHandler(MsgNoS2C.GET_PRODUCTLIST_S2C, GetProductionHandler);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_CoreHall_OrederInfoC2S, GetOrderInfoHandler);
        //GameMgr.Instance.AddMsgHandler(MsgNoS2C.PAY_SUCCESS_S2C, PaySuccessHandler);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Get_Ranking_List, GetRankHandler);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Get_HallRoomList, GetHallRoomListHandler);
    }

    /// <summary>
    /// 登陆大厅获取签到信息
    /// </summary>
    /// <param name="bytes">签到信息</param>
    private void GetSignInInfoResponse(byte[] bytes)
    {
        GetCheckInInfoS2C package = NetMgr.Instance.DeSerializes<GetCheckInInfoS2C>(bytes);
        this.HallInfo.signInDay = package.days;
        this.HallInfo.signInState = package.status;
    }

    /// <summary>
    /// 刷新签到数据
    /// </summary>
    /// <param name="bytes">签到信息</param>
    private void RefreshSignInInfoResponse(byte[] bytes)
    {
        CheckInS2C package = NetMgr.Instance.DeSerializes<CheckInS2C>(bytes);
        this.HallInfo.signInDay = package.days;
        this.HallInfo.signInState = package.status;
        Facade.SendNotification(NotificationConstant.MEDI_SIGNIN_REFRESHSIGN);//发送刷新签到视图消息
    }
    /// <summary>
    /// 创建房间响应
    /// </summary>
    /// <param name="bytes"></param>
    private void CreateRoomResponse(byte[] bytes)
    {
        CheckCreateRoomS2C package = NetMgr.Instance.DeSerializes<CheckCreateRoomS2C>(bytes);
        if (package.clientCode == (int)ErrorCode.SUCCESS)
        {
            this.HallInfo.battleSeverIP = package.roomServerIp;
            this.HallInfo.battleSeverPort = package.roomServerPort;
            this.HallInfo.roomCode = package.roomCode;
            this.HallInfo.seat = package.seat;
            NSocket.UpdateRoomID(HallInfo.roomCode == "" ? 0 : int.Parse(HallInfo.roomCode));
            this.BattleServerConnect(NotificationConstant.MEDI_HALL_CUTCREATESCENE);

        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "创建房间失败";
            if (package.clientCode == 3)
            {
                dialogVO.content = "最多可创建5个房间！";
            }else
                dialogVO.content = "创建房间失败,请检查网络连接";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }
    /// <summary>
    /// 加入房间响应
    /// </summary>
    /// <param name="bytes"></param>
    private void JoinRoomResponse(byte[] bytes)
    {
        JoinInRoomS2C package = NetMgr.Instance.DeSerializes<JoinInRoomS2C>(bytes);
        if (package.clientCode == (int)ErrorCode.SUCCESS)
        {
            this.HallInfo.innings = (GameMode)package.roomRounds;
            this.HallInfo.gameRule = (GameRule)package.roomRule;
            this.HallInfo.battleSeverIP = package.roomServerIp;
            this.HallInfo.battleSeverPort = package.roomServerPort;
            this.HallInfo.seat = package.seat;
            NSocket.UpdateRoomID(HallInfo.roomCode == "" ? 0 : int.Parse(HallInfo.roomCode));
            this.BattleServerConnect(NotificationConstant.MEDI_HALL_CUTJOINSCENE);
        }
        else if(package.clientCode == (int)ErrorCode.NO_ROOM)
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
    /// 报名成功消息
    /// </summary>
    /// <param name="bytes"></param>
    private void ApplySucceed(byte[] bytes)
    {
        ApplyCompetitionS2C package = NetMgr.Instance.DeSerializes<ApplyCompetitionS2C>(bytes);
        if (package.status == 1)
        {
            HallInfo.arenaStatus = 1;
            Facade.SendNotification(NotificationConstant.MEDI_HALL_APPLYSUCCEED);
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "报名失败";
            dialogVO.content = "报名失败,请检查网络连接";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }
    /// <summary>
    /// 检查报名时间
    /// </summary>
    private void CheckApplyState(byte[] bytes)
    {
        CheckApplyStatusS2C package = NetMgr.Instance.DeSerializes<CheckApplyStatusS2C>(bytes);
        int state = package.status;
        HallInfo.arenaStatus = package.status;
        HallInfo.currentTime = package.currentTime;
        HallInfo.startTime = package.startTime;
        HallInfo.endTime = package.endTime;
        //HallInfo.applyStartTime = package.applyStartTime;
        //HallInfo.applyEndTime = package.applyEndTime;
        //HallInfo.ruleDesc = package.ruleDesc;
        //HallInfo.rewardDesc = package.rewardDesc;
        UIManager.Instance.ShowUI(UIViewID.ARENA_VIEW);//比赛界面
    }
    
    /// <summary>
    /// 连接游戏服务器
    /// </summary>
    /// <param name="msgNo"></param>
    private void BattleServerConnect(string msgNo)
    {
        battleMsgNo = msgNo;
        NetMgr.Instance.CreateConnect(SocketType.BATTLE, HallInfo.battleSeverIP, HallInfo.battleSeverPort, BattleConnectHandler);
    }

    /// <summary>
    /// 战斗服务器连接回调
    /// </summary>
    private void BattleConnectHandler()
    {
        Debug.Log("连接游戏服务器成功");
        NetMgr.Instance.ConnentionDic[SocketType.BATTLE].OnConnectSuccessful = null;
        var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as Platform.Model.Battle.BattleProxy;
        battleProxy.isReport = false;
        if (battleMsgNo == NotificationConstant.MEDI_HALL_CUTCREATESCENE)
        {
            NetMgr.Instance.SendBuff<EnterRoomServerC2S>(SocketType.BATTLE, MsgNoC2S.C2S_ENTER_ROOM.GetHashCode(), 0, new EnterRoomServerC2S());
        }
        else if (battleMsgNo == NotificationConstant.MEDI_HALL_CUTJOINSCENE)
        {
            JoinRoomC2S joinC2S = new JoinRoomC2S();
            joinC2S.seat = this.HallInfo.seat;
            joinC2S.roomCode = this.HallInfo.roomCode;
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_JOIN_ROOM.GetHashCode(), 0, joinC2S);
        }
    }

    /// <summary>
    /// 获取战绩信息
    /// </summary>
    /// <param name="bytes"></param>
    private void GetGradeInfo(byte[] bytes)
    {
        GetGradeInfoS2C package = NetMgr.Instance.DeSerializes<GetGradeInfoS2C>(bytes);
        if (package != null)
        {
            SendNotification(NotificationConstant.MEDI_HALL_INITGRADEINFO, package);
        }
    }

    /// <summary>
    /// 获取房间具体对战信息
    /// </summary>
    /// <param name="bytes"></param>
    private void GetRoundInfo(byte[] bytes)
    {
        GetRoundInfoS2C package = NetMgr.Instance.DeSerializes<GetRoundInfoS2C>(bytes);
        if (package != null)
        {
            SendNotification(NotificationConstant.MEDI_HALL_GETROUNDINFO, package);
        }
    }
    /// <summary>
    /// 匹配
    /// </summary>
    /// <param name="bytes"></param>
    private void StartMatching(byte[] bytes)
    {
        JoinCompetitionS2C package = NetMgr.Instance.DeSerializes<JoinCompetitionS2C>(bytes);
        if (package.status == 1)
        {
            Debug.Log("开始匹配");
            UIManager.Instance.HideUI(UIViewID.ARENA_VIEW, 
                () => 
                {
                    UIManager.Instance.ShowUI(UIViewID.MATCHING_VIEW);
                });
        }
        else if(package.status == 0)
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "匹配失败";
            dialogVO.content = "匹配失败,请检查网络连接";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }
    /// <summary>
    /// 领取房卡
    /// </summary>
    /// <param name="bytes"></param>
    private void GetCheckInreward(byte[] bytes)
    {
        GetCardInfoS2C package = NetMgr.Instance.DeSerializes<GetCardInfoS2C>(bytes);
        if (package.status == 1)
        {
            Debug.Log("领取奖励成功");
            UIManager.Instance.HideUI(UIViewID.SIGNIN_VIEW);
            PlayerInfoProxy playerProxy = Facade.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            foreach (UserItem item in package.userItems)
            {
                if (playerProxy.userItems.ContainsKey(item.type))
                {
                    playerProxy.userItems[item.type] = item;
                }
                else
                {
                    playerProxy.userItems.Add(item.type, item);
                }
            }
            SendNotification(NotificationConstant.MEDI_HALL_REFRESHITEM);
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "领取奖励失败";
            dialogVO.content = "领取奖励失败,请检查网络连接";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }
    /// <summary>
    /// 验证码检验成功
    /// </summary>
    /// <param name="bytes"></param>
    private void InviteCodeSucceed(byte[] bytes)
    {
        CheckInvitationCodeS2C package = NetMgr.Instance.DeSerializes<CheckInvitationCodeS2C>(bytes);
        if (package.status == ErrorCode.SUCCESS)
        {
            UIManager.Instance.HideUI(UIViewID.INVITE_VIEW,
                ()=> 
                {
                    UIManager.Instance.ShowUI(UIViewID.SHOPPING_VIEW);
                });
            PlayerInfoProxy pip = Facade.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            if (pip.boundAgency == ErrorCode.FAILT)
            {
                pip.boundAgency = ErrorCode.SUCCESS;
            }
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "输入验证码错误";
            dialogVO.content = "输入验证码错误,请重新输入";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_CLEARINPUTTEXT);
    }
    /// <summary>
    /// 更新喇叭内容
    /// </summary>
    /// <param name="bytes"></param>
    private void UpdateAnnouncementContent(byte[] bytes)
    {
        PushAnnouncementS2C package = NetMgr.Instance.DeSerializes<PushAnnouncementS2C>(bytes);
        this.HallInfo.announcementQueue.Enqueue(new AnnouncementData(package.content, package.cirCount));
        //SendNotification(NotificationConstant.MEDI_HALL_REFRESHANNOUNCEMENT);
    }
    /// <summary>
    /// 更新大厅公告
    /// </summary>
    private void GetNoticeInfo(byte[] bytes)
    {
        NoticeConfigS2C package = NetMgr.Instance.DeSerializes<NoticeConfigS2C>(bytes);
        foreach (NoticeConfigDataS2C data in package.noticeConfigData)
        {
            if (this.HallInfo.noticeList.ContainsKey((HallNoticeType)data.type))
            {
                this.HallInfo.noticeList[(HallNoticeType)data.type] = data;
            }
            else
            {
                this.HallInfo.noticeList.Add((HallNoticeType)data.type,data);
            }
            if (data.type == 2)//喇叭
            {
                HallInfo.announcementQueue.Enqueue(new AnnouncementData(data.content, 1));
            }
        }
        //(NotificationConstant.MEDI_HALL_REFRESHANNOUNCEMENT); 
    }
    /// <summary>
    /// 匹配成功
    /// </summary>
    /// <param name="bytes"></param>
    private void MatchingSucceed(byte[] bytes)
    {
        Debug.Log("匹配成功");
        MatchingSucceedS2C package = NetMgr.Instance.DeSerializes<MatchingSucceedS2C>(bytes);
        this.HallInfo.roomCode = package.roomCode.ToString();
        this.HallInfo.innings = (GameMode)package.roomRounds;
        this.HallInfo.gameRule = (GameRule)package.roomRule;
        this.HallInfo.battleSeverIP = package.roomServerIp;
        this.HallInfo.battleSeverPort = package.roomServerPort;
        this.HallInfo.seat = package.seat;
        this.BattleServerConnect(NotificationConstant.MEDI_HALL_CUTJOINSCENE);
    }

    /// <summary>
    /// 取消匹配
    /// </summary>
    /// <param name="bytes"></param>
    private void CancelMatching(byte[] bytes)
    {
        CancelMatchingS2C package = NetMgr.Instance.DeSerializes<CancelMatchingS2C>(bytes);
        if (package.status == (int)ErrorCode.SUCCESS)
        {
            UIManager.Instance.HideUI(UIViewID.MATCHING_VIEW);
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "取消匹配失败";
            dialogVO.content = "取消匹配失败,请检查网络连接";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }    
    
    /// <summary>
    /// 断线重连
    /// </summary>
    /// <param name="bytes"></param>
    private void ReConnectHandler(byte[] bytes)
    {
        ReConnectS2C reconnectS2C = NetMgr.Instance.DeSerializes<ReConnectS2C>(bytes);
        this.HallInfo.roomCode = reconnectS2C.roomCode;
        this.HallInfo.battleSeverIP = reconnectS2C.roomIp;
        this.HallInfo.battleSeverPort = reconnectS2C.roomPort;
        NSocket.UpdateRoomID(reconnectS2C.roomCode == "" ? 0 : int.Parse(reconnectS2C.roomCode));
        if (reconnectS2C.roomId > 0)
        {
            this.BattleServerConnect(NotificationConstant.MEDI_HALL_CUTJOINSCENE);
        }
        else
        {
            SendNotification(NotificationConstant.MEDI_LOGIN_SWITCHHALLSCENE);
        }
    }

    /// <summary>
    /// 获取商品信息返回
    /// </summary>
    /// <param name="bytes"></param>
    //private void GetProductionHandler(byte[] bytes)
    //{
    //    var getProductListS2C = NetMgr.Instance.DeSerializes<GetProductListS2C>(bytes);
    //    productList = getProductListS2C.productInfo;
    //    SendNotification(NotificationConstant.MEDI_HALL_PRODUCTUPDATE);
    //}

    /// <summary>
    /// 获取订单信息返回
    /// </summary>
    /// <param name="bytes"></param>
    private void GetOrderInfoHandler(byte[] bytes)
    {
        var getOrderInfoS2C = NetMgr.Instance.DeSerializes<GetOrderInfoS2C>(bytes);
        PayVO payVO = new PayVO();
        payVO.money = (getOrderInfoS2C.amount*100).ToString();
        payVO.subject = getOrderInfoS2C.goodsName;
        payVO.pricePointDec = getOrderInfoS2C.goodsName;
        payVO.outTradeNo = getOrderInfoS2C.payid;
        Debug.Log(JsonUtility.ToJson(payVO));

        if (GlobalData.sdkPlatform == SDKPlatform.ANDROID)
        {
            AndroidSdkInterface.FWPay(JsonUtility.ToJson(payVO));
        }
        else if (GlobalData.sdkPlatform == SDKPlatform.IOS)
        {
            IOSSdkInterface.otherPay(JsonUtility.ToJson(payVO));
            orderTimeId = Timer.Instance.AddTimer(5, 24, 5, () => {
                NetMgr.Instance.SendBuff<GetUserInfoC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_GET_USERINFO.GetHashCode(), 0, new GetUserInfoC2S(), true);
            });

        }
    }

    /// <summary>
    /// 支付成功返回
    /// </summary>
    /// <param name="bytes"></param>
    //private void PaySuccessHandler(byte[] bytes)
    //{
    //    var paySucessS2C = NetMgr.Instance.DeSerializes<PaySuccessS2C>(bytes);
    //    if (paySucessS2C.status == 2)
    //    {
    //        DialogMsgVO dialogMsgVO = new DialogMsgVO();
    //        dialogMsgVO.title = "充值提示";
    //        dialogMsgVO.content = "服务器充值失败";
    //        dialogMsgVO.dialogType = DialogType.ALERT;
    //        DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
    //        dialogView.data = dialogMsgVO;
    //        return;
    //    }
    //    PlayerInfoProxy playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYERINFO) as PlayerInfoProxy;
    //    if (playerInfoProxy.UserItems.ContainsKey(paySucessS2C.type))
    //    {
    //        playerInfoProxy.UserItems[paySucessS2C.type].amount = paySucessS2C.amount;
    //    }
    //    else
    //    {
    //        var userItem = new UserItem();
    //        userItem.type = paySucessS2C.type;
    //        userItem.amount = paySucessS2C.amount;
    //        playerInfoProxy.UserItems.Add(paySucessS2C.type, userItem);
    //    }
    //    SendNotification(NotificationConstant.MEDI_HALL_REFRESHUSERINFO);
    //}

    /// <summary>
    /// 获取排行榜返回
    /// </summary>
    /// <param name="bytes"></param>
    private void GetRankHandler(byte[] bytes)
    {
        var getRankS2C = NetMgr.Instance.DeSerializes<GetRankS2C>(bytes);
        ranks = getRankS2C.ranks;
        if (ranks.Count == 0)
        {
            DialogMsgVO dialogMsgVO = new DialogMsgVO();
            dialogMsgVO.content = "第一局比赛场尚未结束，比赛场获取不到历史排行数据";
            dialogMsgVO.dialogType = DialogType.ALERT;
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogMsgVO;
            return;
        }
        UIManager.Instance.ShowUI(UIViewID.RANK_VIEW);
    }
    private void GetHallRoomListHandler(byte[] bytes)
    {
        var getRoomListInfo = NetMgr.Instance.DeSerializes<GetHallRoomListS2C>(bytes);
        hallRoomListItem = getRoomListInfo.hallRoomInfo;

        ArrayList roomArr = new ArrayList();
        //Debug.Log("hallRoomListItem = "+ hallRoomListItem.Count);
        if (hallRoomListItem != null && HallRoomListView.roomlistTable != null)
        {
            roomArr.AddRange(hallRoomListItem);
            HallRoomListView.roomlistTable.DataProvider = roomArr;
            //Debug.Log("Refresh hallroomlistview...");
            
        }

    }
}
