using PureMVC.Patterns;
using PureMVC.Interfaces;
using Platform.Model.Battle;
/// <summary>
/// MVC管理类
/// </summary>
public class ApplicationFacade : Facade
{
    public new static IFacade Instance
    {
        get
        {
            if(m_instance == null)
            {
                lock(m_staticSyncRoot)
                {
                    if (m_instance == null)
                    {
                        m_instance = new ApplicationFacade();
                    }
                }
            }
            return m_instance;
        }
    }
    protected ApplicationFacade()
    {
    }
    static ApplicationFacade()
    {

    }
    protected override void InitializeFacade()
    {
        base.InitializeFacade();
    }

    /// <summary>
    /// Model模块注册
    /// </summary>
    protected override void InitializeModel()
    {
        base.InitializeModel();
        this.RegisterProxy(new GameMgrProxy(Proxys.GAMEMGR_PROXY));
        this.RegisterProxy(new LoginProxy(Proxys.LOGIN_PROXY));
        this.RegisterProxy(new HallProxy(Proxys.HALL_PROXY));
        this.RegisterProxy(new BattleProxy(Proxys.BATTLE_PROXY));
        this.RegisterProxy(new PlayerInfoProxy(Proxys.PLAYER_PROXY));
    }
    /// <summary>
    /// Controller模块注册
    /// </summary>
    protected override void InitializeController()
    {
        base.InitializeController();
        this.RegisterCommand(NotificationConstant.COMM_GAMEMGR_INIT, typeof(GameMgrCommand));
        this.RegisterCommand(NotificationConstant.COMM_CHECK_VERSION, typeof(VersionCheckCommand));
    }
    
}
