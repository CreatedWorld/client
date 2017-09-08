using System.Collections;
using DG.Tweening;
using Platform.Model;
using UnityEngine;
using UnityEngine.UI;
using Platform.Model.Battle;
using Platform.Net;
using Utils;
using Platform.Utils;

/// <summary>
/// 战斗场景内单个头像框
/// </summary>
public class HeadItem : MonoBehaviour
{
    /// <summary>
    /// 节点玩家数据
    /// </summary>
    private PlayerInfoVOS2C _data;
    /// <summary>
    /// 战斗模块数据
    /// </summary>
    private BattleProxy battleProxy;
    /// <summary>
    /// 玩家模块数据
    /// </summary>
    private PlayerInfoProxy playerInfoProxy;
    /// <summary>
    /// 头像按钮
    /// </summary>
    private Button headBtn;
    /// <summary>
    /// 玩家名称
    /// </summary>
    private Text nameTxt;
    /// <summary>
    /// 积分值
    /// </summary>
    private Text coinTxt;
    /// <summary>
    /// 头像图标
    /// </summary>
    private RawImage headIcon;
    /// <summary>
    /// 准备图标
    /// </summary>
    private GameObject readyIcon;
    /// <summary>
    /// 庄家图标
    /// </summary>
    private GameObject bankerIcon;
    
    /// <summary>
    /// 正在播放声音图标
    /// </summary>
    private GameObject voicePlayIcon;
    /// <summary>
    /// 音量大小标志
    /// </summary>
    private Image voiceValueIcon;
    /// <summary>
    /// 聊天显示区域
    /// </summary>
    private GameObject chatView;
    /// <summary>
    /// 聊天文本框
    /// </summary>
    private Text chatTxt;
    /// <summary>
    /// 动作特效
    /// </summary>
    private GameObject actEffect;
    /// <summary>
    /// 表情图标
    /// </summary>
    private Image faceIcon;
    /// <summary>
    /// 等待特效
    /// </summary>
    private GameObject waitingIcon;
    /// <summary>
    /// 胡牌特效容器
    /// </summary>
    private Transform huView;
    /// <summary>
    /// 胡牌特效
    /// </summary>
    private GameObject huEffect;

    void Awake()
    {
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
        headBtn = transform.FindChild("Head").GetComponent<Button>();
        nameTxt = transform.FindChild("NameTxt").GetComponent<Text>();
        coinTxt = transform.FindChild("CoinTxt").GetComponent<Text>();
        headIcon = transform.FindChild("Head/HeadIcon").GetComponent<RawImage>();
        readyIcon = transform.FindChild("ReadyIcon").gameObject;
        bankerIcon = transform.FindChild("BankerIcon").gameObject;
        voicePlayIcon = transform.FindChild("VoicePlayIcon").gameObject;
        voiceValueIcon = transform.FindChild("VoicePlayIcon/VoiceValueIcon").GetComponent<Image>();
        chatView = transform.FindChild("ChatView").gameObject;
        chatTxt = transform.FindChild("ChatView/ChatTxt").GetComponent<Text>();
        faceIcon = transform.FindChild("FaceIcon").GetComponent<Image>();
        //creatorIcon = transform.FindChild("Head/CreatorIcon").gameObject;
        waitingIcon = transform.FindChild("WaitingIcon").gameObject;
        huView = transform.FindChild("HuView");
        //offlineIcon = transform.FindChild("Head/OfflineIcon").gameObject;

        headBtn.onClick.AddListener(OpenPlayerInfo);
    }

    /// <summary>
    /// 图标上次更新时间
    /// </summary>
    float perUpdateTime = 0;
	void Update () {
        if (voicePlayIcon.activeSelf && Time.time - perUpdateTime > 0.2 && AudioSystem.Instance.curChatAudioSource != null)
        {
            perUpdateTime = Time.time;
            AudioSource audio = AudioSystem.Instance.curChatAudioSource;
            float volume = 1 + (3 * audio.volume);
            int value = Mathf.RoundToInt(volume);
            Sprite targetSprite = Resources.Load<Sprite>(string.Format("Textures/VoicePlayIcon/VoicePlayIcon{0}", value));
            voiceValueIcon.sprite = targetSprite;
        }
	}

