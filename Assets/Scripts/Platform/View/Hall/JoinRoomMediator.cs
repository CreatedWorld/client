using System;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
using UnityEngine.UI;
using Platform.Model;
using Platform.Net;

public class JoinRoomMediator : Mediator, IMediator
{
    private HallProxy hallProxy;
    public JoinRoomMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }
    public JoinRoomView View
    {
        get
        {
            return (JoinRoomView)ViewComponent;
        }
    }

    public override void OnRegister()
    {
        base.OnRegister();
        this.View.ButtonAddListening(this.View.CloseButton,
        () =>
        {
            UIManager.Instance.HideUI(UIViewID.JOINROOM_VIEW);
            this.ClearRoomID();
        });

        foreach (Transform child in this.View.NumberTrans)
        {
            Button numBtn = child.GetComponent<Button>();
            this.View.ButtonAddListening(numBtn, InputRoomID, false,numBtn.gameObject.name);
        }

        this.View.RoomIDMAX = this.View.RoomNumTrans.childCount;
        foreach (Transform child in this.View.RoomNumTrans)
        {
            Text text = child.FindChild("NumText").GetComponent<Text>();
            this.View.RoomNumTexts.Add(text);
        }
        this.View.ButtonAddListening(this.View.DelButton, DeleteRoomID);
        this.View.ButtonAddListening(this.View.ResButton, ClearRoomID);
        this.hallProxy = Facade.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
    }
    public override void OnRemove()
    {
        base.OnRemove();
    }
    public void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    var startKeyValue = KeyCode.Keypad0.GetHashCode();
                    var endKeyValue = KeyCode.Keypad9.GetHashCode();
                    var keyCodeValue = keyCode.GetHashCode();
                    if (keyCodeValue >= startKeyValue && keyCodeValue <= endKeyValue)
                    {
                        string[] inputNum = { (keyCodeValue - startKeyValue).ToString() };
                        InputRoomID(inputNum);
                    }
                    startKeyValue = KeyCode.Alpha0.GetHashCode();
                    endKeyValue = KeyCode.Alpha9.GetHashCode();
                    if (keyCodeValue >= startKeyValue && keyCodeValue <= endKeyValue)
                    {
                        string[] inputNum = { (keyCodeValue - startKeyValue).ToString() };
                        InputRoomID(inputNum);
                    }
                    if (keyCodeValue == KeyCode.Backspace.GetHashCode())
                    {
                        DeleteRoomID();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 输入房间号
    /// </summary>
    /// <param name="param">输入数字,数字为按钮名字</param>
    private void InputRoomID(params object[] param)
    {
        string roomNum = param[0] as string;
        if (this.View.RoomNums.Count < this.View.RoomIDMAX)
        {
            Text t = this.View.RoomNumTexts[this.View.RoomNums.Count];
            t.text = roomNum;
            this.View.RoomNums.Add(roomNum);
            if (this.CheckRoomID())
            {
                this.SendRoomIDMsg();
            }
        }
    }
    /// <summary>
    /// 删除房间号
    /// </summary>
    private void DeleteRoomID()
    {
        if (this.View.RoomNums.Count > 0)
        {
            this.View.RoomNums.RemoveAt(this.View.RoomNums.Count - 1);
            Text t = this.View.RoomNumTexts[this.View.RoomNums.Count];
            t.text = "";
        }
    }
    /// <summary>
    /// 重置房间号
    /// </summary>
    private void ClearRoomID()
    {
        if (this.View.RoomNums.Count > 0)
        {
            foreach (Text t in this.View.RoomNumTexts)
            {
                t.text = "";
            }
            this.View.RoomNums.Clear();
        }
    }
    /// <summary>
    /// 发送房间号
    /// </summary>
    private void SendRoomIDMsg()
    {
        JoinInRoomC2S package = new JoinInRoomC2S();
        package.roomCode = this.hallProxy.HallInfo.roomCode;
        package.seat = 0;
        NetMgr.Instance.SendBuff<JoinInRoomC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_JOIN_IN_ROOM.GetHashCode(), 0, package);
    }

    /// <summary>
    /// 获取房间号
    /// </summary>
    /// <returns>房间号,string类型 -> int类型</returns>
    private string GetRoomID()
    {
        string roomID = "";
        foreach (string num in this.View.RoomNums)
        {
            roomID += num;
        }
        return roomID;
    }
    /// <summary>
    /// 检查房间号是否输入完成
    /// </summary>
    /// <returns>输入完成,返回true,没有返回false</returns>
    private bool CheckRoomID()
    {
        this.hallProxy.HallInfo.roomCode = this.GetRoomID();
        return this.View.RoomNums.Count >= this.View.RoomIDMAX;
    }
}

