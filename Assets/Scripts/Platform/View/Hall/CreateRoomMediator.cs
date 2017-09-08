using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using Platform.Net;
using Platform.Model;
using UnityEngine.UI;
using UnityEngine;

public class CreateRoomMediator : Mediator, IMediator
{
    private HallProxy hallProxy;
    private PlayerInfoProxy playerInfoProxy;
    private int round;
    private int payway;
    private List<int> ruleList = new List<int>();
    public CreateRoomMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }

    public CreateRoomView View
    {
        get
        {
            return (CreateRoomView)ViewComponent;
        }
    }
    public override void OnRegister()
    {
        base.OnRegister();
        hallProxy = Facade.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        playerInfoProxy = Facade.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
        View.ButtonAddListening(this.View.CloseButton,
        () =>
        {
            UIManager.Instance.HideUI(UIViewID.CREATEROOM_VIEW);
        });
        
        View.ButtonAddListening(this.View.CreateButton, SendCreateRoom);
        //hallProxy.HallInfo.innings = GameMode.EIGHT_ROUND;
        //hallProxy.HallInfo.gameRule = GameRule.WORD;
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        return list;
    }
    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            default:
                break;
        }
    }
    /// <summary>规则
    /// 40——8局，41——一锅
    /// 42——房主扣，43——玩家均摊
    /// 44——三家炮，45——一家炮
    /// 46——蛋带翻，47——蛋不翻
    /// 48——长跑3，49——长跑5，50——长跑10
    /// </summary>
    private void SendCreateRoom()
    {
        if (this.CheckRoomCard())
        {
            //局数
            if (View.EightRound.GetComponent<Toggle>().isOn) { round = 40; PlayerPrefs.SetString(PrefsKey.ROUND,"8"); }
            else { { round = 41; PlayerPrefs.SetString(PrefsKey.ROUND, "一锅"); } }
            //房费
            if (View.CreatorPay.GetComponent<Toggle>().isOn) { ruleList.Add(42); PlayerPrefs.SetString(PrefsKey.PAYWAY, "房主支付"); }
            else { { ruleList.Add(43); PlayerPrefs.SetString(PrefsKey.PAYWAY, "玩家均摊"); } }

            //玩法
            if (View.OneShoot.GetComponent<Toggle>().isOn) { ruleList.Add(45); PlayerPrefs.SetString(PrefsKey.RULE1, "一家炮"); }
            else { ruleList.Add(44); PlayerPrefs.SetString(PrefsKey.RULE1, "三家炮"); }

            if (View.EggCanLieDown.GetComponent<Toggle>().isOn) { ruleList.Add(46); PlayerPrefs.SetString(PrefsKey.RULE2, "蛋带翻"); }
            else { ruleList.Add(47); PlayerPrefs.SetString(PrefsKey.RULE2, "蛋不翻"); }

            if (View.Run3.GetComponent<Toggle>().isOn) { ruleList.Add(48); PlayerPrefs.SetString(PrefsKey.RULE3, "长跑3"); }
            else if (View.Run5.GetComponent<Toggle>().isOn) { ruleList.Add(49); PlayerPrefs.SetString(PrefsKey.RULE3, "长跑5"); }
            else { ruleList.Add(50); PlayerPrefs.SetString(PrefsKey.RULE3, "长跑10"); }

            CheckCreateRoomC2S package = new CheckCreateRoomC2S();
            package.roomRounds = round == 40 ? 8 : 1000000;//无用
            package.roomRule = round;
            package.playType.AddRange(ruleList);

            NetMgr.Instance.SendBuff<CheckCreateRoomC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_CREATE_ROOM.GetHashCode(), 0, package);
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "创建房间失败";
            dialogVO.content = "您的房卡不足,请充值";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }

    private bool CheckRoomCard()
    {
        int cost;
        switch (this.hallProxy.HallInfo.innings)
        {
            case GameMode.EIGHT_ROUND:
                cost = 4;
                break;
            case GameMode.INFINITY:
                cost = 8;
                break;
            default:
                cost = 0;
                break;
        }
        if (!this.playerInfoProxy.userItems.ContainsKey(ItemType.DIAMONDS))
        {
            return false;
        }
        int roomCard = this.playerInfoProxy.userItems[ItemType.DIAMONDS].amount;
        return roomCard >= cost;
        //return true;
    }
    private void ChangeGameModel(bool isOn, GameMode model)
    {
        if (isOn)
        {
            this.hallProxy.HallInfo.innings = model;
        }
    }
    private void ChangeGameRule(bool isOn, GameRule rule)
    {
        if (isOn)
        {
            this.hallProxy.HallInfo.gameRule = rule;
        }
    }
}