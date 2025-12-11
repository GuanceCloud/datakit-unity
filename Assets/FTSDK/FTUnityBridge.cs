using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEditor;
using Unity.VisualScripting;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Collections;
using System.Linq;

namespace FTSDK.Unity.Bridge
{
    /// <summary>
    ///  SDK Configuration
    /// </summary>
    public class SDKConfig
    {
        /// <summary>
        /// datakit access URL address, example: http://10.0.0.1:9529, default port 9529. Note: The device installing the SDK must be able to access this address
        /// </summary>
        /// 
        [Obsolete("This params is deprecated. Replace with 'datakitUrl' instead")]
        public string serverUrl { get; set; }

        /// <summary>
        /// datakit access URL address, example: http://10.0.0.1:9529, default port 9529. Note: The device installing the SDK must be able to access this address
        /// </summary>
        public string datakitUrl { get; set; }

        /// <summary>
        /// dataway access URL address, example: http://10.0.0.1:9528, default port 9528, note: the device installing the SDK must be able to access this address. Note: choose one between datakit and dataway configuration
        /// </summary>
        public string datawayUrl { get; set; }

        /// <summary>
        /// Authentication token, needs to be configured together with datawayUrl
        /// </summary>
        public string clientToken { get; set; }
        /// <summary>
        /// Whether to enable Debug mode
        /// </summary>
        public bool debug { get; set; }
        /// <summary>
        /// Data upload environment, default prod. prod: production environment; gray: gray environment; pre: pre-release environment; common: daily environment; local: local environment, supports customization
        /// </summary>
        public string env { get; set; }
        public Dictionary<string, string> globalContext { get; set; }
        /// <summary>
        /// Application service name android df_rum_android, iOS df_rum_ios
        /// </summary>
        public string serviceName { get; set; }
        /// <summary>
        /// Whether to automatically sync data to server after collection, default true. When false, use FTUnityBridge.flushSyncData() to manage data synchronization manually
        /// </summary>
        public bool autoSync { get; set; }
        /// <summary>
        /// Set synchronization request entry count. Range [5,). Note: The larger the request entry count, the more computational resources data synchronization occupies, default is 10
        /// </summary>
        public int syncPageSize { get; set; }
        /// <summary>
        /// Set synchronization interval time. Range [0,5000], unit milliseconds, default not set
        /// </summary>
        public int syncSleepTime { get; set; }
        /// <summary>
        /// Recommended to enable when coexistence with web data is needed. This configuration is used to handle web data type storage compatibility issues, enabled by default
        /// </summary>
        public bool enableDataIntegerCompatible { get; set; }
        /// <summary>
        /// Enable deflate compression for uploaded synchronization data, disabled by default
        /// </summary>
        public bool compressIntakeRequests { get; set; }
        /// <summary>
        /// Enable using db to limit data size, default 100MB, unit Byte, larger database means greater disk pressure, disabled by default.
        /// Note: After enabling, Log configuration logCacheLimitCount and RUM configuration rumCacheLimitCount will be invalid
        /// </summary>
        public bool enableLimitWithDbSize { get; set; }
        /// <summary>
        /// DB cache limit size. Range [30MB,), default 100MB, unit byte
        /// </summary>
        public long dbCacheLimit { get; set; }
        /// <summary>
        /// Set data discard rules in database.
        /// Discard strategy: discard discard new data (default), discardOldest discard old data
        /// </summary>
        public DBCacheDiscard dbDiscardStrategy { get; set; }
        /// <summary>
        /// Modify individual fields
        /// </summary>
        public Dictionary<string, object> dataModifier { get; set; }
        /// <summary>
        ///  Modify individual data entries.
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> lineDataModifier { get; set; }

        /// <summary>
        /// Unity SDK version number
        /// </summary>
        public string sdkVersion
        {
            get
            {
                return FTUnityBridge.SDK_VERSION;
            }

        }

    }

    /// <summary>
    ///  RUM Configuration
    /// </summary>
    public class RUMConfig
    {
        /// <summary>
        /// Android RUM AppId
        /// </summary>
        public string androidAppId { get; set; }
        /// <summary>
        /// iOS RUM AppId
        /// </summary>
        public string iOSAppId { get; set; }

