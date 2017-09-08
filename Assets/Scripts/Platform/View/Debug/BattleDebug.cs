using Platform.Model;
using Platform.Model.Battle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Platform.Net;
using Utils;
/// <summary>
/// 牌面调试UI
/// </summary>
public class BattleDebug : MonoBehaviour {
    /// <summary>
    /// 战斗数据
    /// </summary>
    private BattleProxy battleProxy;
    /// <summary>
    /// 牌池字典
    /// </summary>
    private Dictionary<int,GameObject> cardPoolDic = new Dictionary<int, GameObject>();
    /// <summary>
    /// 当前选定的玩家节点
    /// </summary>
    private DebugPlayerItem curSelectPlayerItem;
    /// <summary>
    /// 玩家节点字典{座位号:DebugPlayerItem}
    /// </summary>
    private List<DebugPlayerItem> playerItemArr = new List<DebugPlayerItem>();
    /// <summary>
    /// 手牌单选框
    /// </summary>
    private Toggle handCardToggle;
    /// <summary>
    /// 摸牌单选
    /// </summary>
    private Toggle getCardToggle;
    /// <summary>
    /// 下轮摸牌
    /// </summary>
    private Toggle nextCardToggle;
    // Use this for initialization
    void Start () {
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        //获取玩家节点
        var playerItem1 = transform.Find("PlayerItem1").GetComponent<DebugPlayerItem>();
        var playerItem2 = transform.Find("PlayerItem2").GetComponent<DebugPlayerItem>();
        var playerItem3 = transform.Find("PlayerItem3").GetComponent<DebugPlayerItem>();
        var playerItem4 = transform.Find("PlayerItem4").GetComponent<DebugPlayerItem>();
        playerItemArr.Add(playerItem1);
        playerItemArr.Add(playerItem2);
        playerItemArr.Add(playerItem3);
        playerItemArr.Add(playerItem4);
        playerItem1.selectCallBack = SelctPlayerItem;
        playerItem2.selectCallBack = SelctPlayerItem;
        playerItem3.selectCallBack = SelctPlayerItem;
        playerItem4.selectCallBack = SelctPlayerItem;
        playerItem1.returnCardCallBack = ReturnCard;
        playerItem2.returnCardCallBack = ReturnCard;
        playerItem3.returnCardCallBack = ReturnCard;
        playerItem4.returnCardCallBack = ReturnCard;
        //确定取消
        transform.Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(ConfirmSetCard);
        transform.Find("ClearBtn").GetComponent<Button>().onClick.AddListener(ClearCardSet);
        transform.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(CloseCardSet);
        //牌面类型设置
        handCardToggle = transform.Find("HandCardToggle").GetComponent<Toggle>();
        getCardToggle = transform.Find("GetCardToggle").GetComponent<Toggle>();
        nextCardToggle = transform.Find("NextCardToggle").GetComponent<Toggle>();
        handCardToggle.onValueChanged.AddListener((bool value) =>
        {
            if (value)
            {
                SelectToggle(handCardToggle);
            }
        });
        getCardToggle.onValueChanged.AddListener((bool value) =>
        {
            if (value)
            {
                SelectToggle(getCardToggle);
            }
        });
        nextCardToggle.onValueChanged.AddListener((bool value) =>
        {
            if (value)
            {
                SelectToggle(nextCardToggle);
            }
        });
        ApplicationFacade.Instance.RegisterMediator(new BattleDebugMediator(Mediators.DEBUG_MEDIATOR, this));
    }

    private void SelectToggle(Toggle selectToggle)
    {
        Timer.Instance.AddTimer(0, 1, 0.2f, () => {
            if (selectToggle != nextCardToggle)
            {
                nextCardToggle.isOn = false;
            }
            if (selectToggle != getCardToggle)
            {
                getCardToggle.isOn = false;
            }
            if (selectToggle != handCardToggle)
            {
                handCardToggle.isOn = false;
            }
        });
    }

