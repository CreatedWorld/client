using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 分享View
/// </summary>
public class ShareView : UIView
{
    private Button closeButton;
    private Button friendButton;
    private Button communityButton;
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

    public Button FriendButton
    {
        get
        {
            return friendButton;
        }

        set
        {
            friendButton = value;
        }
    }

    public Button CommunityButton
    {
        get
        {
            return communityButton;
        }

        set
        {
            communityButton = value;
        }
    }

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Share/ShareView");
        this.CloseButton = this.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        this.FriendButton = this.viewRoot.transform.FindChild("FriendButton").GetComponent<Button>();
        this.CommunityButton = this.viewRoot.transform.FindChild("CommunityButton").GetComponent<Button>();
        ApplicationFacade.Instance.RegisterMediator(new ShareMediator(Mediators.HALL_SHARE, this));
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

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.SHARE_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), base.OnHide);

    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Share/ShareView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_SHARE);
    }
}
