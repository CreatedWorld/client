using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine.SceneManagement;
/// <summary>
/// 管理模块控制器
/// </summary>
public class GameMgrCommand : SimpleCommand,ICommand {
    /// <summary>
    /// 控制器消息
    /// </summary>
    /// <param PlayerName="notification">消息</param>
    public override void Execute(INotification notification)
    {
        switch (notification.Name)
        {
            case (NotificationConstant.COMM_GAMEMGR_INIT):
                var loadInfo = new LoadSceneInfo(ESceneID.SCENE_LOGIN, LoadSceneType.SYNC, LoadSceneMode.Single);
                SendNotification(NotificationConstant.MEDI_GAMEMGR_LOADSCENE, loadInfo);
                break;
            default:
                break;
        }
    }
}
