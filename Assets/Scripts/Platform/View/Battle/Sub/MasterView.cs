using System.Collections.Generic;
using Platform.Model.Battle;
using UnityEngine;
using Platform.Model;
using Utils;

/// <summary>
/// 牌局内转圈界面
/// </summary>
public class MasterView : MonoBehaviour
{
    /// <summary>
    /// 方向图标数组
    /// </summary>
    private List<GameObject> dirIconArr;
    /// <summary>
    /// 方向图标位置数组
    /// </summary>
    private List<Vector3> dirIconPostionArr;
    /// <summary>
    /// 战斗模块数据
    /// </summary>
    private BattleProxy battleProxy;
    /// <summary>
    /// 游戏模块数据
    /// </summary>
    private GameMgrProxy gameMgrProxy;
    /// <summary>
    /// 玩家信息模块数据
    /// </summary>
    private PlayerInfoProxy playerInfoProxy;
    /// <summary>
    /// 旋转的定时器id
    /// </summary>
    private int rotateTimeId;
    /// <summary>
    /// 倒计时定时器id
    /// </summary>
    private int remainTimeId;

    /// <summary>
    /// 箭头标志容器
    /// </summary>
    private Transform arrowContainer;
    /// <summary>
    /// 箭头标志
    /// </summary>
    private SpriteRenderer arrowIcon;
    /// <summary>
    /// 庄家标志
    /// </summary>
    private SpriteRenderer masterIcon;
    private bool ComfirSit = false;
    /// <summary>
    /// 出牌剩余时间
    /// </summary>
    private TextMesh remainTimeTxt;

    private SpriteRenderer colorEastIcon;
    GameObject eastIcon;
    // Use this for initialization
    void Awake()
    {
        dirIconArr = new List<GameObject>();
        dirIconPostionArr = new List<Vector3>();
        battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        gameMgrProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
        playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
        remainTimeTxt = transform.Find("RemainTimeTxt").GetComponent<TextMesh>();
        masterIcon = transform.Find("MasterIcon").GetComponent<SpriteRenderer>();
         eastIcon = transform.Find("EastIcon").gameObject;
        var southIcon = transform.Find("SouthIcon").gameObject;
        var westIcon = transform.Find("WestIcon").gameObject;
        var northIcon = transform.Find("NorthIcon").gameObject;
        dirIconArr.Add(eastIcon);
        dirIconArr.Add(southIcon);
        dirIconArr.Add(westIcon);
        dirIconArr.Add(northIcon);
        dirIconPostionArr.Add(eastIcon.transform.localPosition);
        dirIconPostionArr.Add(southIcon.transform.localPosition);
        dirIconPostionArr.Add(westIcon.transform.localPosition);
        dirIconPostionArr.Add(northIcon.transform.localPosition);
        arrowContainer = transform.Find("ArrowContainer");
        arrowIcon = transform.Find("ArrowContainer/ArrowIcon").GetComponent<SpriteRenderer>();
        
    }

