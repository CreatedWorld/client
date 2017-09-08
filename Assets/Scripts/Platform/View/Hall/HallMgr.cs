using Platform.Model;
using Platform.Net;
using Platform.Utils;
using System.Collections.Generic;
using UnityEngine;
public class HallMgr : MonoBehaviour {
    private HallView hallView;
    private TopMenuView topView;
    private MiddleMenuView middleView;
    private BottomMenuView bottomView;
    public TopMenuView TopView
    {
        get
        {
            return topView;
        }

        set
        {
            topView = value;
        }
    }

    public MiddleMenuView MiddleView
    {
        get
        {
            return middleView;
        }

        set
        {
            middleView = value;
        }
    }

    public BottomMenuView BottomView
    {
        get
        {
            return bottomView;
        }

        set
        {
            bottomView = value;
        }
    }

    public HallView HallView
    {
        get
        {
            return hallView;
        }

        set
        {
            hallView = value;
        }
    }
    public HallRoomListView HallRoomListView;
    void Awake()
    {
        ApplicationFacade.Instance.RegisterMediator(new HallMediator(Mediators.HALL_MEDIATOR,this));
    }
    void Start ()
    {
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_REFRESHUSERINFO);
        //ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_REFRESHHALLNOTICE);
    }
    void Update () {
        this.RollAnnouncement();
    }
    void OnDestroy()
    {
        ApplicationFacade.Instance.RemoveMediator(Mediators.HALL_MEDIATOR);
        if (UIManager.Instance.Background != null)
        {
            UIManager.Instance.Background.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 跑马灯
    /// </summary>
    private void RollAnnouncement()
    {
        if (this.MiddleView.announcementText == null || this.MiddleView.announcementText.text == "")
        {
            return;
        }
        this.MiddleView.announcementText.transform.Translate(Vector3.right * Time.deltaTime * -0.3f);
        if (this.MiddleView.announcementText.rectTransform.localPosition.x < -510)
        {
            ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_ANNOUNCEMENTFINISH);
        }
    }
}
