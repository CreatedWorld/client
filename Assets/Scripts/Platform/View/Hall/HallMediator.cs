using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;
using Platform.Net;
using Platform.Model;
using Platform.Model.Battle;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Net;
using Platform.Global;
using Platform.Utils;

/// <summary>
/// 大厅中介
/// </summary>
public class HallMediator : Mediator, IMediator
{
	/// <summary>
	/// 大厅数据
	/// </summary>
	private HallProxy hallProxy;
	/// <summary>
	/// 登录中介
	/// </summary>
	private LoginProxy loginProxy;

	public HallMediator (string mediatorName, object viewComponent) : base (mediatorName, viewComponent)
	{
	}

	public HallMgr View {
		get {
			return (HallMgr)ViewComponent;
		}
	}

	public override void OnRegister ()
	{
		base.OnRegister ();
		hallProxy = ApplicationFacade.Instance.RetrieveProxy (Proxys.HALL_PROXY) as HallProxy;
		loginProxy = ApplicationFacade.Instance.RetrieveProxy (Proxys.LOGIN_PROXY) as LoginProxy;
		View.HallView = (HallView)UIManager.Instance.ShowUI (UIViewID.HALL_VIEW);
		View.TopView = (TopMenuView)UIManager.Instance.ShowUI (UIViewID.TOPMENU_VIEW);
		View.HallRoomListView = (HallRoomListView)UIManager.Instance.ShowUI (UIViewID.HALLROOMLIST_VIEW);
		View.MiddleView = (MiddleMenuView)UIManager.Instance.ShowUI (UIViewID.MIDDLEMENU_VIEW);
		View.BottomView = (BottomMenuView)UIManager.Instance.ShowUI (UIViewID.BOTTOMMENU_VIEW);
		TopMenuAddEvent ();
		MiddleMenuAddEvent ();
		BottomMenuAddEvent ();
		RefreshUserInfo ();
		AudioSystem.Instance.PlayBgm ("Voices/Bgm/91bgmusic");
		if (GlobalData.LoginServer != "127.0.0.1" && NetMgr.Instance.ConnentionDic.ContainsKey (SocketType.BATTLE)) {
			NetMgr.Instance.StopTcpConnection (SocketType.BATTLE);
			if (!NetMgr.Instance.ConnentionDic.ContainsKey (SocketType.HALL)) {
				NetMgr.Instance.CreateConnect (SocketType.HALL, loginProxy.hallServerIP, loginProxy.hallServerPort);
			}
		}
		StartUp ();
	}

    public override void OnRemove()
    {
        base.OnRemove ();
    }

    public override IList<string> ListNotificationInterests ()
	{
		IList<string> list = new List<string> ();
		list.Add (NotificationConstant.MEDI_HALL_REFRESHUSERINFO);
		list.Add (NotificationConstant.MEDI_HALL_REFRESHITEM);
		list.Add (NotificationConstant.MEDI_HALL_REFRESHANNOUNCEMENT);
		list.Add (NotificationConstant.MEDI_HALL_ANNOUNCEMENTFINISH);
		list.Add (NotificationConstant.MEDI_HALL_REFRESHHALLNOTICE);
		list.Add (NotificationConstant.MEDI_RECIVE_STARTUP);
		list.Add (NotificationConstant.MEDI_HALL_ROOMLIST);
		return list;
	}

