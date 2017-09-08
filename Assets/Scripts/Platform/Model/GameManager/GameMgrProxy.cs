using System;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;
/// <summary>
/// ����ģ������
/// </summary>
public class GameMgrProxy : Proxy, IProxy
{
    /// <summary>
    /// ������ʱ
    /// </summary>
    public int pingBackMS;
    /// <summary>
    /// �첽��������
    /// </summary>
    public AsyncOperation async;
    private long _systemTime;
    private long _systemDateUT;
    private long _scaleSystemDateUT;
    private DateTime _systemDateTime;

    public GameMgrProxy(string NAME) : base(NAME)
    {
    }

    /// <summary>
    /// ��������ǰʱ��,����ʱ������Ӱ��
    /// </summary>
    public long systemTime
    {
        get
        {
            return _systemTime + TimeHandle.Instance.GetTimestamp() - _systemDateUT;
        }
        set
        {
            _systemDateUT = TimeHandle.Instance.GetTimestamp();
            _scaleSystemDateUT = (long)Time.time * 1000;
            _systemTime = value;
        }
    }

    /// <summary>
    /// ����������ʱ��,����ʱ������Ӱ��
    /// </summary>
    public DateTime systemDateTime
    {
        get
        {
            _systemDateTime = TimeHandle.Instance.GetDateTimeByTimestamp(systemTime);
            return _systemDateTime;
        }
    }

    /// <summary>
    /// ��������ǰʱ��,��ʱ������Ӱ��
    /// </summary>
    public long scaleSystemTime
    {
        get
        {
            return _systemTime + (long)(Time.time * 1000) - _scaleSystemDateUT;
        }
    }

    /// <summary>
    /// ��������ʱ���
    /// </summary>
    public void ReviseScaleSystemTime()
    {
        var curSystemTime = systemTime;
        _scaleSystemDateUT = _systemTime + (long)(Time.time * 1000) - curSystemTime;
    }

    /// <summary>
    /// ����ʱ�䵽Ŀ��ʱ��
    /// </summary>
    /// <param name="targetTime"></param>
    public void ReviseScaleTimeTo(long targetTime)
    {
        _scaleSystemDateUT = _systemTime + (long)(Time.time * 1000) - targetTime;
    }
}
