using UnityEngine;
using System.Collections;

/// <summary>  
/// @desc:       滑动列表单个组件  
/// @author:     gyx  
/// </summary>  
[DisallowMultipleComponent]
public class TableViewItem : MonoBehaviour
{
    protected int index;
    protected object _data;
    /// <summary>  
    /// 设置节点下标  
    /// </summary>  
    public int Index
    {
        set
        {
            index = value;
        }
        get
        {
            return index;
        }
    }

    public virtual void Updata(object data)
    {
        _data = data;
    }
}