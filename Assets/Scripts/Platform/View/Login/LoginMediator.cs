using System.Collections.Generic;
using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using Platform.Net;
using Platform.Model;
using UnityEngine.SceneManagement;
using Utils;
using Platform.Utils;
using System;
using Object = UnityEngine.Object;
using System.Collections;
using System.Text;

public class LoginMgrMediator : Mediator, IMediator
{
    private LoginProxy loginProxy;
    private LoginView loginView;
    public LoginMgrMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {
    }
    
    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_LOGIN_SWITCHHALLSCENE);
        list.Add(NotificationConstant.MEDI_LOGIN_WXLOGINSUCCEED);
        return list;
    }


    public override void OnRegister()
    {
        base.OnRegister();
        this.loginProxy = Facade.RetrieveProxy(Proxys.LOGIN_PROXY) as LoginProxy;
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            //Debug.Log(commandLineArgs);
            for (int i = 0; i < commandLineArgs.Length; i++)
            {
                Debug.Log(commandLineArgs[i]);
            }
            if (commandLineArgs.Length > 1)
            {
                GlobalData.LoginServer = commandLineArgs[1];
                GlobalData.LoginPort = int.Parse(commandLineArgs[2]);
                GlobalData.UserName = commandLineArgs[3];
                GlobalData.UserMac = commandLineArgs[4];
                loginProxy.autoLogin = true;
                CheckUserAgreement();
                return;
            }
        }
        if (loginProxy.autoLogin)
        {
            CheckUserAgreement();
        }
        else
        {
            loginView = (LoginView)UIManager.Instance.ShowUI(UIViewID.LOGIN_VIEW);
            loginView.ButtonAddListening(loginView.loginBtn, CheckUserAgreement);
            AudioSystem.Instance.PlayBgm("Voices/Bgm/91bgmusic");
        }
        //GameMgr.Instance.StartCoroutine(DownloadAssetBundle());

    }

    private IEnumerator DownloadAssetBundle()
    {
        WWW mainFestWWW = new WWW("http://localhost/assetbundle/StreamingAssets");
        yield return mainFestWWW;
        AssetBundleManifest manifest = mainFestWWW.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string [] depends = manifest.GetAllDependencies("ShareView.prefab");
        Hash128 hash;
        for (int index = 0; index < depends.Length; index++)
        {
            hash = manifest.GetAssetBundleHash(depends[index]);
            WWW dependWWW = WWW.LoadFromCacheOrDownload("http://localhost/assetbundle/" + depends[index], hash);
            yield return dependWWW;
        }
        hash = manifest.GetAssetBundleHash("ShareView.prefab");
        WWW shareViewWWW = WWW.LoadFromCacheOrDownload("http://localhost/assetbundle/shareview.prefab", hash);
        yield return shareViewWWW;
        AssetBundle pngAssetBundle = shareViewWWW.assetBundle;
        GameObject perfab = pngAssetBundle.LoadAsset<GameObject>("Assets/Resources/ShareView.prefab");
        GameObject.Instantiate(perfab);
        pngAssetBundle.Unload(false);
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_LOGIN_SWITCHHALLSCENE:
                if (loginView != null)
                {
                    loginView.loginAnimator.SetBool("LoginSucceed", true);
                }
                var loadInfo = new LoadSceneInfo(ESceneID.SCENE_HALL, LoadSceneType.ASYNC, LoadSceneMode.Additive);
                ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
                break;
            case NotificationConstant.MEDI_LOGIN_WXLOGINSUCCEED:
                this.SendLoginSucceed();
                break;
            default:
                break;
        }
    }
    private void CheckUserAgreement()
    {
        if (loginProxy.autoLogin)
        {
            this.LoginServerConnent();
            return;
        }
        switch (GlobalData.sdkPlatform)
        {
            case SDKPlatform.ANDROID:

                if (loginView.toggle.isOn)
                {
                    if (PlayerPrefs.HasKey(PrefsKey.USERMAC))
                    {
                        this.SendLoginSucceed();
                    }
                    else
                    {
                        AndroidSdkInterface.SendWeiXinLogin();
                    }
                }
                else
                {
                    LoginFailDialog();
                }
                break;
            case SDKPlatform.IOS:

                if (loginView.toggle.isOn)
                {
                    if (PlayerPrefs.HasKey(PrefsKey.USERMAC))
                    {
                        this.SendLoginSucceed();
                    }
                    else
                    {
                        IOSSdkInterface.weChatLogin();
                    }
                }
                else
                {
                    LoginFailDialog();
                }
                break;
            case SDKPlatform.LOCAL:
                if (loginView.toggle.isOn)
                {
                    SendLoginSucceed();
                }
                else
                {
                    LoginFailDialog();
                }
                break;
        }
    }

    public void Update()
    {
        //if (Input.GetKeyDown(KeyCode.KeypadEnter))
        //{
        //    CheckUserAgreement();
        //}
    }

    /// <summary>
    /// 登录服务器连接
    /// </summary>
    private void LoginServerConnent()
    {
        Debug.Log("登录服务器连接");
        if (loginProxy.autoLogin)
        {
            NetMgr.Instance.StopTcpConnection(SocketType.LOGIN);
            NetMgr.Instance.CreateConnect(SocketType.LOGIN, GlobalData.LoginServer, GlobalData.LoginPort, LoginConnectHandler);
            return;
        }

        switch (GlobalData.sdkPlatform)
        {
            case SDKPlatform.LOCAL:
                GlobalData.UserName = loginView.nameTxt.text;
                GlobalData.UserMac = loginView.macTxt.text;
                PlayerPrefs.SetString(PrefsKey.USERMAC, GlobalData.UserMac);
                PlayerPrefs.SetString(PrefsKey.USERNAME, GlobalData.UserName);
                break;
            case SDKPlatform.ANDROID:
                GlobalData.UserMac = PlayerPrefs.GetString(PrefsKey.USERMAC);
                GlobalData.UserName = PlayerPrefs.GetString(PrefsKey.USERNAME);
                break;
            case SDKPlatform.IOS:
                GlobalData.UserMac = PlayerPrefs.GetString(PrefsKey.USERMAC);
                GlobalData.UserName = PlayerPrefs.GetString(PrefsKey.USERNAME);
                break;
            default:
                break;
        }
        GlobalData.UserPwd = loginView.pwdTxt.text;
        GlobalData.LoginServer = loginView.serverTxt.text;
        GlobalData.LoginPort = int.Parse(loginView.portTxt.text);
        PlayerPrefs.SetString(PrefsKey.SERVERIP, GlobalData.LoginServer);
        PlayerPrefs.SetInt(PrefsKey.SERVERPORT, GlobalData.LoginPort);
        NetMgr.Instance.StopTcpConnection(SocketType.LOGIN);
        NetMgr.Instance.CreateConnect(SocketType.LOGIN, GlobalData.LoginServer, GlobalData.LoginPort, LoginConnectHandler);
    }

    /// <summary>
    /// 连接成功回调
    /// </summary>
    private void LoginConnectHandler()
    {
        NetMgr.Instance.ConnentionDic[SocketType.LOGIN].OnConnectSuccessful = null;
        Debug.Log("登陆服务器连接成功:" + NetMgr.Instance.ConnentionDic[SocketType.LOGIN].ServerAddress);
        Timer.Instance.AddDeltaTimer(0, 1, 0, () => { this.SendLoginRequest(); });
        
    }

    /// <summary>
    /// 发送登陆请求
    /// </summary>
    private void SendLoginRequest()
    {
        LoginC2S package = new LoginC2S();
        switch (GlobalData.sdkPlatform)
        {
            case SDKPlatform.ANDROID:
                package.mac = PlayerPrefs.GetString(PrefsKey.USERMAC);
                package.name = PlayerPrefs.GetString(PrefsKey.USERNAME);
                package.imageUrl = PlayerPrefs.GetString(PrefsKey.HEADURL);
                package.sex = PlayerPrefs.GetInt(PrefsKey.SEX);
                break;
            case SDKPlatform.IOS:
                package.mac = PlayerPrefs.GetString(PrefsKey.USERMAC);
                package.name = PlayerPrefs.GetString(PrefsKey.USERNAME);
                package.imageUrl = PlayerPrefs.GetString(PrefsKey.HEADURL);
                package.sex = PlayerPrefs.GetInt(PrefsKey.SEX);
                break;
            case SDKPlatform.LOCAL:
                package.mac = GlobalData.UserMac;
                package.name = GlobalData.UserName;
                package.imageUrl = GlobalData.ImageUrl;
                package.sex = 1;
                break;
        }
        package.psw = GlobalData.UserPwd;

        string[] str = Application.version.Split('.');
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            sb.Append(str[i]);
        }
        package.version = int.Parse(sb.ToString());
        NetMgr.Instance.SendBuff<LoginC2S>(SocketType.LOGIN, MsgNoC2S.C2S_LOGIN.GetHashCode(), 0, package);
        Debug.Log("发送登陆请求");
        loginProxy.autoLogin = false;
    }

    private void SendLoginSucceed()
    {
        if (loginView.serverTxt.text == "127.0.0.1")
        {
            GlobalData.LoginServer = loginView.serverTxt.text;
            GlobalData.LoginPort = int.Parse(loginView.portTxt.text);
            PlayerPrefs.SetString(PrefsKey.SERVERIP, GlobalData.LoginServer);
            PlayerPrefs.SetInt(PrefsKey.SERVERPORT, GlobalData.LoginPort);
            SendNotification(NotificationConstant.MEDI_LOGIN_SWITCHHALLSCENE);
        }
        else
        {
            this.LoginServerConnent();
        }
    }

    private void LoginFailDialog()
    {
        DialogMsgVO dialogVO = new DialogMsgVO();
        dialogVO.dialogType = DialogType.ALERT;
        dialogVO.title = "登录失败";
        dialogVO.content = "请先阅读用户协议";
        DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
        dialogView.data = dialogVO;
    }
}
