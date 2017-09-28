using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// UI管理器
/// </summary>
public class UIManager
{
	/// <summary>
	/// 蒙板关联的UI数组
	/// </summary>
	private List<UIViewID> maskBindUIList;
	/// <summary>
	/// UI管理器单例
	/// </summary>
	private static UIManager instance;

	/// <summary>
	/// UI集合父物体
	/// </summary>
	private readonly GameObject uiRoot;
	/// <summary>
	/// 弹出UI集合父物体
	/// </summary>
	private readonly GameObject popUpUIRoot;
	/// <summary>
	/// UI蒙板
	/// </summary>
	private readonly GameObject mask;

	/// <summary>
	/// UIView集合,字典形式存储
	/// </summary>
	private Dictionary<UIViewID, UIView> viewDic;
	/// <summary>
	/// 等待标志
	/// </summary>
	private readonly GameObject waitIcon;
	/// <summary>
	/// 旋转标志
	/// </summary>
	private readonly GameObject rotateIcon;
	private readonly Image background;
	/// <summary>
	/// loading进度条
	/// </summary>
	private GameObject loading;
	/// <summary>
	/// loading前景
	/// </summary>
	private Image loadingFg;

	/// <summary>
	/// 构造函数,初始化UIRoot
	/// </summary>
	private UIManager ()
	{
		uiRoot = GameObject.Find ("UIRoot");
		popUpUIRoot = GameObject.Find ("UIRoot/PopUpUI");
		mask = GameObject.Find ("UIRoot/PopUpUI/Mask");
		waitIcon = uiRoot.transform.FindChild ("WaitIcon").gameObject;
		rotateIcon = uiRoot.transform.FindChild ("WaitIcon/Icon").gameObject;
		background = uiRoot.transform.FindChild ("Backgournd").GetComponent<Image> ();
		loading = uiRoot.transform.FindChild ("Loading").gameObject;
		loadingFg = uiRoot.transform.FindChild ("Loading/LoadingFG/Image").GetComponent<Image> ();
		viewDic = new Dictionary<UIViewID, UIView> ();
		maskBindUIList = new List<UIViewID> ();
	}

	public static UIManager Instance {
		get {
			if (instance == null)
				instance = new UIManager ();
			return instance;
		}
	}

	public GameObject UiRoot {
		get { return uiRoot; }
	}

	public GameObject PopUpUIRoot {
		get { return popUpUIRoot; }
	}

	public Image Background {
		get {
			return background;
		}
	}

	/// <summary>
	/// UI加载进度
	/// </summary>
	private float _loadUIPercent = 0;
	/// <summary>
	/// 场景加载进度
	/// </summary>
	private float _loadScenePercent = 0;

	/// <summary>
	/// UI加载进度
	/// </summary>
	public float loadUIPercent {
		set {
			_loadUIPercent = value;
			UpdateLoadingPercent (0.5f * (_loadUIPercent + _loadScenePercent));
		}
	}

	/// <summary>
	/// 场景加载进度
	/// </summary>
	public float loadScenePercent {
		set {
			_loadScenePercent = value;
			UpdateLoadingPercent (0.5f * (_loadUIPercent + _loadScenePercent));
		}
	}

	/// <summary>
	/// 更新loading显示
	/// </summary>
	public void UpdateLoadignActive (bool isActive)
	{
		loadUIPercent = 0;
		loadScenePercent = 0;
		loading.SetActive (isActive);
	}

	/// <summary>
	/// 更新加载进度
	/// </summary>
	/// <param name="percent"></param>
	private void UpdateLoadingPercent (float percent)
	{
		loadingFg.fillAmount = percent;
		if (percent >= 1) {
			loading.SetActive (false);
			_loadUIPercent = 0;
			_loadScenePercent = 0;
		}
	}

