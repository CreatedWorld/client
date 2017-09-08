using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 输入邀请码View
/// </summary>
public class InviteView : UIView
{
    /// <summary>
    /// 确定按钮
    /// </summary>
    private Button confirmButton;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;
    /// <summary>
    /// 输入框
    /// </summary>
    private InputField inputField;

    public Button ConfirmButton
    {
        get
        {
            return confirmButton;
        }

        set
        {
            confirmButton = value;
        }
    }

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

    public InputField InputField
    {
        get
        {
            return inputField;
        }

        set
        {
            inputField = value;
        }
    }

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Invite/InviteView");
        this.ConfirmButton = this.viewRoot.transform.FindChild("ConfirmButton").GetComponent<Button>();
        this.CloseButton = this.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        this.InputField = this.viewRoot.transform.FindChild("Input").FindChild("InputField").GetComponent<InputField>();
        ApplicationFacade.Instance.RegisterMediator(new InviteMediator(Mediators.HALL_INVITE, this));
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
        UIManager.Instance.ShowUIMask(UIViewID.INVITE_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(),base.OnHide);
    }
    public override void OnHide(Action callBack)
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), 
            ()=> 
            {
                base.OnHide();
                callBack();
            });
    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Invite/InviteView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_INVITE);
    }
}