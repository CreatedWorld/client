using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// 树形菜单控制器
/// </summary>
public class TreeViewControl : MonoBehaviour
{
    /// <summary>
    /// 当前树形菜单的数据源
    /// </summary>
    [HideInInspector]
    public List<TreeViewData> Data = null;
    /// <summary>
    /// 树形菜单中元素的模板
    /// </summary>
    public GameObject template;
    /// <summary>
    /// 树形菜单中元素的根物体
    /// </summary>
    public Transform treeItems;
    /// <summary>
    /// 树形菜单的纵向排列间距
    /// </summary>
    public int verticalItemSpace = 2;
    /// <summary>
    /// 树形菜单的横向排列间距
    /// </summary>
    public int horizontalItemSpace = 25;
    /// <summary>
    /// 树形菜单中元素的宽度
    /// </summary>
    public int itemWidth = 230;
    /// <summary>
    /// 树形菜单中元素的高度
    /// </summary>
    public int itemHeight = 35;
    /// <summary>
    /// 所有子元素的鼠标点击回调事件
    /// </summary>
    public delegate void ClickItemdelegate(GameObject item);
    public event ClickItemdelegate ClickItemEvent;

    /// <summary>
    /// 当前树形菜单中的所有元素
    /// </summary>
    private List<GameObject> treeViewItems;
    /// <summary>
    /// 当前树形菜单中的所有元素克隆体（刷新树形菜单时用于过滤计算）
    /// </summary>
    private List<GameObject> treeViewItemsClone;
    /// <summary>
    /// 树形菜单当前刷新队列的元素位置索引
    /// </summary>
    private int yIndex = 0;
    /// <summary>
    /// 树形菜单当前刷新队列的元素最大层级
    /// </summary>
    private int hierarchy = 0;
    /// <summary>
    /// 正在进行刷新
    /// </summary>
    private bool isRefreshing = false;

    void Awake()
    {
        ClickItemEvent += ClickItemTemplate;
    }
    /// <summary>
    /// 鼠标点击子元素事件
    /// </summary>
    public void ClickItem(GameObject item)
    {
        ClickItemEvent(item);
    }
    void ClickItemTemplate(GameObject item)
    {
        //空的事件，不这样做的话ClickItemEvent会引发空引用异常
    }

    /// <summary>
    /// 返回指定名称的子元素是否被勾选
    /// </summary>
    public bool ItemIsCheck(string itemName)
    {
        for (int i = 0; i < treeViewItems.Count; i++)
        {
            if (treeViewItems[i].transform.FindChild("TreeViewText").GetComponent<Text>().text == itemName)
            {
                return treeViewItems[i].transform.FindChild("TreeViewToggle").GetComponent<Toggle>().isOn;
            }
        }
        return false;
    }
    /// <summary>
    /// 返回树形菜单中被勾选的所有子元素名称
    /// </summary>
    public List<string> ItemsIsCheck()
    {
        List<string> items = new List<string>();

        for (int i = 0; i < treeViewItems.Count; i++)
        {
            if (treeViewItems[i].transform.FindChild("TreeViewToggle").GetComponent<Toggle>().isOn)
            {
                items.Add(treeViewItems[i].transform.FindChild("TreeViewText").GetComponent<Text>().text);
            }
        }

        return items;
    }

    /// <summary>
    /// 生成树形菜单
    /// </summary>
    public void GenerateTreeView()
    {
        //删除可能已经存在的树形菜单元素
        if (treeViewItems != null)
        {
            for (int i = 0; i < treeViewItems.Count; i++)
            {
                Destroy(treeViewItems[i]);
            }
            treeViewItems.Clear();
        }
        //重新创建树形菜单元素
        treeViewItems = new List<GameObject>();
        for (int i = 0; i < Data.Count; i++)
        {
            GameObject item = Instantiate(template);

            if (Data[i].parentID == -1)
            {
                item.GetComponent<TreeViewItem>().SetHierarchy(0);
                item.GetComponent<TreeViewItem>().SetParent(null);
            }
            else
            {
                TreeViewItem tvi = treeViewItems[Data[i].parentID].GetComponent<TreeViewItem>();
                item.GetComponent<TreeViewItem>().SetHierarchy(tvi.GetHierarchy() + 1);
                item.GetComponent<TreeViewItem>().SetParent(tvi);
                tvi.AddChildren(item.GetComponent<TreeViewItem>());
            }

            item.transform.name = "TreeViewItem";
            item.transform.FindChild("TreeViewText").GetComponent<Text>().text = Data[i].name;
            item.transform.SetParent(treeItems);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item.SetActive(true);

            treeViewItems.Add(item);
        }
    }

    /// <summary>
    /// 刷新树形菜单
    /// </summary>
    public void RefreshTreeView()
    {
        //上一轮刷新还未结束
        if (isRefreshing)
        {
            return;
        }

        isRefreshing = true;
        yIndex = 0;
        hierarchy = 0;

        //复制一份菜单
        treeViewItemsClone = new List<GameObject>(treeViewItems);

        //用复制的菜单进行刷新计算
        for (int i = 0; i < treeViewItemsClone.Count; i++)
        {
            //已经计算过或者不需要计算位置的元素
            if (treeViewItemsClone[i] == null || !treeViewItemsClone[i].activeSelf)
            {
                continue;
            }

            TreeViewItem tvi = treeViewItemsClone[i].GetComponent<TreeViewItem>();

            treeViewItemsClone[i].GetComponent<RectTransform>().localPosition = new Vector3(tvi.GetHierarchy() * horizontalItemSpace, yIndex,0);
            yIndex += (-(itemHeight + verticalItemSpace));
            if (tvi.GetHierarchy() > hierarchy)
            {
                hierarchy = tvi.GetHierarchy();
            }

            //如果子元素是展开的，继续向下刷新
            if (tvi.isExpanding)
            {
                RefreshTreeViewChild(tvi);
            }

            treeViewItemsClone[i] = null;
        }

        //重新计算滚动视野的区域
        float x = hierarchy * horizontalItemSpace + itemWidth;
        float y = Mathf.Abs(yIndex);
        transform.GetComponent<ScrollRect>().content.sizeDelta = new Vector2(x, y);

        //清空复制的菜单
        treeViewItemsClone.Clear();

        isRefreshing = false;
    }
    /// <summary>
    /// 刷新元素的所有子元素
    /// </summary>
    void RefreshTreeViewChild(TreeViewItem tvi)
    {
        for (int i = 0; i < tvi.GetChildrenNumber(); i++)
        {
            tvi.GetChildrenByIndex(i).gameObject.GetComponent<RectTransform>().localPosition = new Vector3(tvi.GetChildrenByIndex(i).GetHierarchy() * horizontalItemSpace, yIndex, 0);
            yIndex += (-(itemHeight + verticalItemSpace));
            if (tvi.GetChildrenByIndex(i).GetHierarchy() > hierarchy)
            {
                hierarchy = tvi.GetChildrenByIndex(i).GetHierarchy();
            }

            //如果子元素是展开的，继续向下刷新
            if (tvi.GetChildrenByIndex(i).isExpanding)
            {
                RefreshTreeViewChild(tvi.GetChildrenByIndex(i));
            }

            int index = treeViewItemsClone.IndexOf(tvi.GetChildrenByIndex(i).gameObject);
            if (index >= 0)
            {
                treeViewItemsClone[index] = null;
            }
        }
    }
}
