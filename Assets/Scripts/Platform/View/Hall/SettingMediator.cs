using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class SettingMediator : Mediator, IMediator
{
    /// <summary>
    /// 玩家信息数据
    /// </summary>
    private PlayerInfoProxy playerInfoProxy;
    public SettingMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }

    public SettingView View
    {
        get
        {
            return (SettingView)ViewComponent;
        }
    }

    public override void OnRegister()
    {
        base.OnRegister();
        playerInfoProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.PLAYER_PROXY) as PlayerInfoProxy;
        View.UserName.text = playerInfoProxy.userName;
        GameMgr.Instance.StartCoroutine(DownIcon(playerInfoProxy.headIconUrl));
        this.View.ButtonAddListening(this.View.CloseButton,
        () =>
        {
            PlayerPrefs.SetFloat(PrefsKey.SOUNDSET, this.View.SoundSlider.value);
            PlayerPrefs.SetFloat(PrefsKey.MUSICSET, this.View.MusicSlider.value);
            PlayerPrefs.SetInt(PrefsKey.LUANAGE, this.View.BackGround.isOn ? 1 : -1);
            UIManager.Instance.HideUI(UIViewID.SETTING_VIEW);
        });
        this.View.ButtonAddListening(this.View.SwitchAccountButton,
            () =>
            {
                UIManager.Instance.Background.color = new Color(1, 1, 1, 0);
                UIManager.Instance.Background.gameObject.SetActive(true);
                UIManager.Instance.Background.DOColor(new Color(1, 1, 1, 1), 0.5f).SetEase(Ease.Linear).OnComplete(
                    () =>
                    {
                        if (Application.platform == RuntimePlatform.Android)
                        {
                            PlayerPrefs.DeleteKey(PrefsKey.USERMAC);
                            PlayerPrefs.DeleteKey(PrefsKey.USERNAME);
                            PlayerPrefs.DeleteKey(PrefsKey.HEADURL);
                            PlayerPrefs.DeleteKey(PrefsKey.SEX);
                        }
                        var loadInfo = new LoadSceneInfo(ESceneID.SCENE_LOGIN, LoadSceneType.SYNC, LoadSceneMode.Single);
                        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
                    });
            });
        this.View.MusicSlider.onValueChanged.AddListener(
                   (float value)
                   =>
                   {
                       PlayerPrefs.SetFloat(PrefsKey.MUSICSET, value);
                       AudioSystem.Instance.SetBgmAudioVolume(PlayerPrefs.GetFloat(PrefsKey.MUSICSET));
                   });
        this.View.SoundSlider.onValueChanged.AddListener(
            (float value) =>
            {
                PlayerPrefs.SetFloat(PrefsKey.SOUNDSET, value);
                AudioSystem.Instance.SetEffectAudioVolume(PlayerPrefs.GetFloat(PrefsKey.SOUNDSET));
            });
        this.View.CheckMark.onValueChanged.AddListener(
            (bool isOn) =>
            {
               
            });
        this.View.BackGround.onValueChanged.AddListener(
            (bool isOn) =>
            {

            });
    }

    IEnumerator DownIcon(string headUrl)
    {
        WWW www = new WWW(headUrl);
        yield return www;
        if (www.error == null)
        {
            View.UserPhoto.texture = www.texture;
        }
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}