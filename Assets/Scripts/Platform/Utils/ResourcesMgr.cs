using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Platform.Utils
{
    /// <summary>
    /// 资源管理类
    /// </summary>
    public class ResourcesMgr
    {
        /// <summary>
        /// 牌面名称和网格数据数组
        /// </summary>
        private Dictionary<string, Mesh> meshsDic = new Dictionary<string, Mesh>();
        /// <summary>
        /// 牌面序号和牌面名称
        /// </summary>
        private Dictionary<int, string> meshNameDic = new Dictionary<int, string>();
        /// <summary>
        /// 牌面资源字典
        /// </summary>
        private Dictionary<int, Sprite> cardSpriteDic = new Dictionary<int, Sprite>();
        /// <summary>
        /// 牌面的预设
        /// </summary>
        private GameObject cardPerfab;
        /// <summary>
        /// 牌面按钮的预设
        /// </summary>
        private GameObject cardBtnPerfab;
        /// <summary>
        /// 卡牌池数组
        /// </summary>
        private List<GameObject> cardPoolList = new List<GameObject>();
        /// <summary>
        /// 卡牌池字典
        /// </summary>
        private Dictionary<int, GameObject> cardDic = new Dictionary<int, GameObject>();
        /// <summary>
        /// 表情字典
        /// </summary>
        public Dictionary<int, Sprite[]> stickerLib = new Dictionary<int, Sprite[]>();

        private static ResourcesMgr m_instance;

        public static ResourcesMgr Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new ResourcesMgr();
                }
                return m_instance;
            }
        }

        public ResourcesMgr()
        {
            Mesh[] meshs = Resources.LoadAll<Mesh>("Models");
            for (int i = 0; i < meshs.Length; i++)
            {
                meshsDic.Add(meshs[i].name, meshs[i]);
            }
            Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/Card");
            for (int i = 0; i < sprites.Length; i++)
            {
                cardSpriteDic.Add(int.Parse(sprites[i].name), sprites[i]);
            }
            for (int i = 0; i < GlobalData.CardValues.Length; ++i)
            {
                meshNameDic.Add(GlobalData.CardValues[i], GlobalData.MeshNames[i]);
            }
            cardPerfab = Resources.Load<GameObject>("Prefab/Battle/Card");
            cardBtnPerfab = Resources.Load<GameObject>("Prefab/UI/Battle/CardSelectBtn");
            for (int i = 1; i <= GlobalData.STICKER_NUM; ++i)
            {
                stickerLib.Add(i, Resources.LoadAll<Sprite>("Textures/Sticker/sticker" + i));
            }
        }

        /// <summary>
        /// 根据牌面值获取牌面的网格数据
        /// </summary>
        /// <param name="card">牌面值 GlobalData.CardValues </param>
        /// <returns></returns>
        private Mesh GetCardMesh(int card)
        {
            string meshName = meshNameDic[card];
            return meshsDic[meshName];
        }

        /// <summary>
        /// 将牌加入到池内
        /// </summary>
        /// <param name="cardObj"></param>
        public void Add2Pool(GameObject cardObj)
        {
            int cardInstanceId = cardObj.GetInstanceID();
            if (cardDic.ContainsKey(cardInstanceId))//牌已缓存
            {
                return;
            }
            cardObj.transform.DOKill();
            cardObj.transform.SetParent(null);
            cardObj.SetActive(false);
            cardPoolList.Add(cardObj);
            cardDic.Add(cardInstanceId, cardObj);
        }

        /// <summary>
        /// 重置池内的对象
        /// </summary>
        public void RecoveryAll()
        {
            foreach (GameObject cardObj in cardPoolList)
            {
                cardObj.transform.DOKill();
                cardObj.transform.SetParent(null);
                cardObj.SetActive(false);
            }
        }

        /// <summary>
        /// 从牌池内获取牌
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        public GameObject GetFromPool(int cardValue)
        {
            GameObject addCard;
            if (cardPoolList.Count > 0)
            {
                addCard = cardPoolList[cardPoolList.Count - 1];
                cardPoolList.RemoveAt(cardPoolList.Count - 1);
                addCard.SetActive(true);
                cardDic.Remove(addCard.GetInstanceID());
            }
            else
            {
                addCard = GameObject.Instantiate(cardPerfab);
            }
            var meshFilter = addCard.GetComponent<MeshFilter>();
            meshFilter.mesh = GetCardMesh(cardValue);
            meshFilter.mesh.name = meshFilter.mesh.name.Replace(" Instance", "");
            return addCard;
        }

        /// <summary>
        /// 牌按钮数组
        /// </summary>
        private List<GameObject> cardBtnPoolList = new List<GameObject>();
        /// <summary>
        /// 牌字典
        /// </summary>
        private Dictionary<int, GameObject> cardBtnDic = new Dictionary<int, GameObject>();
        /// <summary>
        /// 从牌池内获取牌
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        public GameObject GetCardBtnFromPool(int cardValue)
        {
            GameObject addCardBtn;
            if (cardBtnPoolList.Count > 0)
            {
                addCardBtn = cardBtnPoolList[cardBtnPoolList.Count - 1];
                cardBtnPoolList.RemoveAt(cardBtnPoolList.Count - 1);
                addCardBtn.SetActive(true);
                cardBtnDic.Remove(addCardBtn.GetInstanceID());
            }
            else
            {
                addCardBtn = GameObject.Instantiate(cardBtnPerfab);
            }
            addCardBtn.transform.Find("CardFront").GetComponent<Image>().sprite = cardSpriteDic[cardValue];
            return addCardBtn;
        }

        /// <summary>
        /// 将牌按钮加入到池内
        /// </summary>
        /// <param name="cardObj"></param>
        public void AddBtn2Pool(GameObject cardObj)
        {
            int cardInstanceId = cardObj.GetInstanceID();
            if (cardBtnDic.ContainsKey(cardInstanceId))//牌已缓存
            {
                return;
            }
            cardObj.GetComponent<Button>().onClick.RemoveAllListeners();
            (cardObj.transform as RectTransform).anchorMin = Vector2.one * 0.5f;
            (cardObj.transform as RectTransform).anchorMax = Vector2.one * 0.5f;
            cardObj.transform.SetParent(null);
            cardObj.SetActive(false);
            cardBtnPoolList.Add(cardObj);
            cardBtnDic.Add(cardInstanceId, cardObj);
        }

        /// <summary>
        /// 设置牌面
        /// </summary>
        /// <param name="addCard"></param>
        /// <param name="cardValue"></param>
        public void SetCardMesh(GameObject addCard,int cardValue)
        {
            var meshFilter = addCard.GetComponent<MeshFilter>();
            meshFilter.mesh = GetCardMesh(cardValue);
            meshFilter.mesh.name = meshFilter.mesh.name.Replace(" Instance", "");
        }

        /// <summary>
        /// 清空牌池
        /// </summary>
        public void ClearPool()
        {
            foreach (GameObject item in cardPoolList)
            {
                GameObject.Destroy(item);
            }
            cardPoolList.Clear();
            cardDic.Clear();
            foreach (GameObject item in cardBtnPoolList)
            {
                GameObject.Destroy(item);
            }
            cardBtnPoolList.Clear();
            cardBtnDic.Clear();
        }
    }
}
