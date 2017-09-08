using Platform.Model.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声音管理类
/// </summary>
public class AudioSystem : MonoBehaviour
{
    /// <summary>
    /// 当前背景音乐地址
    /// </summary>
    private string curBgmUrl;
    /// <summary>
    /// 背景音效
    /// </summary>
    private AudioSource bgmSource;
    /// <summary>
    /// 音效音频数组
    /// </summary>
    private List<AudioSource> effectAudioList;
    /// <summary>
    /// 正在播放的音频数组
    /// </summary>
    private List<AudioSource> playingAudioList;

    /// <summary>
    /// 正在播放的音频url地址
    /// </summary>
    private List<string> playingUrlList;
    /// <summary>
    /// 声音字典
    /// </summary>
    private Dictionary<string, AudioClip> audioDic;
    /// <summary>
    /// 是否暂停声音
    /// </summary>
    private bool isPause;
    public static AudioSystem Instance;

	// Use this for initialization
	void Awake () {
        Instance = this;
	    bgmSource = GetComponent<AudioSource>();
	    bgmSource.volume = GlobalData.BGMVolume;
        effectAudioList = new List<AudioSource>();
	    var effectAudio1 = transform.Find("EffectAudio1").GetComponent<AudioSource>();
        var effectAudio2 = transform.Find("EffectAudio2").GetComponent<AudioSource>();
	    effectAudioList.Add(effectAudio1);
        effectAudioList.Add(effectAudio2);
	    effectAudio1.volume = GlobalData.AudioVolume;
        effectAudio2.volume = GlobalData.AudioVolume;
        playingAudioList = new List<AudioSource>();
	    playingUrlList = new List<string>();
        audioDic = new Dictionary<string, AudioClip>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 播放背景声音
    /// </summary>
    /// <param name="bgmUrl"></param>
    public void PlayBgm(string bgmUrl)
    {
        if (bgmUrl == curBgmUrl)
        {
            return;
        }
        curBgmUrl = bgmUrl;
        AudioClip clip = GetAudioClipFormPool(bgmUrl);
        bgmSource.clip = clip;
        bgmSource.Play();
    }
    /// <summary>
    /// 上个音效长度
    /// </summary>
    [HideInInspector]
    public float perAudioLength;

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public IEnumerator PlayEffectAudio(string clipUrl,float delayTime=0,bool isLoop=false)
    {
        if (isPause)
        {
            yield break;
        }
        AudioClip clip = GetAudioClipFormPool(clipUrl);
        perAudioLength = clip.length;
        if (delayTime > 0)
        {
            yield return new WaitForSeconds(delayTime);
        }
        AudioSource playAudioSource;
        if (effectAudioList.Count > 0)
        {
            playAudioSource = effectAudioList[0];
            effectAudioList.RemoveAt(0);
        }
        else
        {
            var audioObj = new GameObject();
            audioObj.transform.SetParent(transform);
            playAudioSource = audioObj.AddComponent<AudioSource>();
            playAudioSource.loop = false;
        }
        playAudioSource.volume = GlobalData.AudioVolume;
        playingAudioList.Add(playAudioSource);
        playingUrlList.Add(clipUrl);
        playAudioSource.clip = clip;
        playAudioSource.loop = isLoop;
        playAudioSource.Play();
        if (!isLoop)
        {
            yield return new WaitForSeconds(clip.length * 2);
            playingAudioList.Remove(playAudioSource);
            playingUrlList.Remove(clipUrl);
            effectAudioList.Add(playAudioSource);
        }
    }

    /// <summary>
    /// 当前正在播放的聊天音效
    /// </summary>
    [HideInInspector]
    public AudioSource curChatAudioSource;
    /// <summary>
    /// 播放指定声音
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public IEnumerator PlayEffectAudio(AudioClip clip)
    {
        if (isPause)
        {
            yield break;
        }
        curChatAudioSource = null;
        if (effectAudioList.Count > 0)
        {
            curChatAudioSource = effectAudioList[0];
            effectAudioList.RemoveAt(0);
        }
        else
        {
            var audioObj = new GameObject();
            audioObj.transform.SetParent(transform);
            curChatAudioSource = audioObj.AddComponent<AudioSource>();
            curChatAudioSource.loop = false;
        }
        curChatAudioSource.volume = GlobalData.AudioVolume;
        playingAudioList.Add(curChatAudioSource);
        playingUrlList.Add("");
        curChatAudioSource.clip = clip;
        curChatAudioSource.Play();
        yield return new WaitForSeconds(clip.length);
        playingAudioList.Remove(curChatAudioSource);
        playingUrlList.Remove("");
        effectAudioList.Add(curChatAudioSource);
        curChatAudioSource = null;
    }

    /// <summary>
    /// 停止音效播放
    /// </summary>
    /// <param name="clipUrl"></param>
    /// <returns></returns>
    public void StopEffectAudio(string clipUrl)
    {
        var clipIndex = playingUrlList.IndexOf(clipUrl);
        var playAudioSource = playingAudioList[clipIndex];
        playAudioSource.Stop();
        playingUrlList.RemoveAt(clipIndex);
        effectAudioList.Add(playAudioSource);
    }

    /// <summary>
    /// 停止所有声音
    /// </summary>
    public void StopAllAudio()
    {
        foreach (AudioSource audioSource in playingAudioList)
        {
            audioSource.Stop();
            audioSource.clip = null;
            effectAudioList.Add(audioSource);
        }
        playingAudioList.Clear();
        playingUrlList.Clear();
        audioDic.Clear();
        StopAllCoroutines();
    }

    /// <summary>
    /// 设置背景音量大小
    /// </summary>
    public void SetBgmAudioVolume(float volume)
    {
        GlobalData.BGMVolume = volume;
        bgmSource.volume = volume;
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBgm()
    {
        bgmSource.Pause();
        isPause = true;
    }

    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void ResumeBgm()
    {
        bgmSource.Play();
        isPause = false;
    }

    /// <summary>
    /// 设置音效大小
    /// </summary>
    public void SetEffectAudioVolume(float volume)
    {
        foreach (AudioSource audioSource in playingAudioList)
        {
            audioSource.volume = volume;
        }
        GlobalData.AudioVolume = volume;
    }

    /// <summary>
    /// 从池中获取声音
    /// </summary>
    /// <returns></returns>
    private AudioClip GetAudioClipFormPool(string clipUrl)
    {
        if (audioDic.ContainsKey(clipUrl))
        {
            return audioDic[clipUrl];
        }
        audioDic.Add(clipUrl, Resources.Load<AudioClip>(clipUrl));
        return audioDic[clipUrl];
    }

    /// <summary>
    /// 设置语言类型
    /// </summary>
    public void SetLanguageType(bool volume)
    {

        GlobalData.languagebool = volume;
    }
}
