using UnityEngine;
using UnityEngine.UI;
public class MatchingView : UIView
{
    private Button closeButton;
    private MatchingMediator macthingMedi;
    private Text round;
    private Text minuteText;
    private Text secondText;
    public Button CloseButton
    {
        get
        {
            return closeButton;
        }
    }

    public Text Round
    {
        get
        {
            return round;
        }
    }

    public Text MinuteText
    {
        get
        {
            return minuteText;
        }
    }

    public Text SecondText
    {
        get
        {
            return secondText;
        }
    }

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Arena/MatchingView");
        this.closeButton = this.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        this.round = this.viewRoot.transform.FindChild("RoundTitle").FindChild("Round").GetComponent<Text>();
        this.minuteText = this.viewRoot.transform.FindChild("Timer").FindChild("Minute").GetComponent<Text>();
        this.secondText = this.viewRoot.transform.FindChild("Timer").FindChild("Second").GetComponent<Text>();
        this.macthingMedi = new MatchingMediator(Mediators.HALL_MATCHING,this);
        ApplicationFacade.Instance.RegisterMediator(this.macthingMedi);
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

    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Arena/MatchingView");
    }
    public override void OnShow()
    {
        base.OnShow();
        UIManager.Instance.ShowUIMask(UIViewID.MATCHING_VIEW);
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
    }
    public override void OnHide()
    {
        UIManager.Instance.HidenDOTween(this.viewRoot.GetComponent<RectTransform>(), base.OnHide);
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_INITMATCHINGTIME);
    }
    public override void OnDestroy()
    {
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_MATCHING);
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_INITMATCHINGTIME);
        base.OnDestroy();
    }
    public override void Update()
    {
        base.Update();
        this.macthingMedi.TimeCount();
    }
}
