using System.Collections;
using Platform.Model;
using Platform.Model.Battle;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Platform.Utils;

/// <summary>
/// 单局结算头像节点
/// </summary>
public class MatchResultPlayerItem : MonoBehaviour
{
    /// <summary>
    /// 节点玩家数据
    /// </summary>
    private PlayerMatchResultVOS2C _data;
    /// <summary>
    /// 手上的牌数组
    /// </summary>
    private List<GameObject> handCards;
    /// <summary>
    /// 牌之间的间距
    /// </summary>
    private Vector3 cardHGap;

    /// <summary>
    /// 本局获得的积分
    /// </summary>
    private Text addScoreTxt;

    /// <summary>
    /// 庄家标志
    /// </summary>
    private GameObject bankerIcon;

    /// <summary>
    /// 头像
    /// </summary>
    private RawImage heroIcon;

    /// <summary>
    /// 玩家名称
    /// </summary>
    private Text nameTxt;
    /// <summary>
    /// 牌的容器
    /// </summary>
    private Transform cardContainer;
    /// <summary>
    /// 胡牌类型图标
    /// </summary>
    private Image huImg;
    /// <summary>
    /// 胡牌描述
    /// </summary>
    private Text huDesc;
    /// <summary>
    /// 第一张牌
    /// </summary>
    private GameObject card1;

    // Use this for initialization
    void Awake()
    {
        heroIcon = transform.Find("HeroIcon").GetComponent<RawImage>();
        nameTxt = transform.Find("NameTxt").GetComponent<Text>();
        addScoreTxt = transform.Find("AddScoreTxt").GetComponent<Text>();
        bankerIcon = transform.Find("BankerIcon").gameObject;
        huImg = transform.Find("HuImg").GetComponent<Image>();
        huDesc = transform.Find("HuDesc").GetComponent<Text>();
        cardContainer = transform.Find("Card");
        card1 = cardContainer.Find("Card1").gameObject;
        var card2 = cardContainer.Find("Card2");
        cardHGap = card2.localPosition - card1.transform.localPosition;
    }

    /// <summary>
    /// 头像框对应的玩家数据
    /// </summary>
    public PlayerMatchResultVOS2C data
    {
        get { return _data; }
        set
        {
            _data = value;
            var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            var playerInfoVO = battleProxy.playerIdInfoDic[value.userId];
            GameMgr.Instance.StartCoroutine(DownIcon(playerInfoVO.headIcon));
            nameTxt.text = playerInfoVO.name;
            addScoreTxt.text = value.addScore > 0 ? "+" + value.addScore : value.addScore.ToString();
            bankerIcon.SetActive(playerInfoVO.userId == battleProxy.perBankerId);
            handCards = new List<GameObject>();
            float cardNum = 0;
            RectTransform card1Rect = card1.GetComponent<RectTransform>();
            for (int i = 0; i < _data.pengGangs.Count; i++)
            {
                for (int j = 0; j < _data.pengGangs[i].pengGangCards.Count; j++)
                {
                    GameObject cardItem = ResourcesMgr.Instance.GetCardBtnFromPool(_data.pengGangs[i].pengGangCards[j]);
                    RectTransform cardRect = cardItem.GetComponent<RectTransform>();
                    cardRect.anchorMin = card1Rect.anchorMin;
                    cardRect.anchorMax = card1Rect.anchorMax;
                    cardItem.transform.SetParent(cardContainer);
                    cardItem.transform.localScale = Vector3.one;
                    cardItem.transform.localEulerAngles = Vector3.zero;
                    cardRect.localPosition = card1Rect.localPosition + GetCardPositionByIndex(cardNum);
                    handCards.Add(cardItem);
                    cardNum++;
                }
            }
            if (_data.pengGangs.Count > 0)
            {
                cardNum += 0.125f;
            }

            if (battleProxy.matchResultS2C.huUserId.Contains(_data.userId))//胡牌不在手牌内
            {
                _data.handCards.Remove(battleProxy.matchResultS2C.huedCard);
            }
            for (int i = 0; i < _data.handCards.Count; i++)
            {
                GameObject cardItem = ResourcesMgr.Instance.GetCardBtnFromPool(_data.handCards[i]);
                RectTransform cardRect = cardItem.GetComponent<RectTransform>();
                cardItem.transform.SetParent(cardContainer);
                cardRect.anchorMin = card1Rect.anchorMin;
                cardRect.anchorMax = card1Rect.anchorMax;
                cardItem.transform.localScale = Vector3.one;
                cardItem.transform.localEulerAngles = Vector3.zero;
                cardItem.transform.localPosition = card1Rect.localPosition + GetCardPositionByIndex(cardNum);
                handCards.Add(cardItem);
                cardNum++;
            }
            if (battleProxy.matchResultS2C.huUserId.Contains(data.userId))//添加胡别人的牌
            {
                cardNum += 0.125f;
                GameObject cardItem = ResourcesMgr.Instance.GetCardBtnFromPool(battleProxy.matchResultS2C.huedCard);
                RectTransform cardRect = cardItem.GetComponent<RectTransform>();
                cardItem.transform.SetParent(cardContainer);
                cardRect.anchorMin = card1Rect.anchorMin;
                cardRect.anchorMax = card1Rect.anchorMax;
                cardItem.transform.localScale = Vector3.one;
                cardItem.transform.localEulerAngles = Vector3.zero;
                cardRect.localPosition = card1Rect.localPosition + GetCardPositionByIndex(cardNum);
                handCards.Add(cardItem);
            }
            if (battleProxy.matchResultS2C.ziMoUserId == _data.userId)
            {
                huImg.gameObject.SetActive(true);
                //huIcon.sprite = Resources.Load<Sprite>("Textures/HuType/SelfHu");
            }
            else if (battleProxy.matchResultS2C.huedUserId == _data.userId)
            {
                huImg.gameObject.SetActive(false);
                //huIcon.sprite = Resources.Load<Sprite>("Textures/HuType/Hued");
            }
            else if (battleProxy.matchResultS2C.huedUserId > 0 && battleProxy.matchResultS2C.huUserId.Contains(_data.userId))
            {
                huImg.gameObject.SetActive(true);
                //huIcon.sprite = Resources.Load<Sprite>("Textures/HuType/OtheHu");
            }
            else
            {
                huImg.gameObject.SetActive(false); 
            }
            string desc = "";
            if (_data.huDesc != null)
            {
                desc += _data.huDesc;
            }
            if (_data.anGangCount > 0)
            {
                if (desc.Length > 0)
                {
                    desc += " ";
                }
                desc += string.Format("暗杠+{0}", _data.anGangCount);
            }
            if (_data.mingGangCount > 0)
            {
                if (desc.Length > 0)
                {
                    desc += " ";
                }
                desc += string.Format("明杠+{0}", _data.mingGangCount);
            }
            huDesc.text = desc;
        }
    }

