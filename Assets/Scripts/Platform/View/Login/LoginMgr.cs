using UnityEngine;
using Platform.Net;
using LZR.Data.NetWork.Client;

public class LoginMgr : MonoBehaviour
{
    void Awake()
    {
        NetMgr.Instance.StopAllTcpConnection();
        Debug.Log("断开所有与服务器连接");
        NSocket.UpdateUserID(0);
        NSocket.UpdateRoomID(0);
        ApplicationFacade.Instance.RegisterMediator(new LoginMgrMediator(Mediators.LOGIN_MGR_MEDIATOR, this));
        if (Application.isMobilePlatform)
        {
            ApplicationFacade.Instance.SendNotification(NotificationConstant.COMM_CHECK_VERSION);
        }
    }

    void OnDestroy()
    {
        ApplicationFacade.Instance.RemoveMediator(Mediators.LOGIN_MGR_MEDIATOR);
    }
}