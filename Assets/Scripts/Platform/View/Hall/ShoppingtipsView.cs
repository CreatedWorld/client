/**
 *Copyright(C) 2017 by Alan   
 *All rights reserved.             
 *Author:       PC-20170617HLDC           
 *Version:      1.0          
 *UnityVersion：5.5.0f3     
 *Date:         2017-08-18 16:19:26             
 *Description:                     
 *History:    
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using PureMVC.Core;

public class ShoppingtipsView : UIView
{
	private Text wxID;
	private Button closeButton;

    /// <summary>
	/// 大厅数据
	/// </summary>
	private HallProxy hallProxy;
    public override void OnInit ()
	{
        hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        this.viewRoot = this.LaunchUIView ("Prefab/UI/Shopping/shoppingtips");
		wxID = viewRoot.transform.Find ("Text").GetComponent<Text> ();
		closeButton = viewRoot.transform.Find ("close").GetComponent<Button> ();

		closeButton.onClick.AddListener (() => {
			UIManager.Instance.HideUI (UIViewID.SHOPPINGTIPS_VIEW);
		});
	}

	public override void OnShow ()
	{
		base.OnShow ();
		UIManager.Instance.ShowUIMask (UIViewID.SHOPPINGTIPS_VIEW);
		UIManager.Instance.ShowDOTween (this.viewRoot.GetComponent<RectTransform> ());

        wxID.text = hallProxy.HallInfo.noticeList[HallNoticeType.HALL_DIAMONDS].content;

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
		this.viewRootCache = Resources.Load<GameObject> ("Prefab/UI/Shopping/shoppingtips");
	}

}