    private void Start()
    {
        colorEastIcon = eastIcon.transform.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        colorEastIcon.color  = new Color(1,1,1, Mathf.PingPong(Time.time*2,1));
        if (battleProxy.isStart && !ComfirSit)
        {
            ComfirSit = true;
            if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].sit == 1)
            {
                gameObject.transform.localPosition = new Vector3(-0.009f, 0.029f, 0.055f);
                gameObject.transform.localEulerAngles = new Vector3(50,0,0);
            }else if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].sit == 2)
            {
                gameObject.transform.localPosition = new Vector3(-0.003f,0.029f,0.055f);
                gameObject.transform.localEulerAngles = new Vector3(50f, 0, -90);
            }
            else if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].sit == 3)
            {
                gameObject.transform.localPosition = new Vector3(-0.004f, 0.025f, 0.049f);
                gameObject.transform.localEulerAngles = new Vector3(50f, 0, 180);
            }
            else if (battleProxy.playerIdInfoDic[playerInfoProxy.userID].sit == 4)
            {
                gameObject.transform.localPosition = new Vector3(-0.001f, 0.025f, 0.049f);
                gameObject.transform.localEulerAngles = new Vector3(50f, 0, 90);
            }
        }
    }


    /// <summary>
    /// 开始播放庄家旋转动画
    /// </summary>
    public void PlayRotate()
    {
        gameObject.GetComponent<Animator>().Play(MasterAnimationName.MasterRotate, 0, 0);
    }

    /// <summary>
    /// 抛出开始旋转事件
    /// </summary>
    public void DispatchStartRotate()
    {
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio("Voices/Effect/MasterRotate",0,true));
        rotateTimeId = Timer.Instance.AddTimer(0.08f, 0, 0.08f, UpdateRotateAngle);
    }

    /// <summary>
    /// 更新旋转角度
    /// </summary>
    private void UpdateRotateAngle()
    {
        arrowContainer.localEulerAngles = new Vector3(0,0, arrowContainer.localEulerAngles.z + 90);
    }

    /// <summary>
    /// 抛出停止旋转事件
    /// </summary>
    public void DispatchEndRotate()
    {
        AudioSystem.Instance.StopEffectAudio("Voices/Effect/MasterRotate");
        Timer.Instance.CancelTimer(rotateTimeId);
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var bankerInfoVO = battleProxy.GetBankerPlayerInfoVOS2C();
        var sitIndex = (bankerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        arrowContainer.localEulerAngles = new Vector3(0, 0, 180 + sitIndex * 90);
    }

    /// <summary>
    /// 抛出显示庄家图标事件
    /// </summary>
    public void DispatchMasterShow()
    {
        GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio("Voices/Effect/MasterConfirm"));
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_BATTLEVIEW_SHOWBANKERICON);
    }

    /// <summary>
    /// 开始显示东南西北
    /// </summary>
    public void DispatchShowDir()
    {
        UpdateMasterInfo(false);
    }

    /// <summary>
    /// 旋转完成隐藏旋转
    /// </summary>
    public void DispatchRotateComplete()
    {
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_BATTLE_SENDCARD);
    }

    /// <summary>
    /// 更新东西南北信息
    /// </summary>
    public void UpdateMasterInfo(bool needStopAnimator)
    {
        if (needStopAnimator)
        {
            gameObject.GetComponent<Animator>().Stop();
        }
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        var masterPlayerInfoVO = battleProxy.GetMasterPlayerInfoVOS2C();
        var sitIndex = (masterPlayerInfoVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;//计算方向偏移量
        for (int i = 0; i < dirIconArr.Count; i++)
        {
            //dirIconArr[i].GetComponent<SpriteRenderer>().color = new Color(0.086f,0.266f,0.415f,1);
            dirIconArr[i].transform.localPosition = dirIconPostionArr[(i + sitIndex) % GlobalData.SIT_NUM];
        }
    }

    /// <summary>
    /// 显示玩家操作提示
    /// </summary>
    public void ShowPlayActTip()
    {
        gameObject.GetComponent<Animator>().Stop();
        var selfInfoVO = battleProxy.playerIdInfoDic[playerInfoProxy.userID];
        PlayerInfoVOS2C tipPlayerVO = null;
        if (battleProxy.GetPlayerActTipS2C() == null && battleProxy.GetPlayerActS2C() == null)
        {
            return;
        }
        if (battleProxy.curGuide == GuideType.ACT_TIP)
        {
            tipPlayerVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActTipS2C().optUserId];
            bool isPengGang = false;
            foreach (PlayerActType item in battleProxy.hidenActTypes)
            {
                if (battleProxy.GetPlayerActTipS2C().acts.Contains(item))
                {
                    isPengGang = true;
                    break;
                }
            }
            if (isPengGang && battleProxy.playerIdInfoDic.ContainsKey(battleProxy.GetPlayerActTipS2C().targetUserId))
            {
                tipPlayerVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActTipS2C().targetUserId];
                //var nextSitId = GlobalData.GetNextSit(tipPlayerVO.sit, 1);
                //tipPlayerVO = battleProxy.playerSitInfoDic[nextSitId];
            }
        }else if (battleProxy.curGuide == GuideType.ACT)
        {
            tipPlayerVO = battleProxy.playerIdInfoDic[battleProxy.GetPlayerActS2C().userId];
            if (battleProxy.GetPlayerActS2C().act == PlayerActType.PASS)
            {
                return;
            }
        }
        if (tipPlayerVO == null)
        {
            return;
        }
        var sitIndex = (tipPlayerVO.sit - selfInfoVO.sit + GlobalData.SIT_NUM) % GlobalData.SIT_NUM;
        arrowContainer.localEulerAngles = new Vector3(0, 0, 180 + sitIndex * 90);
        arrowIcon.color = Color.white;
        masterIcon.color = new Color(1, 1, 1, 0);
        remainTimeId = Timer.Instance.AddDeltaTimer(1, 0, 1, UpdateTipRemain);
        remainTimeTxt.gameObject.SetActive(true);
        UpdateTipRemain();
    }

    /// <summary>
    /// 设置剩余时间
    /// </summary>
    private void UpdateTipRemain()
    {
        if (battleProxy.GetPlayerActTipS2C() == null)
        {
            remainTimeTxt.gameObject.SetActive(false);
            return;
        }
        float curRemainTime = 0;
        if (battleProxy.isReport)
        {
            curRemainTime = battleProxy.GetPlayerActTipS2C().tipRemainTime - (gameMgrProxy.scaleSystemTime - battleProxy.GetPlayerActTipS2C().tipRemainUT) / 1000;
        }
        else
        {
            bool isHidenAct = false;
            foreach (PlayerActType item in battleProxy.hidenActTypes)
            {
                if (battleProxy.GetPlayerActTipS2C().acts.Contains(item))
                {
                    isHidenAct = true;
                    break;
                }
            }
            if (isHidenAct)
            {
                if (battleProxy.GetPlayerActTipS2C().optUserId == playerInfoProxy.userID)
                {
                    curRemainTime = battleProxy.GetPlayerActTipS2C().tipRemainTime - (gameMgrProxy.systemTime - battleProxy.GetPlayerActTipS2C().tipRemainUT) / 1000;
                }
                else
                {
                    curRemainTime = battleProxy.GetPlayerActTipS2C().tipRemainTime;
                }
            }
            else
            {
                curRemainTime = battleProxy.GetPlayerActTipS2C().tipRemainTime - (gameMgrProxy.systemTime - battleProxy.GetPlayerActTipS2C().tipRemainUT) / 1000;
            }
        }
        
        curRemainTime = Mathf.Max(curRemainTime, 0);
        curRemainTime = Mathf.Ceil(curRemainTime);
        remainTimeTxt.gameObject.SetActive(true);
        if (battleProxy.isStart)
        {
            remainTimeTxt.text = curRemainTime.ToString();
        }
        else
        {
            remainTimeTxt.gameObject.SetActive(false);
            Timer.Instance.CancelTimer(remainTimeId);
        }
    }
}

/// <summary>
/// 动作名称
/// </summary>
internal class MasterAnimationName
{
    public const string MasterRotate = "MasterRotate";

    public const string Empty = "Empty";
}
