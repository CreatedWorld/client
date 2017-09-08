/* 
*┌──────────────────────────────────┐
*│　         Copyright(C) 2017 by Antiphon.All rights reserved.       │
*│                Author:by Locke Xie 2017-03-31  　　　　　　　　  　│
*└──────────────────────────────────┘
*
* 功 能： 
* 类 名： RecorderSystem.cs
* 
* 修改历史：
* 
* 
*/

using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class RecorderSystem
{
    /// <summary>
    /// 是否录音
    /// </summary>
    bool recorderFlag = false;
    /// <summary>
    /// 录音数据
    /// </summary>
    float[] sampleBuffer;
    /// <summary>
    /// 录音起始位置(采样数)
    /// </summary>
    int sampleIndex = 0;
    /// <summary>
    /// 录音长度
    /// </summary>
    float audioLength = 0;
    /// <summary>
    /// 麦克风名称
    /// </summary>
    string device;
    /// <summary>
    /// 录音文件
    /// </summary>
    AudioClip clip = null;
    /// <summary>
    /// 录音音量
    /// </summary>
    float audioVolume;
    /// <summary>
    /// 缓存字典
    /// </summary>
    static SortedDictionary<int, AudioCache> AudioCacheDic = new SortedDictionary<int, AudioCache>();

    /// <summary>
    /// 录制完成回调
    /// </summary>
    public Action<int, byte[]> OnReComplete;
    /// <summary>
    /// 接收数据回调
    /// </summary>
    public Action<AudioPacket> OnGetComplete;

    /// <summary>
    /// 默认录音采样频率
    /// </summary>
    public int frequency = 8000;
    /// <summary>
    /// 降噪比率（每次发送采样数）
    /// </summary>
    public int sampleSize = 800;
    /// <summary>
    /// 最大录音长度 秒
    /// </summary>
    public int MaxAudioLength = 10;
    /// <summary>
    /// 最大同时接收语音人数
    /// </summary>
    public static int MaxSpeakNum = 4;
    /// <summary>
    /// 数据存活周期 秒
    /// </summary>
    public float MaxCacheLife = 5f;
    /// <summary>
    /// 音量回馈灵敏度
    /// </summary>
    public float VolumeSensibility = 4f;

    /// <summary>
    /// 录音标志
    /// </summary>
    public bool RecorderFlag { get { return recorderFlag; } }

    public int test;

    public RecorderSystem()
    {
        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
    }

    /// <summary>
    /// 获取录音音量大小
    /// </summary>
    /// <param name="min">音量取值下限</param>
    /// <param name="max">音量取值上限</param>
    /// <returns></returns>
    public float GetMicoVolume(float min = 0, float max = 1)
    {
        if (max < min)
        {
            Debug.LogError(string.Format("音量取值上限：{0}，小于音量取值下限{1}！", max, min));
            min = 0;
            max = 1;
        }
        return min + ((max - min) * audioVolume);
    }

    /// <summary>
    /// 更新函数
    /// </summary>
	public void Update()
    {
        if (recorderFlag)
        {
            int recordingPos = Microphone.GetPosition(device);
            audioLength = (float)recordingPos / (float)frequency;
            //满一次指定采样数就发送一次
            while (sampleIndex + sampleSize <= recordingPos)
            {
                if (recorderFlag)
                    OnResample();
                //#if DEBUG
                //Debug.Log("录音：" + recordingPos);
                //#endif
            }

            //设置录音上限
            if (audioLength >= MaxAudioLength)
            {
                //录音停止
                if (recorderFlag)
                    StopRecording();
            }
        }

        if (AudioCacheDic == null)
            AudioCacheDic = new SortedDictionary<int, AudioCache>();

        //缓存检测
        if (AudioCacheDic.Count > 0)
        {
            var item = AudioCacheDic.First();

            //录音取消或达到最大录音上限
            if (item.Value.flag == 2 || item.Value.flag != -1 && (Time.time - item.Value.lastUpdateTime) >= MaxCacheLife)
            {
                AudioCacheDic.Remove(item.Key);
            }

            //读取已完成的数据
            if (item.Value.flag == 1 && OnGetComplete != null)
            {
                AudioPacket packet = new AudioPacket();
                packet.LocalId = item.Value.id;
                packet.AudioLength = item.Value.audioLength;
                packet.Clip = AudioClip.Create("speak: " + item.Key, (int)(item.Value.audioLength * frequency), 1, frequency, false);
                packet.Clip.SetData(item.Value.data.ToArray(), 0);
                item.Value.Reset();
                OnGetComplete(packet);
            }
        }
    }

    /// <summary>
    /// 数据释放
    /// </summary>
    public void OnDisable()
    {
        Microphone.End(null);
        recorderFlag = false;
        audioLength = 0;
        sampleIndex = 0;
        audioVolume = 0;
        AudioCacheDic.Clear();
    }

    //开始录音
    public void Recording()
    {
        if (Microphone.devices.Length > 0)
        {
            device = Microphone.devices[0];
        }

        if (device == null)
            return;

        //以最高采样度进行采样
        //获取每秒采样率
        sampleBuffer = new float[sampleSize];
        Microphone.End(null);
        clip = Microphone.Start(device, false, MaxAudioLength, frequency);
        recorderFlag = true;
    }

    /// <summary>
    /// 取消录音
    /// </summary>
    public void CancelReacording()
    {
        if (recorderFlag)
        {
            Microphone.End(null);
            if (audioLength > .2f)
                EndResample(2);
            recorderFlag = false;
            audioLength = 0;
            sampleIndex = 0;
            audioVolume = 0;
        }
    }

    /// <summary>
    /// 结束录音
    /// </summary>
    public void StopRecording()
    {
        if (recorderFlag)
        {
            Microphone.End(null);
            if (audioLength > .2f)
                EndResample(1);
            recorderFlag = false;
            audioLength = 0;
            sampleIndex = 0;
            test = 0;
        }
    }

    /// <summary>
    /// 获取音频
    /// </summary>
    /// <param name="id"></param>
    /// <param name="flag"></param>
    /// <param name="source"></param>
    public static void GetAudioPacket(int id, int flag, byte[] source)
    {
        CompressPacket packet = AudioDecoder(source);
        byte[] data = new byte[packet.DataLength];
        Buffer.BlockCopy(packet.Data, 0, data, 0, packet.CompLength);
        float[] sample = Speex.DeCompress(data, packet.CompLength);

        AudioCache Cache = null;
        if (AudioCacheDic.Keys.Contains(id))
        {
            Cache = AudioCacheDic[id];
        }
        else
        {
            //最大接收人数限制
            if (AudioCacheDic.Count < MaxSpeakNum)
            {
                Cache = new AudioCache(id, flag);
                AudioCacheDic.Add(id, Cache);
            }
        }
        if (Cache != null)
            Cache.UpData(flag, packet.AudioLength, sample);
        //#if DEBUG
        //        Debug.Log("接收：" + Cache.data.Count);
        //#endif
    }

    //截取录音
    void OnResample()
    {
        if (clip)
        {
            clip.GetData(sampleBuffer, sampleIndex);
            float[] buff = new float[sampleSize];
            GetVolume(sampleBuffer);
            Resample(sampleBuffer, buff);
            sampleIndex += sampleSize;
            TransmitBuffer(0, buff);
        }
    }

    /// <summary>
    /// 录音结束 最后一次打包
    /// </summary>
    /// <param name="flag">2 取消录音  1 录音完成</param>
    void EndResample(int flag)
    {
        if (clip)
        {
            clip.GetData(sampleBuffer, sampleIndex);
            float[] buff = new float[sampleSize];
            Resample(sampleBuffer, buff);
            sampleIndex += sampleSize;
            TransmitBuffer(flag, buff);
        }
    }

    /// <summary>
    /// 录音大小
    /// </summary>
    /// <param name="samples"></param>
    void GetVolume(float[] samples)
    {
        float sum = 0;
        for (int i = 0; i < samples.Length; ++i)
        {
            sum += Mathf.Abs(samples[i]);
        }
        audioVolume = Mathf.Min(sum * VolumeSensibility / samples.Length, 1);
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_BATTLEVIEW_UPDATEVOLUME, GetMicoVolume(1,5));
    }

    /// <summary>
    /// 数据封装发送 flag  flag 2表示取消  0表示未完成  1表示完成
    /// </summary>
    /// <param name="flag"></param>
    /// <param name="sampleBuffer"></param>
    void TransmitBuffer(int flag, float[] sampleBuffer)
    {
        //#if DEBUG
        //        Debug.Log("发送：" + (test += sampleBuffer.Length));
        //#endif
        int length = 0;
        byte[] buffer = Speex.SpeexCompress(sampleBuffer, out length);
        CompressPacket packet = new CompressPacket();
        byte[] availableSampleBuffer = new byte[length];
        Buffer.BlockCopy(buffer, 0, availableSampleBuffer, 0, length);
        packet.AudioLength = audioLength;
        packet.Data = availableSampleBuffer;
        packet.CompLength = length;
        packet.DataLength = buffer.Length;
        if (OnReComplete != null)
        {
            OnReComplete(flag, AudioEncoder(packet));
        }
    }

    /// <summary>
    /// 音频去噪
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dst"></param>
    void Resample(float[] src, float[] dst)
    {
        if (src.Length == dst.Length)
        {
            Array.Copy(src, 0, dst, 0, src.Length);
        }
        else
        {
            float rec = 1.0f / (float)dst.Length;

            for (int i = 0; i < dst.Length; ++i)
            {
                //将源数据平均分配至指定大小
                float interp = rec * (float)i * (float)src.Length;
                dst[i] = src[(int)interp];
            }
        }
    }

    byte[] AudioEncoder(CompressPacket packet)
    {
        //留出前三个int32长度分别存 压缩长度 压缩后数据长度 录音长度 
        byte[] buffer = new byte[12 + packet.Data.Length];
        byte[] lengthBuffer = BitConverter.GetBytes(packet.CompLength);
        Buffer.BlockCopy(lengthBuffer, 0, buffer, 0, lengthBuffer.Length);
        byte[] dataLengtBuffer = BitConverter.GetBytes(packet.DataLength);
        Buffer.BlockCopy(dataLengtBuffer, 0, buffer, 4, dataLengtBuffer.Length);
        byte[] audioLengthBuffer = BitConverter.GetBytes(packet.AudioLength);
        Buffer.BlockCopy(audioLengthBuffer, 0, buffer, 8, audioLengthBuffer.Length);
        //三个int32长度之后写入音频数据
        for (int iter = 0; iter < packet.Data.Length; ++iter)
        {
            buffer[12 + iter] = packet.Data[iter];
        }
        return buffer;
    }

    static CompressPacket AudioDecoder(byte[] buffer, int offest = 0)
    {
        CompressPacket packet = new CompressPacket();
        packet.CompLength = BitConverter.ToInt32(buffer, offest);
        packet.DataLength = BitConverter.ToInt32(buffer, offest + 4);
        packet.AudioLength = BitConverter.ToSingle(buffer, offest + 8);
        packet.Data = new byte[packet.CompLength];
        Buffer.BlockCopy(buffer, offest + 12, packet.Data, 0, packet.CompLength);
        return packet;
    }

    class CompressPacket
    {
        //压缩长度
        public int CompLength;
        //压缩后数据长度
        public int DataLength;
        //数据
        public byte[] Data;
        //录音长度
        public float AudioLength;
    }

    class AudioCache : IComparable<AudioCache>
    {
        //最后更新时间
        public float lastUpdateTime;
        //当前状态 -1空闲  0接收数据中  1接收数据完成 2数据取消
        public int flag = -1;
        //ID 识别id
        public int id = -1;
        //语音时长 秒
        public float audioLength = -1;
        //音频数据
        public List<float> data = new List<float>();

        public AudioCache(int id, int flag)
        {
            lastUpdateTime = Time.time;
            this.flag = flag;
            this.id = id;
            data = new List<float>();
        }

        public void UpData(int flag, float length, float[] data)
        {
            lastUpdateTime = Time.time;
            this.flag = flag;
            this.audioLength = length;
            this.data.AddRange(data);
        }

        public void Reset()
        {
            flag = 0;
            audioLength = -1;
            data.Clear();
        }

        public int CompareTo(AudioCache other)
        {
            return lastUpdateTime.CompareTo(other.lastUpdateTime);
        }
    }
}

public class AudioPacket
{
    public int LocalId;
    public float AudioLength;
    public AudioClip Clip;
}