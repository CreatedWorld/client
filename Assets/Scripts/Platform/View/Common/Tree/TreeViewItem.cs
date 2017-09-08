using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
/// <summary>
/// 树形菜单元素
/// </summary>
public class TreeViewItem : MonoBehaviour
{
    /// <summary>
    /// 树形菜单控制器
    /// </summary>
    public TreeViewControl controler;
    /// <summary>
    /// 当前元素的子元素是否展开（展开时可见）
    /// </summary>
    public bool isExpanding = false;

    /// <summary>
    /// 当前元素在树形图中所属的层级
    /// </summary>
    private int hierarchy = 0;
    /// <summary>
    /// 当前元素指向的父元素
    /// </summary>
    private TreeViewItem parentItem;
    /// <summary>
    /// 当前元素的所有子元素
    /// </summary>
    private List<TreeViewItem> children;
    /// <summary>
    /// 正在进行刷新
    /// </summary>
    private bool isRefreshing = false;
    /// <summary>
    /// 展开按钮
    /// </summary>
    private Button contextButton;

    void Awake()
    {
        //上下文按钮点击回调
        contextButton = transform.FindChild("ContextButton").GetComponent<Button>();
        contextButton.onClick.AddListener(ContextButtonClick);
        contextButton.gameObject.SetActive(false);
        transform.FindChild("TreeViewButton").GetComponent<Button>().onClick.AddListener(delegate () {
            controler.ClickItem(gameObject);
        });
    }
    /// <summary>
    /// 点击上下文菜单按钮，元素的子元素改变显示状态
    /// </summary>
    void ContextButtonClick()
    {
        //上一轮刷新还未结束
        if (isRefreshing)
        {
            return;
        }

        isRefreshing = true;

        if (isExpanding)
        {
            transform.FindChild("ContextButton").GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
            isExpanding = false;
            ChangeChildren(this, false);
        }
        else
        {
            transform.FindChild("ContextButton").GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
            isExpanding = true;
            ChangeChildren(this, true);
        }

        //刷新树形菜单
        controler.RefreshTreeView();

        isRefreshing = false;
    }
    /// <summary>
    /// 改变某一元素所有子元素的显示状态
    /// </summary>
    void ChangeChildren(TreeViewItem tvi, bool value)
    {
        for (int i = 0; i < tvi.GetChildrenNumber(); i++)
        {
            tvi.GetChildrenByIndex(i).gameObject.SetActive(value);
            if (tvi.GetChildrenByIndex(i).isExpanding)
            {
                ChangeChildren(tvi.GetChildrenByIndex(i), value);
            }
        }
    }

    #region 属性访问
    public int GetHierarchy()
    {
        return hierarchy;
    }
    public void SetHierarchy(int hierarchy)
    {
        this.hierarchy = hierarchy;
    }
    public TreeViewItem GetParent()
    {
        return parentItem;
    }
    public void SetParent(TreeViewItem parent)
    {
        this.parentItem = parent;
    }
    public void AddChildren(TreeViewItem children)
    {
        if (this.children == null)
        {
            this.children = new List<TreeViewItem>();
        }
        this.children.Add(children);
        contextButton.gameObject.SetActive(this.children.Count > 0);
    }
    public void RemoveChildren(TreeViewItem children)
    {
        if (this.children == null)
        {
            return;
        }
        this.children.Remove(children);
        contextButton.gameObject.SetActive(this.children.Count > 0);
    }
    public void RemoveChildren(int index)
    {
        if (children == null || index < 0 || index >= children.Count)
        {
            return;
        }
        children.RemoveAt(index);
        contextButton.gameObject.SetActive(children.Count > 0);
    }
    public int GetChildrenNumber()
    {
        if (children == null)
        {
            return 0;
        }
        return children.Count;
    }
    public TreeViewItem GetChildrenByIndex(int index)
    {
        if (index >= children.Count)
        {
            return null;
        }
        return children[index];
    }
    #endregion
}
