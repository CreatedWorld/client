using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 大厅底部界面
/// </summary>
public class BottomMenuView : UIView
{
	/// <summary>
	/// 商城按钮
	/// </summary>
	public Button shopButton;
	/// <summary>
	/// 分享按钮
	/// </summary>
	public Button shareButton;
	/// <summary>
	/// 战绩按钮
	/// </summary>
	public Button militaryExploitsButton;
	/// <summary>
	/// 公告按钮
	/// </summary>
	public Button newsButton;
	/// <summary>
	/// 设置按钮
	/// </summary>
	public Button MenuBtn;
    /// <summary>
    /// 菜单的子项Panel
    /// </summary>
    public GameObject PanelObj;
    /// <summary>
    /// 问号按钮
    /// </summary>
    public Button RuleBtn;
    /// <summary>
    /// 声音设置按钮
    /// </summary>
    public Button SettingBtn;
    /// <summary>
    /// 退出按钮
    /// </summary>
    public Button ExitBtn;

	public override void OnInit ()
	{
        Transform HallViewTransform = UIManager.Instance.GetUIView(UIViewID.HALL_VIEW).viewRoot.transform;
        viewRoot = this.LaunchUIView ("Prefab/UI/Hall/BottomMenuView", HallViewTransform);
		shopButton = this.viewRoot.transform.FindChild ("Buttons").FindChild ("ShopButton").GetComponent<Button> ();
		shareButton = this.viewRoot.transform.FindChild ("Buttons").FindChild ("ShareButton").GetComponent<Button> ();
		militaryExploitsButton = this.viewRoot.transform.FindChild ("Buttons").FindChild ("MilitaryExploitsButton").GetComponent<Button> ();
		newsButton = this.viewRoot.transform.Find ("Buttons/NewsButton").GetComponent<Button> ();
		MenuBtn = this.viewRoot.transform.FindChild ("Buttons").FindChild ("MenuButton").GetComponent<Button> ();
        PanelObj = viewRoot.transform.FindChild("Buttons/MenuButton/Panel").gameObject;
        RuleBtn = viewRoot.transform.FindChild("Buttons/MenuButton/Panel/RuleBtn/Image").GetComponent<Button>();
        SettingBtn = viewRoot.transform.FindChild("Buttons/MenuButton/Panel/SettingBtn/Image").GetComponent<Button>();
        ExitBtn = viewRoot.transform.FindChild("Buttons/MenuButton/Panel/ExitBtn/Image").GetComponent<Button>();

        //UIManager.Instance.BringToBottom(UIViewID.BOTTOMMENU_VIEW);

        MenuBtn.onClick.AddListener(()=> {
            if (!PanelObj.activeSelf)  PanelObj.SetActive(true);
        });

        ExitBtn.onClick.AddListener(()=> {
            UIManager.Instance.Background.color = new Color(1, 1, 1, 0);
            UIManager.Instance.Background.gameObject.SetActive(true);
            UIManager.Instance.Background.DOColor(new Color(1, 1, 1, 1), 0.5f).SetEase(Ease.Linear).OnComplete(
                () =>
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        PlayerPrefs.DeleteKey(PrefsKey.USERMAC);
                        PlayerPrefs.DeleteKey(PrefsKey.USERNAME);
                        PlayerPrefs.DeleteKey(PrefsKey.HEADURL);
                        PlayerPrefs.DeleteKey(PrefsKey.SEX);
                    }
                    var loadInfo = new LoadSceneInfo(ESceneID.SCENE_LOGIN, LoadSceneType.SYNC, LoadSceneMode.Single);
                    ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
                });
        }
        );
    }

	public override ESceneID UISceneID {
		get {
			return ESceneID.SCENE_HALL;
		}

		set {
			base.UISceneID = value;
		}
	}

	public override void LoadUI ()
	{
		this.viewRootCache = Resources.Load<GameObject> ("Prefab/UI/Hall/BottomMenuView");
	}
}
