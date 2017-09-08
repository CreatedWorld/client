using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine.SceneManagement;
/// <summary>
/// ����ģ�������
/// </summary>
public class GameMgrCommand : SimpleCommand,ICommand {
    /// <summary>
    /// ��������Ϣ
    /// </summary>
    /// <param PlayerName="notification">��Ϣ</param>
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
