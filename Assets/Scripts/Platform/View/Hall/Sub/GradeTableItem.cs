using Platform.Model;
using Platform.Net;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 战绩信息View
/// </summary>
public class GradeTableItem : TableViewItem {
    /// <summary>
    /// 序号
    /// </summary>
    private Text rankingID;
    /// <summary>
    /// 显示房间号
    /// </summary>
    private Text roomCode;
    /// <summary>
    /// 时间
    /// </summary>
    private Text time;
    /// <summary>
    /// 参赛者名字控件集合
    /// </summary>
    private List<Text> userNames;
    /// <summary>
    /// 选择按钮
    /// </summary>
    private Button selectButton;
    /// <summary>
    /// 当前条目数据
    /// </summary>
    private GradeScrollData scrollViewData;
    public GradeTableItem()
    {
        
    }

    public override void Updata(object data)
    {
        if (data == null)
        {
            return;
        }
        base.Updata(data);
        scrollViewData = (GradeScrollData)data;
        rankingID.text = (scrollViewData.RankingID + 1).ToString();
        roomCode.text = scrollViewData.RoomCode;
        time.text = TimeHandle.Instance.GetDateTimeByTimestamp(scrollViewData.Time).ToString("yy-MM-dd HH:mm:ss");
        for (int i = 0;i < scrollViewData.UsersInfo.Count ;i++)
        {
            userNames[i].text = scrollViewData.UsersInfo[i].userName + ":" + scrollViewData.UsersInfo[i].score.ToString();
        }
    }

    private void Awake()
    {
        rankingID = transform.FindChild("RankingText").GetComponent<Text>();
        roomCode = transform.FindChild("RoomNumText").GetComponent<Text>();
        time = transform.FindChild("TimeText").GetComponent<Text>();
        selectButton = transform.FindChild("SelectButton").GetComponent<Button>();
        selectButton.onClick.AddListener(RecourdButtonEvent);
        userNames = new List<Text>();
        foreach (Transform playerInfo in transform.FindChild("Players"))
        {
            userNames.Add(playerInfo.GetComponent<Text>());
        }
    }

    /// <summary>
    /// 绑定回调按钮
    /// </summary>
    /// <param name="param"></param>
    private void RecourdButtonEvent()
    {
        GetRoundInfoC2S package = new GetRoundInfoC2S();
        package.roomID = scrollViewData.RoomID;
        NetMgr.Instance.SendBuff<GetRoundInfoC2S>(SocketType.HALL, MsgNoC2S.C2S_Hall_GET_BATTLE_DETAIL.GetHashCode(), 0, package);
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_HALL_VIEWROOM_RECORD);
    }
}
