
using Platform.Model;
/**
*Copyright(C) 2017 by antiphon   
*All rights reserved.             
*Author:       PC-20170617HLDC           
*Version:      1.0          
*UnityVersion：5.5.0f3     
*Date:         2017-08-15 11:22:49             
*Description:  房间列表界面的初始化                  
*History:    v1.0
*/
using Platform.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HallRoomListView : UIView
{
    /// <summary>
    /// 大厅房间列表滚动条,控制器组件
    /// </summary>
    public static TableView roomlistTable;

    public override void OnInit()
    {
        this.viewRoot = this.LaunchUIView("Prefab/UI/Hall/HallRoomListView", UIManager.Instance.GetUIView(UIViewID.HALL_VIEW).viewRoot.transform);
        roomlistTable = viewRoot.transform.Find("HallRoomListScrollView").GetComponent<TableView>();
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
        UIManager.Instance.ShowDOTween(this.viewRoot.GetComponent<RectTransform>());
        var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        ArrayList roomArr = new ArrayList();
        if (hallProxy.hallRoomListItem != null)
        {
            roomArr.AddRange(hallProxy.hallRoomListItem);
            roomlistTable.DataProvider = roomArr;
        }
        else
        {
            if (GlobalData.LoginServer == "127.0.0.1")
            {
                HallRoomInfo roomInfo = new HallRoomInfo();
                roomInfo.roomId = 123654+"";
                roomInfo.numOfGames = "1/8";
                roomInfo.pepole = 1;
                HallRoomInfo roomInfo2 = new HallRoomInfo();
                roomInfo2.roomId = 123654+"";
                roomInfo2.numOfGames = "一锅";
                roomInfo2.pepole = 3;
                List<HallRoomInfo> roomlist = new List<HallRoomInfo>();
                roomlist.Add(roomInfo);
                roomlist.Add(roomInfo2);

                roomArr.AddRange(roomlist);
                roomlistTable.DataProvider = roomArr;
            }

        }
    }


    public override void LoadUI()
    {
        this.viewRootCache = Resources.Load<GameObject>("Prefab/UI/Hall/HallRoomListView");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

}