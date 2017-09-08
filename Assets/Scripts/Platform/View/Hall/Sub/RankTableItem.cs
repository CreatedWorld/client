using Platform.Model;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 排行榜列表项
/// </summary>
public class RankTableItem : TableViewItem
{
    /// <summary>
    /// 排行数据
    /// </summary>
    private RankItem rankData;
    /// <summary>
    /// 是否初始化
    /// </summary>
    private bool isAwake;

    /// <summary>
    /// 排行值
    /// </summary>
    private Text rankTxt;
    /// <summary>
    /// 排行图标
    /// </summary>
    private Image rankIcon;
    /// <summary>
    /// 玩家名称
    /// </summary>
    private Text nameTxt;
    /// <summary>
    /// 玩家id
    /// </summary>
    private Text playerIdTxt;
    /// <summary>
    /// 积分
    /// </summary>
    private Text scoreTxt;
    public override void Updata(object data)
    {
        if (data == null)
        {
            return;
        }
        base.Updata(data);
        rankData = data as RankItem;
        if (isAwake)
        {
            UpdateView();
        }
    }


    private void Awake()
    {
        rankTxt = transform.Find("RankTxt").GetComponent<Text>();
        rankIcon = transform.Find("RankIcon").GetComponent<Image>();
        nameTxt = transform.Find("NameTxt").GetComponent<Text>();
        playerIdTxt = transform.Find("PlayerIdTxt").GetComponent<Text>();
        scoreTxt = transform.Find("ScoreTxt").GetComponent<Text>();

        isAwake = true;
        if (rankData != null)
        {
            UpdateView();
        }
    }

    /// <summary>
    /// 更新界面数据
    /// </summary>
    private void UpdateView()
    {
        if (Index + 1 <= 3)
        {
            rankIcon.gameObject.SetActive(true);
            rankTxt.text = "";
            rankIcon.sprite = Resources.Load<Sprite>(string.Format("Textures/RankIcon/{0}",Index + 1));
        }
        else
        {
            rankIcon.gameObject.SetActive(false);
            rankTxt.text = (Index + 1).ToString();
        }
        nameTxt.text = rankData.name;
        playerIdTxt.text = rankData.playerId.ToString();
        scoreTxt.text = rankData.score.ToString();
    }
}