	/// <summary>
	/// 初始化所有UI
	/// </summary>
	public void InitUI (ESceneID sceneId)
	{
		maskBindUIList.Clear ();
		foreach (var dic in viewDic) {
			var view = dic.Value;
			view.isInit = false;
			view.isShow = false;
			view.OnDestroy ();
			if (view.viewRoot != null)
				Object.Destroy (view.viewRoot);
			view.viewRoot = null;
		}
		if (mask != null) {
			mask.SetActive (false);
		}
		if (viewDic.Count == 0) {
			viewDic = new Dictionary<UIViewID, UIView> ();
			RegisterUI (UIViewID.LOGIN_VIEW, new LoginView ());
			RegisterUI (UIViewID.HALL_VIEW, new HallView ());
			RegisterUI (UIViewID.TOPMENU_VIEW, new TopMenuView ());
			RegisterUI (UIViewID.BATTLE_VIEW, new BattleView ());
			RegisterUI (UIViewID.DIALOG_VIEW, new DialogView ());
			RegisterUI (UIViewID.MATCH_RESULT_VIEW, new MatchResultView ());
			RegisterUI (UIViewID.ROOM_RESULT_VIEW, new RoomResultView ());
			RegisterUI (UIViewID.BOTTOMMENU_VIEW, new BottomMenuView ());
			RegisterUI (UIViewID.MIDDLEMENU_VIEW, new MiddleMenuView ());
			RegisterUI (UIViewID.PLATER_INFO_VIEW, new PlayerInfoView ());
			RegisterUI (UIViewID.SHOPPING_VIEW, new ShoppingView ());
			RegisterUI (UIViewID.SIGNIN_VIEW, new SignInView ());
			RegisterUI (UIViewID.HELP_VIEW, new HelpView ());
			RegisterUI (UIViewID.INVITE_VIEW, new InviteView ());
			RegisterUI (UIViewID.SHARE_VIEW, new ShareView ());
			RegisterUI (UIViewID.GRADE_VIEW, new GradeView ());
			RegisterUI (UIViewID.ACTIVITY_VIEW, new ActivityView ());
			RegisterUI (UIViewID.SETTING_VIEW, new SettingView ());
			RegisterUI (UIViewID.ARENA_VIEW, new ArenaView ());
			RegisterUI (UIViewID.CREATEROOM_VIEW, new CreateRoomView ());
			RegisterUI (UIViewID.JOINROOM_VIEW, new JoinRoomView ());
			RegisterUI (UIViewID.CHAT_VIEW, new ChatView ());
			RegisterUI (UIViewID.DISLOVE_APPLY_VIEW, new DisloveApplyView ());
			RegisterUI (UIViewID.DISLOVE_STATISTICS_VIEW, new DisloveStatisticsView ());
			RegisterUI (UIViewID.MATCHING_VIEW, new MatchingView ());
			RegisterUI (UIViewID.RANK_VIEW, new RankView ());
			RegisterUI (UIViewID.HALLROOMLIST_VIEW, new HallRoomListView ());
			RegisterUI (UIViewID.SHOPPINGTIPS_VIEW, new ShoppingtipsView ());
			RegisterUI (UIViewID.NEWS_VIEW, new NewsView ());
            RegisterUI(UIViewID.RULE_VIEW,new RuleView());
			GameMgr.Instance.StartCoroutine (LoadUI (sceneId));
		} else {
			loadUIPercent = 1;
		}
	}

	/// <summary>
	/// 加载UI
	/// </summary>
	/// <returns></returns>
	private IEnumerator LoadUI (ESceneID sceneId)
	{
		foreach (KeyValuePair<UIViewID, UIView> keyValue in viewDic) {
			loadUIPercent = _loadUIPercent + ((float)1) / viewDic.Count;
			//if (keyValue.Value.UISceneID == sceneId || keyValue.Value.UISceneID == ESceneID.SCENE_START)
			//{
			keyValue.Value.LoadUI ();
			yield return new WaitForEndOfFrame ();
			//}
		}
		loadUIPercent = 1;
	}