    /// <summary>
    /// 初始化牌面
    /// </summary>
    public void InitCardPool()
    {
        var cardPoolContainer = transform.Find("CardPoolContainer").GetComponent<RectTransform>();
        var poolFirstCard = transform.Find("CardPoolContainer/CardSelectBtn1").GetComponent<RectTransform>();
        var poolFirstCardPos = transform.Find("CardPoolContainer/CardSelectBtn1").GetComponent<RectTransform>().localPosition;
        var poolSecondCardPos = transform.Find("CardPoolContainer/CardSelectBtn2").GetComponent<RectTransform>().localPosition;
        var poolThridCardPos = transform.Find("CardPoolContainer/CardSelectBtn3").GetComponent<RectTransform>().localPosition;
        var poolCardHGap = poolThridCardPos - poolFirstCardPos;
        var poolCardVGap = poolSecondCardPos - poolFirstCardPos;
        foreach (int cardValue in GlobalData.CardValues)
        {
            var poolCard = GetCardObject(cardValue);
            var cardRectTransform = poolCard.GetComponent<RectTransform>();
            cardRectTransform.SetParent(cardPoolContainer);
            cardRectTransform.anchorMin = poolFirstCard.anchorMin;
            cardRectTransform.anchorMax = poolFirstCard.anchorMax;
            cardRectTransform.localScale = poolFirstCard.localScale;
            cardRectTransform.localEulerAngles = poolFirstCard.localEulerAngles;
            cardRectTransform.localPosition = poolFirstCard.localPosition + poolCardVGap * (cardValue % 10 - 1) + poolCardHGap * (cardValue / 10 - 1);
            EventTrigger buttonTrigger = poolCard.AddComponent<EventTrigger>();
            buttonTrigger.triggers = new List<EventTrigger.Entry>();
            EventTrigger.Entry pointClick = new EventTrigger.Entry();
            buttonTrigger.triggers.Add(pointClick);
            pointClick.eventID = EventTriggerType.PointerClick;
            pointClick.callback.AddListener(PoolCardClickHandler);
            poolCard.name = "CardPool_" + cardValue;
            if (battleProxy.cardValuePool.Contains(cardValue))
            {
                poolCard.GetComponent<Button>().interactable = true;
                poolCard.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            }
            else
            {
                poolCard.GetComponent<Button>().interactable = false;
                poolCard.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            cardPoolDic.Add(cardValue, poolCard);
        }
        foreach (DebugPlayerItem item in playerItemArr)
        {
            if (battleProxy.playerSitInfoDic.ContainsKey(item.sit))
            {
                var playerInfoVO = battleProxy.playerSitInfoDic[item.sit];
                for (int i = 0; i < playerInfoVO.handCards.Count; i++)
                {
                    item.AddHandCard(playerInfoVO.handCards[i]);
                }
                if (playerInfoVO.getCard > 0)
                {
                    item.AddGetCard(playerInfoVO.getCard);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (playerItemArr.Count > 0)
        {
            ApplicationFacade.Instance.RegisterMediator(new BattleDebugMediator(Mediators.DEBUG_MEDIATOR, this));
        }
    }

    private void OnDisable()
    {
        foreach (DebugPlayerItem item in playerItemArr)
        {
            item.ReturnAllCard();
        }
        foreach (KeyValuePair<int,GameObject> keyValue in cardPoolDic)
        {
            GameObject.Destroy(keyValue.Value);
        }
        cardPoolDic.Clear();
        ApplicationFacade.Instance.RemoveMediator(Mediators.DEBUG_MEDIATOR);
    }

    /// <summary>
    /// 点击牌池内的牌
    /// </summary>
    /// <param name="arg0"></param>
    private void PoolCardClickHandler(BaseEventData arg0)
    {
        var poolCard = arg0.selectedObject;
        if (poolCard == null)
        {
            PopMsg.Instance.ShowMsg("当前类型的牌面已发完");
            return;
        }
        if (curSelectPlayerItem == null)
        {
            PopMsg.Instance.ShowMsg("请选择要操作的玩家");
            return;
        }
        if (!battleProxy.playerSitInfoDic.ContainsKey(curSelectPlayerItem.sit))
        {
            PopMsg.Instance.ShowMsg("牌局内不存在当前位置");
            return;
        }
        if (handCardToggle.isOn && curSelectPlayerItem.handCardArr.Count >= GlobalData.PLAYER_CARD_NUM)
        {
            PopMsg.Instance.ShowMsg("当前玩家手牌已达上限");
            return;
        }
        if (getCardToggle.isOn && curSelectPlayerItem.getCard != null)
        {
            PopMsg.Instance.ShowMsg("当前玩家已设置摸牌");
            return;
        }
        if (nextCardToggle.isOn && curSelectPlayerItem.nextCard != null)
        {
            PopMsg.Instance.ShowMsg("当前玩家已设置下轮发牌");
            return;
        }
        int cardValue = int.Parse(poolCard.name.Split('_')[1]);
        battleProxy.cardValuePool.Remove(cardValue);
        if (battleProxy.cardValuePool.Contains(cardValue))
        {
            poolCard.GetComponent<Button>().interactable = true;
            poolCard.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }
        else
        {
            poolCard.GetComponent<Button>().interactable = false;
            poolCard.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        if (handCardToggle.isOn)
        {
            curSelectPlayerItem.AddHandCard(cardValue);
        }
        else if (getCardToggle.isOn)
        {
            curSelectPlayerItem.AddGetCard(cardValue);
        }
        else if (nextCardToggle.isOn)
        {
            curSelectPlayerItem.AddNextCard(cardValue);
        }
    }

    /// <summary>
    /// 选择回调
    /// </summary>
    /// <param name="playerItem"></param>
    private void SelctPlayerItem(DebugPlayerItem playerItem)
    {
        curSelectPlayerItem = playerItem;
        foreach (DebugPlayerItem item in playerItemArr)
        {
            if (item.sit == playerItem.sit)
            {
                item.isSelect = true;
            }
            else
            {
                item.isSelect = false;
            }
        }
    }

    /// <summary>
    /// 返回牌
    /// </summary>
    /// <param name="returnCard"></param>
    private void ReturnCard(int returnCard)
    {
        battleProxy.cardValuePool.Add(returnCard);
        cardPoolDic[returnCard].GetComponent<Button>().interactable = true;
        cardPoolDic[returnCard].GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    /// <summary>
    /// 获取牌面对象
    /// </summary>
    /// <param name="cardValue"></param>
    /// <returns></returns>
    private GameObject GetCardObject(int cardValue)
    {
        GameObject cardPerfab = Resources.Load<GameObject>("Prefab/UI/Debug/CardSelectBtn");
        var cardGameObject = Instantiate(cardPerfab);
        cardGameObject.transform.Find("CardFront").GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("Textures/Card/{0}", cardValue));
        return cardGameObject;
    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// 清除牌面设置
    /// </summary>
    private void ClearCardSet()
    {
        foreach (DebugPlayerItem item in playerItemArr)
        {
            while (item.handCardArr.Count > 0)
            {
                item.RemoveHandCard(item.handCardArr[0]);
            }
            if (item.getCard != null)
            {
                item.RemoveGetCard(item.getCard);
            }
            if (item.nextCard != null)
            {
                item.RemoveNextCard(item.nextCard);
            }
        }
    }

    /// <summary>
    /// 保存牌面设置
    /// </summary>
    private void ConfirmSetCard()
    {
        var setCardC2S = new SetCardC2S();
        foreach (DebugPlayerItem item in playerItemArr)
        {
            if (battleProxy.playerSitInfoDic.ContainsKey(item.sit))
            {
                var playerSet = new PlayerCardSet();
                var playerInfoVOS2C = battleProxy.playerSitInfoDic[item.sit];
                if (item.handCardArr.Count != playerInfoVOS2C.handCards.Count)
                {
                    PopMsg.Instance.ShowMsg(string.Format("{0}的手牌数量跟之前数量不一致", playerInfoVOS2C.name));
                    return;
                }
                if (item.getCard == null && playerInfoVOS2C.getCard != 0)
                {
                    PopMsg.Instance.ShowMsg(string.Format("{0}需要设置摸牌", playerInfoVOS2C.name));
                    return;
                }
                if (item.getCard != null && playerInfoVOS2C.getCard == 0)
                {
                    PopMsg.Instance.ShowMsg(string.Format("{0}手中没有摸牌", playerInfoVOS2C.name));
                    return;
                }
                for (int i = 0; i < item.handCardArr.Count; i++)
                {
                    int itemCardValue = int.Parse(item.handCardArr[i].name.Split('_')[1]);
                    playerSet.handCards.Add(itemCardValue);
                }
                if (item.getCard != null)
                {
                    int itemCardValue = int.Parse(item.getCard.name.Split('_')[1]);
                    playerSet.getCard = itemCardValue;
                }
                if (item.nextCard != null)
                {
                    int itemCardValue = int.Parse(item.nextCard.name.Split('_')[1]);
                    playerSet.nextGetCard = itemCardValue;
                }
                playerSet.sit = item.sit;
                setCardC2S.cardSets.Add(playerSet);
            }               
        }
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_SetCardC2S.GetHashCode(), 0, setCardC2S);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    private void CloseCardSet()
    {
        gameObject.SetActive(false);
    }

}
/// <summary>
/// 选择玩家的委托
/// </summary>
/// <param name="playerItem"></param>
public delegate void SelectPlayerCallBack(DebugPlayerItem playerItem);
/// <summary>
/// 牌返还的委托
/// </summary>
/// <param name="playerItem"></param>
public delegate void ReturnCardCallBack(int returnCard);