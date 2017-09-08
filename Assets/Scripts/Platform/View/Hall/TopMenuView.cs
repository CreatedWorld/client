using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 顶部按钮
/// </summary>
public class TopMenuView : UIView
{
    /// <summary>
    /// 头像
    /// </summary>
    public RawImage photoIcon;
    /// <summary>
    /// 用户名
    /// </summary>
    public Text usernameTxt;
    /// <summary>
    /// 用户id
    /// </summary>
    public Text userIdTxt;
    /// <summary>
    /// 房卡
    /// </summary>
    public Text roomCardText;
    /// <summary>
    /// 头像按钮
    /// </summary>
    public Button photoButton;
    /// <summary>
    /// 购买按钮
    /// </summary>
    public Button roomCardButton;
    /// <summary>
    /// 签到按钮
    /// </summary>
    public Button signinButton;
    /// <summary>
    /// 帮助按钮
    /// </summary>
    public Button helpButton;
    /// <summary>
    /// 返回登陆界面按钮
    /// </summary>
    public Button backButton;

    public override void OnInit()
    {
        viewRoot = LaunchUIView("Prefab/UI/Hall/TopMenuView", UIManager.Instance.GetUIView(UIViewID.HALL_VIEW).viewRoot.transform);
        photoIcon = viewRoot.transform.Find("UserInfo/PhotoMask/Photo").GetComponent<RawImage>();
        photoButton = viewRoot.transform.Find("UserInfo/PhotoMask/Photo").GetComponent<Button>();
        usernameTxt = viewRoot.transform.Find("UserInfo/NameText").GetComponent<Text>();
        userIdTxt = viewRoot.transform.Find("UserInfo/IDText").GetComponent<Text>();
        roomCardText = viewRoot.transform.Find("UserInfo/RoomCardText").GetComponent<Text>();
        roomCardButton = viewRoot.transform.Find("UserInfo/RoomCardButton").GetComponent<Button>();
        signinButton = viewRoot.transform.Find("Buttons/SigninButton").GetComponent<Button>();
        helpButton = viewRoot.transform.Find("Buttons/HelpButton").GetComponent<Button>();
        backButton = viewRoot.transform.Find("Buttons/BackButton").GetComponent<Button>();

    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_HALL;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Hall/TopMenuView");
    }
}
