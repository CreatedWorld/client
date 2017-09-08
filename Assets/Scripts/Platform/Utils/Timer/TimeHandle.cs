using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TimeHandle{
    private static TimeHandle instance;
    private TimeHandle()
    {
    }
    public static TimeHandle Instance
    {
        get
        {
            if (instance == null) {
                instance = new TimeHandle();
            }
            return instance;
        }
    }
    /// <summary>
    /// 获取当前系统时间戳
    /// </summary>
    /// <returns></returns>
    public long GetTimestamp()
    {
        long timestamp = (long)DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds;
        return timestamp;
    }
    /// <summary>
    /// 根据时间戳获取一个DateTime
    /// </summary>
    /// <param name="timestamp">时间戳</param>
    /// <returns></returns>
    public DateTime GetDateTimeByTimestamp(long timeStamp)
    {
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
        DateTime dataTime = startTime.AddMilliseconds(timeStamp);
        return dataTime;
    }

    /// <summary>
    /// 转换秒数为 mm:ss
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public string ParseSecond(int second)
    {
        int min = second / 60;
        int sec = second - min * 60;
        string result = "";
        if (min < 10)
        {
            result += "0";
        }
        result += min + ":";
        if (sec < 10)
        {
            result += "0";
        }
        result += sec;
        return result;
    }
}