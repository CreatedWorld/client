using System;
using System.Collections;
using System.Collections.Generic;
using Platform.Model.Battle;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;
using Platform.Utils;

namespace Platform.View.Battle
{
    /// <summary>
    /// 房间结算中介
    /// </summary>
    internal class RoomResultViewMediator : Mediator, IMediator
    {
        /// <summary>
        ///     战斗模块数据中介
        /// </summary>
        private BattleProxy battleProxy;

        /// <summary>
        ///     游戏数据中介
        /// </summary>
        private GameMgrProxy gameMgrProxy;

        public RoomResultViewMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
        {
        }

        public RoomResultView View
        {
            get { return (RoomResultView) ViewComponent; }
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
            gameMgrProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
            View.closeBtn.onClick.AddListener(OnCloseClick);
            View.shareBtn.onClick.AddListener(OnShaderClick);

            InitUI();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.closeBtn.onClick.RemoveListener(OnCloseClick);
            View.shareBtn.onClick.RemoveListener(OnShaderClick);
        }

        public override void HandleNotification(INotification notification)
        {
        }

        /// <summary>
        ///     初始化UI数据
        /// </summary>
        private void InitUI()
        {
            var roomResultS2C = battleProxy.roomResultS2C;
            var maxIndex = 0; //赢分最高的玩家序号
            var maxScore = 0;
            for (var i = 0; i < roomResultS2C.resultInfos.Count; i++)
            {
                if (roomResultS2C.resultInfos[i].addScore > maxScore)
                {
                    maxScore = roomResultS2C.resultInfos[i].addScore;
                    maxIndex = i;
                }
                View.playerItems[i].data = roomResultS2C.resultInfos[i];
            }
            View.winnerIcon.localPosition = new Vector3(View.winnerIcon.localPosition.x + View.itemGap * maxIndex,
                View.winnerIcon.localPosition.y, 0);
            var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
            View.roomIdTxt.text = String.Format("房间:{0}", hallProxy.HallInfo.roomCode);
            View.roundValueTxt.text = String.Format("{0}/{1}", battleProxy.curInnings, hallProxy.HallInfo.innings.GetHashCode());
            View.ruleValueTxt.text = hallProxy.HallInfo.gameRule == GameRule.WORD ? "有风" : "无风";
            View.timeTxt.text = gameMgrProxy.systemDateTime.ToString("yyyy-MM-dd HH:mm");
            View.viewRoot.GetComponent<Animator>().Play("RoomResultOpen");
        }

        /// <summary>
        ///     播放关闭动作
        /// </summary>
        private void OnCloseClick()
        {
            GameMgr.Instance.StartCoroutine(PlayCloseEffect());
        }

        /// <summary>
        ///     播放关闭窗口动作,并移除窗口
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayCloseEffect()
        {
            View.viewRoot.GetComponent<Animator>().Play("RoomResultClose");
            yield return new WaitForSeconds(0.4f);
            UIManager.Instance.HideUI(UIViewID.ROOM_RESULT_VIEW);
            var loadInfo = new LoadSceneInfo(ESceneID.SCENE_HALL, LoadSceneType.ASYNC, LoadSceneMode.Additive);
            SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
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