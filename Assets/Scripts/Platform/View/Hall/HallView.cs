using UnityEngine;
/// <summary>
/// 大厅View,背景
/// </summary>
public class HallView : UIView
{
    private CanvasGroup canvasGroup;

    public CanvasGroup CanvasGroup
    {
        get
        {
            return canvasGroup;
        }
    }

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Hall/HallView");
        this.canvasGroup = this.viewRoot.GetComponent<CanvasGroup>();
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
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Hall/HallView");
    }
}
