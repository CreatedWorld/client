using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Platform.Model;
using Platform.Net;
/// <summary>
/// 对战信息View
/// </summary>
public class ParticularsTableItem : TableViewItem {
    /// <summary>
    /// 返回按钮
    /// </summary>
    private Button playbackButton;
    /// <summary>
    /// 房间号
    /// </summary>
    private Text roomNumText;
    /// <summary>
    /// 回合
    /// </summary>
    private Text roundNumText;
    /// <summary>
    /// 时间
    /// </summary>
    private Text time;
    /// <summary>
    /// 参赛者信息
    /// </summary>
    private List<Text> userNames;
    /// <summary>
    /// 对战信息数据
    /// </summary>
    private ParticularsScrollData particularsScrollData;

    public ParticularsTableItem()
    {
    }
    public override void Updata(object data)
    {
        if (data == null)
        {
            return;
        }
        base.Updata(data);
        particularsScrollData = (ParticularsScrollData)data;
        roundNumText.text = "第" + particularsScrollData.ScrollID.ToString() + "场";
        roomNumText.text = particularsScrollData.RoomCode;
        time.text = TimeHandle.Instance.GetDateTimeByTimestamp(particularsScrollData.Time).ToString("yy-MM-dd HH:mm:ss");
        for (int i = 0; i < particularsScrollData.UsersInfo.Count ; i++)
        {
            userNames[i].text = particularsScrollData.UsersInfo[i].userName + ":" + particularsScrollData.UsersInfo[i].score.ToString();
        }
    }

    private void Awake()
    {
        roomNumText = transform.FindChild("RoomNumText").GetComponent<Text>();
        roundNumText = transform.FindChild("RoundNum").GetComponent<Text>();
        time = transform.FindChild("RoundTime").GetComponent<Text>();
        playbackButton = transform.FindChild("PlaybackButton").GetComponent<Button>();
        playbackButton.onClick.AddListener(SendPlayback);
        userNames = new List<Text>();
        foreach (Transform particularsInfo in transform.FindChild("Players"))
        {
            userNames.Add(particularsInfo.GetComponent<Text>());
        }
    }

    /// <summary>
    /// 回放按钮
    /// </summary>
    /// <param name="body"></param>
    private void SendPlayback()
    {
        PlayVideoC2S package = new PlayVideoC2S();
        package.round = particularsScrollData.ReportId;
        package.roomId = particularsScrollData.RoomID;
        NetMgr.Instance.SendBuff<PlayVideoC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_Get_Replay_Data.GetHashCode(), 0, package);
    }
}
