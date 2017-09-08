using Platform.Model.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 创建房间View
/// </summary>
public class CreateRoomView : UIView
{
	/// <summary>
	/// 关闭按钮
	/// </summary>
	public Button CloseButton;
	/// <summary>
	/// 创建房间按钮
	/// </summary>
	public Button CreateButton;
	/// <summary>
	/// 选择游戏模式控件
	/// </summary>
	public Transform ModeTrans;
	/// <summary>
	/// 游戏局数
	/// </summary>
	public Toggle EightRound;//8局房
    public Toggle InfinityRound;//一锅

    /// <summary>
    /// 房费
    /// </summary>
    public Toggle CreatorPay;
    public Toggle AAPay;

    /// <summary>
    /// 游戏规则
    /// </summary>
    public Toggle ThreeShoot;//3jiapao
	public Toggle OneShoot;//1jiapao 
    public Toggle EggCanLieDown;
    public Toggle EggCannotLieDown;
    public Toggle Run3;
    public Toggle Run5;
    public Toggle Run10;
    public Button RoundTipsBtn;
    public GameObject RoundTipsObj;
    public Button SanjiapaoTipsBtn;
    public Button YijiapaoTipsBtn;
    public GameObject RunTipsObj;
    public Text RunTipsText;

    public override void OnInit ()
	{
		viewRoot = LaunchUIView ("Prefab/UI/Hall/CreateRoomView");
		CloseButton = viewRoot.transform.FindChild ("CloseButton").GetComponent<Button> ();
		//ModeTrans = viewRoot.transform.FindChild ("Round");

        EightRound = viewRoot.transform.FindChild ("Round/EightMode").GetComponent<Toggle> ();
		InfinityRound = viewRoot.transform.FindChild ("Round/InfinityMode").GetComponent<Toggle> ();
        RoundTipsBtn = viewRoot.transform.FindChild("Round/EightMode/ModelDescribeImage").GetComponent<Button>();
        RoundTipsObj = viewRoot.transform.FindChild("Round/what").gameObject;

        CreatorPay = viewRoot.transform.FindChild("PayWay/CreatorPay").GetComponent<Toggle>();
        AAPay = viewRoot.transform.FindChild("PayWay/AAPay").GetComponent<Toggle>();

        ThreeShoot = viewRoot.transform.FindChild("Rule/Rule_0/Three").GetComponent<Toggle>();
        SanjiapaoTipsBtn = viewRoot.transform.FindChild("Rule/Rule_0/Three/ModelDescribeImage").GetComponent<Button>();
        OneShoot = viewRoot.transform.FindChild("Rule/Rule_0/One").GetComponent<Toggle>();
        YijiapaoTipsBtn = viewRoot.transform.FindChild("Rule/Rule_0/One/ModelDescribeImage").GetComponent<Button>();
        RunTipsObj = viewRoot.transform.FindChild("Rule/Rule_0/what").gameObject;
        RunTipsText = viewRoot.transform.FindChild("Rule/Rule_0/what/Text").GetComponent<Text>();

        EggCanLieDown = viewRoot.transform.FindChild("Rule/Rule_1/EggCanLieDown").GetComponent<Toggle>();
        EggCannotLieDown = viewRoot.transform.FindChild("Rule/Rule_1/EggCannotLieDown").GetComponent<Toggle>();
        Run3 = viewRoot.transform.FindChild("Rule/Rule_2/Three").GetComponent<Toggle>();
        Run5 = viewRoot.transform.FindChild("Rule/Rule_2/Five").GetComponent<Toggle>();
        Run10 = viewRoot.transform.FindChild("Rule/Rule_2/Ten").GetComponent<Toggle>();   

        CreateButton = this.viewRoot.transform.FindChild ("CreateButton").GetComponent<Button> ();

        RoundTipsBtn.onClick.AddListener(()=> { if (RoundTipsObj.activeSelf) RoundTipsObj.SetActive(false); else RoundTipsObj.SetActive(true); });
        YijiapaoTipsBtn.onClick.AddListener(()=> {
            if (RunTipsObj.activeSelf) {
                if (RunTipsText.text == "点炮一家付分")
                {
                    RunTipsObj.SetActive(false);
                }
                else
                {
                    RunTipsText.text = "点炮一家付分";
                }
            }
            else
            {
                RunTipsObj.SetActive(true);
                RunTipsText.text = "点炮一家付分";
            }
        });
        SanjiapaoTipsBtn.onClick.AddListener(() => {
            if (RunTipsObj.activeSelf)
            {
                if (RunTipsText.text == "点炮三家付分")
                {
                    RunTipsObj.SetActive(false);
                }
                else
                {
                    RunTipsText.text = "点炮三家付分";
                }
            }
            else
            {
                RunTipsObj.SetActive(true);
                RunTipsText.text = "点炮三家付分";
            }
        });
        ApplicationFacade.Instance.RegisterMediator (new CreateRoomMediator (Mediators.HALL_CREATEROOM, this));
	}

	public override ESceneID UISceneID {
		get {
			return ESceneID.SCENE_HALL;
		}

		set {
			base.UISceneID = value;
		}
	}

	public override void OnShow ()
	{
		base.OnShow ();
		UIManager.Instance.ShowUIMask (UIViewID.CREATEROOM_VIEW);
		UIManager.Instance.ShowDOTween (this.viewRoot.GetComponent<RectTransform> ());

        if (PlayerPrefs.GetString(PrefsKey.ROUND)!="8")
        {
            InfinityRound.isOn = true;
        }
        else
        {
            EightRound.isOn = true;
        }
        if (PlayerPrefs.GetString(PrefsKey.PAYWAY)!= "玩家均摊")
        {
            CreatorPay.isOn = true;
        }
        else
        {
            AAPay.isOn = true;
        }
        if (PlayerPrefs.GetString(PrefsKey.RULE1) != "一家炮")
        {
            ThreeShoot.isOn = true;
        }
        else
        {
            OneShoot.isOn = true;
        }
        if (PlayerPrefs.GetString(PrefsKey.RULE2) != "蛋不翻")
        {
            EggCanLieDown.isOn = true;
        }
        else
        {
            EggCannotLieDown.isOn = true;
        }
        if (PlayerPrefs.GetString(PrefsKey.RULE3) == "长跑10")
        {
            Run10.isOn = true;
        }
        else if(PlayerPrefs.GetString(PrefsKey.RULE3) == "长跑5")
        {
            Run5.isOn = true;
        }
        else
        {
            Run3.isOn = true;
        }

    }

	public override void OnHide ()
	{
		UIManager.Instance.HidenDOTween (this.viewRoot.GetComponent<RectTransform> (), base.OnHide);

	}

	public override void LoadUI ()
	{
		this.viewRootCache = Resources.Load<GameObject> ("Prefab/UI/Hall/CreateRoomView");
	}

	public override void OnDestroy ()
	{
		base.OnDestroy ();
		ApplicationFacade.Instance.RemoveMediator (Mediators.HALL_CREATEROOM);
	}
}
