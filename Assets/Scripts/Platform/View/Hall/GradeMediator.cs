using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Timers;
using Platform.Net;
using Platform.Model;
public class GradeMediator : Mediator, IMediator
{
    //private ArrayList gradeList;
    public GradeMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {
        //this.gradeList = new ArrayList();
    }
    public GradeView View
    {
        get
        {
            return (GradeView)ViewComponent;
        }
        
    }
    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_HALL_VIEWROOM_RECORD);
        list.Add(NotificationConstant.MEDI_HALL_GETGRADEINFO);
        list.Add(NotificationConstant.MEDI_HALL_INITGRADEINFO);
        list.Add(NotificationConstant.MEDI_HALL_GETROUNDINFO);
        return list;
    }
    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_HALL_VIEWROOM_RECORD:
                this.View.RecordScrollView.SetActive(false);
                this.View.ParticularsScrollView.SetActive(true);
                View.RecordTitleObj.SetActive(false);
                break;
            case NotificationConstant.MEDI_HALL_GETGRADEINFO:
                this.SendGetGrade();
                break;
            case NotificationConstant.MEDI_HALL_INITGRADEINFO:
                this.InitGradeInfo(notification.Body);
                break;
            case NotificationConstant.MEDI_HALL_GETROUNDINFO:
                this.InifRoundInfo(notification.Body);
                break;
            default:
                break;
        }
    }
    public override void OnRegister()
    {
        base.OnRegister();
        this.View.CloseButton = this.View.viewRoot.transform.FindChild("CloseButton").GetComponent<Button>();
        this.View.ButtonAddListening(this.View.CloseButton,
            () =>
            {
                UIManager.Instance.HideUI(UIViewID.GRADE_VIEW);
            });
    }
    public override void OnRemove()
    {
        base.OnRemove();
    }

    //模拟获取战绩数据
    private void SendGetGrade()
    {
        GetGradeInfoC2S package = new GetGradeInfoC2S();
        NetMgr.Instance.SendBuff<GetGradeInfoC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_GET_BATTLE_HISTORY.GetHashCode(),0,package);
    }

    /// <summary>
    /// 初始化战绩数据
    /// </summary>
    /// <param name="body"></param>
    private void InitGradeInfo(object body)
    {
        ArrayList gradeList = new ArrayList();
        GetGradeInfoS2C package = (GetGradeInfoS2C)body;
        for (int i = 0; i < package.gradeDataS2C.Count; i++)
        {
            GradeDataS2C gradeInfo = package.gradeDataS2C[i];
            gradeList.Add(new GradeScrollData(i,gradeInfo.roomID,gradeInfo.time,gradeInfo.roomCode,gradeInfo.usersInfo));
        }
        this.View.GradeTableView.DataProvider = gradeList;
    }
    /// <summary>
    /// 初始化战绩房间数据
    /// </summary>
    /// <param name="body"></param>
    private void InifRoundInfo(object body)
    {
        ArrayList roundList = new ArrayList();
        GetRoundInfoS2C package = (GetRoundInfoS2C)body;
        for (int i = 0;i < package.roundData.Count;i++)
        {
            RoundData roundData = package.roundData[i];
            roundList.Add(new ParticularsScrollData(i + 1,roundData.roomID,roundData.roomCode,roundData.time, roundData.reportId,roundData.usersInfo));
        }
        this.View.ParticularsTableView.DataProvider = roundList;
    }
    
}