	public void Update ()
	{
		foreach (var dic in viewDic) {
			var view = dic.Value;
			if (view.isShow) {
				view.Update ();
			}
		}
	}

	public void FixedUpdate ()
	{
		foreach (var dic in viewDic) {
			var view = dic.Value;
			if (view.isShow) {
				view.FixedUpdate ();
			}
		}
	}

	/// <summary>
	/// 显示UIView
	/// </summary>
	/// <param name="viewID">UIView的ID</param>
	public UIView ShowUI (UIViewID viewID)
	{
		if (viewDic.ContainsKey (viewID)) {
			var view = viewDic [viewID];
			if (view.viewRootCache == null) {
				view.LoadUI ();
			}
			if (!view.isInit) {
				view.OnInit ();
				view.isInit = true;
			}
			if (!view.isShow) {
				view.OnShow ();
				view.isShow = true;
			}
			BringToTop (viewID);
			return view;
		}
		Debug.LogError ("无效ViewID:" + viewID);
		return null;
	}

	/// <summary>
	/// 设置UI到顶层
	/// </summary>
	public void BringToTop (UIViewID viewID,int i =1)
	{
		if (viewDic.ContainsKey (viewID)) {
			var view = viewDic [viewID];
			if (!view.isInit) {
				return;
			}
			var nextUI = GetUIView (viewID);
			nextUI.viewRoot.transform.SetSiblingIndex (nextUI.viewRoot.transform.parent.childCount - i);
		}                
	}

	/// <summary>
	/// 设置UI到底层
	/// </summary>
	/// <param name="viewID"></param>
	public void BringToBottom (UIViewID viewID ,int i =0)
	{
		if (viewDic.ContainsKey (viewID)) {
			var view = viewDic [viewID];
			if (!view.isInit) {
				return;
			}
			var nextUI = GetUIView (viewID);
			nextUI.viewRoot.transform.SetSiblingIndex (i);
		}
	}

	/// <summary>
	/// 隐藏UIView
	/// </summary>
	/// <param name="viewID">UIView的ID</param>
	public void HideUI (UIViewID viewID)
	{
		if (viewDic.ContainsKey (viewID)) {
			var view = viewDic [viewID];
			if (view.isShow) {
				view.OnHide ();
				view.isShow = false;
			}
		} else {
			Debug.LogError ("无效ViewID:" + viewID);
		}
		maskBindUIList.Remove (viewID);
		if (maskBindUIList.Count == 0) {
			HidenUIMask ();
		} else {
			ResortMaskLayer (maskBindUIList [maskBindUIList.Count - 1]);
		}
	}

	/// <summary>
	/// 包含回调的隐藏
	/// </summary>
	/// <param name="viewID"></param>
	/// <param name="action"></param>
	public void HideUI (UIViewID viewID, Action action)
	{
		if (viewDic.ContainsKey (viewID)) {
			var view = viewDic [viewID];
			if (view.isShow) {
				view.OnHide (action);
				view.isShow = false;
			} else {
				Debug.Log ("View已关闭:" + viewID);
			}
		} else {
			Debug.LogError ("无效ViewID:" + viewID);
		}
		maskBindUIList.Remove (viewID);
		if (maskBindUIList.Count == 0) {
			HidenUIMask ();
		} else {
			ResortMaskLayer (maskBindUIList [maskBindUIList.Count - 1]);
		}
	}

	/// <summary>
	/// 删除UIView
	/// </summary>
	/// <param name="viewID">UIView的ID</param>
	public void DestroyUI (UIViewID viewID)
	{
		if (viewDic.ContainsKey (viewID)) {
			var view = viewDic [viewID];
			view.OnDestroy ();
			view.isInit = false;
			view.isShow = false;
		} else {
			Debug.LogError ("无效ViewID:" + viewID);
		}
		maskBindUIList.Remove (viewID);
		if (maskBindUIList.Count == 0) {
			HidenUIMask ();
		} else {
			ResortMaskLayer (maskBindUIList [maskBindUIList.Count - 1]);
		}
	}

