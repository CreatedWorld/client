using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
/// <summary>
/// 创建房间,游戏规则复选框
/// </summary>
public class GameRuleInfo
{
    public List<int> gameRule91List = new List<int>();
    public GameRuleInfo(Toggle toggle, GameRule91 rule91)
    {
        Toggle = toggle;
        Rule91 = rule91;
        if (toggle.isOn)
        {
            gameRule91List.Add((int)rule91);
        }
    }
    /// <summary>
    /// 游戏规则
    /// </summary>
    public GameRule Rule { get; private set; }
    /// <summary>
    /// 游戏玩法
    /// </summary>
    public GameRule91 Rule91 { get; private set; }
    /// <summary>
    /// 选项控件
    /// </summary>
    public Toggle Toggle { get; private set; }
}