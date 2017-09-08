using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// 设置View
/// </summary>
public class SettingView : UIView
{
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;
    /// <summary>
    /// 切换账号按钮
    /// </summary>
    private Button switchAccountButton;
    /// <summary>
    /// 用户头像
    /// </summary>
    private RawImage userPhoto;
    /// <summary>
    /// 角色名称
    /// </summary>
    public Text UserName;
    /// <summary>
    /// 音效按钮
    /// </summary>
    private Slider soundSlider;
    /// <summary>
    /// 音乐按钮
    /// </summary>
    private Slider musicSlider;
    /// <summary>
    /// 方言
    /// </summary>
    private Toggle backGround;
    /// <summary>
    /// 普通话
    /// </summary>
    private Toggle checkMark;
    public Button CloseButton
    {
        get
        {
            return closeButton;
        }

        set
        {
            closeButton = value;
        }
    }
    public Button SwitchAccountButton
    {
        get
        {
            return switchAccountButton;
        }

        set
        {
            switchAccountButton = value;
        }
    }

    public RawImage UserPhoto
    {
        get
        {
            return userPhoto;
        }

        set
        {
            userPhoto = value;
        }
    }

    public Slider SoundSlider
    {
        get
        {
            return soundSlider;
        }
    }

    public Slider MusicSlider
    {
        get
        {
            return musicSlider;
        }
    }
    public Toggle BackGround
    {
        get
        {
            return backGround;
        }
    }
    public Toggle CheckMark
    {
        get
        {
            return checkMark;
        }
    }
    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Setting/SettingView");
        this.CloseButton = this.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        this.SwitchAccountButton = this.viewRoot.transform.FindChild("SwitchAccountButton").GetComponent<Button>();
        this.UserPhoto = this.viewRoot.transform.FindChild("UserInfo").FindChild("PhotoMask").FindChild("UserPhoto").GetComponent<RawImage>();
        this.soundSlider = this.viewRoot.transform.FindChild("SoundInfo").FindChild("SoundSlider").GetComponent<Slider>();
        this.musicSlider = this.viewRoot.transform.FindChild("SoundInfo").FindChild("MusicSlider").GetComponent<Slider>();
        this.backGround = this.viewRoot.transform.FindChild("SoundInfo").FindChild("LanguageText/Background").GetComponent<Toggle>();
        this.checkMark = this.viewRoot.transform.FindChild("SoundInfo").FindChild("LanguageText/Checkmark").GetComponent<Toggle>();
        UserName = this.viewRoot.transform.FindChild("UserInfo/UserName").GetComponent<Text>();
        ApplicationFacade.Instance.RegisterMediator(new SettingMediator(Mediators.HALL_SETTING, this));
        this.soundSlider.value = PlayerPrefs.GetFloat(PrefsKey.SOUNDSET);
        this.musicSlider.value = PlayerPrefs.GetFloat(PrefsKey.MUSICSET);
        this.backGround.isOn = PlayerPrefs.GetInt(PrefsKey.LUANAGE) > 0 ? true : false;
        this.checkMark.isOn = PlayerPrefs.GetInt(PrefsKey.LUANAGE) > 0 ? false : true;
    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_START;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.SETTING_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
        //if (GlobalData.AudioVolume == 0)
        //{
        //    this.soundToggle.isOn = false;
        //}
        //else
        //{
        //    this.soundToggle.isOn = true;
        //}
        //if (GlobalData.BGMVolume == 0)
        //{
        //    this.musicToggle.isOn = false;
        //}
        //else
        //{
        //    this.musicToggle.isOn = true;
        //}


        //if (SceneManager.GetActiveScene().name == SceneName.BATTLE)
        //{
        //    SwitchAccountButton.gameObject.SetActive(false);
        //}
        //else
        //{
        //    SwitchAccountButton.gameObject.SetActive(true);
        //}
    }

    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), base.OnHide);
    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Setting/SettingView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_SETTING);
    }
}