	/// <summary>
	/// 注册UIView
	/// </summary>
	/// <param name="viewID">UIView的ID</param>
	/// <param name="uiView">UIView对象</param>
	public void RegisterUI (UIViewID viewID, UIView uiView)
	{
		if (uiView != null)
		if (!viewDic.ContainsKey (viewID)) {
			viewDic.Add (viewID, uiView);
		} else {
			Debug.LogError ("该UI:" + viewID + "已存在,请勿重复注册");
		}
		else
			Debug.LogError ("注册失败,无效ViewID:" + viewID);
	}

	/// <summary>
	/// 移除UIView注册
	/// </summary>
	/// <param name="viewID">UIView的ID</param>
	public void RemoveUI (UIViewID viewID)
	{
		if (viewDic.ContainsKey (viewID)) {
			var view = viewDic [viewID];
			view.OnRemove ();
			viewDic.Remove (viewID);
			view = null;
		} else {
			Debug.LogError ("无效ViewID:" + viewID);
		}
	}

	/// <summary>
	/// 获取UIView
	/// </summary>
	/// <param name="viewID">UIView的ID</param>
	/// <returns></returns>
	public UIView GetUIView (UIViewID viewID)
	{
		if (viewDic.ContainsKey (viewID)) {
			return viewDic [viewID];
		}
		Debug.LogError ("无效ViewID:" + viewID);
		throw new NullReferenceException ();
	}

	/// <summary>
	/// 创建UIView,UIRoot下
	/// </summary>
	/// <param PlayerName="path">路径</param>
	/// <returns></returns>
	public GameObject CreateUIView (string path)
	{
		return CreateUIView (path, popUpUIRoot.transform);
	}

	/// <summary>
	/// 创建UIView
	/// </summary>
	/// <param PlayerName="path">路径</param>
	/// <param PlayerName="parent">父物体</param>
	/// <returns></returns>
	public GameObject CreateUIView (string path, Transform parent)
	{
		var uiView = Object.Instantiate (Resources.Load<GameObject> (path), parent);
		UIViewResetTrans (uiView.transform);
		return uiView;
	}

	public GameObject CreateUIView (GameObject viewCache)
	{
		return CreateUIView (viewCache, popUpUIRoot.transform);
	}

	public GameObject CreateUIView (GameObject viewCache, Transform parent)
	{
		var uiView = Object.Instantiate (viewCache, parent);
		UIViewResetTrans (uiView.transform);
		return uiView;
	}

	/// <summary>
	/// 重新设置UI的变换
	/// </summary>
	/// <param name="trans">UIView物体</param>
	private void UIViewResetTrans (Transform trans)
	{
		trans.localPosition = Vector3.zero;
		trans.rotation = new Quaternion (0, 0, 0, 0);
		trans.localScale = Vector3.one;
	}

	/// <summary>
	/// 播放显示缓动
	/// </summary>
	/// <param name="rectTransform"></param>
	public void ShowDOTween (RectTransform rectTransform)
	{
		rectTransform.DOKill ();
		rectTransform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		rectTransform.DOScale (1f, 0.2f).SetEase (Ease.Linear);
		rectTransform.DOScale (1.1f, 0.125f).SetDelay (0.2f).SetEase (Ease.Linear);
		rectTransform.DOScale (0.9f, 0.125f).SetDelay (0.316f).SetEase (Ease.Linear);
		rectTransform.DOScale (1f, 0.125f).SetDelay (0.441f).SetEase (Ease.Linear);
	}

	/// <summary>
	/// 播放隐藏缓动
	/// </summary>
	/// <param name="rectTransform"></param>
	public void HidenDOTween (RectTransform rectTransform, TweenCallback hidenCallback)
	{
		rectTransform.DOKill ();
		rectTransform.localScale = Vector3.one;
		rectTransform.DOScale (1.1f, 0.2f).SetEase (Ease.Linear);
		rectTransform.DOScale (0f, 0.15f).SetDelay (0.2f).SetEase (Ease.Linear).OnComplete (hidenCallback);
	}

