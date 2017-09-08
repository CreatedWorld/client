using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI基类
/// </summary>
public abstract class UIView : IUIView
{
    public delegate void UIViewAction(params object[] parameter);
    /// <summary>
    /// UI对应的预设
    /// </summary>
    public GameObject viewRootCache;
    /// <summary>
    /// UI对应的实例化对象
    /// </summary>
    public GameObject viewRoot;
    /// <summary>
    /// UI是否初始化
    /// </summary>
    public bool isInit;
    /// <summary>
    /// UI是否显示
    /// </summary>
    public bool isShow;
    public UIView()
    {
        this.viewRootCache = null;
    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {
        
    }
    /// <summary>
    /// UI所属场景
    /// </summary>
    public virtual ESceneID UISceneID
    {
        get;
        set;
    }
    public abstract void OnInit();
    public abstract void LoadUI();
    public virtual void OnShow()
    {
        this.viewRoot.SetActive(true);
    }
    public virtual void OnHide()
    {
        this.viewRoot.SetActive(false);
    }
    public virtual void OnHide(Action callBack)
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(),
        () =>
        {
            this.viewRoot.SetActive(false);
            callBack();
        });
    }
    public virtual void OnDestroy()
    {
        if (this.viewRoot != null)
        {
            GameObject.Destroy(this.viewRoot);
            this.viewRoot = null;
        }
        //if (this.viewRootCache != null)
        //{
        //    this.viewRootCache = null;
        //}
    }
    public virtual void OnRemove()
    {
        if (this.viewRoot != null)
        {
            GameObject.Destroy(this.viewRoot);
            this.viewRoot = null;
        }
        //if (this.viewRootCache != null)
        //{
        //    this.viewRootCache = null;
        //}
    }
    public GameObject LaunchUIView(string path, Transform parent = null)
    {
        GameObject viewRoot = null;
        if (this.viewRootCache == null)
        {
            if (parent == null)
            {
                viewRoot = UIManager.Instance.CreateUIView(path);
            }
            else
            {
                viewRoot = UIManager.Instance.CreateUIView(path, parent);
            }
        }
        else
        {
            if (parent == null)
            {
                viewRoot = UIManager.Instance.CreateUIView(this.viewRootCache);
            }
            else
            {
                viewRoot = UIManager.Instance.CreateUIView(this.viewRootCache, parent);
            }
        }
        return viewRoot;
    }
    public void ButtonAddListening(Button button, UnityAction action,bool isScale = false)
    {
        button.onClick.AddListener(action);
        //if (isScale)
        //{
        //    this.ButtonAddScaleResult(button);
        //}
    }
    public void ButtonAddListening(Button button, UIViewAction callBack, bool isScale,params object[] parameter)
    {
        button.onClick.AddListener(()=> { callBack(parameter);});
        //if (isScale)
        //{
        //    this.ButtonAddScaleResult(button);
        //}
    }

    private void ButtonAddScaleResult(Button button)
    {
        EventTrigger buttonTrigger = button.GetComponent<EventTrigger>();
        buttonTrigger.triggers = new List<EventTrigger.Entry>();

        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener(
            (BaseEventData arg0) => 
            {
                button.transform.localScale = new Vector3(1.15f,1.15f,1.15f);
            });

        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener(
            (BaseEventData arg0) =>
            {
                button.transform.localScale = new Vector3(1f, 1f, 1f);
            });
        buttonTrigger.triggers.Add(pointerUp);
        buttonTrigger.triggers.Add(pointerDown);
    }
}