        /// <summary>
        /// Collection rate value range [0,1], default value 1
        /// </summary>
        public float sampleRate { get; set; }

        /// <summary>
        /// Error session sampling rate value range [0,1], default value 0. For Sessions not sampled, when ERROR sampling is hit, collect data from 1 minute before the error occurs
        /// </summary>
        public float sessionOnErrorSampleRate { get; set; }

        /// <summary>
        /// Add SDK global properties
        /// </summary>
        public Dictionary<string, string> globalContext { get; set; }
        /// <summary>
        /// Whether to enable Native Action collection, default false
        /// </summary>
        public bool enableNativeUserAction { get; set; }

        /// <summary>
        /// Whether to enable Native View collection, default false
        /// </summary>
        public bool enableNativeUserView { get; set; }
        /// <summary>
        /// Whether to enable Native Resource requests, Android supports Okhttp, iOS uses NSURLSession, default false
        /// </summary>
        public bool enableNativeUserResource { get; set; }
        /// <summary>
        /// Whether to collect IP address of request target domain. Scope: only affects default collection when enableNativeUserResource is true. iOS: supported on >= iOS 13. Android: single Okhttp has IP caching mechanism for same domain, same OkhttpClient will only generate once under the premise that the connected server IP doesn't change
        /// </summary>
        public bool enableResourceHostIP { get; set; }
        /// <summary>
        /// Whether to collect Native Java Crash, C/C++ Crash
        /// </summary>
        public bool enableTrackNativeCrash { get; set; }
        /// <summary>
        /// Whether to enable `Native ANR` monitoring, default `false`
        /// </summary>
        public bool enableTrackNativeAppANR { get; set; }
        /// <summary>
        /// Whether to perform `Native Freeze` automatic tracking, default `false`
        /// </summary>
        public bool enableTrackNativeFreeze { get; set; }
        /// <summary>
        /// Set threshold for collecting `Native Freeze` stuttering, value range [100,), unit milliseconds. iOS default 250ms, Android default 1000ms
        /// </summary>
        public int nativeFreezeDurationMs { get; set; }
        /// <summary>
        /// Error monitoring supplement types: all, battery, memory, cpu
        /// </summary>
        public ErrorMonitorType extraMonitorTypeWithError { get; set; }
        /// <summary>
        /// Page monitoring supplement types: all, battery (Android only), memory, cpu, fps
        /// </summary>
        public DeviceMetricsMonitorType deviceMonitorType { get; set; }
        /// <summary>
        /// normal (default), frequent, rare
        /// </summary>
        public DetectFrequency detectFrequency { get; set; }
        /// <summary>
        /// Local cache maximum RUM entry count limit [10_000,), default 100_000
        /// </summary>
        public int rumCacheLimitCount { get; set; }
        /// <summary>
        /// Discard strategy: `discard` discard new data (default), `discardOldest` discard old data
        /// </summary>
        public RUMCacheDiscard rumDiscardStrategy { get; set; }

    }

    /// <summary>
    ///  Trace link configuration
    /// </summary>
    public class TraceConfig
    {
        /// <summary>
        /// Collection rate value range [0,1], default value 1
        /// </summary>
        public float sampleRate { get; set; }
        /// <summary>
        /// Link type: ddTrace (default), zipkinMultiHeader, zipkinSingleHeader, traceparent, skywalking, jaeger
        /// </summary>
        public TraceType traceType { get; set; }
        /// <summary>
        /// Whether to associate with RUM data, default false
        /// </summary>
        public bool enableLinkRumData { get; set; } = false;

        /// <summary>
        /// Whether to enable automatic Trace header addition, Android supports Okhttp, iOS uses NSURLSession
        /// </summary>
        public bool enableNativeAutoTrace { get; set; } = false;

    }
    /// <summary>
    /// Log Configuration
    /// </summary>
    public class LogConfig
    {
        /// <summary>
        /// Collection rate value range >= 0, <= 1, default value 1
        /// </summary>

        public float sampleRate { get; set; }
        /// <summary>
        /// Whether to associate with RUM data
        /// </summary>
        public bool enableLinkRumData { get; set; }

