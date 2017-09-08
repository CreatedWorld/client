using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsView : UIView
{
	private Text text;
	private Button closeButton;
    /// <summary>
	/// ´óÌüÊý¾Ý
	/// </summary>
	private HallProxy hallProxy;
    public override void OnInit ()
	{
        hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        this.viewRoot = this.LaunchUIView ("Prefab/UI/Shopping/shoppingtips");
		text = viewRoot.transform.Find ("Scroll View/Viewport/Content").GetComponent<Text> ();
		closeButton = viewRoot.transform.Find ("close").GetComponent<Button> ();

		closeButton.onClick.AddListener (() => {
			UIManager.Instance.HideUI (UIViewID.NEWS_VIEW);
		});
	}

	public override void OnShow ()
	{
		base.OnShow ();
		UIManager.Instance.ShowUIMask (UIViewID.NEWS_VIEW);
		UIManager.Instance.ShowDOTween (this.viewRoot.GetComponent<RectTransform> ());

        text.text = hallProxy.HallInfo.noticeList[HallNoticeType.HALL_NEWS].content;
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
		this.viewRootCache = Resources.Load<GameObject> ("Prefab/UI/News/NewsView");
	}

}
