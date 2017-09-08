using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
/// <summary>
/// 创建房间,游戏类型复选框
/// </summary>
public class GameModelInfo
{
    public GameModelInfo(Toggle toggle, GameMode model)
    {
        Toggle = toggle;
        Model = model;
    }
    /// <summary>
    /// 游戏类型
    /// </summary>
    public GameMode Model { get; private set; }
    
    /// <summary>
    /// 复选框控件
    /// </summary>
    public Toggle Toggle { get; private set; }
}