    /// <summary>
    /// 头像框对应的玩家数据
    /// </summary>
    public PlayerInfoVOS2C data
    {
        get { return _data; }
        set
        {
            if (value != null)
            {
                waitingIcon.SetActive(false);
                nameTxt.text = value.name;
                coinTxt.text = value.score.ToString();
                UpdateOnline(value.isOnline);
                if (battleProxy.isStart)
                {
                    readyIcon.SetActive(false);
                    bankerIcon.SetActive(value.isBanker);
                }
                else
                {
                    readyIcon.SetActive(value.isReady);
                    readyIcon.GetComponent<Image>().color = new Color(1,1,1,1);
                    bankerIcon.SetActive(false);
                }
                tryCount = 0;
                GameMgr.Instance.StartCoroutine(DownIcon(value.headIcon));
                if (_data == null)//头像由没有->有
                {
                    var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
                    var sitOffset = (value.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
                    if (sitOffset != 0)
                    {
                        headBtn.gameObject.GetComponent<Animator>().Play(HeadItemAnimationName.ShowNameArr[sitOffset],0,0);
                    }
                    GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio("Voices/Effect/PlayerEnter"));
                }
            }
            else
            {
                waitingIcon.SetActive(true);
                readyIcon.SetActive(false);
                bankerIcon.SetActive(false);
                if (_data != null)//头像由有->没有
                {
                    var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
                    var sitOffset = (_data.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
                    headBtn.gameObject.GetComponent<Animator>().Play(HeadItemAnimationName.HidenNameArr[sitOffset],0,0);
                    GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio("Voices/Effect/PlayerEnter"));
                }
            }
            _data = value;
        }
    }

    /// <summary>
    /// 加载重试次数
    /// </summary>
    private int tryCount;
    IEnumerator DownIcon(string headUrl)
    {
        WWW www = new WWW(headUrl);
        yield return www;
        if (www.error == null)
        {
            headIcon.texture = www.texture;
        }
        else
        {
            tryCount++;
            if (tryCount < 10)
            {
                GameMgr.Instance.StartCoroutine(DownIcon(headUrl));
            }            
        }
    }

