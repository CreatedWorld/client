using Platform.Model.Battle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

/// <summary>
/// 调试玩家节点
/// </summary>
public class DebugPlayerItem : MonoBehaviour {
    /// <summary>
    /// 玩家座位号
    /// </summary>
    public int sit;
    private bool _isSelect = false;
    /// <summary>
    /// 选择回调方法
    /// </summary>
    [HideInInspector]
    public SelectPlayerCallBack selectCallBack;
    /// <summary>
    /// 牌返还回调方法
    /// </summary>
    [HideInInspector]
    public ReturnCardCallBack returnCardCallBack;    
    /// <summary>
    /// 手牌容器
    /// </summary>
    private RectTransform handCardContainer;
    /// <summary>
    /// 手牌第一张牌
    /// </summary>
    private RectTransform handFirstCard;
    /// <summary>
    /// 手牌间距
    /// </summary>
    private Vector3 handCardGap;
    /// <summary>
    /// 摸牌容器
    /// </summary>
    private RectTransform getCardContainer;
    /// <summary>
    /// 摸到的第一张牌
    /// </summary>
    private RectTransform getFirstCard;
    /// <summary>
    /// 摸到的牌
    /// </summary>
    [HideInInspector]
    public GameObject getCard;
    /// <summary>
    /// 下轮摸牌容器
    /// </summary>
    private RectTransform nextCardContainer;
    /// <summary>
    /// 下轮摸到的第一张牌
    /// </summary>
    private RectTransform nextFirstCard;
    /// <summary>
    /// 下轮摸到的牌
    /// </summary>
    [HideInInspector]
    public GameObject nextCard;
    /// <summary>
    /// 玩家名字
    /// </summary>
    private Text playerNameTxt;
    /// <summary>
    /// 手牌数组
    /// </summary>
    public List<GameObject> handCardArr = new List<GameObject>();
    /// <summary>
    /// 选择按钮图标
    /// </summary>
    [HideInInspector]
    public Image selectBtnImg;

    // Use this for initialization
    void Start () {
        handCardContainer = transform.Find("HandCardContainer").GetComponent<RectTransform>();
        handFirstCard = transform.Find("HandCardContainer/CardSelectBtn1").GetComponent<RectTransform>();
        var handSecondCard = transform.Find("HandCardContainer/CardSelectBtn2").GetComponent<RectTransform>();
        handCardGap = handSecondCard.localPosition - handFirstCard.localPosition;
        getCardContainer = transform.Find("GetCardContainer").GetComponent<RectTransform>();
        getFirstCard = transform.Find("GetCardContainer/CardSelectBtn").GetComponent<RectTransform>();
        nextCardContainer = transform.Find("NextCardContainer").GetComponent<RectTransform>();
        nextFirstCard = transform.Find("NextCardContainer/CardSelectBtn").GetComponent<RectTransform>();
        playerNameTxt = transform.Find("PlayerNameTxt").GetComponent<Text>();
        selectBtnImg = transform.Find("SelectBtn").GetComponent<Image>();
        transform.Find("SelectBtn").GetComponent<Button>().onClick.AddListener(SelectPlayerItem);
        var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        if (battleProxy.playerSitInfoDic.ContainsKey(sit))
        {
            playerNameTxt.text = battleProxy.playerSitInfoDic[sit].name;
        }
    }

    /// <summary>
    /// 选择玩家
    /// </summary>
    private void SelectPlayerItem()
    {
        selectCallBack(this);
    }
    /// <summary>
    /// 是否选择
    /// </summary>
    [HideInInspector]
    public bool isSelect
    {
        get
        {
            return _isSelect;
        }
        set
        {
            if (value)
            {
                selectBtnImg.color = Color.white;
            }
            else
            {
                selectBtnImg.color = Color.clear;
            }
            _isSelect = value;
        }
    }


    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// 返还所有的牌
    /// </summary>
    public void ReturnAllCard()
    {
        while (handCardArr.Count > 0)
        {
            RemoveHandCard(handCardArr[0]);
        }
        if (getCard != null)
        {
            RemoveGetCard(getCard);
        }
        if (nextCard != null)
        {
            RemoveNextCard(nextCard);
        }
    }


    /// <summary>
    /// 添加牌
    /// </summary>
    /// <param name="cardValue"></param>
    public void AddHandCard(int cardValue)
    {
        var addCard = GetCardObject(cardValue).GetComponent<RectTransform>();
        EventTrigger buttonTrigger = addCard.gameObject.AddComponent<EventTrigger>();
        buttonTrigger.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry pointClick = new EventTrigger.Entry();
        buttonTrigger.triggers.Add(pointClick);
        pointClick.eventID = EventTriggerType.PointerClick;
        addCard.name = "CardPool_" + cardValue;
        addCard.SetParent(handCardContainer);
        addCard.anchorMin = handCardContainer.anchorMin;
        addCard.anchorMax = handCardContainer.anchorMax;
        addCard.localScale = handFirstCard.localScale;
        addCard.localEulerAngles = handFirstCard.localEulerAngles;
        addCard.localPosition = handFirstCard.localPosition + handCardGap * handCardArr.Count;
        handCardArr.Add(addCard.gameObject);
        pointClick.callback.AddListener(RemoveHandCard);
    }

