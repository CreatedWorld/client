using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;

public class PlayerInfoMediator : Mediator, IMediator
{
    public PlayerInfoMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }
    public PlayerInfoView View
    {
        get
        {
            return (PlayerInfoView)ViewComponent;
        }
    }

    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        return list;
    }

    public override void OnRegister()
    {
        base.OnRegister();
        this.View.ButtonAddListening(this.View.CloseButton,
        () =>
        {
            UIManager.Instance.HideUI(UIViewID.PLATER_INFO_VIEW);
        });

        //刷新用户信息
        //this.RefreshPlayerInfo();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    public override void HandleNotification(INotification notification)
    {
        
    }

    /// <summary>
    /// 更新玩家信息
    /// </summary>
    public void RefreshPlayerInfo()
    {
        View.UserID.text = View.data.userId.ToString();
        View.UsernameText.text = View.data.userName;
        View.CardText.text = View.data.userItems[0].amount.ToString();
        View.data.ip = View.data.ip.Replace("/", "");
        View.IpText.text = string.Format(View.data.ip);
        GameMgr.Instance.StartCoroutine(DownIcon(View.data.imageUrl));
    }

    System.Collections.IEnumerator DownIcon(string headUrl)
    {
        WWW www = new WWW(headUrl);
        yield return www;
        if (www.error == null)
        {
            View.HeadIcon.texture = www.texture;
        }
    }
}