    /// <summary>
    /// 隐藏准备标志
    /// </summary>
    public void HidenReady()
    {
        readyIcon.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.5f);
    }

    /// <summary>
    /// 隐藏庄家标志
    /// </summary>
    public void HidemBanker()
    {
        bankerIcon.SetActive(false);
    }

    /// <summary>
    /// 设置庄家图标显示
    /// </summary>
    /// <param name="bankerLocalId">庄家id</param>
    public void ShowBankerIcon(int bankerLocalId)
    {
        if (_data == null)
        {
            return;
        }
        if (_data.userId == bankerLocalId)
        {
            bankerIcon.SetActive(true);
            var rectTransform = bankerIcon.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(3,3);
            Tweener tweener = rectTransform.DOScale(new Vector3(1, 1, 1), 0.5f);
            tweener.SetAutoKill(true);

            bankerIcon.GetComponent<Image>().color = new Color(1,1,1,0);
            Tweener tweener2 = bankerIcon.GetComponent<Image>().DOColor(Color.white, 0.5f);
            tweener2.SetAutoKill(true);
        }
    }

    /// <summary>
    /// 播放胡牌特效
    /// </summary>
    public void PlayHu()
    {
        var huEffectPerfab = Resources.Load<GameObject>("Effect/HuEffect/HuEffect");
        huEffect = GameObject.Instantiate(huEffectPerfab);
        huEffect.GetComponent<RectTransform>().SetParent(huView);
        huEffect.GetComponent<RectTransform>().localScale = Vector3.one;
        huEffect.GetComponent<RectTransform>().localPosition = Vector3.zero;
        huEffect.GetComponent<Animator>().enabled = true;
        Timer.Instance.AddDeltaTimer(1, 1, 3, RemoveHuEffect);
    }

    /// <summary>
    /// 移除胡牌动作
    /// </summary>
    private void RemoveHuEffect()
    {
        GameObject.Destroy(huEffect);
        huEffect = null;
        battleProxy.isPlayHu = false;
        if (battleProxy.matchResultS2C != null && !UIManager.Instance.GetUIView(UIViewID.MATCH_RESULT_VIEW).isShow)
        {
            UIManager.Instance.ShowUI(UIViewID.MATCH_RESULT_VIEW);
        }
    }

    /// <summary>
    /// 播放玩家动作
    /// </summary>
    /// <param name="act"></param>
    public void PlayAct(PlayerActType act)
    {
        GameObject effectPerfab = null;
        switch (act)
        {
            case PlayerActType.COMMON_AN_GANG:
            case PlayerActType.BACK_AN_GANG:
            case PlayerActType.COMMON_PENG_GANG:
            case PlayerActType.BACK_PENG_GANG:
            case PlayerActType.ZHI_GANG:
                effectPerfab = Resources.Load<GameObject>("Effect/GangEffect/GangEffect");
                break;
            case PlayerActType.PENG:
                effectPerfab = Resources.Load<GameObject>("Effect/PengEffect/PengEffect");
                break;
            case PlayerActType.CHI:
                effectPerfab = Resources.Load<GameObject>("Effect/ChiEffect/ChiEffect");
                break;
        }
        if (effectPerfab != null)
        {
            actEffect = Instantiate(effectPerfab);
            var perPosition = actEffect.GetComponent<RectTransform>().localPosition;
            actEffect.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>());
            actEffect.GetComponent<RectTransform>().localPosition = perPosition;
            actEffect.GetComponent<RectTransform>().localScale = Vector3.one;
            actEffect.GetComponent<Animator>().enabled = true;
            Timer.Instance.AddDeltaTimer(0.5f, 1, 2.5f, RemoveActEffect);
        }        
    }

    /// <summary>
    /// 移除动作提示特效
    /// </summary>
    private void RemoveActEffect()
    {
        Destroy(actEffect);
        actEffect = null;
    }

    /// <summary>
    /// 显示播放语言标志
    /// </summary>
    public void ShowVoicePlayIcon()
    {
        voicePlayIcon.SetActive(true);
    }

    /// <summary>
    /// 隐藏播放语言标志
    /// </summary>
    public void HidenVoicePlayIcon()
    {
        voicePlayIcon.SetActive(false);
    }

    /// <summary>
    /// 显示聊天消息
    /// </summary>
    public void ShowChatInfo(string chatStr)
    {
        chatView.SetActive(true);
        chatTxt.text = chatStr;
        Timer.Instance.AddDeltaTimer(1, 1, 3, HidenChatInfo);
    }

    /// <summary>
    /// 隐藏聊天消息
    /// </summary>
    private void HidenChatInfo()
    {
        chatView.SetActive(false);
    }

    /// <summary>
    /// 打开玩家信息
    /// </summary>
    private void OpenPlayerInfo()
    {
        if (_data == null)
        {
            return;
        }
        var getPlayerInfoC2S = new GetUserInfoByIdC2S();
        getPlayerInfoC2S.userId = _data.userId;
        NetMgr.Instance.SendBuff(SocketType.HALL, MsgNoC2S.C2S_Hall_Get_UserInfo_By_Id.GetHashCode(),0, getPlayerInfoC2S);
    }
    /// <summary>
    /// 表情协程
    /// </summary>
    private Coroutine faceCoroutine;
    /// <summary>
    /// 播放表情包
    /// </summary>
    /// <param name="faceIndex"></param>
    public void ShowFace(int faceIndex)
    {
        if (faceCoroutine != null)
        {
            StopCoroutine(faceCoroutine);
        }
        faceCoroutine = StartCoroutine(PlayFace(faceIndex));
    }

    private IEnumerator PlayFace(int faceIndex)
    {
        faceIcon.gameObject.SetActive(true);

        float start = Time.time + GlobalData.STICKER_LENGTH;
        Sprite[] stickers = ResourcesMgr.Instance.stickerLib[faceIndex];
        while (start > Time.time)
        {
            for (int i = 0; i < stickers.Length; ++i)
            {
                faceIcon.sprite = stickers[i];
                yield return new WaitForSeconds(GlobalData.STICKER_SPEED);
            }
        }
        //关闭表情
        faceIcon.gameObject.SetActive(false);
        faceCoroutine = null;
    }

    /// <summary>
    /// 设置是否在线
    /// </summary>
    /// <param name="value"></param>
    public void UpdateOnline(bool value)
    {
        //offlineIcon.SetActive(!value);
    }
}

/// <summary>
///     动作名称
/// </summary>
internal class HeadItemAnimationName
{
    /// <summary>
    /// 头像隐藏动画名称
    /// </summary>
    public static string[] HidenNameArr = { "","RightHeadHiden", "UpHeadHiden", "LeftHeadHiden" };
    /// <summary>
    /// 头像显示动画名称
    /// </summary>
    public static string[] ShowNameArr = { "", "RightHeadShow", "UpHeadShow", "LeftHeadShow" };
}

/// <summary>
/// 操作类型
/// </summary>
internal enum OperateType
{
    PENG,
    GANG,
    HU,
    PASS,
    CHI
}