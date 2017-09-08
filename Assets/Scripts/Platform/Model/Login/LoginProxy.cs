using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using Platform.Net;
using Platform.Model;
using LZR.Data.NetWork.Client;
/// <summary>
/// 登录数据代理
/// </summary>
public class LoginProxy : Proxy, IProxy
{
    /// <summary>
    /// 大厅服务器ip
    /// </summary>
    public string hallServerIP;
    /// <summary>
    /// 大厅服务器端口
    /// </summary>
    public int hallServerPort;
    /// <summary>
    /// 是否自动登录
    /// </summary>
    public bool autoLogin;
    public LoginProxy(string NAME) : base(NAME)
    {
        GameMgr.Instance.AddMsgHandler(MsgNoS2C.S2C_LOGIN, LoginResponse);
    }
    private void LoginResponse(byte[] bytes)
    {
        Debug.Log("收到登陆响应");
        LoginS2C package = NetMgr.Instance.DeSerializes<LoginS2C>(bytes);
        int loginStatus = package.status;
        if (loginStatus == 1)
        {
            Debug.Log("验证成功");
            int userID = package.userId;
            hallServerIP = package.serverIp;
            hallServerPort = package.port;
            ((GameMgrProxy)Facade.RetrieveProxy(Proxys.GAMEMGR_PROXY)).systemTime = package.time;
            PlayerInfoProxy pip = Facade.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            pip.userID = userID;
            NSocket.UpdateUserID(userID);
            NetMgr.Instance.StopTcpConnection(SocketType.LOGIN);
            NetMgr.Instance.CreateConnect(SocketType.HALL, hallServerIP, hallServerPort, HallConnectHandler);
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
        NetMgr.Instance.ConnentionDic[SocketType.HALL].OnConnectSuccessful = null;
        Debug.Log("请求大厅控制器,执行登陆");
        SendNotification(NotificationConstant.MEDI_GAMEMGR_CONNECTSERVER);
    }
}
