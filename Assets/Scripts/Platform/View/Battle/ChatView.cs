using Platform.Model.Battle;
using Platform.View.Battle;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 聊天输入界面
/// </summary>
public class ChatView : UIView
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button closeBtn;
    /// <summary>
    /// 聊天文本列表
    /// </summary>
    public TableView chatList;
    /// <summary>
    /// 发送输入框
    /// </summary>
    public InputField sendInput;
    /// <summary>
    /// 发送按钮
    /// </summary>
    public Button sendBtn;
    /// <summary>
    /// 表情选择界面
    /// </summary>
    public Transform faceList;

    public override void OnInit()
    {
        viewRoot = LaunchUIView("Prefab/UI/Battle/ChatView");
        closeBtn = viewRoot.transform.Find("CloseBtn").GetComponent<Button>();
        chatList = viewRoot.transform.Find("ChatList").GetComponent<TableView>();
        sendInput = viewRoot.transform.Find("SendInput").GetComponent<InputField>();
        sendBtn = viewRoot.transform.Find("SendBtn").GetComponent<Button>();
        faceList = viewRoot.transform.Find("FaceSelectView/FaceList");

        chatList.DataProvider = new ArrayList(GlobalData.Chat_Const);
        sendInput.characterLimit = GlobalData.MaxCharNum;
        EventTriggerListener.Get(sendInput.gameObject).onDown = OnInputDown;
        sendInput.onEndEdit.AddListener(OnEndEdit);


        ApplicationFacade.Instance.RegisterMediator(new ChatViewMediator(Mediators.CHAT_VIEW_MEDIATOR, this));
    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_BATTLE;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    /// <summary>
    /// 开始输入
    /// </summary>
    /// <param name="go"></param>
    private void OnInputDown(GameObject go)
    {
        BattleProxy battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        battleProxy.isEnterInput = true;
    }

    /// <summary>
    /// 输入结束
    /// </summary>
    /// <param name="value"></param>
    private void OnEndEdit(string value)
    {
        BattleProxy battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        battleProxy.isEnterInput = false;
    }

    public override void LoadUI()
    {
        viewRootCache = Resources.Load<GameObject>("Prefab/UI/Battle/ChatView");
    }

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.CHAT_VIEW);
        UIManager.Instance.ShowDOTween(viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(viewRoot.GetComponent<RectTransform>(), base.OnHide);
    }

    public override void OnDestroy()
    {
        ApplicationFacade.Instance.RemoveMediator(Mediators.CHAT_VIEW_MEDIATOR);
        base.OnDestroy();
    }
}
public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(gameObject);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject);
    }
}