    /// <summary>
    /// 坐标重新计算
    /// </summary>
    public void ResetCardPos()
    {
        var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        var playerInfoVO = battleProxy.playerIdInfoDic[_data.userId];
        float cardNum = 0;
        int index = 0;
        RectTransform card1Rect = card1.GetComponent<RectTransform>();
        for (int i = 0; i < _data.pengGangs.Count; i++)
        {
            for (int j = 0; j < _data.pengGangs[i].pengGangCards.Count; j++)
            {
                GameObject cardItem = handCards[index];
                RectTransform cardRect = cardItem.GetComponent<RectTransform>();
                cardRect.anchorMin = card1Rect.anchorMin;
                cardRect.anchorMax = card1Rect.anchorMax;
                cardItem.transform.SetParent(cardContainer);
                cardItem.transform.localScale = Vector3.one;
                cardItem.transform.localEulerAngles = Vector3.zero;
                cardRect.localPosition = card1Rect.localPosition + GetCardPositionByIndex(cardNum);
                cardNum++;
                index++;
            }
        }
        if (_data.pengGangs.Count > 0)
        {
            cardNum += 0.125f;
        }

        for (int i = 0; i < _data.handCards.Count; i++)
        {
            GameObject cardItem = handCards[index];
            RectTransform cardRect = cardItem.GetComponent<RectTransform>();
            cardItem.transform.SetParent(cardContainer);
            cardRect.anchorMin = card1Rect.anchorMin;
            cardRect.anchorMax = card1Rect.anchorMax;
            cardItem.transform.localScale = Vector3.one;
            cardItem.transform.localEulerAngles = Vector3.zero;
            cardItem.transform.localPosition = card1Rect.localPosition + GetCardPositionByIndex(cardNum);
            cardNum++;
            index++;
        }
        if (battleProxy.matchResultS2C.huUserId.Contains(data.userId))//添加胡别人的牌
        {
            cardNum += 0.125f;
            GameObject cardItem = handCards[index];
            RectTransform cardRect = cardItem.GetComponent<RectTransform>();
            cardItem.transform.SetParent(cardContainer);
            cardRect.anchorMin = card1Rect.anchorMin;
            cardRect.anchorMax = card1Rect.anchorMax;
            cardItem.transform.localScale = Vector3.one;
            cardItem.transform.localEulerAngles = Vector3.zero;
            cardRect.localPosition = card1Rect.localPosition + GetCardPositionByIndex(cardNum);
            index++;
        }
    }

    /// <summary>
    /// 根据序号获取牌的位置
    /// </summary>
    private Vector3 GetCardPositionByIndex(float cardIndex)
    {
        var result = cardIndex * cardHGap;
        return result;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    /// <summary>
    /// 异步加载头像
    /// </summary>
    /// <param name="headUrl"></param>
    /// <returns></returns>
    private IEnumerator DownIcon(string headUrl)
    {
        var www = new WWW(headUrl);
        yield return www;
        if (www.error == null)
            heroIcon.texture = www.texture;
    }



    /// <summary>
    /// 回收所有牌
    /// </summary>
    public void SaveAllCard()
    {
        foreach (GameObject card in handCards)
        {
            ResourcesMgr.Instance.AddBtn2Pool(card);
        }
        handCards.Clear();
    }
}