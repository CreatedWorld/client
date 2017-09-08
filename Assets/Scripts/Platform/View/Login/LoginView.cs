using System;
using UnityEngine;
using UnityEngine.UI;


public class LoginView : UIView
{
    /// <summary>
    /// 登录输入区域
    /// </summary>
    public GameObject loginInputArea;
    /// <summary>
    /// 登录ip
    /// </summary>
    public InputField serverTxt;
    /// <summary>
    /// 登录端口
    /// </summary>
    public InputField portTxt;
    /// <summary>
    /// 用户id
    /// </summary>
    public InputField macTxt;
    /// <summary>
    /// 登录名称
    /// </summary>
    public InputField nameTxt;
    /// <summary>
    /// 登录密码
    /// </summary>
    public InputField pwdTxt;
    /// <summary>
    /// 登录中介
    /// </summary>
    private LoginMgrMediator mediator;
    /// <summary>
    /// 登录按钮
    /// </summary>
    public Button loginBtn;
    /// <summary>
    /// 切换
    /// </summary>
    public Toggle toggle;
    /// <summary>
    /// UI动画
    /// </summary>
    public Animator loginAnimator;

    private Button localBtn;
    private Button Btn143;
    private Button Btn8;
    private Button ExtranetBtn;

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Login/LoginView");
        this.loginBtn = this.viewRoot.transform.FindChild("LoginButton").GetComponent<Button>();
        localBtn = this.viewRoot.transform.FindChild("Button").GetComponent<Button>();
        Btn143 = this.viewRoot.transform.FindChild("Button1").GetComponent<Button>();
        Btn8 = this.viewRoot.transform.FindChild("Button2").GetComponent<Button>();
        ExtranetBtn = viewRoot.transform.FindChild("Button3").GetComponent<Button>();
        this.toggle = this.viewRoot.transform.FindChild("Toggle").GetComponent<Toggle>();
        this.loginAnimator = this.viewRoot.GetComponent<Animator>();
        loginInputArea = viewRoot.transform.FindChild("LoginInputArea").gameObject;
        if (GlobalData.sdkPlatform == SDKPlatform.ANDROID || GlobalData.sdkPlatform == SDKPlatform.IOS)
        {
            loginInputArea.transform.localPosition = new Vector3(9999, 0, 0);
        }
        serverTxt = viewRoot.transform.FindChild("LoginInputArea/ServerValueTxt").GetComponent<InputField>();
        portTxt = viewRoot.transform.FindChild("LoginInputArea/PortValueTxt").GetComponent<InputField>();
        macTxt = viewRoot.transform.FindChild("LoginInputArea/MacValueTxt").GetComponent<InputField>();
        nameTxt = viewRoot.transform.FindChild("LoginInputArea/UserNameValueTxt").GetComponent<InputField>();
        pwdTxt = viewRoot.transform.FindChild("LoginInputArea/PwdValueTxt").GetComponent<InputField>();
        serverTxt.text = GlobalData.LoginServer;
        portTxt.text = GlobalData.LoginPort.ToString();
        nameTxt.text = GlobalData.UserName;
        macTxt.text = GlobalData.UserMac;
        pwdTxt.text = GlobalData.UserPwd;
        if (GlobalData.UserMac == "PhoneMac")
        {
            nameTxt.text = GetRandAccount();
            macTxt.text = nameTxt.text;
        }
        localBtn.onClick.AddListener(()=> { serverTxt.text = "127.0.0.1"; portTxt.text = "8149"; macTxt.text = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss"); });
        Btn143.onClick.AddListener(() => { serverTxt.text = "192.168.4.143"; portTxt.text = "8149"; macTxt.text = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss"); });
        Btn8.onClick.AddListener(() => { serverTxt.text = "192.168.4.8"; portTxt.text = "8149"; macTxt.text = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss"); });
        ExtranetBtn.onClick.AddListener(() => { serverTxt.text = "47.95.34.172"; portTxt.text = "8009"; macTxt.text = DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss"); });
    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_LOGIN;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    string GetRandAccount()
    {
        int curtv = (int)Mathf.Floor((float)(DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds / 1000));
        int ranval = UnityEngine.Random.Range(10, 99);
        string randacc = String.Format("{0:X}", curtv) + String.Format("{0:X}", ranval);
        return randacc;
    }

    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Login/LoginView");
    }

    public override void Update()
    {
        if (mediator == null)
        {
            mediator = ApplicationFacade.Instance.RetrieveMediator(Mediators.LOGIN_MGR_MEDIATOR) as LoginMgrMediator;
        }
        mediator.Update();
    }
}