        /// <summary>
        /// Whether to enable custom logs
        /// </summary>
        public bool enableCustomLog { get; set; }

        /// <summary>
        /// Log discard strategy: discard discard new data (default), discardOldest discard old data
        /// </summary>
        public LogCacheDiscard discardStrategy { get; set; }
        /// <summary>
        /// Log level filtering, array needs to fill log levels: info prompt, warning warning, error error, critical, ok recovery, no filtering by default
        /// </summary>
        public List<LogLevel> logLevelFilters { get; set; }
        /// <summary>
        /// Add Log global properties
        /// </summary>
        public Dictionary<string, string> globalContext { get; set; }
        /// <summary>
        /// Local cache maximum log entry count limit [1000,), larger logs mean greater disk cache pressure, default 5000
        /// </summary>
        public int logCacheLimitCount { get; set; }
    }

    /// <summary>
    /// User data
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// User id
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// User email
        /// </summary>
        public string userEmail { get; set; }
        /// <summary>
        /// User's additional information
        /// </summary>
        public Dictionary<string, string> extra { get; set; }
    }

    /// <summary>
    ///  Resource content metrics
    /// </summary>
    public class ResourceParams
    {
        /// <summary>
        /// Request url
        /// </summary>
        public string url { get; set; } = "";
        /// <summary>
        /// Request headers
        /// </summary>
        public Dictionary<string, string> requestHeader { get; set; }
        /// <summary>
        /// Response headers
        /// </summary>
        public Dictionary<string, string> responseHeader { get; set; }
        /// <summary>
        /// Response connection
        /// </summary>
        public string responseConnection { get; set; } = "";
        /// <summary>
        /// Response ContentType
        /// </summary>
        public string responseContentType { get; set; } = "";
        /// <summary>
        /// Response ContentEncoding
        /// </summary>
        public string responseContentEncoding { get; set; } = "";
        /// <summary>
        /// http method
        /// </summary>
        public string resourceMethod { get; set; } = "";
        /// <summary>
        /// Return data body
        /// </summary>
        public string responseBody { get; set; } = "";
        /// <summary>
        /// Request result status code
        /// </summary>
        public int resourceStatus { get; set; } = -1;

    }

    // /// <summary>
    // /// Resource network time metrics
    // /// </summary>
    // public class NetStatus
    // {

    //     /// <summary>
    //     /// Request task start time
    //     /// </summary>
    //     public long callStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// tcp connection time
    //     /// </summary>
    //     public long tcpStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// tcp end time
    //     /// </summary>
    //     public long tcpEndTime { get; set; } = -1L;
    //     /// <summary>
    //     /// dns start time
    //     /// </summary>
    //     public long dnsStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// dns end time
    //     /// </summary>
    //     public long dnsEndTime { get; set; } = -1L;
    //     /// <summary>
    //     /// Response start time
    //     /// </summary>
    //     public long bodyStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// Response end time
    //     /// </summary>
    //     public long bodyEndTime { get; set; } = -1L;
    //     /// <summary>
    //     /// ssl start time
    //     /// </summary>
    //     public long sslStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// ssl end time
    //     /// </summary>
    //     public long sslEndTime { get; set; } = -1L;

    // }

    /// <summary>
    /// Error additional data
    /// </summary>
    public enum ErrorMonitorType 
    {
        All,
        Battery,
        Memory,
        CPU
    }

    /// <summary>
    /// Page monitoring metrics
    /// </summary>
    public enum DeviceMetricsMonitorType 
    {
        All,
        /// <summary>
        /// Only supports Android
        /// </summary>
        Battery,
        Memory,
        CPU,
        FPS
    }

    /// <summary>
    /// Scan frequency
    /// </summary>
    public enum DetectFrequency { Normal, Frequent, Rare }

    /// <summary>
    /// Link type
    /// </summary>
    public enum TraceType
    {
        DDTrace,
        ZipkinMultiHeader,
        ZipkinSingleHeader,
        Traceparent,
        Skywalking,
        Jaeger
    }

    /// <summary>
    /// Log cache discard strategy
    /// </summary>
    public enum LogCacheDiscard { Discard, DiscardOldest }
    /// <summary>
    /// Data cache discard strategy
    /// </summary>
    public enum DBCacheDiscard { Discard, DiscardOldest }
    /// <summary>
    /// RUM cache discard strategy
    /// </summary>
    public enum RUMCacheDiscard { Discard, DiscardOldest }


    /// <summary>
    /// Log level
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Critical,
        Ok,
    }

    /// <summary>
    /// Enum to String conversion
    /// </summary>
    public class BridgeEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DetectFrequency || value is TraceType || value is LogCacheDiscard || value is LogLevel || value is ErrorMonitorType || value is DeviceMetricsMonitorType)
            {
                writer.WriteValue(value.ToString().ToLower());
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }
        }
    }

    /// <summary>
    /// Remove null values from dictionary
    /// </summary>
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> WithoutNullValues<TKey, TValue>(this IDictionary<TKey, TValue> source)
            where TValue : class
        {
            return source
                .Where(kv => kv.Value != null)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }


    /// <summary>
    /// Unity bridge interface
    /// </summary>
    public class FTUnityBridge
    {
        public const string SDK_VERSION = "1.1.0-alpha.2";
        
        /// <summary>
        /// Thread-safe static dictionary for storing bridge context that will be automatically merged with property parameters
        /// </summary>
        private static readonly Dictionary<string, object> _bridgeContext = new Dictionary<string, object>();
        
        /// <summary>
        /// Lock object for thread-safe access to the bridge context dictionary
        /// </summary>
        private static readonly object _contextLock = new object();
        
        /// <summary>
        /// Static constructor to initialize bridge context with SDK version information
        /// </summary>
        static FTUnityBridge()
        {
            _bridgeContext["sdk_bridge_info"] = $"{{\"unity\":\"{SDK_VERSION}\"}}";
        }
        private const string KEY_METHOD_INSTALL = "Install";
        private const string KEY_METHOD_INIT_RUM_CONFIG = "InitRUMConfig";
        private const string KEY_METHOD_INIT_LOG_CONFIG = "InitLogConfig";
        private const string KEY_METHOD_INIT_TRACE_CONFIG = "InitTraceConfig";
        private const string KEY_METHOD_BIND_USER_DATA = "BindUserData";
        private const string KEY_METHOD_UN_BIND_USER_DATA = "UnBindUserdata";
        private const string KEY_METHOD_APPEND_GLOBAL_CONTEXT = "AppendGlobalContext";
        private const string KEY_METHOD_APPEND_LOG_GLOBAL_CONTEXT = "AppendLogGlobalContext";
        private const string KEY_METHOD_APPEND_RUM_GLOBAL_CONTEXT = "AppendRUMGlobalContext";
        private const string KEY_METHOD_ADD_ACTION = "AddAction";
        private const string KEY_METHOD_START_ACTION = "StartAction";
        private const string KEY_METHOD_CREATE_VIEW = "CreateView";
        private const string KEY_METHOD_START_VIEW = "StartView";
        private const string KEY_METHOD_STOP_VIEW = "StopView";
        private const string KEY_METHOD_ADD_ERROR = "AddError";
        private const string KEY_METHOD_ADD_LONG_TASK = "AddLongTask";
        private const string KEY_METHOD_START_RESOURCE = "StartResource";
        private const string KEY_METHOD_STOP_RESOURCE = "StopResource";
        private const string KEY_METHOD_ADD_RESOURCE = "AddResource";
        private const string KEY_METHOD_ADD_LOG = "AddLog";
        private const string KEY_METHOD_GET_TRACE_HEADER = "GetTraceHeader";
        private const string KEY_METHOD_FLUSH_SYNC_DATA = "FlushSyncData";
        private const string KEY_METHOD_CLEAR_ALL_DATA = "ClearAllData";
        private const string KEY_METHOD_DE_INIT = "DeInit";
        private const string DEFAULT_ERROR_TYPE = "unity_crash";

        private static JsonSerializerSettings JSON_HANDLER = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Converters = { new BridgeEnumConverter() }
        };





