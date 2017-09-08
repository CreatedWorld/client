using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using Platform.Net;
using Platform.Model;
using UnityEngine;

public class InviteMediator : Mediator, IMediator
{
    public InviteMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_HALL_CLEARINPUTTEXT);
        return list;
    }
    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_HALL_CLEARINPUTTEXT:
                this.ClearInputText();
                break;
            default:
                break;
        }
    }
    public InviteView View
    {
        get
        {
            return (InviteView)ViewComponent;
        }
    }

    public override void OnRegister()
    {
        base.OnRegister();
        this.View.ButtonAddListening(this.View.ConfirmButton,
            () =>
            {
                CheckInvitationCodeC2S package = new CheckInvitationCodeC2S();
                try
                {
                    package.invitationId = int.Parse(this.View.InputField.text);
                }
                catch (FormatException)
                {
                    DialogMsgVO dialogVO = new DialogMsgVO();
                    dialogVO.dialogType = DialogType.ALERT;
                    dialogVO.title = "输入验证码错误";
                    dialogVO.content = "输入验证码错误,请重新输入";
                    DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
                    dialogView.data = dialogVO;
                    this.ClearInputText();
                    return;
                }
                NetMgr.Instance.SendBuff<CheckInvitationCodeC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_Check_Invitation_Code.GetHashCode(), 0, package);
            });
        this.View.ButtonAddListening(this.View.CloseButton,
            () =>
            {
                UIManager.Instance.HideUI(UIViewID.INVITE_VIEW);
            });
    }
    public override void OnRemove()
    {
        base.OnRemove();
    }
    private void ClearInputText()
    {
        this.View.InputField.text = "";
    }
}
