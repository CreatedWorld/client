using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Consolation
{
    /// <summary>  
    /// A console to display Unity's debug logs in-game.  
    /// </summary>  
    class LogConsole : MonoBehaviour
    {
        //fps相关
        public float f_UpdateInterval = 0.5F;
        private float f_LastInterval;
        private int i_Frames = 0;
        private float f_Fps;
        private GUIStyle style;
       
        /// <summary>
        /// 日志列表
        /// </summary>
        private TableView logList;
        /// <summary>
        /// 复制按钮
        /// </summary>
        private Button copyBtn;
        /// <summary>
        /// 日志清除
        /// </summary>
        private Button clearBtn;
        /// <summary>
        /// 调试按钮
        /// </summary>
        private Button debugBtn;
        /// <summary>
        /// 牌面布局按钮
        /// </summary>
        private Button battleDebugBtn;
        /// <summary>
        /// 牌面设置界面
        /// </summary>
        private GameObject debugView;
        /// <summary>
        /// 输入文本框
        /// </summary>
        private Text debugInputValueTxt;
        /// <summary>
        /// 压力测试
        /// </summary>
        private Button pressDebugBtn;
        /// <summary>
        /// 隐藏按钮
        /// </summary>
        private Button hidenBtn;

        private void Awake()
        {
            style = new GUIStyle();
            style.fontSize = 30;
            style.normal.textColor = Color.yellow;

            logList = transform.Find("LogList").GetComponent<TableView>();
            copyBtn = transform.Find("CopyBtn").GetComponent<Button>();
            clearBtn = transform.Find("ClearBtn").GetComponent<Button>();
            debugBtn = transform.Find("DebugBtn").GetComponent<Button>();
            battleDebugBtn = transform.Find("BattleDebugBtn").GetComponent<Button>();
            debugView = transform.Find("DebugView").gameObject;
            debugInputValueTxt = transform.Find("DebugInput/DebugInputValueTxt").GetComponent<Text>();
            hidenBtn = transform.Find("HidenBtn").GetComponent<Button>();
            pressDebugBtn = transform.Find("PressDebugBtn").GetComponent<Button>();

            copyBtn.onClick.AddListener(CopyLogs);
            clearBtn.onClick.AddListener(ClearLog);
            debugBtn.onClick.AddListener(DebugHandler);
            battleDebugBtn.onClick.AddListener(OpenBattleDebug);
            hidenBtn.onClick.AddListener(HidenView);
            pressDebugBtn.onClick.AddListener(StartPressDebug);
        }

        void Start()
        {
            f_LastInterval = Time.realtimeSinceStartup;
            i_Frames = 0;
        }

        /// <summary>
        /// 点击的起始坐标
        /// </summary>
        Vector2 moveStart = Vector2.zero;
        /// <summary>
        /// 解锁步骤
        /// </summary>
        uint moveIndex = 0;
        void Update()
        {
            ++i_Frames;
            if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
            {
                UpdateUsed();
                f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);
                i_Frames = 0;
                f_LastInterval = Time.realtimeSinceStartup;
            }
            if (Input.GetMouseButtonDown(0))
            {
                moveStart = Input.mousePosition;
                moveIndex = 0;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    var curPos = Input.mousePosition;
                    if (moveIndex == 0 && curPos.y - moveStart.y > 100)
                    {
                        moveIndex++;
                        moveStart = curPos;
                    }
                    if (moveIndex == 1 && curPos.x - moveStart.x > 100)
                    {
                        moveIndex++;
                        moveStart = curPos;
                    }
                    if (moveIndex == 2 && moveStart.y - curPos.y > 100)
                    {
                        moveIndex++;
                        debugBtn.gameObject.SetActive(!debugBtn.gameObject.activeSelf);
                        battleDebugBtn.gameObject.SetActive(!battleDebugBtn.gameObject.activeSelf);
                    }
                }
            }
        }

        //Memory
        private string sUserMemory;
        private string s;
        private uint MonoUsedM;
        private uint AllMemory;
        void UpdateUsed()
        {
            sUserMemory = "";
            MonoUsedM = Profiler.GetMonoUsedSize() / 1000000;
            AllMemory = Profiler.GetTotalAllocatedMemory() / 1000000;


            sUserMemory += "MonoUsed:" + MonoUsedM + "M" + "\n";
            sUserMemory += "AllMemory:" + AllMemory + "M" + "\n";
            sUserMemory += "UnUsedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000000 + "M" + "\n";


            s = "";
            s += " MonoHeap:" + Profiler.GetMonoHeapSize() / 1000 + "k";
            s += " MonoUsed:" + Profiler.GetMonoUsedSize() / 1000 + "k";
            s += " Allocated:" + Profiler.GetTotalAllocatedMemory() / 1000 + "k";
            s += " Reserved:" + Profiler.GetTotalReservedMemory() / 1000 + "k";
            s += " UnusedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000 + "k";
            s += " UsedHeap:" + Profiler.usedHeapSize / 1000 + "k";
        }

        void OnGUI()  
        {
            GUI.Label(new Rect(0, 0, 200, 200), " FPS:" + f_Fps.ToString("f2"), style);
            GUI.Label(new Rect(0, 50, 200, 200), sUserMemory, style);
        }

        /// <summary>
        /// 清除日志
        /// </summary>
        private void ClearLog()
        {
            GlobalData.logs.Clear();
            logList.DataProvider = GlobalData.logs;
        }

        /// <summary>
        /// 打开牌面设置界面
        /// </summary>
        private void OpenBattleDebug()
        {
            debugView.SetActive(true);
        }

        /// <summary>
        /// 动作特效
        /// </summary>
        private GameObject actEffect;
        /// <summary>
        /// debug调试
        /// </summary>
        private void DebugHandler()
        {
            GlobalData.isDebugModel = !GlobalData.isDebugModel;
            debugInputValueTxt.text = "";
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 复制日志
        /// </summary>
        void CopyLogs()
        {
            StringBuilder result = new StringBuilder();
            foreach (LogVO log in GlobalData.logs)
            {
                if (result.Length > 0)
                {
                    result.Append("\n");
                }
                result.Append(log.message + log.stackTrace);
            }
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidSdkInterface.CopyToClip(result.ToString());
            }
            if (!Application.isMobilePlatform)
            {
                TextEditor textEditor = new TextEditor();
                textEditor.text = result.ToString();
                textEditor.OnFocus();
                textEditor.Copy();
            }
        }

        /// <summary>
        /// 开始压力测试
        /// </summary>
        void StartPressDebug()
        {
            int clientNum = 0;
            int.TryParse(debugInputValueTxt.text,out clientNum);
            if (clientNum == 0)
            {
                PopMsg.Instance.ShowMsg("请再输入框内输入要模拟的客户端数量");
                return;
            }
            if (clientNum%4 != 0)
            {
                PopMsg.Instance.ShowMsg("客户端数量需要为4的倍数");
                return;
            }
            GameMgr.Instance.RemoveAllMsgHandler();
            PressDebug.Instance.StartTest(clientNum);
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        void HidenView()
        {
            foreach (Transform child in transform)
            {
                if (child == hidenBtn.transform || child == debugView.transform)
                {
                    continue;
                }
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
            if (logList.gameObject.activeSelf)
            {
                logList.DataProvider = GlobalData.logs;
            }
        }

    }
}