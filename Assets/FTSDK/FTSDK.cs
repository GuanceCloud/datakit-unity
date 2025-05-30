using System.Collections;
using FTSDK.Unity.Bridge;
using UnityEngine;

namespace FTSDK.Unity
{
#pragma warning disable 4014
    public class FTSDK : MonoBehaviour
    {
        private static FTSDK _Instance;

        public GameObject ViewObserver;
        public GameObject MainThreadDispatch;


        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
                StartCoroutine(_LoadPrefabs());
                // 在此之后，当前对象就是唯一的实例
                DontDestroyOnLoad(gameObject);
            }
            else if (_Instance != this)
            {
                // 如果实例已经存在并且不是当前对象，则销毁当前对象
                Destroy(gameObject);
                // Instantiate(ViewObserver);
            }
        }


        /// <summary>
        ///  SDK 初始化，按需更改 SDK 初始化配置
        /// </summary>
        private void _InitSDK()
        {
            FTUnityBridge.Install(new SDKConfig
            {
                datakitUrl = "http://10.0.0.1:9529",
                env = "prod",
                debug = true,
                // serviceName = "Your Services",
                // globalContext = new Dictionary<string, string>{
                //     {"custom_key","custom value"}
                // }

            });

            //对应修改  androidAppId 和 iOSAppId
            FTUnityBridge.InitRUMConfig(new RUMConfig()
            {
                androidAppId = "appid_androidAppId",
                iOSAppId = "appid_iOSAppId",
                sampleRate = 0.8f,
                extraMonitorTypeWithError = ErrorMonitorType.All
                // enableNativeUserResource = true,
                // globalContext = new Dictionary<string, string>{
                //     {"rum_custom","rum custom value"}
                // }
            });

            FTUnityBridge.InitLogConfig(new LogConfig
            {
                sampleRate = 0.9f,
                enableCustomLog = true,
                enableLinkRumData = true,
                // logLevelFilters = new List<LogLevel> { LogLevel.Info }
                // globalContext = new Dictionary<string, string>{
                //     {"log_custom","log custom value"}
                // }
            });

            FTUnityBridge.InitTraceConfig(new TraceConfig
            {
                sampleRate = 0.9f,
                enableLinkRumData = true,
                traceType = TraceType.DDTrace

            });

        }
        IEnumerator _LoadPrefabs()
        {
            yield return Instantiate(MainThreadDispatch);
            // 如果是 Native 工程已集成 SDK，可以跳过这个一步初始化，避免重复设置
            _InitSDK();
            yield return Instantiate(ViewObserver);
        }


        void OnEnable()
        {
            Application.logMessageReceived += LogCallBack;

        }

        void OnDisable()
        {
            Application.logMessageReceived -= LogCallBack;
        }

        void LogCallBack(string condition, string stackTrace, LogType type)
        {
            ///开启崩溃监听和日志 Debug.Log 日志监听取消以下代码注释
            // if (type == LogType.Exception)
            // {
            //     FTUnityBridge.AddError(stackTrace, condition);
            // }
            // else
            // {

            //     FTUnityBridge.AddLog(condition, LogLevel.Info);
            // }

        }




    }

#pragma warning restore 4014

}

