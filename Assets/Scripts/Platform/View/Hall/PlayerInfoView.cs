using UnityEngine;
using UnityEngine.UI;
using Platform.Model;
/// <summary>
/// 用户信息按钮
/// </summary>
public class PlayerInfoView : UIView
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;
    private RawImage headIcon;
    /// <summary>
    /// 用户显示ID
    /// </summary>
    private Text userID;
    /// <summary>
    /// 用户名
    /// </summary>
    private Text usernameText;
    /// <summary>
    /// 房卡
    /// </summary>
    private Text cardText;
    /// <summary>
    /// IP地址
    /// </summary>
    private Text ipText;

    private GetUserInfoByIdS2C _data;
    /// <summary>
    /// 界面中介
    /// </summary>
    private PlayerInfoMediator mediator;
    public Button CloseButton
    {
        get
        {
            return closeButton;
        }

        set
        {
            closeButton = value;
        }
    }

    public Text UsernameText
    {
        get
        {
            return usernameText;
        }

        set
        {
            usernameText = value;
        }
    }

    public Text CardText
    {
        get
        {
            return cardText;
        }

        set
        {
            cardText = value;
        }
    }

    public Text UserID
    {
        get
        {
            return userID;
        }

        set
        {
            userID = value;
        }
    }

    public Text IpText
    {
        get
        {
            return ipText;
        }

        set
        {
            ipText = value;
        }
    }

    public RawImage HeadIcon
    {
        get
        {
            return headIcon;
        }

        set
        {
            headIcon = value;
        }
    }

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/PlayerInfo/PlayerInfoView");
        this.CloseButton = this.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        this.UsernameText = this.viewRoot.transform.FindChild("Name").Find("Text").GetComponent<Text>();
        this.CardText = this.viewRoot.transform.FindChild("Card").Find("CardText").GetComponent<Text>();
        this.UserID = this.viewRoot.transform.FindChild("UserID").Find("IDText").GetComponent<Text>();
        this.IpText = this.viewRoot.transform.FindChild("IP").Find("IPText").GetComponent<Text>();
        this.HeadIcon = this.viewRoot.transform.Find("PhotoMask/Photo").GetComponent<RawImage>();
        mediator = new PlayerInfoMediator(Mediators.HALL_PLAYERINFO,this);
        ApplicationFacade.Instance.RegisterMediator(mediator);
    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_START;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.PLATER_INFO_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), base.OnHide);

    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/PlayerInfo/PlayerInfoView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_PLAYERINFO);
    }

    /// <summary>
    /// 界面玩家数据
    /// </summary>
    public GetUserInfoByIdS2C data
    {
        set
        {
            _data = value;
            mediator.RefreshPlayerInfo();
        }
        get
        {
            return _data;
        }
    }
}