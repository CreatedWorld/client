using System.Collections.Generic;
using Platform.Model.Battle;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using Platform.Net;
using Platform.Model;

/// <summary>
/// 房间结算中介
/// </summary>
public class BattleDebugMediator : Mediator, IMediator
{
    public BattleDebugMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {
    }

    public BattleDebug View
    {
        get { return (BattleDebug)ViewComponent; }
    }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_INIT_CARDPOOL);
        return list;
    }


    public override void OnRegister()
    {
        base.OnRegister();
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_GetAllCardC2S.GetHashCode(), 0, new GetAllCardC2S());
    }

    public override void OnRemove()
    {
        base.OnRemove();

    }

    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_INIT_CARDPOOL:
                View.InitCardPool();
                break;
        }
    }

}