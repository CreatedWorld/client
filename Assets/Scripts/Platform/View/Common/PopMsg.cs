using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 飘字提示组件
/// </summary>
public class PopMsg : MonoBehaviour
{
    public static PopMsg Instance;
    /// <summary>
    /// 飘字组件节点
    /// </summary>
    public GameObject msgItemPerfab;
    /// <summary>
    /// 未飘字文本数组
    /// </summary>
    private List<GameObject> unShowTxtArr;
    /// <summary>
    /// 正在显示的飘字数组
    /// </summary>
    private List<GameObject> showingPopTxtArr;
    /// <summary>
    /// 未显示的飘字数组
    /// </summary>
    private List<string> unShowMsgArr;
    /// <summary>
    /// 上次飘字时间
    /// </summary>
    private float perShowTime = 0;
    // Use this for initialization
    void Awake () {
        Instance = this;
	    unShowTxtArr = new List<GameObject>();
	    showingPopTxtArr = new List<GameObject>();
	    unShowMsgArr = new List<string>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 显示系统飘字
    /// </summary>
    /// <param name="msg"></param>
    public void ShowMsg(string msg)
    {
        if (showingPopTxtArr.Count > GlobalData.PopMsgMax)
        {
            unShowMsgArr.Add(msg);
            return;
        }
        ShowMsgItem(msg);
    }

    /// <summary>
    /// 显示单条飘字
    /// </summary>
    /// <param name="msg"></param>
    private void ShowMsgItem(string msg)
    {
        if (Time.time - perShowTime < 0.1f)
        {
            return;
        }
        perShowTime = Time.time;
        GameObject msgItem;
        if (unShowTxtArr.Count > 0)
        {
            msgItem = unShowTxtArr[0];
            msgItem.SetActive(true);
            unShowTxtArr.RemoveAt(0);
        }
        else
        {
            msgItem = Instantiate(msgItemPerfab);
        }
        showingPopTxtArr.Add(msgItem);
        var textField = msgItem.GetComponent<Text>();
        var rectTransform = msgItem.GetComponent<RectTransform>();
        rectTransform.SetParent(GetComponent<RectTransform>());
        rectTransform.localScale = Vector3.one;
        textField.text = msg;
        textField.color = Color.yellow;
        textField.DOColor(new Color(1, 0.92f, 0.016f, 0.5f), 1.5f).OnComplete(ItemMoveComplete);
        rectTransform.localPosition = new Vector3(0,-25,0);
        rectTransform.DOLocalMove(new Vector3(0, 25, 0), 1f);
    }

    /// <summary>
    /// 节点移动完成,回收节点
    /// </summary>
    private void ItemMoveComplete()
    {
        GameObject completeItem = showingPopTxtArr[0];
        completeItem.SetActive(true);
        unShowTxtArr.Add(showingPopTxtArr[0]);
        showingPopTxtArr.RemoveAt(0);
        if (unShowMsgArr.Count > 0)
        {
            ShowMsgItem(unShowMsgArr[0]);
            unShowMsgArr.RemoveAt(0);
        }
        else
        {
            completeItem.SetActive(false);
        }
    }

}
