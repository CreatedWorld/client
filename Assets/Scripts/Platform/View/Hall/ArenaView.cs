using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 比赛场界面
/// </summary>
public class ArenaView : UIView
{
    /// <summary>
    /// 申请时间描述
    /// </summary>
    public Text applyTimeValueTxt;
    /// <summary>
    /// 开局时间描述
    /// </summary>
    public Text battleTimeValueTxt;
    /// <summary>
    /// 规则描述
    /// </summary>
    public Text ruleValueTxt;
    /// <summary>
    /// 奖励描述
    /// </summary>
    public Text rewardValueTxt;
    /// <summary>
    /// 申请按钮
    /// </summary>
    public Button applyBtn;
    /// <summary>
    /// 匹配战斗按钮
    /// </summary>
    public Button battleBtn;
    /// <summary>
    /// 排行榜按钮
    /// </summary>
    public Button rankBtn;
    /// <summary>
    /// 申请成功按钮
    /// </summary>
    public Button applySucessBtn;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button closeBtn;
    public override void OnInit()
    {
        this.viewRoot = LaunchUIView("Prefab/UI/Arena/ArenaView");
        this.applyTimeValueTxt = viewRoot.transform.Find("ApplyTimeValueTxt").GetComponent<Text>();
        this.battleTimeValueTxt = viewRoot.transform.Find("BattleTimeValueTxt").GetComponent<Text>();
        this.ruleValueTxt = viewRoot.transform.Find("RuleValueTxt").GetComponent<Text>();
        this.rewardValueTxt = viewRoot.transform.Find("RewardValueTxt").GetComponent<Text>();
        this.applyBtn = viewRoot.transform.Find("ApplyBtn").GetComponent<Button>();
        this.battleBtn = viewRoot.transform.Find("BattleBtn").GetComponent<Button>();
        this.rankBtn = viewRoot.transform.Find("RankBtn").GetComponent<Button>();
        this.applySucessBtn = viewRoot.transform.Find("ApplySucessBtn").GetComponent<Button>();
        this.closeBtn = viewRoot.transform.Find("CloseBtn").GetComponent<Button>();
        ApplicationFacade.Instance.RegisterMediator(new ArenaViewMediator(Mediators.ARENA_MEDIATOR, this));
    }

    public override ESceneID UISceneID
    {
        get
        {
            return ESceneID.SCENE_HALL;
        }

        set
        {
            base.UISceneID = value;
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.ARENA_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }
    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(viewRoot.GetComponent<RectTransform>(), base.OnHide);
    }
    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Arena/ArenaView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ApplicationFacade.Instance.RemoveMediator(Mediators.ARENA_MEDIATOR);
    }
}