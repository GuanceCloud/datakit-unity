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
                // After this, the current object is the only instance
                DontDestroyOnLoad(gameObject);
            }
            else if (_Instance != this)
            {
                // If an instance already exists and is not the current object, destroy the current object
                Destroy(gameObject);
                // Instantiate(ViewObserver);
            }
        }


        /// <summary>
        ///  SDK initialization, modify SDK initialization configuration as needed
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

            // Modify androidAppId and iOSAppId accordingly
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
            // If the native project has already integrated the SDK, you can skip this initialization step to avoid duplicate settings
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
            /// Enable crash monitoring and log Debug.Log monitoring by uncommenting the following code
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

