using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platform.Model;
/// <summary>
/// 对战信息数据
/// </summary>
public class ParticularsScrollData {
    private int roomID;
    /// <summary>
    /// 信息ID
    /// </summary>
    private int scrollID;
    /// <summary>
    /// 房间号
    /// </summary>
    private string roomCode;
    /// <summary>
    /// 战报id
    /// </summary>
    private int reportId;
    /// <summary>
    /// 时间
    /// </summary>
    private long time;
    /// <summary>
    /// 用户信息
    /// </summary>
    private List<UsersInfoS2C> usersInfo;
    public ParticularsScrollData(int scrollID ,int roomID,string roomCode, long time, int reportId,List<UsersInfoS2C> usersInfo)
    {
        this.scrollID = scrollID;
        this.roomID = roomID;
        this.roomCode = roomCode;
        this.time = time;
        this.reportId = reportId;
        this.usersInfo = usersInfo;
    }
    public string RoomCode
    {
        get
        {
            return roomCode;
        }
    }

    public long Time
    {
        get
        {
            return time;
        }
    }

    public List<UsersInfoS2C> UsersInfo
    {
        get
        {
            return usersInfo;
        }
    }

    public int ScrollID
    {
        get
        {
            return scrollID;
        }
    }

    public int RoomID
    {
        get
        {
            return roomID;
        }
    }

    public int ReportId
    {
        get
        {
            return reportId;
        }
    }
}
