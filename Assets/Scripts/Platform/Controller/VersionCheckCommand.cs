using LitJson;
using Platform.Utils;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class VersionCheckCommand: SimpleCommand, ICommand
{
    public override void Execute(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.COMM_CHECK_VERSION:
                CheckVersion();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 检查版本更新
    /// </summary>
    /// <param name="version"></param>
    public void CheckVersion()
    {
        GameMgr.Instance.StartCoroutine(POST());
    }

    //POST请求
    IEnumerator POST()
    {
        string curVersion = "";
        int customType = 2;
        if (Application.platform == RuntimePlatform.Android)
        {
            curVersion = AndroidSdkInterface.GetVersion();
            customType = 2;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            curVersion = IOSSdkInterface.GetVersion();
            customType = 1;
        }
        else
        {
            curVersion = GlobalData.VERSIONS;
        }
        string url = string.Format("{0}?version={1}&type={2}", GlobalData.CheckVersionUrl,curVersion, customType);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.Send();

        if (www.isError)
        {
            PopMsg.Instance.ShowMsg("获取版本失败:" + www.error);
        }
        else
        {
            JSONNode jsonData = JSON.Parse(www.downloadHandler.text);
            if (int.Parse(jsonData["success"].ToString()) == 0)
            {
                PopMsg.Instance.ShowMsg("获取版本失败:" + jsonData["message"].ToString());
            }
            else
            {
                if (int.Parse(jsonData["is_update"].ToString()) == 1)
                {
                    DialogMsgVO dialogMsgVO = new DialogMsgVO();
                    dialogMsgVO.dialogType = DialogType.CONFIRM;
                    dialogMsgVO.content = string.Format("有新版本：{0}\n您是否要更新？", jsonData["version"].ToString());
                    dialogMsgVO.confirmCallBack = () =>
                    {
                        if (GlobalData.sdkPlatform == SDKPlatform.ANDROID)
                        {
                            AndroidSdkInterface.DownloadFile(jsonData["filepath"].ToString());
                        }
                        else if (GlobalData.sdkPlatform == SDKPlatform.IOS)
                        {
                            IOSSdkInterface.UpdateApp(GlobalData.ShareUrl);
                        }
                    };
                    DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
                    dialogView.data = dialogMsgVO;
                }
            }           
        }
        
    }

}

