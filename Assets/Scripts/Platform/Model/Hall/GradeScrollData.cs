using Platform.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 战绩信息数据
/// </summary>
public class GradeScrollData {
    /// <summary>
    /// 序号
    /// </summary>
    private int rankingID;
    /// <summary>
    /// 房间ID
    /// </summary>
    private int roomID;
    /// <summary>
    /// 时间
    /// </summary>
    private long time;
    /// <summary>
    /// 显示房间号
    /// </summary>
    private string roomCode;
    /// <summary>
    /// 用户信息
    /// </summary>
    private List<UsersInfoS2C> usersInfo;
    public GradeScrollData(int rankingID, int roomID, long time, string roomCode, List<UsersInfoS2C> usersInfo)
    {
        this.RankingID = rankingID;
        this.RoomID = roomID;
        this.Time = time;
        this.roomCode = roomCode;
        this.UsersInfo = usersInfo;
    }
    public int RankingID
    {
        get
        {
            return rankingID;
        }

        set
        {
            rankingID = value;
        }
    }

    public int RoomID
    {
        get
        {
            return roomID;
        }

        set
        {
            roomID = value;
        }
    }

    public long Time
    {
        get
        {
            return time;
        }

        set
        {
            time = value;
        }
    }

    public string RoomCode
    {
        get
        {
            return roomCode;
        }

        set
        {
            roomCode = value;
        }
    }

    public List<UsersInfoS2C> UsersInfo
    {
        get
        {
            return usersInfo;
        }

        set
        {
            usersInfo = value;
        }
    }
}
