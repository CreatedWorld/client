using Platform.Model.Battle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platform.Utils
{
    /// <summary>
    /// 牌面排序相关工具类
    /// </summary>
    class BattleAreaUtil
    {
        /// <summary>
        /// 获取麻将对应的牌值
        /// </summary>
        /// <param name="obj">麻将对象</param>
        /// <returns></returns>
        public static int GetMeshCardValue(GameObject obj)
        {
            var meshFilter = obj.GetComponent<MeshFilter>();
            var nameIndex = Array.IndexOf(GlobalData.MeshNames, meshFilter.sharedMesh.name);
            return GlobalData.CardValues[nameIndex];
        }

        /// <summary>
        /// 重排牌的位置
        /// </summary>
        public static void ResortCard(BattleAreaItem areaItem)
        {
            ResortHandGangGetCard(areaItem);
            ResortPutCard(areaItem);
        }

        /// <summary>
        /// 重排碰杠 手牌 摸到的牌位置
        /// </summary>
        public static void ResortHandGangGetCard(BattleAreaItem areaItem)
        {
            var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            areaItem.selectCard = null;
            //碰杠牌排序
            Vector3 pengGangPos = Vector3.zero;
            GameObject lastPengGangCard = null;
            int covertIndex = -1;//需要盖住的牌面序号
            var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
            if (areaItem.pengGangCards.Count != areaItem.data.pengGangCards.Count)
            {
                Debug.LogError("显示与数据不一致");
            }
            for (int i = 0; i < areaItem.pengGangCards.Count; i++)
            {
                if (i > 0)
                {
                    pengGangPos += areaItem.pengGangGap * 1.5f;//1.2f
                }
                if (areaItem.data.pengGangCards[i].targetUserId == areaItem.data.userId || areaItem.data.pengGangCards[i].targetUserId == 0)
                {
                    covertIndex = -1;
                }
                else if(battleProxy.playerIdInfoDic.ContainsKey(areaItem.data.pengGangCards[i].targetUserId))
                {
                    var targetPlayerInfoVO = battleProxy.playerIdInfoDic[areaItem.data.pengGangCards[i].targetUserId];
                    var targetIndex = (targetPlayerInfoVO.sit - areaItem.data.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
                    covertIndex = GlobalData.SIT_NUM - targetIndex;
                }
                if (areaItem.pengGangCards[i].Count != areaItem.data.pengGangCards[i].pengGangCards.Count)
                {
                    Debug.LogError("显示与数据不一致");
                }
                for (int j = 0; j < areaItem.pengGangCards[i].Count; j++)
                {
                    if (j > 0)
                    {
                        pengGangPos += areaItem.pengGangGap;
                    }
                    areaItem.pengGangCards[i][j].transform.SetParent(areaItem.pengGangCardContainer);
                    areaItem.pengGangCards[i][j].transform.localScale = Vector3.one;
                    areaItem.pengGangCards[i][j].transform.localEulerAngles = Vector3.zero;
                    if (j == 1)
                    {
                        var targetPlayerInfoVO = battleProxy.playerIdInfoDic[areaItem.data.pengGangCards[i].targetUserId];
                        var targetIndex = targetPlayerInfoVO.sit;//(targetPlayerInfoVO.sit - areaItem.data.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
                        
                        GameObject go = new GameObject();
                        go.AddComponent<SpriteRenderer>();
                        go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/right@2x");
                        go.transform.SetParent(areaItem.pengGangCards[i][j].transform);
                        if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].sit == 1)//自己是东家
                        {
                            if (targetPlayerInfoVO.sit == 1)//被碰的人不会是自己
                            {
                                Debug.Log("sitIndex = 下边");
                                go.transform.localEulerAngles = new Vector3(0, 0, 0);
                            }
                            if (targetPlayerInfoVO.sit == 2)//往右-北家
                            {
                                Debug.Log("sitIndex = 右边");
                                go.transform.localEulerAngles = new Vector3(90, 0, 180);
                            }
                            if (targetPlayerInfoVO.sit == 3)
                            {
                                Debug.Log("sitIndex = 中间");
                                go.transform.localEulerAngles = new Vector3(90, 0, 90);
                            }
                            if (targetPlayerInfoVO.sit == 4)
                            {
                                Debug.Log("sitIndex = 左边");
                                go.transform.localEulerAngles = new Vector3(90, 0, 0);
                            }
                        }
                        else if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].sit == 2)//北家
                        {
                            if (targetPlayerInfoVO.sit == 1)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, -90);
                            }
                            if (targetPlayerInfoVO.sit == 2)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, 180);
                            }
                            if (targetPlayerInfoVO.sit == 3)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, 90);
                            }
                            if (targetPlayerInfoVO.sit == 4)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, 0);
                            }
                        }
                        else if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].sit == 3)
                        {
                            if (targetPlayerInfoVO.sit == 1)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, 180);
                            }
                            if (targetPlayerInfoVO.sit == 2)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, 90);
                            }
                            if (targetPlayerInfoVO.sit == 3)
                            {
                                go.transform.localEulerAngles = new Vector3(0, 0, -0);
                            }
                            if (targetPlayerInfoVO.sit == 4)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, -90);
                            }
                        }
                        else if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].sit == 4)//南家
                        {
                            if (targetPlayerInfoVO.sit == 1)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, -90);//往右-东家
                            }
                            if (targetPlayerInfoVO.sit == 2)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, 180);//往上-北家
                            }
                            if (targetPlayerInfoVO.sit == 3)
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, 90);//往左-西家
                            }
                            if (targetPlayerInfoVO.sit == 4)//被碰的人不会是自己
                            {
                                go.transform.localEulerAngles = new Vector3(90, 0, -90);
                            }
                        }
                        //if (targetIndex == 1) {
                        //    Debug.Log("sitIndex = 右边");
                        //     go.transform.localEulerAngles = new Vector3(90, 0, 180); 
                        //}
                        //if (targetIndex == 2) {
                        //    Debug.Log("sitIndex = 中间");
                        //    go.transform.localEulerAngles = new Vector3(90, 0, 90);
                        //}
                        //if (targetIndex == 3) {
                        //    Debug.Log("sitIndex = 左边");
                        //    go.transform.localEulerAngles = new Vector3(90, 0, 0); 
                        //}

                        go.transform.localPosition = new Vector3(0,-0.197f,0);
                        go.transform.localScale = Vector3.one;
                        if (AreaDir.DOWN == areaItem.dir)
                        {
                            go.layer = GlobalData.SELF_HAND_CARDS;
                        }
                        else
                        {
                            go.layer = GlobalData.OTHER_CARDS;
                        }
                    }
                    
                    areaItem.pengGangCards[i][j].transform.localPosition = pengGangPos;
                    areaItem.pengGangCards[i][j].layer = GlobalData.OTHER_CARDS;
                    lastPengGangCard = areaItem.pengGangCards[i][j];

                    if (AreaDir.DOWN == areaItem.dir)
                    {
                        areaItem.pengGangCards[i][j].layer = GlobalData.SELF_HAND_CARDS;
                    }
                }
                
            }
            //手中的牌排序
            Vector3 handPos = Vector3.zero;
            if (lastPengGangCard != null)
            {
                Vector3 pengGangScreenPos = Camera.main.WorldToScreenPoint(lastPengGangCard.transform.position);
                Vector3 handScreenPos = Camera.main.WorldToScreenPoint(areaItem.firstCard.position);
                pengGangScreenPos.z = handScreenPos.z;
                Vector3 pengGangWorldPos = Camera.main.ScreenToWorldPoint(pengGangScreenPos);
                handPos = areaItem.handCardContainer.InverseTransformPoint(pengGangWorldPos);
                handPos += areaItem.handCardGap;
                handPos.y = 0;
                handPos.z = 0;
            }
            if (areaItem.handCards.Count != areaItem.data.handCards.Count)
            {
                Debug.LogError("显示与数据不一致");
            }
            for (int i = 0; i < areaItem.handCards.Count; i++)
            {
                areaItem.handCards[i].transform.SetParent(areaItem.handCardContainer);
                areaItem.handCards[i].transform.localScale = Vector3.one;
                SetHandCardAngles(areaItem, areaItem.handCards[i]);
                areaItem.handCards[i].transform.localPosition = handPos;
                if (areaItem.data.userId == playerInfoProxy.userID)
                {
                    areaItem.handCards[i].layer = GlobalData.SELF_HAND_CARDS;
                }
                else if (areaItem.dir == AreaDir.RIGHT)
                {
                    areaItem.handCards[i].layer = GlobalData.RIGHT_HAND_CARDS;
                }
                else
                {
                    areaItem.handCards[i].layer = GlobalData.OTHER_CARDS;
                }
                if (i + 1 < areaItem.handCards.Count)
                {
                    handPos += areaItem.handCardGap;
                }
                else
                {
                    handPos += areaItem.getHandCardGap;
                }
            }
            //摸到的牌
            if (areaItem.getCard != null)
            {
                if (areaItem.data.getCard == 0)
                {
                    Debug.LogError("显示与数据不一致");
                }
                areaItem.getCard.transform.SetParent(areaItem.handCardContainer);
                areaItem.getCard.transform.localScale = Vector3.one;
                SetHandCardAngles(areaItem, areaItem.getCard);
                areaItem.getCard.transform.localPosition = handPos;
                if (areaItem.data.userId == playerInfoProxy.userID)
                {
                    areaItem.getCard.layer = GlobalData.SELF_HAND_CARDS;
                }
                else if (areaItem.dir == AreaDir.RIGHT)
                {
                    areaItem.getCard.layer = GlobalData.RIGHT_HAND_CARDS;
                }
                else
                {
                    areaItem.getCard.layer = GlobalData.OTHER_CARDS;
                }
            }
            else
            {
                if (areaItem.data.getCard != 0)
                {
                    Debug.LogError("显示与数据不一致");
                }
            }
        }

        /// <summary>
        /// 重排已出的牌
        /// </summary>
        public static void ResortPutCard(BattleAreaItem areaItem)
        {
            for (int i = 0; i < areaItem.putCards.Count; i++)
            {
                var anglePosition = GetPutCardPosition(areaItem, i);
                areaItem.putCards[i].transform.SetParent(areaItem.putCardContainer);
                areaItem.putCards[i].transform.localScale = Vector3.one;
                areaItem.putCards[i].transform.localEulerAngles = anglePosition[0];
                areaItem.putCards[i].transform.localPosition = anglePosition[1];
                areaItem.putCards[i].layer = GlobalData.OTHER_CARDS;
            }
        }

        /// <summary>
        /// 获取当前牌的角度和坐标
        /// </summary>
        /// <param name="areaItem"></param>
        /// <param name="cardIndex"></param>
        /// <returns></returns>
        public static List<Vector3> GetPutCardPosition(BattleAreaItem areaItem,int cardIndex)
        {
            var anglePositionArr = new List<Vector3>();
            var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
            int column = cardIndex % areaItem.putCardHNum;
            int row = cardIndex / areaItem.putCardHNum;
            var sitIndex = (areaItem.data.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
            if (sitIndex == 2)//顶部
            {
                anglePositionArr.Add(new Vector3(0, -180, 0));
            }
            else
            {
                anglePositionArr.Add(Vector3.zero);
            }
            anglePositionArr.Add(column* areaItem.putCardHGap + row * areaItem.putCardVGap);
            return anglePositionArr;
        }

        /// <summary>
        /// 牌面排序
        /// </summary>
        public static int CompareCard(GameObject card1, GameObject card2)
        {
            var cardValue1 = GetMeshCardValue(card1);
            var cardValue2 = GetMeshCardValue(card2);
            
            if (cardValue1 < cardValue2)
            {
                return -1;
            }
            else if (cardValue1 > cardValue2)
            {
                return 1;
            }
            else
            {
                int cardIndex1 = int.Parse(card1.name.Replace("HandCard", ""));
                int cardIndex2 = int.Parse(card2.name.Replace("HandCard", ""));
                return cardIndex1 < cardIndex2 ? -1 : 1;
            }
        }

        /// <summary>
        /// 初始化所有牌面
        /// </summary>
        /// <param name="areaItem"></param>
        /// <param name="isOnlyInitHandCard">是否只生成手牌</param>
        public static void InitPlayerCards(BattleAreaItem areaItem,bool isOnlyInitHandCard)
        {
            var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            //恢复碰杠的牌
            Vector3 pengGangPos = Vector3.zero;
            GameObject lastPengGangCard = null;
            var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
            if (isOnlyInitHandCard && areaItem.pengGangCards.Count > 0)
            {
                var pengGangCards = areaItem.pengGangCards[areaItem.pengGangCards.Count - 1];
                lastPengGangCard = pengGangCards[pengGangCards.Count - 1];
            }
            else
            {
                int covertIndex = -1;//需要盖住的牌面序号
                for (int i = 0; i < areaItem.data.pengGangCards.Count; i++)
                {
                    if (i > 0)
                    {
                        pengGangPos += areaItem.pengGangGap * 1.2f;
                    }
                    if (areaItem.data.pengGangCards[i].targetUserId == areaItem.data.userId || areaItem.data.pengGangCards[i].targetUserId == 0)
                    {
                        covertIndex = -1;
                    }
                    else
                    {
                        var targetPlayerInfoVO = battleProxy.playerIdInfoDic[areaItem.data.pengGangCards[i].targetUserId];
                        var targetIndex = (targetPlayerInfoVO.sit - areaItem.data.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
                        covertIndex = GlobalData.SIT_NUM - targetIndex;
                    }

                    var cards = new List<GameObject>();
                    for (int j = 0; j < areaItem.data.pengGangCards[i].pengGangCards.Count; j++)
                    {
                        var card = ResourcesMgr.Instance.GetFromPool(areaItem.data.pengGangCards[i].pengGangCards[j]);
                        if (j > 0)
                        {
                            pengGangPos += areaItem.pengGangGap;
                           
                        }
                        card.transform.SetParent(areaItem.pengGangCardContainer);
                        card.transform.localScale = Vector3.one;
                        if (j == 1)
                        {
                            var targetPlayerInfoVO = battleProxy.playerIdInfoDic[areaItem.data.pengGangCards[i].targetUserId];
                            var targetIndex = (targetPlayerInfoVO.sit - areaItem.data.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
                            GameObject go = new GameObject();
                            go.AddComponent<SpriteRenderer>();
                            if (targetIndex == 1)
                            {
                                Debug.Log("sitIndex = 右边");

                                go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/right@2x");
                                go.transform.SetParent(card.transform);
                                if (areaItem.dir == AreaDir.UP)
                                {
                                    go.transform.localEulerAngles = new Vector3(90, 0, 0); 
                                }
                                if (areaItem.dir == AreaDir.DOWN) { go.transform.localEulerAngles = new Vector3(90, 0, 180); }
                            }
                            if (targetIndex == 2)
                            {
                                Debug.Log("sitIndex = 中间");
                                go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/right@2x");
                                go.transform.SetParent(card.transform);
                                go.transform.localEulerAngles = new Vector3(90, 0, 90);
                            }
                            if (targetIndex == 3)
                            {
                                Debug.Log("sitIndex = 左边");
                                go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/91city_chipenggang/right@2x");
                                go.transform.SetParent(card.transform);
                                if (areaItem.dir == AreaDir.UP)
                                {
                                    go.transform.localEulerAngles = new Vector3(90, 0, 180); 
                                }
                                if (areaItem.dir == AreaDir.DOWN) { go.transform.localEulerAngles = new Vector3(90, 0, 0); }
                            }
                            go.transform.localPosition = new Vector3(0, -0.197f, 0);
                            go.transform.localScale = Vector3.one;
                            if (AreaDir.DOWN == areaItem.dir)
                            {
                                go.layer = GlobalData.SELF_HAND_CARDS;
                            }
                            else
                            {
                                go.layer = GlobalData.OTHER_CARDS;
                            }
                        }

                        if (covertIndex == -1 && areaItem.data.userId != playerInfoProxy.userID)
                        {
                            //card.transform.localEulerAngles = new Vector3(180, 0, 0);
                        }
                        card.transform.localEulerAngles = Vector3.one;
                        card.transform.localPosition = pengGangPos;
                        card.layer = GlobalData.OTHER_CARDS;
                        lastPengGangCard = card;
                        cards.Add(card);

                        if (areaItem.dir == AreaDir.DOWN)
                        {
                            card.layer = GlobalData.SELF_HAND_CARDS;

                        }
                    }
                    areaItem.pengGangCards.Add(cards);
                }
            }
            
            //恢复手中的牌
            Vector3 handPos = Vector3.zero;
            if (lastPengGangCard != null)
            {
                Vector3 pengGangScreenPos = Camera.main.WorldToScreenPoint(lastPengGangCard.transform.position);
                Vector3 handScreenPos = Camera.main.WorldToScreenPoint(areaItem.firstCard.position);
                pengGangScreenPos.z = handScreenPos.z;
                Vector3 pengGangWorldPos = Camera.main.ScreenToWorldPoint(pengGangScreenPos);
                handPos = areaItem.handCardContainer.InverseTransformPoint(pengGangWorldPos);
                handPos += areaItem.handCardGap;
                handPos.y = 0;
                handPos.z = 0;
            }
            for (int i = 0; i < areaItem.data.handCards.Count; i++)
            {
                var card = ResourcesMgr.Instance.GetFromPool(areaItem.data.handCards[i]);
                card.transform.SetParent(areaItem.handCardContainer);
                card.transform.localScale = Vector3.one;
                SetHandCardAngles(areaItem, card);
                card.transform.localPosition = handPos;
                if (areaItem.data.userId == playerInfoProxy.userID)
                {
                    card.layer = GlobalData.SELF_HAND_CARDS;
                }
                else if (areaItem.dir == AreaDir.RIGHT)
                {
                    card.layer = GlobalData.RIGHT_HAND_CARDS;
                }
                else
                {
                    card.layer = GlobalData.OTHER_CARDS;
                }
                if (i + 1 < areaItem.data.handCards.Count)
                {
                    handPos += areaItem.handCardGap;
                }
                else
                {
                    handPos += areaItem.getHandCardGap;
                }
                areaItem.handCards.Add(card);
            }
            //恢复摸到的牌
            if (areaItem.data.getCard > 0)
            {
                var card = ResourcesMgr.Instance.GetFromPool(areaItem.data.getCard);
                card.transform.SetParent(areaItem.handCardContainer);
                card.transform.localScale = Vector3.one;
                SetHandCardAngles(areaItem, card);
                card.transform.localPosition = handPos;
                areaItem.getCard = card;
                if (areaItem.data.userId == playerInfoProxy.userID)
                {
                    areaItem.getCard.layer = GlobalData.SELF_HAND_CARDS;
                }
                else if (areaItem.dir == AreaDir.RIGHT)
                {
                    areaItem.getCard.layer = GlobalData.RIGHT_HAND_CARDS;
                }
                else
                {
                    areaItem.getCard.layer = GlobalData.OTHER_CARDS;
                }
            }
            //恢复已出的牌
            if (!isOnlyInitHandCard)
            {
                Vector3 putPos = Vector3.zero;
                for (int i = 0; i < areaItem.data.putCards.Count; i++)
                {
                    var card = ResourcesMgr.Instance.GetFromPool(areaItem.data.putCards[i]);
                    int column = i % areaItem.putCardHNum;
                    int row = i / areaItem.putCardHNum;
                    card.transform.SetParent(areaItem.putCardContainer);
                    card.transform.localScale = Vector3.one;
                    var sitIndex = (areaItem.data.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
                    if (sitIndex == 2)//顶部
                    {
                        card.transform.localEulerAngles = new Vector3(0, -180, 0);
                    }
                    else
                    {
                        card.transform.localEulerAngles = Vector3.zero;
                    }
                    card.transform.localPosition = putPos + column * areaItem.putCardHGap + row * areaItem.putCardVGap;
                    card.layer = GlobalData.OTHER_CARDS;
                    areaItem.putCards.Add(card);
                }
                //恢复牌堆的牌
                if (GlobalData.hasHeap)
                {
                    InitHeapCard(areaItem, battleProxy.leftCard);
                }
            }
        }

        /// <summary>
        /// 生成牌堆内的牌
        /// </summary>
        /// <param name="areaItem"></param>
        public static void InitHeapCard(BattleAreaItem areaItem,int leftCard)
        {
            var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            if (!battleProxy.unGetHeapCardIndexs.Contains(areaItem.heapStartIndex) && !battleProxy.unGetHeapCardIndexs.Contains(areaItem.heapEndIndex))
            {
                return;
            }
            List<int> addHeapIndexArr = new List<int>();
            //先将发牌位置右侧的牌堆生成
            int recivedCard = GlobalData.CardWare.Length - leftCard;
            for (int i = Math.Max(battleProxy.sendHeapStartIndex + recivedCard, areaItem.heapStartIndex); i <= areaItem.heapEndIndex; i++)
            {
                if (!battleProxy.unGetHeapCardIndexs.Contains(i))
                {
                    continue;
                }
                addHeapIndexArr.Add(i);
            }
            //再生成发牌位置左侧的牌堆
            for (int i = areaItem.heapStartIndex; i < Math.Min(battleProxy.sendHeapStartIndex, areaItem.heapEndIndex + 1); i++)
            {
                if (!battleProxy.unGetHeapCardIndexs.Contains(i))
                {
                    continue;
                }
                addHeapIndexArr.Add(i);
            }
            foreach (int i in addHeapIndexArr)
            {
                var card = ResourcesMgr.Instance.GetFromPool(GlobalData.CardValues[0]);
                if(GlobalData.LoginServer == "127.0.0.1")
                {
                    if (GlobalData.Test127Queue.Count> 0)
                    {
                        card = ResourcesMgr.Instance.GetFromPool(GlobalData.Test127Queue.Dequeue());
                    }
                    else
                    { 
                        for (int x = 0; x < GlobalData.CardWare.Length; x++)
                        {
                            GlobalData.Test127Queue.Enqueue(GlobalData.CardWare[x]);
                        }
                        card = ResourcesMgr.Instance.GetFromPool(GlobalData.Test127Queue.Dequeue());
                    }
                }
                int column = (i - areaItem.heapStartIndex) / 2;
                int row = i % 2;
                card.transform.SetParent(areaItem.heapCardContainer);
                card.transform.localEulerAngles = Vector3.zero;
                card.transform.localScale = Vector3.one;
                card.transform.localPosition = areaItem.heapFirstCard.localPosition + column * areaItem.heapHGap + row * areaItem.heapVGap;

                if (areaItem.dir == AreaDir.RIGHT)
                {
                    card.layer = GlobalData.RIGHT_HAND_CARDS;
                }
                else
                {
                    card.layer = GlobalData.OTHER_CARDS;
                }
                //card.layer = GlobalData.OTHER_CARDS;
                areaItem.heapCards.Add(card);
            }
            
        }

        /// <summary>
        /// 设置手牌的角度
        /// </summary>
        /// <param name="areaItem"></param>
        /// <param name="card"></param>
        private static void SetHandCardAngles(BattleAreaItem areaItem,GameObject card)
        {
            var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            if (battleProxy.isReport)
            {
                if (areaItem.dir == AreaDir.LEFT)
                {
                    card.transform.localEulerAngles = new Vector3(-90, 0, 0);
                }
                else if (areaItem.dir == AreaDir.RIGHT)
                {
                    card.transform.localEulerAngles = new Vector3(-90, 0, 0);
                }
                else if (areaItem.dir == AreaDir.UP)
                {
                    card.transform.localEulerAngles = new Vector3(-90, 0, 0);
                }
                else
                {
                    card.transform.localEulerAngles = Vector3.zero;
                }
            }
            else
            {
                card.transform.localEulerAngles = Vector3.zero;
            }
        }

        public static GameObject ChiPengGangCardArrow(Transform _transform,AreaDir dir)
        {
            GameObject go = new GameObject();
            go.AddComponent<SpriteRenderer>();
            string str = "";
            if (dir == AreaDir.RIGHT)
            {
                str = "Textures/91city_chipenggang/right@2x";
            }
            else if (dir == AreaDir.UP)
            {
                str = "Textures/91city_chipenggang/up@2x";
            }
            else if (dir == AreaDir.LEFT)
            {
                str = "Textures/91city_chipenggang/left@2x";
            }
            go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(str);
            go.transform.SetParent(_transform);
            return go;
        }

        /// <summary>
        /// 获取当前牌内能吃的组合数组
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static List<List<int>> GetCanChiArr(int card)
        {
            var result = new List<List<int>>();
            var battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            var playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            var handCards = battleProxy.playerIdInfoDic[playerInfoProxy.userID].handCards;

            int[] temp = {card + 1,card + 2, card - 1,  card + 1 , card - 2, card - 1 };
            bool isContainAll = true;

            int count = temp.Length / 2;
            int index = 0;
            List<int> canSelectCard;
            for (int i = 0; i < count; i++)
            {
                isContainAll = true;
                canSelectCard = new List<int>();
                for (int j = 0; j < 2; j++)
                {
                    index = 2 * i + j;
                    canSelectCard.Add(temp[index]);
                    if (!handCards.Contains(temp[index]))
                    {
                        isContainAll = false;
                        break;
                    }
                }
                if (isContainAll)
                {
                    canSelectCard.Remove(card);
                    canSelectCard.Sort();
                    result.Add(canSelectCard);
                }
            }

            for (int a = 0; a < result.Count; a++)
            {
                for (int i = 0; i < result[a].Count; i++)
                {
                    Debug.Log("组合："+result[a][i]);
                }
            }
            return result;
        }
    }
}
