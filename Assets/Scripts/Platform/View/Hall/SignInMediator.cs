using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using Platform.Net;
using Platform.Model;
using DG.Tweening;
using UnityEngine.UI;
using Platform.Global;

public class SignInMediator : Mediator, IMediator
{
    public SignInMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {
    }

    public SignInView View
    {
        get
        {
            return (SignInView)ViewComponent;
        }
    }

    public override void OnRegister()
    {
        base.OnRegister();
        this.View.ButtonAddListening(this.View.CloseButton, 
            () => 
            {
                UIManager.Instance.HideUI(UIViewID.SIGNIN_VIEW);
            });
        this.View.ButtonAddListening(this.View.SignInButton, SendSignInMsg);
        this.View.ButtonAddListening(this.View.GetButton,SendGetCardMsg);
        this.InitSignInInfo();
    }
    public override void OnRemove()
    {
        base.OnRemove();
    }
    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_SIGNIN_REFRESHSIGN);
        return list;
    }

    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_SIGNIN_REFRESHSIGN:
                this.RefreshSignInInfo();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 初始化签到界面
    /// </summary>
    private void InitSignInInfo()
    {
        HallProxy hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        for (int i = 0; i < hallProxy.HallInfo.signInDay; i++)
        {

            Transform dayState = this.View.DataList[i];
            dayState.FindChild("Unused").gameObject.SetActive(false);
            dayState.FindChild("Used").gameObject.SetActive(true);
        }
        if (hallProxy.HallInfo.signInDay >= 7)
        {
            this.View.SignInButton.gameObject.SetActive(false);
            this.View.GetButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 刷新签到界面
    /// </summary>
    private void RefreshSignInInfo()
    {
        HallProxy hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        if (hallProxy.HallInfo.signInState == (int)ErrorCode.SUCCESS)
        {
            for (int i = 0; i < hallProxy.HallInfo.signInDay; i++)
            {
                Transform dayState = this.View.DataList[i];
                if (i == hallProxy.HallInfo.signInDay - 1)
                {
                    SignInInfoAnimation(dayState);
                }
                else
                {
                    dayState.FindChild("Unused").gameObject.SetActive(false);
                    dayState.FindChild("Used").gameObject.SetActive(true);
                }
            }
            if (hallProxy.HallInfo.signInDay >= 7)
            {
                this.View.SignInButton.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.Linear).OnComplete(
                    ()=> 
                    {
                        this.View.SignInButton.transform.gameObject.SetActive(false);
                        this.View.SignInButton.transform.localScale = Vector3.one;
                        this.View.GetButton.transform.localScale = Vector3.zero;
                        this.View.GetButton.gameObject.SetActive(true);
                        this.View.GetButton.transform.DOScale(Vector3.one,0.2f).SetEase(Ease.Linear);
                    });
            }
        }
        else
        {
            DialogMsgVO dialogVO = new DialogMsgVO();
            dialogVO.dialogType = DialogType.ALERT;
            dialogVO.title = "签到失败";
            dialogVO.content = "今日已签到,请勿重复签到";
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogVO;
        }
    }

    /// <summary>
    /// 发送签到请求
    /// </summary>
    private void SendSignInMsg()
    {
        CheckInC2S package = new CheckInC2S();
        NetMgr.Instance.SendBuff<CheckInC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_CHECKIN.GetHashCode(), 0, package);
    }

    /// <summary>
    /// 签到动画
    /// </summary>
    /// <param name="dayTrans"></param>
    private void SignInInfoAnimation(Transform dayTrans)
    {
        Transform unused = dayTrans.FindChild("Unused");
        Transform used = dayTrans.FindChild("Used");
        Transform mask = used.FindChild("Mask");
        Transform isOn = used.FindChild("IsOn");
        Transform usedText = used.Find("Text");
        Image maskImage = mask.GetComponent<Image>();
        Image usedImage = used.GetComponent<Image>();
        used.gameObject.SetActive(true);
        usedText.gameObject.SetActive(false);
        usedImage.color = new Color(1f,1f,1f,0f);

        maskImage.DOColor(new Color(1f, 1f, 1f, 1f), 0.2f).SetEase(Ease.Linear).OnComplete(
            () =>
            {
                unused.gameObject.SetActive(false);
                usedText.gameObject.SetActive(true);
                usedImage.color = new Color(1f, 1f, 1f, 1f);
                maskImage.DOColor(new Color(1f, 1f, 1f, 0f), 0.2f).SetEase(Ease.Linear);
            });
        isOn.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        isOn.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// 发送领取房卡请求
    /// </summary>
    private void SendGetCardMsg()
    {
        GetCardInfoC2S package = new GetCardInfoC2S();
        NetMgr.Instance.SendBuff<GetCardInfoC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_GET_CHECKIN_REWARD.GetHashCode(),0,package);
    }
}

