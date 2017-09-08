
/**
*Copyright(C) 2017 by antiphon   
*All rights reserved.             
*Author:       PC-20170617HLDC           
*Version:      1.0          
*UnityVersion：5.5.0f3     
*Date:         2017-08-15 11:16:49             
*Description:    显示大厅房间列表项             
*History:    v1.0
*/
using Platform.Model;
using Platform.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HallRoomListItem : TableViewItem
{
    /// <summary>
    /// 大厅的房间列表信息集合
    /// </summary>
    private HallRoomInfo hallRoomInfo;
    /// <summary>
    /// 房间号
    /// </summary>
    private Text roomId;
    /// <summary>
    /// 局数
    /// </summary>
    private Text numOfGames;
    /// <summary>
    /// 人数
    /// </summary>
    private Text people;
    /// <summary>
    /// 邀请按钮
    /// </summary>
    private Button invitationBtn;
    /// <summary>
    /// 点击列表加入房间按钮
    /// </summary>
    private Button joinRoomBtn;
    /// <summary>
    /// 是否初始化
    /// </summary>
    private bool isAwake;

    private RectTransform itemTransform;
    private void Awake()
    {
        itemTransform = transform.GetComponent<RectTransform>();
        roomId = transform.FindChild("roomId").GetComponent<Text>();
        numOfGames = transform.FindChild("numOfGames").GetComponent<Text>();
        people = transform.FindChild("people").GetComponent<Text>();
        invitationBtn = transform.FindChild("invitation").GetComponent<Button>();
        joinRoomBtn = transform.FindChild("joinRoomBtn").GetComponent<Button>();
        isAwake = true;

        invitationBtn.onClick.AddListener(InvitatePepole);
        joinRoomBtn.onClick.AddListener(JoinRoomByHallRoomList);
    }

    public override void Updata(object data)
    {
        if (data == null)
        {
            return;
        }
        base.Updata(data);
        hallRoomInfo = data as HallRoomInfo;
        if (isAwake)
        {
            UpdateView();
        }
    }

    /// <summary>
    /// 更新界面数据
    /// </summary>
    private void UpdateView()
    {
        roomId.text = hallRoomInfo.roomId.ToString();
        numOfGames.text = hallRoomInfo.numOfGames;
        people.text = hallRoomInfo.pepole.ToString()+"/4";
    }

    private void JoinRoomByHallRoomList()
    {
        JoinInRoomC2S package = new JoinInRoomC2S();
        package.roomCode = roomId.text;
        package.seat = 0;
        NetMgr.Instance.SendBuff<JoinInRoomC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_JOIN_IN_ROOM.GetHashCode(), 0, package);
        Debug.Log("加入房间："+ roomId.text);
    } 

    /// <summary>
    /// 弹出微信邀请界面
    /// </summary>
    private void InvitatePepole()
    {
        UnityEngine.Debug.Log("弹出微信页面");
    }
}