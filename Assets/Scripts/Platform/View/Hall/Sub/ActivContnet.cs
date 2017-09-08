using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
/// <summary>
/// 公告对象
/// </summary>
public class ActivContnet
{
    /// <summary>
    /// 公告按钮
    /// </summary>
    private readonly Button btn;
    /// <summary>
    /// 已被选择图片
    /// </summary>
    private readonly GameObject selected;
    /// <summary>
    /// 公告内容
    /// </summary>
    private Text text;
    /// <summary>
    /// 标题
    /// </summary>
    private readonly GameObject title;

    public Text Text
    {
        get
        {
            return text;
        }

        set
        {
            text = value;
        }
    }

    public ActivContnet(GameObject contnetRoot, HallNoticeType noticeType)
    {
        btn = contnetRoot.transform.FindChild("Button").GetComponent<Button>();
        selected = contnetRoot.transform.FindChild("Selected").gameObject;
        title = contnetRoot.transform.FindChild("Title").gameObject;
        Text = contnetRoot.transform.FindChild("Text").GetComponent<Text>();
        HallProxy hallProxy = (HallProxy)ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY);
        Text.text = hallProxy.HallInfo.noticeList[noticeType].content;
    }

    public void AddOnClickEvent(UnityAction callBack)
    {
        btn.onClick.AddListener(callBack);
    }

    public void Show()
    {
        btn.gameObject.SetActive(false);
        selected.SetActive(true);
        title.SetActive(true);
        Text.gameObject.SetActive(true);
    }

    public void Hide()
    {
        btn.gameObject.SetActive(true);
        selected.SetActive(false);
        title.SetActive(false);
        Text.gameObject.SetActive(false);
    }
}