	/// <summary>
	/// 显示界面蒙板
	/// </summary>
	/// <param name="uiObj">蒙板关联的UIId</param>
	public void ShowUIMask (UIViewID uiid)
	{
		mask.SetActive (true);
		mask.GetComponent<Image> ().DOKill ();
		mask.GetComponent<Image> ().color = Color.clear;
		mask.GetComponent<Image> ().DOColor (new Color (0, 0, 0, 0.7f), 0.3f);
		ResortMaskLayer (uiid);
		if (!maskBindUIList.Contains (uiid)) {
			maskBindUIList.Add (uiid);
		}
	}

	/// <summary>
	/// 重排蒙板层次
	/// </summary>
	/// <param name="bindUIId"></param>
	private void ResortMaskLayer (UIViewID bindUIId)
	{
		var uiView = GetUIView (bindUIId);
		var childArr = new List<Transform> ();
		var resultArr = new List<Transform> ();
		foreach (Transform transform in popUpUIRoot.transform) {
			if (transform.gameObject != mask) {
				childArr.Add (transform);
			}
		}
		foreach (Transform transform in childArr) {
			if (transform != uiView.viewRoot.transform) {
				resultArr.Add (transform);
			} else {
				resultArr.Add (mask.transform);
				resultArr.Add (uiView.viewRoot.transform);
			}
		}
		for (int i = 0; i < resultArr.Count; i++) {
			resultArr [i].SetSiblingIndex (i);
		}
	}

	/// <summary>
	/// 缓动隐藏蒙板
	/// </summary>
	private void HidenUIMask ()
	{
		if (mask && mask.activeSelf) {
			mask.GetComponent<Image> ().DOKill ();
			mask.GetComponent<Image> ().DOColor (new Color (0, 0, 0, 0f), 0.5f).OnComplete (() => {
				mask.SetActive (false);
			});
		}        
	}

	/// <summary>
	/// 是否需要截屏
	/// </summary>
	public bool needSaveScreen = false;
	/// <summary>
	/// 截屏回调
	/// </summary>
	private Action<Texture2D> saveCallBack;

	/// <summary>
	/// 开始截屏
	/// </summary>
	/// <param name="callBack">截屏回调</param>
	public void StartSaveScreen (Action<Texture2D> callBack)
	{
		saveCallBack = callBack;
		needSaveScreen = true;
	}

	/// <summary>
	/// 获取截屏内容
	/// </summary>
	/// <returns></returns>
	public void SaveScreenTexture ()
	{
		// 先创建一个的空纹理，大小可根据实现需要来设置  
		Texture2D screenShot = new Texture2D (Screen.width, Screen.height, TextureFormat.ARGB32, false);

		// 读取屏幕像素信息并存储为纹理数据，  
		screenShot.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0);
		screenShot.Apply ();
		needSaveScreen = false;
		saveCallBack (screenShot);
	}

	/// <summary>
	/// 设置等待标志显示
	/// </summary>
	/// <param name="value"></param>
	public void SetWaitIconActive (bool value)
	{
		if (GlobalData.LoginServer == "127.0.0.1") {
			return;
		}
		if (value) {
			if (!waitIcon.activeSelf) {
				waitIcon.SetActive (value);
				waitIcon.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
				rotateIcon.SetActive (false);
				waitIcon.GetComponent<Image> ().DOColor (new Color (0, 0, 0, 0.4f), 0.1f).SetDelay (2f).OnComplete (() => {
					rotateIcon.SetActive (true);
				});
			}            
		} else {
			if (waitIcon.activeSelf) {
				waitIcon.GetComponent<Image> ().DOKill ();
				waitIcon.SetActive (value);
			}
		}        
	}
}