    /// <summary>
    /// 添加摸牌
    /// </summary>
    /// <param name="cardValue"></param>
    public void AddGetCard(int cardValue)
    {
        var addCard = GetCardObject(cardValue).GetComponent<RectTransform>();
        EventTrigger buttonTrigger = addCard.gameObject.AddComponent<EventTrigger>();
        buttonTrigger.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry pointClick = new EventTrigger.Entry();
        buttonTrigger.triggers.Add(pointClick);
        pointClick.eventID = EventTriggerType.PointerClick;
        addCard.name = "CardPool_" + cardValue;
        addCard.SetParent(getCardContainer);
        addCard.anchorMin = getCardContainer.anchorMin;
        addCard.anchorMax = getCardContainer.anchorMax;
        addCard.localScale = getFirstCard.localScale;
        addCard.localEulerAngles = getFirstCard.localEulerAngles;
        addCard.localPosition = getFirstCard.localPosition;
        getCard = addCard.gameObject;
        pointClick.callback.AddListener(RemoveGetCard);
    }

    /// <summary>
    /// 添加下轮摸牌
    /// </summary>
    /// <param name="cardValue"></param>
    public void AddNextCard(int cardValue)
    {
        var addCard = GetCardObject(cardValue).GetComponent<RectTransform>();
        EventTrigger buttonTrigger = addCard.gameObject.AddComponent<EventTrigger>();
        buttonTrigger.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry pointClick = new EventTrigger.Entry();
        buttonTrigger.triggers.Add(pointClick);
        pointClick.eventID = EventTriggerType.PointerClick;
        addCard.name = "CardPool_" + cardValue;
        addCard.SetParent(nextCardContainer);
        addCard.anchorMin = nextCardContainer.anchorMin;
        addCard.anchorMax = nextCardContainer.anchorMax;
        addCard.localScale = nextFirstCard.localScale;
        addCard.localEulerAngles = nextFirstCard.localEulerAngles;
        addCard.localPosition = nextFirstCard.localPosition;
        nextCard = addCard.gameObject;
        pointClick.callback.AddListener(RemoveNextCard);
    }

    /// <summary>
    /// 移除手牌
    /// </summary>
    /// <param name="arg0"></param>
    private void RemoveHandCard(BaseEventData arg0)
    {
        var removeCard = arg0.selectedObject;
        RemoveHandCard(removeCard);
    }

    /// <summary>
    /// 移除手牌
    /// </summary>
    /// <param name="removeCard"></param>
    public void RemoveHandCard(GameObject removeCard)
    {
        int cardValue = int.Parse(removeCard.name.Split('_')[1]);
        foreach (GameObject item in handCardArr)
        {
            int itemCardValue = int.Parse(item.name.Split('_')[1]);
            if (itemCardValue == cardValue)
            {
                handCardArr.Remove(item);
                GameObject.Destroy(item);
                break;
            }
        }
        for (int i = 0; i < handCardArr.Count; i++)
        {
            var item = handCardArr[i];
            item.GetComponent<RectTransform>().localPosition = handFirstCard.localPosition + handCardGap * i;
        }
        returnCardCallBack(cardValue);
    }

    /// <summary>
    /// 移除已摸牌
    /// </summary>
    /// <param name="arg0"></param>
    private void RemoveGetCard(BaseEventData arg0)
    {
        var removeCard = arg0.selectedObject;
        RemoveGetCard(removeCard);
    }

    /// <summary>
    /// 移除已摸牌
    /// </summary>
    /// <param name="removeCard"></param>
    public void RemoveGetCard(GameObject removeCard)
    {
        int cardValue = int.Parse(removeCard.name.Split('_')[1]);
        GameObject.Destroy(getCard);
        getCard = null;
        returnCardCallBack(cardValue);
    }

    /// <summary>
    /// 移除下轮摸牌
    /// </summary>
    /// <param name="arg0"></param>
    private void RemoveNextCard(BaseEventData arg0)
    {
        var removeCard = arg0.selectedObject;
        RemoveNextCard(removeCard);
    }

    /// <summary>
    /// 移除下轮摸牌
    /// </summary>
    /// <param name="removeCard"></param>
    public void RemoveNextCard(GameObject removeCard)
    {
        int cardValue = int.Parse(removeCard.name.Split('_')[1]);
        GameObject.Destroy(nextCard);
        nextCard = null;
        returnCardCallBack(cardValue);
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
}
