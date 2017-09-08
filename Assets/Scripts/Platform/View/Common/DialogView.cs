using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 二次确认框
/// </summary>
public class DialogView : UIView
{
    private DialogMsgVO _data;
    /// <summary>
    /// 取消按钮
    /// </summary>
    private Button cancelBtn;
    /// <summary>
    /// 确认按钮
    /// </summary>
    private Button confirmBtn;
    /// <summary>
    /// 提示内容文本
    /// </summary>
    private Text contentTxt;
    /// <summary>
    /// 标题文本
    /// </summary>
    private Text titleTxt;
    /// <summary>
    /// 不再显示
    /// </summary>
    private Toggle unShowChk;

    public override void OnInit()
    {
        viewRoot = LaunchUIView("Prefab/UI/Common/DialogView");
        titleTxt = viewRoot.transform.Find("TitleTxt").GetComponent<Text>();
        contentTxt = viewRoot.transform.Find("ContentTxt").GetComponent<Text>();
        unShowChk = viewRoot.transform.Find("UnShowChk").GetComponent<Toggle>();
        confirmBtn = viewRoot.transform.Find("ConfirmBtn").GetComponent<Button>();
        cancelBtn = viewRoot.transform.Find("CancelBtn").GetComponent<Button>();

        confirmBtn.onClick.AddListener(ConfirmCallBack);
        cancelBtn.onClick.AddListener(CancelCallBack);
    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_START;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    public override void LoadUI()
    {
        viewRootCache = Resources.Load<GameObject>("Prefab/UI/Common/DialogView");
    }

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.DIALOG_VIEW);
        UIManager.Instance.ShowDOTween(viewRoot.GetComponent<RectTransform>());
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(viewRoot.GetComponent<RectTransform>(),base.OnHide);
    }

    /// <summary>
    /// 对话框消息内容
    /// </summary>
    public DialogMsgVO data
    {
        set
        {
            _data = value;
            if (value.showDontShowAgain && GlobalData.DontShowAgainDic.ContainsKey(value.showAgainKey) && GlobalData.DontShowAgainDic[value.showAgainKey])//勾选不再显示,直接回调
            {
                ConfirmCallBack();
                return;
            }

            if (value.title != null)
            {
                titleTxt.text = value.title;
            }
            else
            {
                titleTxt.text = "二次确认";
            }
            if (value.content != null)
            {
                contentTxt.text = value.content;
            }
            unShowChk.gameObject.SetActive(value.showDontShowAgain);
            if (value.confirmBtnStr != null)
            {
                confirmBtn.transform.Find("Text").GetComponent<Text>().text = value.confirmBtnStr;
            }
            else
            {
                confirmBtn.transform.Find("Text").GetComponent<Text>().text = "确认";
            }
            if (value.cancelBtnStr != null)
            {
                cancelBtn.transform.Find("Text").GetComponent<Text>().text = value.cancelBtnStr;
            }
            else
            {
                cancelBtn.transform.Find("Text").GetComponent<Text>().text = "取消";
            }
            if (value.dialogType == DialogType.ALERT)
            {
                var perPos = confirmBtn.gameObject.GetComponent<RectTransform>().localPosition;
                confirmBtn.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, perPos.y);
                cancelBtn.gameObject.SetActive(false);
            }
            else if (value.dialogType == DialogType.CONFIRM)
            {
                var perPos = confirmBtn.gameObject.GetComponent<RectTransform>().localPosition;
                confirmBtn.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(-144, perPos.y);
                cancelBtn.gameObject.SetActive(true);
                cancelBtn.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(144, perPos.y);
            }
            contentTxt.text = _data.content;
        }
    }

    /// <summary>
    /// 确认回调
    /// </summary>
    private void ConfirmCallBack()
    {
        if (_data.showDontShowAgain)
        {
            GlobalData.DontShowAgainDic[_data.showAgainKey] = unShowChk.isOn;
        }
        if (_data.confirmCallBack != null)
        {
            _data.confirmCallBack();
        }
        UIManager.Instance.HideUI(UIViewID.DIALOG_VIEW);
    }

    /// <summary>
    /// 取消回调
    /// </summary>
    private void CancelCallBack()
    {
        if (_data.cancelCallBack != null)
        {
            _data.cancelCallBack();
        }
        UIManager.Instance.HideUI(UIViewID.DIALOG_VIEW);
    }
}
/// <summary>
/// 二次确认消息体
/// </summary>
public class DialogMsgVO
{
    /// <summary>
    /// 对话框类型
    /// </summary>
    public DialogType dialogType;
    /// <summary>
    /// 提示标题
    /// </summary>
    public string title;
    /// <summary>
    /// 提示内容
    /// </summary>
    public string content;
    /// <summary>
    /// 是否显示不再显示
    /// </summary>
    public bool showDontShowAgain = false;
    /// <summary>
    /// 不再显示key值
    /// </summary>
    public DontShowAgain showAgainKey;
    /// <summary>
    /// 确认回调方法
    /// </summary>
    public UnityAction confirmCallBack;
    /// <summary>
    /// 取消回调方法
    /// </summary>
    public UnityAction cancelCallBack;
    /// <summary>
    /// 确认按钮文本
    /// </summary>
    public string confirmBtnStr;
    /// <summary>
    /// 取消按钮文本
    /// </summary>
    public string cancelBtnStr;
}
