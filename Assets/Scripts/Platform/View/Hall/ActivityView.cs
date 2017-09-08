using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 活动模块View
/// </summary>
public class ActivityView : UIView
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;
    /// <summary>
    /// 公告信息
    /// </summary>
    private ActivContnet information;
    private ActivContnet contact;
    private ActivContnet announcement;
    private ActivContnet generalize;

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

    public ActivContnet Information
    {
        get
        {
            return information;
        }

        set
        {
            information = value;
        }
    }

    public ActivContnet Contact
    {
        get
        {
            return contact;
        }

        set
        {
            contact = value;
        }
    }

    public ActivContnet Announcement
    {
        get
        {
            return announcement;
        }

        set
        {
            announcement = value;
        }
    }

    public ActivContnet Generalize
    {
        get
        {
            return generalize;
        }

        set
        {
            generalize = value;
        }
    }

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Activity/ActivityView");
        this.CloseButton = this.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        this.Information = new ActivContnet(this.viewRoot.transform.FindChild("Information").gameObject, HallNoticeType.MENU_INFORMATION);
        this.Contact = new ActivContnet(this.viewRoot.transform.FindChild("Contact").gameObject, HallNoticeType.MENU_CONTACT);
        this.Announcement = new ActivContnet(this.viewRoot.transform.FindChild("Announcement").gameObject, HallNoticeType.MENU_ANNOUNCEMENT);
        this.Generalize = new ActivContnet(this.viewRoot.transform.FindChild("Generalize").gameObject, HallNoticeType.MENU_GENERALIZE);
        ApplicationFacade.Instance.RegisterMediator(new ActivityMediator(Mediators.HALL_ACTIVITY, this));
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
        UIManager.Instance.ShowUIMask(UIViewID.ACTIVITY_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(),base.OnHide);
        
    }

    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Activity/ActivityView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_ACTIVITY);
    }
}