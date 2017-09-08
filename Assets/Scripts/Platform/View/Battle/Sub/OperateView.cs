using Platform.Model;
using Platform.Model.Battle;
using Platform.Net;
using Platform.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 出牌操作和显示UI
/// </summary>
public class OperateView : MonoBehaviour {
    /// <summary>
    /// 战斗模块数据
    /// </summary>
    private BattleProxy battleProxy;
    /// <summary>
    /// 操作提示按钮
    /// </summary>
    private List<Button> actionBtns;
    /// <summary>
    /// 操作提示按钮容器
    /// </summary>
    private List<RectTransform> actionBtnContainers;
    /// <summary>
    /// 听牌的图标
    /// </summary>
    private GameObject tingIcon;
    // Use this for initialization
    void Start () {
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        var actionBtn1 = transform.Find("ActionBtnContainer1/ActionBtn1").GetComponent<Button>();
        var actionBtn2 = transform.Find("ActionBtnContainer2/ActionBtn2").GetComponent<Button>();
        var actionBtn3 = transform.Find("ActionBtnContainer3/ActionBtn3").GetComponent<Button>();
        var actionBtn4 = transform.Find("ActionBtnContainer4/ActionBtn4").GetComponent<Button>();
        var actionBtn5 = transform.Find("ActionBtnContainer5/ActionBtn5").GetComponent<Button>();
        actionBtns = new List<Button>();
        actionBtnContainers = new List<RectTransform>();
        actionBtns.Add(actionBtn1);
        actionBtns.Add(actionBtn2);
        actionBtns.Add(actionBtn3);
        actionBtns.Add(actionBtn4);
        actionBtns.Add(actionBtn5);
        actionBtns[0].onClick.AddListener(() => { ActHandler(0); });
        actionBtns[1].onClick.AddListener(() => { ActHandler(1); });
        actionBtns[2].onClick.AddListener(() => { ActHandler(2); });
        actionBtns[3].onClick.AddListener(() => { ActHandler(3); });
        actionBtns[4].onClick.AddListener(() => { ActHandler(4); });
        actionBtnContainers.Add(transform.Find("ActionBtnContainer1").GetComponent<RectTransform>());
        actionBtnContainers.Add(transform.Find("ActionBtnContainer2").GetComponent<RectTransform>());
        actionBtnContainers.Add(transform.Find("ActionBtnContainer3").GetComponent<RectTransform>());
        actionBtnContainers.Add(transform.Find("ActionBtnContainer4").GetComponent<RectTransform>());
        actionBtnContainers.Add(transform.Find("ActionBtnContainer5").GetComponent<RectTransform>());
        tingIcon = transform.Find("TingIcon").gameObject;
        UpdateTingIcon();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 听牌的卡牌数组
    /// </summary>
    private List<GameObject> tingCards = new List<GameObject>();
    /// <summary>
    /// 更新听牌数组
    /// </summary>
    public void UpdateTingIcon()
    {
        ClearTingCardBtns();
        if (battleProxy.tingCards.Count == 0)
        {
            tingIcon.SetActive(false);
            return;
        }
        tingIcon.SetActive(true);
        int startX = 91;
        for (int i = 0; i < battleProxy.tingCards.Count; i++)
        {
            var cardGameObject = ResourcesMgr.Instance.GetCardBtnFromPool(battleProxy.tingCards[i]);
            cardGameObject.transform.SetParent(tingIcon.transform);
            cardGameObject.transform.localScale = Vector3.one;
            cardGameObject.transform.localPosition = new Vector3(startX, 0, 0);
            startX += 78;
            if (i < battleProxy.tingCards.Count - 1)
            {
                startX += 5;
            }
            tingCards.Add(cardGameObject);
        }
    }

    /// <summary>
    /// 清除显示的停牌
    /// </summary>
    private void ClearTingCardBtns()
    {
        foreach (GameObject btn in tingCards)
        {
            ResourcesMgr.Instance.AddBtn2Pool(btn);
        }
        tingCards.Clear();
    }

    /// <summary>
    /// 动作提示数组
    /// </summary>
    private List<OperateType> operates;
    /// <summary>
    /// 动作提示的牌数组
    /// </summary>
    private Dictionary<OperateType, List<int>> operateCards;
    /// <summary>
    /// 动作提示对应的后端操作数组
    /// </summary>
    private Dictionary<OperateType, List<PlayerActType>> operateActs;
    /// <summary>
    /// 提示点击的牌数组
    /// </summary>
    private List<GameObject> operateCardBtns;
    /// <summary>
    /// 可以吃的牌数组
    /// </summary>
    private List<List<int>> chiSelectArr;
    /// <summary>
    /// 显示玩家操作提示
    /// </summary>
    public void ShowPlayActTip()
    {
        operates = new List<OperateType>();
        operateCards = new Dictionary<OperateType, List<int>>();
        operateActs = new Dictionary<OperateType, List<PlayerActType>>();
        operateCardBtns = new List<GameObject>();
        for (int i = 0; i < battleProxy.GetPlayerActTipS2C().acts.Count; i++)
        {
            PlayerActType actType = battleProxy.GetPlayerActTipS2C().acts[i];
            OperateType operateType = OperateType.PASS;
            if (actType == PlayerActType.PUT_CARD)
            {
                continue;
            }
            switch (actType)
            {
                case PlayerActType.ZHI_GANG:
                case PlayerActType.BACK_AN_GANG:
                case PlayerActType.COMMON_AN_GANG:
                case PlayerActType.BACK_PENG_GANG:
                case PlayerActType.COMMON_PENG_GANG:
                    operateType = OperateType.GANG;
                    break;
                case PlayerActType.SELF_HU:
                case PlayerActType.QIANG_ZHI_GANG_HU:
                case PlayerActType.QIANG_PENG_GANG_HU:
                case PlayerActType.QIANG_AN_GANG_HU:
                case PlayerActType.CHI_HU:
                    operateType = OperateType.HU;
                    break;
                case PlayerActType.PASS:
                    operateType = OperateType.PASS;
                    break;
                case PlayerActType.PENG:
                    operateType = OperateType.PENG;
                    break;
                case PlayerActType.CHI:
                    operateType = OperateType.CHI;
                    chiSelectArr = BattleAreaUtil.GetCanChiArr(battleProxy.GetPlayerActTipS2C().actCards[i]);
                    break;
            }
            if (operates.IndexOf(operateType) == -1)
            {
                operates.Add(operateType);
                operateCards.Add(operateType, new List<int>());
                operateActs.Add(operateType, new List<PlayerActType>());
            }
            operateCards[operateType].Add(battleProxy.GetPlayerActTipS2C().actCards[i]);
            operateActs[operateType].Add(actType);
        }
        for (int i = 0; i < actionBtns.Count; i++)
        {
            if (i >= operateActs.Count)
            {
                actionBtnContainers[i].gameObject.SetActive(false);
                continue;
            }
            actionBtnContainers[i].gameObject.SetActive(true);
            switch (operates[i])
            {
                case OperateType.GANG:
                    actionBtns[i].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/ActionGang");
                    break;                                                                                   
                case OperateType.HU:                                                                          
                    actionBtns[i].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/ActionHu");
                    break;                                                                                   
                case OperateType.PASS:                                                                        
                    actionBtns[i].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/ActionGuo");
                    break;                                                                                   
                case OperateType.PENG:                                                                       
                    actionBtns[i].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/ActionPeng");
                    break;                                                                                   
                case OperateType.CHI:                                                                         
                    actionBtns[i].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/ActionChi");
                    break;
            }
            actionBtns[i].gameObject.GetComponent<Animator>().Play("ShowActBtn" + (i + 1), 0, 0);
        }
        for (int i = 0; i < operateActs.Count; i++)
        {
            var perPosition = actionBtnContainers[i].localPosition;
            perPosition = new Vector3(503.3f - 163.8f * (operateActs.Count - i - 1), perPosition.y, perPosition.z);
            actionBtnContainers[i].localPosition = perPosition;
        }
    }

    /// <summary>
    /// 上次点击准备的时间
    /// </summary>
    private float perClickTime = 0;
    /// <summary>
    /// 点击动作按钮
    /// </summary>
    /// <param name="btnIndex"></param>
    private void ActHandler(int btnIndex)
    {
        if (Time.time - perClickTime < 1)
        {
            return;
        }
        perClickTime = Time.time;
        ClearOperateCardBtns();
        var operateType = operates[btnIndex];
        if (operateType == OperateType.CHI)//吃牌按钮特殊操作
        {
            if (chiSelectArr.Count > 1) //有多种吃牌选择
            {
                ShowChiSelectView(btnIndex);
            }
            else
            {
                onChiClick(PlayerActType.CHI, chiSelectArr[0], btnIndex);
            }
        }
        else
        {
            if (operateCards[operateType].Count > 1) //操作的牌有多张
            {
                ShowOperateSelectView(operateType, btnIndex);
            }
            else
            {
                OperateSingle(operateActs[operateType][0], operateCards[operateType][0], btnIndex);
            }
        }
    }

    /// <summary>
    /// 显示单个操作的候选按钮
    /// </summary>
    /// <param name="operate"></param>
    private void ShowOperateSelectView(OperateType operate, int btnIndex)
    {
        var cardNum = operateCards[operate].Count;
        var clickBtn = actionBtns[btnIndex];
        float startX = -(cardNum * 76 + (cardNum - 1) * 5) / 2;
        for (int i = 0; i < cardNum; i++)
        {
            var card = operateCards[operate][i];
            var act = operateActs[operate][i];
            var cardGameObject = ResourcesMgr.Instance.GetCardBtnFromPool(card);
            cardGameObject.GetComponent<Button>().onClick.AddListener(() => { OperateSingle(act, card, btnIndex); });
            cardGameObject.transform.SetParent(clickBtn.transform);
            cardGameObject.transform.localScale = Vector3.one;
            cardGameObject.transform.localPosition = new Vector3(startX + 38, 121, 0);
            startX += 78;
            if (startX < cardNum - 1)
            {
                startX += 5;
            }
            operateCardBtns.Add(cardGameObject);
        }
    }

    /// <summary>
    /// 显示吃牌操作的候选按钮
    /// </summary>
    /// <param name="operate"></param>
    private void ShowChiSelectView(int btnIndex)
    {
        var chiSelectNum = chiSelectArr.Count;
        var clickBtn = actionBtns[btnIndex];
        float startX = -(chiSelectNum * 157 + (chiSelectNum - 1) * 20) / 2;
        for (int i = 0; i < chiSelectNum; i++)
        {
            for (int j = 0; j < chiSelectArr[i].Count; j++)
            {
                var canChiArr = chiSelectArr[i];
                var cardGameObject = ResourcesMgr.Instance.GetCardBtnFromPool(chiSelectArr[i][j]);
                cardGameObject.GetComponent<Button>().onClick.AddListener(() => { onChiClick(PlayerActType.CHI, canChiArr, btnIndex); });
                cardGameObject.transform.SetParent(clickBtn.transform);
                cardGameObject.transform.localScale = Vector3.one;
                cardGameObject.transform.localPosition = new Vector3(startX + 38, 121, 0);
                startX += 78;
                if (j < chiSelectArr[i].Count - 1)
                {
                    startX += 5;
                }
                operateCardBtns.Add(cardGameObject);
            }
            if (startX < chiSelectNum - 1)
            {
                startX += 20;
            }
        }
    }

    /// <summary>
    /// 操作单张牌
    /// </summary>
    /// <param name="act"></param>
    /// <param name="card"></param>
    private void OperateSingle(PlayerActType act, int card, int btnIndex)
    {
        switch (act)
        {
            case PlayerActType.ZHI_GANG:
                onZhiGangClick(act, card);
                break;
            case PlayerActType.BACK_AN_GANG:
            case PlayerActType.COMMON_AN_GANG:
                onAnGangClick(act, card);
                break;
            case PlayerActType.BACK_PENG_GANG:
            case PlayerActType.COMMON_PENG_GANG:
                onPengGangClick(act, card);
                break;
            case PlayerActType.SELF_HU:
            case PlayerActType.QIANG_ZHI_GANG_HU:
            case PlayerActType.QIANG_PENG_GANG_HU:
            case PlayerActType.QIANG_AN_GANG_HU:
            case PlayerActType.CHI_HU:
                onHuClick(act, card);
                break;
            case PlayerActType.PASS:
                onPassClick();
                break;
            case PlayerActType.PENG:
                onPengClick(act, card);
                break;
        }
        ClearOperateCardBtns();
    }

    /// <summary>
    /// 清除显示的候选牌
    /// </summary>
    private void ClearOperateCardBtns()
    {
        foreach (GameObject btn in operateCardBtns)
        {
            ResourcesMgr.Instance.AddBtn2Pool(btn);
        }
        operateCardBtns.Clear();
    }


    /// <summary>
    /// 隐藏玩家操作提示
    /// </summary>
    public void HidenPlayActTip()
    {
        foreach (RectTransform actionBtnContainer in actionBtnContainers)
        {
            actionBtnContainer.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 点击直杠按钮
    /// </summary>
    private void onZhiGangClick(PlayerActType act, int card)
    {
        var actC2S = new ZhiGangC2S();
        actC2S.mahjongCode = card;
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_ZHI_GANG.GetHashCode(), 0, actC2S);
    }
    /// <summary>
    /// 点击暗杠按钮
    /// </summary>
    private void onAnGangClick(PlayerActType act, int card)
    {
        if (act == PlayerActType.COMMON_AN_GANG)
        {
            var actC2S = new CommonAnGangC2S();
            actC2S.mahjongCode = card;
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_COMMON_AN_GANG.GetHashCode(), 0, actC2S);
        }
        else if (act == PlayerActType.BACK_AN_GANG)
        {
            var actC2S = new BackAnGangC2S();
            actC2S.mahjongCode = card;
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_BACK_AN_GANG.GetHashCode(), 0, actC2S);
        }
    }
    /// <summary>
    /// 点击碰杠按钮
    /// </summary>
    private void onPengGangClick(PlayerActType act, int card)
    {
        if (act == PlayerActType.COMMON_PENG_GANG)
        {
            var actC2S = new CommonPengGangC2S();
            actC2S.mahjongCode = card;
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_COMMON_PENG_GANG.GetHashCode(), 0, actC2S);
        }
        else if (act == PlayerActType.BACK_PENG_GANG)
        {
            var actC2S = new BackPengGangC2S();
            actC2S.mahjongCode = card;
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_BACK_PENG_GANG.GetHashCode(), 0, actC2S);
        }
    }

    /// <summary>
    /// 点击胡按钮
    /// </summary>
    private void onHuClick(PlayerActType act, int card)
    {
        if (act == PlayerActType.SELF_HU)
        {
            var actC2S = new ZiMoHuC2S();
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_ZI_MO_HU.GetHashCode(), 0, actC2S);
        }
        else if (act == PlayerActType.QIANG_AN_GANG_HU)
        {
            var actC2S = new QiangAnGangHuC2S();
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_AN_GANG_HU.GetHashCode(), 0, actC2S);
        }
        else if (act == PlayerActType.QIANG_PENG_GANG_HU)
        {
            var actC2S = new QiangPengGangHuC2S();
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_PENG_GANG_HU.GetHashCode(), 0, actC2S);
        }
        else if (act == PlayerActType.QIANG_ZHI_GANG_HU)
        {
            var actC2S = new QiangZhiGangHuC2S();
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_QIANG_ZHI_GANG_HU.GetHashCode(), 0, actC2S);
        }
        else if (act == PlayerActType.CHI_HU)
        {
            var actC2S = new ChiHuC2S();
            NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_CHI_HU.GetHashCode(), 0, actC2S);
        }
    }

    /// <summary>
    /// 点击过按钮
    /// </summary>
    private void onPassClick()
    {
        var actC2S = new GuoC2S();
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PASS.GetHashCode(), 0, actC2S);
    }

    /// <summary>
    /// 点击碰按钮
    /// </summary>
    private void onPengClick(PlayerActType act, int card)
    {
        var actC2S = new PengC2S();
        actC2S.mahjongCode = card;
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_PENG.GetHashCode(), 0, actC2S);
    }

    /// <summary>
    /// 点击吃按钮
    /// </summary>
    /// <param name="act"></param>
    /// <param name="cards"></param>
    private void onChiClick(PlayerActType act, List<int> cards, int btnIndex)
    {
        var actC2S = new ChiC2S();
        for (int i = 0; i < cards.Count; i++)
        {
            actC2S.mahjongCodes.Add(cards[i]);
        }
        var card = battleProxy.GetPlayerActTipS2C().actCards[btnIndex];
        actC2S.mahjongCodes.Add(card);//将自己吃的牌放进数组
        actC2S.forbitCards.Add(card);
        if (cards[0] == card + 1)
        {
            if (Array.IndexOf(GlobalData.CardValues, card + 3) != -1)
            {
                actC2S.forbitCards.Add(card + 3);
            }
        }
        else if (cards[0] == card - 2)
        {
            if (Array.IndexOf(GlobalData.CardValues, card - 3) != -1)
            {
                actC2S.forbitCards.Add(card - 3);
            }
        }
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_CHI.GetHashCode(), 0, actC2S);
    }
}
