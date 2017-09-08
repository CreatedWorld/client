using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 商场View
/// </summary>
public class ShoppingView : UIView
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;
    /// <summary>
    /// 商场滚动条控制组件
    /// </summary>
    private TableView tableView;
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

    public TableView TableView
    {
        get
        {
            return tableView;
        }

        set
        {
            tableView = value;
        }
    }

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Hall/ShoppingView");
        this.CloseButton = this.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        this.TableView = this.viewRoot.transform.FindChild("Commodity/ScrollView").GetComponent<TableView>();
        ApplicationFacade.Instance.RegisterMediator(new ShoppingMediator(Mediators.HALL_SHOPPING, this));
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
        UIManager.Instance.ShowUIMask(UIViewID.SHOPPING_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), base.OnHide);
    }

    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Hall/ShoppingView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_SHOPPING);
    }
}
