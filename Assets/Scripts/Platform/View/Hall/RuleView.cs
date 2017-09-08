
using System;
/**
*Copyright(C) 2017 by Alan   
*All rights reserved.             
*Author:       PC-20170617HLDC           
*Version:      1.0          
*UnityVersion：5.5.0f3     
*Date:         2017-08-30 15:27:48             
*Description:                     
*History:    
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleView : UIView
{

    private Button closeBtn;
    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.RULE_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Rule/RuleView");
        closeBtn = viewRoot.transform.Find("closeBtn").GetComponent<Button>();

        closeBtn.onClick.AddListener(() => {
            UIManager.Instance.HideUI(UIViewID.RULE_VIEW);
        });
    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Rule/RuleView");
    }
}