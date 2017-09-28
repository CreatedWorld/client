using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Platform.Model;
using Platform.Net;
using System.Collections;
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
    /// 对战时间
    /// </summary>
    private Text playTiemText;
    /// <summary>
    /// 时间
    /// </summary>
    private Text time;
    /// <summary>
    /// 头像
    /// </summary>
    private RawImage HeadImg1;
    /// <summary>
    /// 头像
    /// </summary>
    private RawImage HeadImg2;
    /// <summary>
    /// 头像
    /// </summary>
    private RawImage HeadImg3;
    /// <summary>
    /// 头像
    /// </summary>
    private RawImage HeadImg4;
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
        playTiemText.text = "对战时间:"+ TimeHandle.Instance.GetDateTimeByTimestamp(particularsScrollData.Time).ToString("HH:mm:ss"); ;
        roomNumText.text ="房间号:"+ particularsScrollData.RoomCode;
        time.text = "日期:"+TimeHandle.Instance.GetDateTimeByTimestamp(particularsScrollData.Time).ToString("yy/MM/dd");
        for (int i = 0; i < particularsScrollData.UsersInfo.Count ; i++)
        {
            userNames[i].text = particularsScrollData.UsersInfo[i].userName + "\r\n" + particularsScrollData.UsersInfo[i].score.ToString();
        }
        string str = particularsScrollData.UsersInfo[0].headImgUrl;
        Debug.Log(str);
        StartCoroutine(DownHeadImg(HeadImg1,particularsScrollData.UsersInfo[0].headImgUrl));
        StartCoroutine(DownHeadImg(HeadImg2, particularsScrollData.UsersInfo[1].headImgUrl));
        StartCoroutine(DownHeadImg(HeadImg3, particularsScrollData.UsersInfo[2].headImgUrl));
        StartCoroutine(DownHeadImg(HeadImg4, particularsScrollData.UsersInfo[3].headImgUrl));
    }

    private void Awake()
    {
        roomNumText = transform.FindChild("RoomNumText").GetComponent<Text>();
        roundNumText = transform.FindChild("RoundNum").GetComponent<Text>();
        time = transform.FindChild("RoundTime").GetComponent<Text>();
        playbackButton = transform.FindChild("PlaybackButton").GetComponent<Button>();
        playTiemText = transform.FindChild("PlayTime").GetComponent<Text>();
        playbackButton.onClick.AddListener(SendPlayback);

        HeadImg1 = transform.FindChild("information_back/information_back_1/head").GetComponent<RawImage>();
        HeadImg2 = transform.FindChild("information_back/information_back_2/head").GetComponent<RawImage>();
        HeadImg3 = transform.FindChild("information_back/information_back_3/head").GetComponent<RawImage>();
        HeadImg4 = transform.FindChild("information_back/information_back_4/head").GetComponent<RawImage>();

        userNames = new List<Text>();
        foreach (Transform particularsInfo in transform.FindChild("Players"))
        {
            userNames.Add(particularsInfo.GetComponent<Text>());
        }
    }

    /// <summary>
	/// 下载头像
	/// </summary>
	/// <param name="headUrl"></param>
	/// <returns></returns>
	IEnumerator DownHeadImg(RawImage img,string headUrl)//RawImage img,
    {
        WWW www = new WWW(headUrl);
        yield return www;
        if (www.error == null)
        {
            img.texture = www.texture;
            //HeadImg1.texture = www.texture;
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
