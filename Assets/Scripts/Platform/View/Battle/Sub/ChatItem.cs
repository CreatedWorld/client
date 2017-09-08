using Platform.Model;
using Platform.Model.Battle;
using Platform.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 单个聊天选择框
/// </summary>
public class ChatItem : TableViewItem
{
    /// <summary>
    /// 聊天内容
    /// </summary>
    private Text chatTxt;
    /// <summary>
    /// 发送按钮
    /// </summary>
    private Button sendBtn; 
    void Awake ()
    {
        chatTxt = transform.Find("ItemBtn/ContentTxt").GetComponent<Text>();
        sendBtn = transform.Find("ItemBtn").GetComponent<Button>();
        sendBtn.onClick.AddListener(SendMsg);
    }
	
    public override void Updata(object data)
    {
        base.Updata(data);
        chatTxt.text = data as string;
    }
    /// <summary>
    /// 发送聊天内容
    /// </summary>
    private void SendMsg()
    {
        GameMgrProxy gameMgrProxy =
               ApplicationFacade.Instance.RetrieveProxy(Proxys.GAMEMGR_PROXY) as GameMgrProxy;
        BattleProxy battleProxy =
            ApplicationFacade.Instance.RetrieveProxy(Proxys.BATTLE_PROXY) as BattleProxy;
        if (gameMgrProxy.systemTime - battleProxy.perSendChatTime < GlobalData.SendChatInvoke)
        {
            PopMsg.Instance.ShowMsg("请不要频繁发送");
            return;
        }
        SendChatC2S sendChatC2S = new SendChatC2S();
        sendChatC2S.content = chatTxt.text;
        NetMgr.Instance.SendBuff(SocketType.BATTLE, MsgNoC2S.C2S_ROOM_TEXT_CHAT.GetHashCode(), 0, sendChatC2S);
        battleProxy.perSendChatTime = gameMgrProxy.systemTime;
        UIManager.Instance.HideUI(UIViewID.CHAT_VIEW);
    }
}
