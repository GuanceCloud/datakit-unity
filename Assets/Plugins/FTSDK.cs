using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FTSDK.Unity.Bridge;
using UnityEngine;

namespace FTSDK.Unity
{
#pragma warning disable 4014
    public class FTSDK : MonoBehaviour
    {
        private static FTSDK instance;

        public GameObject viewObserver;
        public GameObject mainThreadDispatch;


        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                StartCoroutine(_LoadPrefabs());
            }
            else if (instance != this)
            {
                // 如果实例已经存在并且不是当前对象，则销毁当前对象
                Destroy(gameObject);
            }

            // 在此之后，当前对象就是唯一的实例
            DontDestroyOnLoad(gameObject);
        }


        /// <summary>
        ///  SDK 初始化，按需更改 SDK 初始化配置
        /// </summary>
        private void _InitSDK()
        {
            FTUnityBridge.Install(new SDKConfig
            {
                serverUrl = "http://10.0.0.1:9529",
                envType = "prod",
                debug = true,
                serviceName = "Your Services",
                globalContext = new Dictionary<string, string>{
                    {"custom_key","custom value"}
                }

            });

            FTUnityBridge.InitRUMConfig(new RUMConfig()
            {
                androidAppId = null,
                iOSAppId = "appid_iOSAppId",
                sampleRate = 0.8f,
                enableNativeUserResource = true,
                globalContext = new Dictionary<string, string>{
                    {"rum_custom","rum custom value"}
                }
            });

            FTUnityBridge.InitLogConfig(new LogConfig
            {
                sampleRate = 0.9f,
                enableCustomLog = true,
                enableLinkRumData = true,
                globalContext = new Dictionary<string, string>{
                    {"log_custom","log custom value"}
                }
            });

            FTUnityBridge.InitTraceConfig(new TraceConfig
            {
                sampleRate = 0.9f,
                traceType = "ddtrace",
                enableAutoTrace = true,
                enableLinkRumData = true

            });

        }
        IEnumerator _LoadPrefabs()
        {
            yield return Instantiate(mainThreadDispatch);
            _InitSDK();
            yield return Instantiate(viewObserver);

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
            //     FTUnityBridge.AddError(stackTrace, condition, "native_crash", "run");
            // }
            // else
            // {
            //     FTUnityBridge.AddLog(condition, stackTrace);
            // }

        }




    }

#pragma warning restore 4014

}