	public override void HandleNotification (INotification notification)
	{
		switch (notification.Name) {
		case (NotificationConstant.MEDI_HALL_REFRESHUSERINFO):
			RefreshUserInfo ();
			break;
		case (NotificationConstant.MEDI_HALL_REFRESHITEM):
			RefreshItem ();
			break;
		case (NotificationConstant.MEDI_HALL_REFRESHANNOUNCEMENT):
			RefreshAnnouncement ();
			break;
		case (NotificationConstant.MEDI_HALL_ANNOUNCEMENTFINISH):
			AnnouncementFinish ();
		break;
		case NotificationConstant.MEDI_HALL_REFRESHHALLNOTICE:
			RefreshNotice ();
			break;
		case NotificationConstant.MEDI_RECIVE_STARTUP:
			StartUp ();
			break;
		case NotificationConstant.MEDI_HALL_ROOMLIST:
                //StartUp();
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// 绑定顶部UI按钮事件
	/// </summary>
	private void TopMenuAddEvent ()
	{
		View.TopView.ButtonAddListening (View.TopView.photoButton,
			() => {
				if (GlobalData.LoginServer != "127.0.0.1") {
					PlayerInfoProxy playerInfoProxy = Facade.RetrieveProxy (Proxys.PLAYER_PROXY) as PlayerInfoProxy;
					var getPlayerInfoC2S = new GetUserInfoByIdC2S ();
					getPlayerInfoC2S.userId = playerInfoProxy.userID;
					NetMgr.Instance.SendBuff (SocketType.HALL, MsgNoC2S.C2S_Hall_Get_UserInfo_By_Id.GetHashCode (), 0, getPlayerInfoC2S);
				} else {
					UIManager.Instance.ShowUI (UIViewID.PLATER_INFO_VIEW);
				}
			});
		View.TopView.ButtonAddListening (View.TopView.roomCardButton,
			() => {
				UIManager.Instance.ShowUI (UIViewID.SHOPPINGTIPS_VIEW);
				//PlayerInfoProxy pip = Facade.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
				//if (pip.boundAgency == ErrorCode.SUCCESS)
				//{
				//    UIManager.Instance.ShowUI(UIViewID.SHOPPING_VIEW);
				//}
				//else if (pip.boundAgency == ErrorCode.FAILT)
				//{
				//    UIManager.Instance.ShowUI(UIViewID.INVITE_VIEW);
				//}
			});
		View.TopView.ButtonAddListening (View.TopView.signinButton,
			() => {
				UIManager.Instance.ShowUI (UIViewID.SIGNIN_VIEW);
			});
		View.TopView.ButtonAddListening (View.TopView.helpButton,
			() => {
				UIManager.Instance.ShowUI (UIViewID.HELP_VIEW);
			});
		View.TopView.ButtonAddListening (View.TopView.backButton,
			() => {
				UIManager.Instance.Background.color = new Color (1, 1, 1, 0);
				UIManager.Instance.Background.gameObject.SetActive (true);
				UIManager.Instance.Background.DOColor (new Color (1, 1, 1, 1), 0.5f).SetEase (Ease.Linear).OnComplete (
					() => {
						var loadInfo = new LoadSceneInfo (ESceneID.SCENE_LOGIN, LoadSceneType.SYNC, LoadSceneMode.Single);
						ApplicationFacade.Instance.SendNotification (NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
					});
			});
	}

	/// <summary>
	/// 绑定中部UI按钮事件
	/// </summary>
	private void MiddleMenuAddEvent ()
	{
		View.MiddleView.ButtonAddListening (View.MiddleView.createRoomButton, () => {
			UIManager.Instance.ShowUI (UIViewID.CREATEROOM_VIEW);
		}, true);
		View.MiddleView.ButtonAddListening (View.MiddleView.joinRoomButton, () => {
			UIManager.Instance.ShowUI (UIViewID.JOINROOM_VIEW);
		});

		View.MiddleView.ButtonAddListening (View.MiddleView.athleticsButton, () => {
			CheckApplyStatusC2S package = new CheckApplyStatusC2S ();
			NetMgr.Instance.SendBuff<CheckApplyStatusC2S> (SocketType.HALL, MsgNoC2S.C2S_Hall_CHECK_APPLY_STATUS.GetHashCode (), 0, package);
		});
	}

	/// <summary>
	/// 绑定底部UI按钮事件
	/// </summary>
	private void BottomMenuAddEvent ()
	{
		View.BottomView.ButtonAddListening (View.BottomView.shopButton,
			() => {
				PlayerInfoProxy pip = Facade.RetrieveProxy (Proxys.PLAYER_PROXY) as PlayerInfoProxy;
				if (pip.boundAgency == ErrorCode.SUCCESS) {
					UIManager.Instance.ShowUI (UIViewID.SHOPPING_VIEW);
				} else if (pip.boundAgency == ErrorCode.FAILT) {
					UIManager.Instance.ShowUI (UIViewID.INVITE_VIEW);
				}
			});

		View.BottomView.ButtonAddListening (View.BottomView.shareButton,
			() => {
				UIManager.Instance.ShowUI (UIViewID.SHARE_VIEW);
			});

		View.BottomView.ButtonAddListening (View.BottomView.militaryExploitsButton,
			() => {
				UIManager.Instance.ShowUI (UIViewID.GRADE_VIEW);
				if (GlobalData.LoginServer == "127.0.0.1") {
					ClientAIMgr.Instance.PlayReport ();
				}
			});

		View.BottomView.ButtonAddListening (View.BottomView.newsButton,
			() => {
				UIManager.Instance.ShowUI (UIViewID.NEWS_VIEW);
			});
        View.BottomView.ButtonAddListening(View.BottomView.RuleBtn,
            () => {
                UIManager.Instance.ShowUI(UIViewID.RULE_VIEW);
            });

        View.BottomView.ButtonAddListening (View.BottomView.SettingBtn,
			() => {
				UIManager.Instance.ShowUI (UIViewID.SETTING_VIEW);
			});
	}

	/// <summary>
	/// 刷新用户大厅界面
	/// </summary>
	private void RefreshUserInfo ()
	{
		PlayerInfoProxy playerInfoProxy = Facade.RetrieveProxy (Proxys.PLAYER_PROXY) as PlayerInfoProxy;
		View.TopView.usernameTxt.text = playerInfoProxy.userName;
        View.TopView.userIdTxt.text ="ID:"+ playerInfoProxy.showID;
        GameMgr.Instance.StartCoroutine (DownIcon (playerInfoProxy.headIconUrl));
		if (playerInfoProxy.userItems.ContainsKey (ItemType.DIAMONDS)) {
			View.TopView.roomCardText.text = playerInfoProxy.userItems [ItemType.DIAMONDS].amount.ToString ();
		} else {
			View.TopView.roomCardText.text = "0";
		}
	}

	/// <summary>
	/// 下载回调
	/// </summary>
	/// <param name="headUrl"></param>
	/// <returns></returns>
	IEnumerator DownIcon (string headUrl)
	{
		WWW www = new WWW (headUrl);
		yield return www;
		if (www.error == null) {
			PlayerInfoProxy playerProxy = Facade.RetrieveProxy (Proxys.PLAYER_PROXY) as PlayerInfoProxy;
			playerProxy.headIcon = www.texture;
			View.TopView.photoIcon.texture = www.texture;
		}
	}

	/// <summary>
	/// 刷新玩家货币
	/// </summary>
	private void RefreshItem ()
	{
		PlayerInfoProxy playerInfoProxy = Facade.RetrieveProxy (Proxys.PLAYER_PROXY) as PlayerInfoProxy;
		if (playerInfoProxy.userItems.ContainsKey (ItemType.DIAMONDS)) {
			View.TopView.roomCardText.text = playerInfoProxy.userItems [ItemType.DIAMONDS].amount.ToString ();
		} else {
			View.TopView.roomCardText.text = "0";
		}
	}

	/// <summary>
	/// 刷新喇叭
	/// </summary>
	private void RefreshAnnouncement ()
	{
		if (View.MiddleView.announcementText == null || View.MiddleView.announcementText.text == "") {
			if (hallProxy.HallInfo.announcementQueue.Count > 0) {
				AnnouncementData data = hallProxy.HallInfo.announcementQueue.Peek ();
				View.MiddleView.announcementText.text = data.Content;
				View.MiddleView.announcementText.rectTransform.localPosition = new Vector3 (520, View.MiddleView.announcementText.rectTransform.localPosition.y, 0);
			}
		}
	}

	/// <summary>
	/// 喇叭播放完毕回调
	/// </summary>
	private void AnnouncementFinish ()
	{
		View.MiddleView.announcementText.text = "";
		if (hallProxy.HallInfo.announcementQueue.Count > 0) {
			AnnouncementData data = hallProxy.HallInfo.announcementQueue.Peek ();
			if (data.ReduceCirCount () > 0) {
				hallProxy.HallInfo.announcementQueue.Enqueue (data);
			}
			SendNotification (NotificationConstant.MEDI_HALL_REFRESHANNOUNCEMENT);
		}
	}

	/// <summary>
	/// 下载大厅图片
	/// </summary>
	/// <param name="noticeUrl"></param>
	/// <returns></returns>
	IEnumerator DownNoticeIcon (string noticeUrl)
	{
		WWW www = new WWW (noticeUrl);
		yield return www;
		if (www.error == null) {
			View.MiddleView.noticContent.gameObject.SetActive (true);
			View.MiddleView.noticContent.texture = www.texture;
			if (View.MiddleView.notice.gameObject.activeSelf) {
				View.MiddleView.notice.gameObject.SetActive (false);
			}
		}
	}

	/// <summary>
	/// 刷新大厅公告
	/// </summary>
	private void RefreshNotice ()
	{
		if (hallProxy.HallInfo.noticeList.ContainsKey (HallNoticeType.HALL_CONTENT)) {
			GameMgr.Instance.StartCoroutine (DownNoticeIcon (hallProxy.HallInfo.noticeList [HallNoticeType.HALL_CONTENT].content));
		}
		if (hallProxy.HallInfo.noticeList.ContainsKey (HallNoticeType.HALL_NOTICE)) {
			View.MiddleView.noticeTitle.text = hallProxy.HallInfo.noticeList [HallNoticeType.HALL_NOTICE].title;
			View.MiddleView.noticeText.text = hallProxy.HallInfo.noticeList [HallNoticeType.HALL_NOTICE].content;
			View.MiddleView.notice.gameObject.SetActive (true);

		}
        RefreshAnnouncement();

    }

	/// <summary>
	/// 设置启动
	/// </summary>
	private void StartUp ()
	{
		if (!Application.isMobilePlatform) {
			return;
		}
		if (GlobalData.isStartUp) {
			if (GlobalData.sdkPlatform == SDKPlatform.ANDROID) {
				GlobalData.StartUpParam = AndroidSdkInterface.GetStartParam ();
			}
			GlobalData.isStartUp = false;
		}
		Dictionary<string, string> paramDic = StringUtil.ParseParam (GlobalData.StartUpParam);
		if (paramDic.ContainsKey (StartUpParam.ROOMID)) {
			HallProxy hallProxy = ApplicationFacade.Instance.RetrieveProxy (Proxys.HALL_PROXY) as HallProxy;
			hallProxy.HallInfo.roomCode = paramDic [StartUpParam.ROOMID];
			JoinInRoomC2S package = new JoinInRoomC2S ();
			package.roomCode = hallProxy.HallInfo.roomCode;
			package.seat = 0;
			NetMgr.Instance.SendBuff<JoinInRoomC2S> (SocketType.HALL, MsgNoC2S.C2S_Hall_JOIN_IN_ROOM.GetHashCode (), 0, package);
		}
		GlobalData.StartUpParam = null;
	}
}