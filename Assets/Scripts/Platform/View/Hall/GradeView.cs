using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 战绩查询
/// </summary>
public class GradeView : UIView
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;
    /// <summary>
    /// 战绩信息滚动条
    /// </summary>
    private GameObject recordScrollView;
    /// <summary>
    /// 房间信息滚动条
    /// </summary>
    private GameObject particularsScrollView;
    /// <summary>
    /// 战绩信息滚动条,控制器组件
    /// </summary>
    private TableView gradeTableView;
    /// <summary>
    /// 房间信息滚动条,控制器组件
    /// </summary>
    private TableView particularsTableView;
    /// <summary>
    /// 生成战绩信息滚动条信息集合
    /// </summary>
    private List<GradeTableItem> gradeScrollList;
    /// <summary>
    /// 生成房间信息滚动条信息集合
    /// </summary>
    private List<ParticularsTableItem> particularsScrollList;
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

    public GameObject RecordScrollView
    {
        get
        {
            return recordScrollView;
        }

        set
        {
            recordScrollView = value;
        }
    }

    public GameObject ParticularsScrollView
    {
        get
        {
            return particularsScrollView;
        }

        set
        {
            particularsScrollView = value;
        }
    }

    public TableView GradeTableView
    {
        get
        {
            return gradeTableView;
        }

        set
        {
            gradeTableView = value;
        }
    }

    public TableView ParticularsTableView
    {
        get
        {
            return particularsTableView;
        }

        set
        {
            particularsTableView = value;
        }
    }


    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Grade/GradeView");
        this.RecordScrollView = this.viewRoot.transform.FindChild("Grade").FindChild("RecordScrollView").gameObject;
        this.ParticularsScrollView = this.viewRoot.transform.FindChild("Grade").FindChild("ParticularsScrollView").gameObject;
        this.GradeTableView = this.viewRoot.transform.Find("Grade/RecordScrollView").GetComponent<TableView>();
        this.ParticularsTableView = this.viewRoot.transform.Find("Grade/ParticularsScrollView").GetComponent<TableView>();
        ApplicationFacade.Instance.RegisterMediator(new GradeMediator(Mediators.HALL_GRADE, this));
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
        this.RecordScrollView.SetActive(true);
        this.ParticularsScrollView.SetActive(false);
        UIManager.Instance.ShowUIMask(UIViewID.GRADE_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_GETGRADEINFO);
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), 
            ()=> 
            {
                base.OnHide();
                this.RecordScrollView.SetActive(true);
                this.ParticularsScrollView.SetActive(false);
            });
    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Grade/GradeView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_GRADE);
    }
}
