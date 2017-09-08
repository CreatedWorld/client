using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 帮助View
/// </summary>
public class HelpView : UIView
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;
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

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Help/HelpView");
        this.CloseButton = this.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        ApplicationFacade.Instance.RegisterMediator(new HelpMediator(Mediators.HALL_HELP, this));
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
        UIManager.Instance.ShowUIMask(UIViewID.HELP_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), base.OnHide);

    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Help/HelpView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_HELP);
    }
}