#if (UNITY_IOS && !UNITY_EDITOR)

        /// <summary>
        ///  iOS bridge call method
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="json">json format parameters</param>
        /// <returns></returns>
        [DllImport("__Internal")]
        private static extern IntPtr invokeMethod(string method, string json);

#endif

#if (UNITY_ANDROID && !UNITY_EDITOR)
         
        /// <summary>
        /// Android bridge method call class
        /// </summary>
        /// 
        private const string ANDROID_PLUGIN_CLASS_NAME = "com.ft.sdk.unity.bridge.FTUnityBridge";

        private static AndroidJavaObject androidPlugin;

#endif

        /// <summary>
        /// Append context data to the bridge context dictionary
        /// This context will be automatically merged with all property parameters in subsequent method calls
        /// </summary>
        /// <param name="context">Dictionary containing context data to be merged</param>
        public static void AppendBridgeContext(Dictionary<string, object> context)
        {
            if (context == null || context.Count == 0)
                return;
                
            lock (_contextLock)
            {
                foreach (var kvp in context)
                {
                    if (kvp.Value != null)
                    {
                        _bridgeContext[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        
        
        /// <summary>
        /// Get a copy of the current bridge context
        /// </summary>
        /// <returns>Copy of the current bridge context dictionary</returns>
        public static Dictionary<string, object> GetBridgeContext()
        {
            lock (_contextLock)
            {
                return new Dictionary<string, object>(_bridgeContext);
            }
        }
        
        /// <summary>
        /// Merge bridge context with property parameters
        /// </summary>
        /// <param name="property">Original property parameters</param>
        /// <returns>Merged property parameters including bridge context</returns>
        private static Dictionary<string, object> MergeWithBridgeContext(Dictionary<string, object> property)
        {
            if (_bridgeContext.Count == 0)
                return property;
                
            var mergedProperty = property != null ? new Dictionary<string, object>(property) : new Dictionary<string, object>();
            
            lock (_contextLock)
            {
                foreach (var kvp in _bridgeContext)
                {
                    if (!mergedProperty.ContainsKey(kvp.Key))
                    {
                        mergedProperty[kvp.Key] = kvp.Value;
                    }
                }
            }
            
            return mergedProperty;
        }

        /// <summary>
        /// Initialize SDK local configuration data
        /// </summary>
        /// <param name="config"></param>
        public static void Install(SDKConfig config)
        {
            if (config.globalContext == null)
            {
                config.globalContext = new Dictionary<string, string>();
            }
            _InovkeMethod(KEY_METHOD_INSTALL, JsonConvert.SerializeObject(config, JSON_HANDLER));
        }

        /// <summary>
        /// Set RUM configuration
        /// </summary>
        /// <param name="config"></param>
        public static void InitRUMConfig(RUMConfig config)
        {
            _InovkeMethod(KEY_METHOD_INIT_RUM_CONFIG, JsonConvert.SerializeObject(config, JSON_HANDLER));
        }

        /// <summary>
        /// Set log configuration
        /// </summary>
        /// <param name="config"></param>
        public static void InitLogConfig(LogConfig config)
        {
            _InovkeMethod(KEY_METHOD_INIT_LOG_CONFIG, JsonConvert.SerializeObject(config, JSON_HANDLER));
        }

        /// <summary>
        ///  Set Trace configuration
        /// </summary>
        /// <param name="config"></param>
        public static void InitTraceConfig(TraceConfig config)
        {
            _InovkeMethod(KEY_METHOD_INIT_TRACE_CONFIG, JsonConvert.SerializeObject(config, JSON_HANDLER));
        }

        /// <summary>
        /// Bind RUM user information
        /// </summary>
        /// <param name="userId">User unique id</param>
        public static void BindUserData(string userId)
        {
            BindUserData(new UserData()
            {
                userId = userId
            });
        }

        /// <summary>
        /// Bind RUM user information
        /// </summary>
        /// <param name="userData"></param>
        public static void BindUserData(UserData userData)
        {
            _InovkeMethod(KEY_METHOD_BIND_USER_DATA, JsonConvert.SerializeObject(userData, JSON_HANDLER));
        }

        /// <summary>
        /// Unbind user data
        /// </summary>
        public static async Task UnBindUserdata()
        {
            await _InovkeMethodAsync(KEY_METHOD_UN_BIND_USER_DATA, "");
        }

        /// <summary>
        /// Add custom global parameters. Applies to RUM, Log data
        /// </summary>
        public static void AppendGlobalContext(Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_APPEND_GLOBAL_CONTEXT, JsonConvert.SerializeObject(property, JSON_HANDLER));
        }

        /// <summary>
        /// Add custom RUM global parameters. Applies to RUM data
        /// </summary>
        public static void AppendRUMGlobalContext(Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_APPEND_RUM_GLOBAL_CONTEXT, JsonConvert.SerializeObject(property, JSON_HANDLER));
        }

        /// <summary>
        /// Add custom RUM, Log global parameters. Applies to Log data
        /// </summary>
        public static void AppendLogGlobalContext(Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_APPEND_LOG_GLOBAL_CONTEXT, JsonConvert.SerializeObject(property, JSON_HANDLER));
        }

        /// <summary>
        ///   Add Action 
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="actionType">action type</param>
        /// <param name="duartion">nanoseconds, duration</param>
        public static void AddAction(string actionName, string actionType)
        {
            AddAction(actionName, actionType, 0, null);
        }

        /// <summary>
        ///   Add Action 
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="actionType">action type</param>
        /// <param name="duartion">nanoseconds, duration</param>
        public static void AddAction(string actionName, string actionType, long duartion)
        {
            AddAction(actionName, actionType, duartion, null);
        }

        /// <summary>
        ///   Add Action 
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="actionType">action type</param>
        /// <param name="duartion">nanoseconds, duration</param>
        public static void AddAction(string actionName, string actionType, Dictionary<string, object> property)
        {
            AddAction(actionName, actionType, 0, property);
        }

        /// <summary>
        ///   Add Action 
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="actionType">action type</param>
        /// <param name="duartion">nanoseconds, duration</param>
        /// <param name="property">Additional property parameters</param>
        public static void AddAction(string actionName, string actionType, long duration, Dictionary<string, object> property)
        {

            var mergedProperty = MergeWithBridgeContext(property);
            _InovkeMethod(KEY_METHOD_ADD_ACTION, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"actionName" , actionName},
                {"actionType",actionType},
                {"duration" , duration},
                {"property" , mergedProperty}
            }, JSON_HANDLER));
        }

        /// <summary>
        ///   Add Action 
        /// </summary>
        /// <param name="actionName"> action name</param>
        /// <param name="actionType"> action type</param>
        public static void StartAction(string actionName, string actionType)
        {
            StartAction(actionName, actionType, null);
        }

        /// <summary>
        /// Add Action
        /// </summary>
        /// <param name="actionName">action name</param>
        /// <param name="actionType">action type</param>
        /// <param name="property">Additional property parameters</param>
        public static void StartAction(string actionName, string actionType, Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            _InovkeMethod(KEY_METHOD_START_ACTION, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"actionName" , actionName},
                {"actionType",actionType},
                {"property" , mergedProperty}
            }.WithoutNullValues(), JSON_HANDLER));

        }

        /// <summary>
        /// Create View
        /// </summary>
        /// <param name="viewName"> Current page name</param>
        /// <param name="loadTime">Load time, nanoseconds</param>
        public static void CreateView(string viewName, long loadTime)
        {
            _InovkeMethod(KEY_METHOD_CREATE_VIEW, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"viewName" , viewName },
                {"loadTime" , loadTime },
            }, JSON_HANDLER));
        }

        /// <summary>
        ///  View start
        /// </summary>
        /// <param name="viewName">Current page name</param>
        public static void StartView(string viewName)
        {
            StartView(viewName, null);
        }

        /// <summary>
        /// View start
        /// </summary>
        /// <param name="viewName">Current page name</param>
        /// <param name="property">Additional property parameters</param>
        public static void StartView(string viewName, Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            _InovkeMethod(KEY_METHOD_START_VIEW, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"viewName" , viewName },
                {"property" , mergedProperty },
            }.WithoutNullValues(), JSON_HANDLER));
        }

        /// <summary>
        /// View end
        /// </summary>
        public static void StopView()
        {
            StopView(null);
        }

        /// <summary>
        /// View end
        /// </summary>
        /// <param name="property">Additional property parameters</param>
        public static void StopView(Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            _InovkeMethod(KEY_METHOD_STOP_VIEW, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
               {"property" , mergedProperty },
            }.WithoutNullValues(), JSON_HANDLER));
        }

        /// <summary>
        /// Add error information
        /// </summary>
        /// <param name="log">Log</param>
        /// <param name="message">Message</param>
        /// <returns></returns>
        public static async Task AddError(string log, string message, Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            await AddError(log, message, DEFAULT_ERROR_TYPE, mergedProperty);
        }


        /// <summary>
        /// Add error information
        /// </summary>
        /// <param name="log">Log</param>
        /// <param name="message">Message</param>
        /// <returns></returns>
        public static async Task AddError(string log, string message)
        {
            await AddError(log, message, DEFAULT_ERROR_TYPE, null);
        }


        /// <summary>
        /// Add error information
        /// </summary>
        /// <param name="log">Log</param>
        /// <param name="errorType">Error type</param>
        /// <param name="message">Message</param>
        /// <returns></returns>
        public static async Task AddError(string log, string message, string errorType)
        {
            await AddError(log, message, errorType, null);
        }

        /// <summary>
        /// Add error information
        /// </summary>
        /// <param name="log">Log</param>
        /// <param name="message">Message</param>
        /// <param name="errorType">Error type</param>
        /// <param name="property">Additional property parameters</param>
        /// <returns></returns>
        public static async Task AddError(string log, string message, string errorType,
            Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            string state = "run";
            await _InovkeMethodAsync(KEY_METHOD_ADD_ERROR, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"log", log },
                {"message" , message },
                {"errorType" , errorType },
                {"state" , state },
                {"property" , mergedProperty }
            }, JSON_HANDLER));
        }

        /// <summary>
        /// Add long duration task
        /// </summary>
        /// <param name="log">Log content</param>
        /// <param name="duration">Duration, nanoseconds</param>
        /// <returns></returns>
        public static async Task AddLongTask(string log, long duration)
        {
            await AddLongTask(log, duration, null);

        }

        /// <summary>
        /// Add long duration task
        /// </summary>
        /// <param name="log">Log content</param>
        /// <param name="duration">Duration, nanoseconds</param>
        /// <param name="property">Additional property parameters</param>
        /// <returns></returns>
        public static async Task AddLongTask(string log, long duration, Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            await _InovkeMethodAsync(KEY_METHOD_ADD_LONG_TASK, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"log" , log },
                {"duration" , duration},
                {"property" , mergedProperty}
            }.WithoutNullValues(), JSON_HANDLER));

        }

        /// <summary>
        ///  resource start
        /// </summary>
        /// <param name="resourceId">Resource Id</param>
        /// <returns></returns>
        public static async Task StartResource(string resourceId)
        {
            await StartResource(resourceId, null);
        }

        /// <summary>
        /// resource start
        /// </summary>
        /// <param name="resourceId">Resource Id</param>
        /// <param name="property">Additional property parameters</param>
        /// <returns></returns>
        public static async Task StartResource(string resourceId, Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            await _InovkeMethodAsync(KEY_METHOD_START_RESOURCE, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"resourceId" , resourceId},
                {"property" , mergedProperty},
            }.WithoutNullValues(), JSON_HANDLER));
        }

        /// <summary>
        /// resource end
        /// </summary>
        /// <param name="resourceId">Resource Id</param>
        /// <returns></returns>
        public static async Task StopResource(string resourceId)
        {
            await StopResource(resourceId, null);
        }

        /// <summary>
        /// resource end
        /// </summary>
        /// <param name="resourceId">Resource Id</param>
        /// <param name="property">Additional property parameters</param>
        public static async Task StopResource(string resourceId, Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            await _InovkeMethodAsync(KEY_METHOD_STOP_RESOURCE, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"resourceId" , resourceId},
                {"property" , mergedProperty},
            }.WithoutNullValues(), JSON_HANDLER));
        }

        /// <summary>
        /// Add network transmission content and metrics
        /// </summary>
        /// <param name="resourceId">Resource Id</param>
        /// <param name="resourceParams">Data transmission content</param>
        /// <param name="netStatus">Network metrics data</param>
        public static async Task AddResource(string resourceId, ResourceParams resourceParams)
        {
            await _InovkeMethodAsync(KEY_METHOD_ADD_RESOURCE, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"resourceId" , resourceId},
                {"resourceParams" , resourceParams},
                // {"netStatus" , netStatus},
            }.WithoutNullValues(), JSON_HANDLER));

        }
        /// <summary>
        /// Add log
        /// </summary>
        /// <param name="log">Log content</param>
        /// <param name="level">Log level info, warning, error, critical, ok</param>
        /// <returns></returns>
        public static async Task AddLog(string log, LogLevel level)
        {
            await AddLog(log, level, null);
        }

        /// <summary>
        /// Add log
        /// </summary>
        /// <param name="log">Log content</param>
        /// <param name="level">Log level info, warning, error, critical, ok</param>
        /// <param name="property">Additional property parameters</param>
        /// <returns></returns>
        public static async Task AddLog(string log, LogLevel level, Dictionary<string, object> property)
        {
            var mergedProperty = MergeWithBridgeContext(property);
            await _InovkeMethodAsync(KEY_METHOD_ADD_LOG, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"log" , log},
                {"level" , level},
                {"property" , mergedProperty},
            }.WithoutNullValues(), JSON_HANDLER));
        }
        /// <summary>
        /// Get link
        /// </summary>
        /// <param name="resourceId">Resource Id</param>
        /// <param name="url">url address</param>
        /// <returns>json string</returns>

        public static async Task<string> GetTraceHeader(string resourceId, string url)
        {
            return await _InovkeMethodAsync(KEY_METHOD_GET_TRACE_HEADER, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"resourceId" , resourceId},
                {"url" , url},
            }, JSON_HANDLER));
        }

        /// <summary>
        /// Get link Id
        /// </summary>
        /// <param name="url">url address</param>
        /// <returns>json string</returns>
        public static async Task<string> GetTraceHeaderWithUrl(string url)
        {
            return await GetTraceHeader(null, url);
        }


        /// <summary>
        /// Actively sync data
        /// </summary>
        public static async void flushSyncData()
        {
            await _InovkeMethodAsync(KEY_METHOD_FLUSH_SYNC_DATA, "");
        }

        /// <summary>
        /// Clear unreported cached data
        /// </summary>
        public static async void cleanAllData()
        {
            await _InovkeMethodAsync(KEY_METHOD_CLEAR_ALL_DATA, "");
        }


        /// <summary>
        /// SDK release
        /// </summary>
        public static void DeInit()
        {
            _InovkeMethod(KEY_METHOD_DE_INIT, "");
        }

        /// <summary>
        /// Asynchronous cross-platform parameter passing
        /// </summary>
        /// <param name="method"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        private static async Task<string> _InovkeMethodAsync(string method, string json)
        {
            string result = "";
            var taskCompletionSource = new TaskCompletionSource<string>();

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                           {
                               result = _InovkeMethod(method, json);
                               taskCompletionSource.SetResult(result);
                           });

            return await taskCompletionSource.Task; ;
        }

        /// <summary>
        /// Responsible for cross-platform parameter passing for various methods
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="json">json format parameters</param>
        /// <returns></returns>
        private static string _InovkeMethod(string method, string json)
        {

            UnityEngine.Debug.Log(json);

#if (UNITY_IOS && !UNITY_EDITOR)
        IntPtr ptr = invokeMethod(method,json);
        return Marshal.PtrToStringAnsi(ptr);
#endif

            // Call Android plugin methods on Android
#if (UNITY_ANDROID && !UNITY_EDITOR)

            if (androidPlugin == null)
            {
                androidPlugin = new AndroidJavaObject(ANDROID_PLUGIN_CLASS_NAME);
            }

            return androidPlugin.CallStatic<string>("invokeMethod", method, json);
#endif
            return null;

        }
    }
}