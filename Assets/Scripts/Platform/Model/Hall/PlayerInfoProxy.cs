using PureMVC.Interfaces;
using PureMVC.Patterns;
using Platform.Net;
using Platform.Model;
using Utils;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家信息数据代理
/// </summary>
public class PlayerInfoProxy : Proxy, IProxy
{
    /// <summary>
    /// 微信制定的唯一id
    /// </summary>
    public int userID;
    /// <summary>
    /// 角色名称
    /// </summary>
    public string userName;
    /// <summary>
    /// 货币
    /// </summary>
    public Dictionary<int, UserItem> userItems = new Dictionary<int, UserItem>();
    /// <summary>
    /// 当前积分
    /// </summary>
    public int score;

    ///<summary>
    /// 性别
    /// </summary>
    public int sex;
    /// <summary>
    /// 本机IP
    /// </summary>
    public string localIP;

    /// <summary>
    /// 玩家显示ID
    /// </summary>
    public string showID;

    /// <summary>
    /// 微信的头像url地址
    /// </summary>
    public string headIconUrl;
    /// <summary>
    /// 头像缓存地址
    /// </summary>
    public Texture headIcon;
    /// <summary>
    /// 是否绑定代理
    /// </summary>
    public int boundAgency;
    public PlayerInfoProxy(string NAME) : base(NAME)
    {
    }

    public override void OnRegister()
    {
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_GET_USERINFO, GetUserInfoResponse);
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_Hall_Push_Item, RefreshUserItem);
    }
    /// <summary>
    /// 更新用户数据
    /// </summary>
    /// <param name="bytes"></param>
    private void GetUserInfoResponse(byte[] bytes)
    {
        GetUserInfoS2C package = NetMgr.Instance.DeSerializes<GetUserInfoS2C>(bytes);
        this.userName = package.userName;
        foreach(UserItem item in package.userItems)
        {
            if (!this.userItems.ContainsKey(item.type))
            {
                this.userItems.Add(item.type, item);
            }
            else
            {
                var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
                if (item.amount > this.userItems[item.type].amount && hallProxy.orderTimeId > 0)
                {
                    DialogMsgVO dialogMsgVO = new DialogMsgVO();
                    dialogMsgVO.title = "充值提示";
                    dialogMsgVO.content = "充值成功";
                    dialogMsgVO.dialogType = DialogType.ALERT;
                    DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
                    dialogView.data = dialogMsgVO;
                    Timer.Instance.CancelTimer(hallProxy.orderTimeId);
                    hallProxy.orderTimeId = 0;
                }
                this.userItems[item.type] = item;
            }
        }
        this.showID = package.showId;
        //string ip = package.ip;
        //ip = ip.Substring(0, ip.Length - 5);
        //ip = ip.Replace("/", "");
        this.localIP = package.ip;
        this.headIconUrl = package.imageUrl;
        this.sex = package.sex;
        this.boundAgency = package.boundAgency;
        SendNotification(NotificationConstant.MEDI_HALL_REFRESHUSERINFO);
    }

    private void RefreshUserItem(byte[] bytes)
    {
        PushUserItem package = NetMgr.Instance.DeSerializes<PushUserItem>(bytes);
        foreach (UserItem item in package.userItems)
        {
            if (!this.userItems.ContainsKey(item.type))
            {
                this.userItems.Add(item.type, item);
            }
            else
            {
                this.userItems[item.type] = item;
            }
        }
        //SendNotification(NotificationConstant.MEDI_HALL_REFRESHITEM);
    }
}