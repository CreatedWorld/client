using System.Collections;
using System.Collections.Generic;
using Platform.Model.Battle;
using Platform.Net;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using Platform.Model;
using System;
using Platform.Utils;
using Utils;

namespace Platform.View.Battle
{
    /// <summary>
    /// 单局结算界面中介
    /// </summary>
    internal class MatchResultViewMediator : Mediator, IMediator
    {
        /// <summary>
        ///     战斗模块数据中介
        /// </summary>
        private BattleProxy battleProxy;
        /// <summary>
        /// 玩家信息数据
        /// </summary>
        private PlayerInfoProxy playerInfoProxy;
        /// <summary>
        /// ai定时器id
        /// </summary>
        private int aiTimerId;
        public MatchResultViewMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
        {
        }

        public MatchResultView View
        {
            get { return (MatchResultView) ViewComponent; }
        }

        public override IList<string> ListNotificationInterests()
        {
            IList<string> list = new List<string>();
            return list;
        }


        public override void OnRegister()
        {
            base.OnRegister();
            battleProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
            playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
            View.startNextBtn.onClick.AddListener(OnStartNextClick);
            //View.shareBtn.onClick.AddListener(OnShaderClick);
            InitUI();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.startNextBtn.onClick.RemoveListener(OnStartNextClick);
        }

        public override void HandleNotification(INotification notification)
        {
        }

        /// <summary>
        /// 初始化UI数据
        /// </summary>
        private void InitUI()
        {
            var matchResultS2C = battleProxy.matchResultS2C;
            for (var i = 0; i < matchResultS2C.resultInfos.Count; i++)
            {
                View.playerItems[i].data = matchResultS2C.resultInfos[i];
            }
            if (matchResultS2C.huUserId.Contains(playerInfoProxy.userID))
            {
                //View.titleIcon.gameObject.SetActive(false);
                //var effectPerfab = Resources.Load<GameObject>("Effect/WinEffect/WinEffect");
                //View.actEffect = GameObject.Instantiate(effectPerfab);
                //var perPosition = View.actEffect.GetComponent<Transform>().localPosition;
                //View.actEffect.GetComponent<Transform>().SetParent(View.viewRoot.GetComponent<RectTransform>());
                //View.actEffect.GetComponent<Transform>().localPosition = perPosition;
                //View.actEffect.GetComponent<Transform>().localScale = Vector3.one;
                //View.actEffect.GetComponent<Animator>().enabled = true;
                //View.viewRoot.GetComponent<Animator>().Play("MatchResultOpen",0,0);
                //GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio("Voices/Effect/Win"));
            }
            else if (matchResultS2C.huUserId.Count == 0)//流局
            {
                //View.titleIcon.gameObject.SetActive(true);
                //View.titleIcon.sprite = Resources.Load<Sprite>("Textures/MatchPassTitle");
                View.viewRoot.GetComponent<Animator>().enabled = true;
                View.viewRoot.GetComponent<Animator>().Play("MatchResultOpen", 0, 0);
                GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio("Voices/Effect/Failt"));
            }
            else
            {
                //View.titleIcon.gameObject.SetActive(true);
                //View.titleIcon.sprite = Resources.Load<Sprite>("Textures/MatchFailtTitle");
                //View.viewRoot.GetComponent<Animator>().enabled = true;
                //View.viewRoot.GetComponent<Animator>().Play("MatchResultOpen", 0, 0);
                //GameMgr.Instance.StartCoroutine(AudioSystem.Instance.PlayEffectAudio("Voices/Effect/Failt"));
            }
            var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
            //if (battleProxy.curInnings == hallProxy.HallInfo.innings.GetHashCode() && battleProxy.matchResultS2C.huUserId.Count > 0)//是否是最后一局
            //{
            //    View.startNextBtnTxt.text = "牌局结算";
            //}
            //else
            //{
            //    View.startNextBtnTxt.text = "开始游戏";
            //}
            if (GlobalData.isDebugModel)
            {
                aiTimerId = Timer.Instance.AddTimer(0, 1, 3, () => { OnStartNextClick(); });
            }
            Timer.Instance.AddTimer(0, 1, 0.5f, () =>
            {
                for (var i = 0; i < matchResultS2C.resultInfos.Count; i++)
                {
                    View.playerItems[i].ResetCardPos();
                }
            });
        }

        /// <summary>
        /// 上次点击准备的时间
        /// </summary>
        private float perClickTime = 0;
        /// <summary>
        /// 开始下一局准备
        /// </summary>
        private void OnStartNextClick()
        {
            if (aiTimerId > 0)
            {
                Timer.Instance.CancelTimer(aiTimerId);
            }
            if (Time.time - perClickTime < 1)
            {
                return;
            }
            perClickTime = Time.time;
            GameMgr.Instance.StartCoroutine(PlayCloseEffect());
        }

        /// <summary>
        ///     播放关闭窗口动作,并移除窗口
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayCloseEffect()
        {
            //View.viewRoot.GetComponent<Animator>().Play("MatchResultClose",0,0);
            UIManager.Instance.HideUI(UIViewID.MATCH_RESULT_VIEW);
            foreach (MatchResultPlayerItem matchResultPlayerItem in View.playerItems)
            {
                matchResultPlayerItem.SaveAllCard();
            }
            if (View.actEffect != null)
            {
                GameObject.Destroy(View.actEffect);
                View.actEffect = null;
            }
            yield return new WaitForSeconds(0.4f);
            if (battleProxy.roomResultS2C != null)//最后一局
            {
                UIManager.Instance.HideUI(UIViewID.MATCH_RESULT_VIEW, ShowRoomResult);
            }
            else
            {
                ApplicationFacade.Instance.SendNotification(NotificationConstant.READY_NEXT);
                battleProxy.AddInnings();
                var readyC2S = new ReadyC2S();
                NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_READY.GetHashCode(), 0, readyC2S);
                UIManager.Instance.HideUI(UIViewID.MATCH_RESULT_VIEW);
            }
        }

        /// <summary>
        /// 显示房间结算
        /// </summary>
        private void ShowRoomResult()
        {
            Timer.Instance.AddTimer(0, 0, 0.3f, () => { UIManager.Instance.ShowUI(UIViewID.ROOM_RESULT_VIEW); });
        }

        /// <summary>
        ///     调用微信分享
        /// </summary>
        private void OnShaderClick()
        {
            if (GlobalData.sdkPlatform == SDKPlatform.ANDROID)
            {
                string desc = "快来全民约牌吧";
                AndroidSdkInterface.WeiXinShareScreen(desc, false);
            }
            else if (GlobalData.sdkPlatform == SDKPlatform.IOS)
            {
                UIManager.Instance.StartSaveScreen((Texture2D screenShot) => {
                    byte[] screenJpg = screenShot.EncodeToJPG();
                    string jpgBase64 = Convert.ToBase64String(screenJpg);
                    IOSSdkInterface.shareBitmap(jpgBase64, false);
                });        
            }
        }
    }
}