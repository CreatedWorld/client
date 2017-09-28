using System;
using System.Collections;
using Platform.Model;
using Platform.Model.Battle;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     房间结算玩家节点
/// </summary>
public class RoomHeadItem : MonoBehaviour
{
    /// <summary>
    ///     节点玩家数据
    /// </summary>
    private PlayerRoomResultVOS2C _data;

    /// <summary>
    ///     暗杠次数
    /// </summary>
    private Text anGangValueTxt;

    /// <summary>
    ///     玩家头像
    /// </summary>
    private RawImage headIcon;

    /// <summary>
    ///     点炮次数
    /// </summary>
    private Text huedValueTxt;

    /// <summary>
    ///     玩家id
    /// </summary>
    private Text idTxt;

    /// <summary>
    ///     明杠次数
    /// </summary>
    private Text mingGangValueTxt;

    /// <summary>
    ///     玩家名称
    /// </summary>
    private Text nameTxt;

    /// <summary>
    ///     接炮次数
    /// </summary>
    //private Text otherHuValueTxt;

    /// <summary>
    ///     获得积分
    /// </summary>
    private Text scoreValueTxt;

    /// <summary>
    ///     胡牌次数
    /// </summary>
    private Text HuValueTxt;

    // Use this for initialization
    void Awake()
    {
        headIcon = transform.Find("HeadIcon").GetComponent<RawImage>();
        nameTxt = transform.Find("NameTxt").GetComponent<Text>();
        idTxt = transform.Find("IDTxt").GetComponent<Text>();
        HuValueTxt = transform.Find("HuValueTxt").GetComponent<Text>();
        //otherHuValueTxt = transform.Find("OtherHuValueTxt").GetComponent<Text>();
        huedValueTxt = transform.Find("HuedValueTxt").GetComponent<Text>();
        anGangValueTxt = transform.Find("AnGangValueTxt").GetComponent<Text>();
        mingGangValueTxt = transform.Find("MingGangValueTxt").GetComponent<Text>();
        scoreValueTxt = transform.Find("ScoreValueTxt").GetComponent<Text>();
    }
    /// <summary>
    ///     头像框对应的玩家数据
    /// </summary>
    public PlayerRoomResultVOS2C data
    {
        get { return _data; }
        set
        {
            _data = value;
            var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            var playerInfoVO = battleProxy.playerIdInfoDic[value.userId];
            GameMgr.Instance.StartCoroutine(DownIcon(playerInfoVO.headIcon));
            nameTxt.text = playerInfoVO.name;
            idTxt.text = String.Format("ID:{0}", playerInfoVO.userId);
            HuValueTxt.text =  (value.selfHuCount+ value.otherHuCount).ToString();
            //otherHuValueTxt.text = value.otherHuCount.ToString();
            huedValueTxt.text = value.sendPaoCount.ToString();
            anGangValueTxt.text = value.anGangCount.ToString();
            mingGangValueTxt.text = value.mingGangCount.ToString();
            scoreValueTxt.text = value.addScore > 0 ? "+" + value.addScore : value.addScore.ToString();
        }
    }


    // Update is called once per frame
    private void Update()
    {
    }

    /// <summary>
    ///     异步加载头像
    /// </summary>
    /// <param name="headUrl"></param>
    /// <returns></returns>
    private IEnumerator DownIcon(string headUrl)
    {
        var www = new WWW(headUrl);
        yield return www;
        if (www.error == null)
            headIcon.texture = www.texture;
    }
}