using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TreeViewTest : MonoBehaviour 
{
    public TreeViewControl TreeView;
    private List<TreeViewData> treeDatas = new List<TreeViewData>();
    
    void Awake()
	{
        //生成数据
        //treeDatas = new List<TreeViewData>();

        //TreeViewData data = new TreeViewData();
        //data.name = "第一章";
        //data.parentID = -1;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.第一节";
        //data.parentID = 0;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.第二节";
        //data.parentID = 0;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.1.第一课";
        //data.parentID = 1;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.2.第一课";
        //data.parentID = 2;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.1.第二课";
        //data.parentID = 1;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.1.1.第一篇";
        //data.parentID = 3;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.1.1.第二篇";
        //data.parentID = 3;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.1.1.2.第一段";
        //data.parentID = 7;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.1.1.2.第二段";
        //data.parentID = 7;
        //treeDatas.Add(data);

        //data = new TreeViewData();
        //data.name = "1.1.1.2.1.第一题";
        //data.parentID = 8;
        //treeDatas.Add(data);

        ////指定数据源
        //TreeView.Data = treeDatas;
        ////重新生成树形菜单
        //TreeView.GenerateTreeView();
        ////刷新树形菜单
        //TreeView.RefreshTreeView();
        ////注册子元素的鼠标点击事件
        //TreeView.ClickItemEvent += CallBack;
    }

    private void Start()
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            AddTreeChildData(rootObjects[i], -1);
        }
        //指定数据源
        TreeView.Data = treeDatas;
        //重新生成树形菜单
        TreeView.GenerateTreeView();
        //刷新树形菜单
        TreeView.RefreshTreeView();
        //注册子元素的鼠标点击事件
        TreeView.ClickItemEvent += CallBack;
    }

    private void AddTreeChildData(GameObject parentObj,int parentId)
    {
        TreeViewData data = new TreeViewData();
        data.name = parentObj.name;
        data.parentID = parentId;
        treeDatas.Add(data);
        foreach (Transform item in parentObj.transform)
        {
            AddTreeChildData(item.gameObject, parentId + 1);
        }
    }

    void Update()
    {
        //判断树形菜单中名为“ 第一章 ”的元素是否被勾选
        if (Input.GetKeyDown(KeyCode.A))
        {
            bool isCheck = TreeView.ItemIsCheck("第一章");
            Debug.Log("当前树形菜单中的元素 第一章 " + (isCheck?"已被选中！":"未被选中！"));
        }
        //获取树形菜单中所有被勾选的元素
        if (Input.GetKeyDown(KeyCode.S))
        {
            List<string> items = TreeView.ItemsIsCheck();
            for (int i = 0; i < items.Count; i++)
            {
                Debug.Log("当前树形菜单中被选中的元素有：" + items[i]);
            }
        }
    }

    void CallBack(GameObject item)
    {
        Debug.Log("点击了 " + item.transform.FindChild("TreeViewText").GetComponent<Text>().text);
    }
}
