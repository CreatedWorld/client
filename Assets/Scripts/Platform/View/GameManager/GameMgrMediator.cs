using System.Collections.Generic;
using Platform.Utils;
using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine.SceneManagement;
using Utils;
using Platform.Model;
using Platform.Net;
using System.Collections;
using DG.Tweening;
using System;
/// <summary>
/// 管理者模块数据模型
/// </summary>
public class GameMgrMediator  : Mediator, IMediator
{
    /// <summary>
    /// 管理模块数据实体代理
    /// </summary>
    private GameMgrProxy gameManagerProxy;
    /// <summary>
    /// 异步加载刷新定时器id
    /// </summary>
    private int updateTimerId;
    public GameMgrMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {
    }

    /// <summary>
    /// 启动模块总管理代理
    /// </summary>
    public GameMgr View
    {
        get
        {
            return (GameMgr)ViewComponent;
        }
    }
    /// <summary>
    /// 启动模块消息注册
    /// </summary>
    /// <returns></returns>
    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_GAMEMGR_LOADSCENE);
        list.Add(NotificationConstant.MEDI_GAMEMGR_CONNECTSERVER);
        return list;
    }

    /// <summary>
    /// 启动模块初始化
    /// </summary>
    public override void OnRegister()
    {
        base.OnRegister();
        this.gameManagerProxy = Facade.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
    }

    /// <summary>
    /// 启动模块响应消息
    /// </summary>
    /// <param PlayerName="notification"></param>
    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_GAMEMGR_LOADSCENE:
                SwitchScene((LoadSceneInfo)notification.Body);
                break;
            case NotificationConstant.MEDI_GAMEMGR_CONNECTSERVER:
                GetUserInfoC2S userInfoPackage = new GetUserInfoC2S();
                GetCheckInInfoC2S userSignInPackage = new GetCheckInInfoC2S();
                ReConnectC2S reConnectC2S = new ReConnectC2S();
                NetMgr.Instance.SendBuff<GetCheckInInfoC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_GET_CHECKIN_INFO.GetHashCode(), 0, userSignInPackage);
                NetMgr.Instance.SendBuff<GetUserInfoC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_GET_USERINFO.GetHashCode(), 0, userInfoPackage);
                NetMgr.Instance.SendBuff<ReConnectC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_Reconnect.GetHashCode(), 0, reConnectC2S);
                
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 每帧回调
    /// </summary>
    public void Update()
    {
        if (gameManagerProxy != null && gameManagerProxy.async != null)//加载完成
        {
            UIManager.Instance.loadScenePercent = gameManagerProxy.async.progress;
            if (gameManagerProxy.async.isDone)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                gameManagerProxy.async = null;
                if (SceneManager.GetActiveScene().name == SceneName.BATTLE || SceneManager.GetActiveScene().name == SceneName.LOGIN)
                {
                    ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_REFRESHHALLNOTICE);
                }
                Timer.Instance.CancelTimer(updateTimerId);
                Resources.UnloadUnusedAssets();
                GC.Collect();
                UIManager.Instance.loadScenePercent = 1;
            }
        }
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param PlayerName="sceneName"></param>
    private void SwitchScene(LoadSceneInfo info)
    {
        Timer.Instance.CancelAllTimer();
        GameMgr.Instance.StopAllCoroutines();
        ResourcesMgr.Instance.ClearPool();
        AudioSystem.Instance.StopAllAudio();
        NetMgr.Instance.waitMsgList.Clear();
        DOTween.KillAll(true);
        switch (info.loadType)
        {
            case (LoadSceneType.SYNC):
                LoadSceneSync(info);
                break;
            case (LoadSceneType.ASYNC):
                GameMgr.Instance.StartCoroutine(LoadSceneAsync(info));
                break;
            default:
                Debug.LogError("错误信息: SceneName:" + info.sceneID.ToString() + " Type:" + info.loadType.ToString() + " Model:" + info.mode.ToString());
                break;
        }
        
    }

    /// <summary>
    /// 直接加载场景
    /// </summary>
    /// <param name="info">场景信息</param>
    public void LoadSceneSync(LoadSceneInfo info)
    {
        SceneManager.LoadScene((int)info.sceneID, info.mode);
        //UIManager.Instance.UpdateLoadignActive(true);
        //UIManager.Instance.loadScenePercent = 1;
        UIManager.Instance.InitUI(info.sceneID);
        //Resources.UnloadUnusedAssets();
        //GC.Collect();
    }
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="info">场景信息</param>
    /// <returns></returns>
    public IEnumerator LoadSceneAsync(LoadSceneInfo info)
    {
        yield return new WaitForEndOfFrame();
        UIManager.Instance.UpdateLoadignActive(true);
        UIManager.Instance.InitUI(info.sceneID);
        updateTimerId = Timer.Instance.AddTimer(0, 0, 0, Update);
        gameManagerProxy.async = SceneManager.LoadSceneAsync((int)info.sceneID, info.mode);
    }
}
