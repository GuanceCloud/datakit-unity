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
    ///  SDK 配置
    /// </summary>
    public class SDKConfig
    {
        /// <summary>
        /// datakit 访问地址 URL 地址，例子：http://10.0.0.1:9529，端口默认 9529。注意：安装 SDK 设备需能访问这地址
        /// </summary>
        /// 
        [Obsolete("This params is deprecated. Replace with 'datakitUrl' instead")]
        public string serverUrl { get; set; }

        /// <summary>
        /// datakit 访问地址 URL 地址，例子：http://10.0.0.1:9529，端口默认 9529。注意：安装 SDK 设备需能访问这地址
        /// </summary>
        public string datakitUrl { get; set; }

        /// <summary>
        /// dataway 访问 URL 地址，例子：http://10.0.0.1:9528，端口默认 9528，注意：安装 SDK 设备需能访问这地址.注意：datakit 和 dataway 配置两者二选一
        /// </summary>
        public string datawayUrl { get; set; }

        /// <summary>
        /// 认证 token，需要与 datawayUrl 同时配置
        /// </summary>
        public string clientToken { get; set; }
        /// <summary>
        /// 是否开启 Debug 模式
        /// </summary>
        public bool debug { get; set; }
        /// <summary>
        /// 数据上传环境，默认 prod。prod：线上环境；gray：灰度环境；pre：预发布环境；common：日常环境；local：本地环境，支持自定义
        /// </summary>
        public string env { get; set; }
        public Dictionary<string, string> globalContext { get; set; }
        /// <summary>
        /// 应用服务名 android df_rum_android, iOS df_rum_ios
        /// </summary>
        public string serviceName { get; set; }
        /// <summary>
        /// 是否在采集数据后自动同步到服务器，默认为 true。当为 false 时使用 FTUnityBridge.flushSyncData() 自行管理数据同步
        /// </summary>
        public bool autoSync { get; set; }
        /// <summary>
        /// 设置同步请求条目数。范围 [5,）注意：请求条目数越大，代表数据同步占用更大的计算资源，默认为 10
        /// </summary>
        public int syncPageSize { get; set; }
        /// <summary>
        /// 设置同步间歇时间。范围 [0,5000]，单位毫秒，默认不设置
        /// </summary>
        public int syncSleepTime { get; set; }
        /// <summary>
        /// 需要与 web 数据共存情况下，建议开启。此配置用于处理 web 数据类型存储兼容问题，默认开启
        /// </summary>
        public bool enableDataIntegerCompatible { get; set; }
        /// <summary>
        /// 对上传同步数据进行 deflate 压缩，默认关闭
        /// </summary>
        public bool compressIntakeRequests { get; set; }
        /// <summary>
        /// 开启使用 db 限制数据大小，默认 100MB，单位 Byte，数据库越大，磁盘压力越大，默认不开启。
        /// 注意：开启之后 Log 配置 logCacheLimitCount 及 RUM 配置rumCacheLimitCount 将失效
        /// </summary>
        public bool enableLimitWithDbSize { get; set; }
        /// <summary>
        /// DB 缓存限制大小。范围 [30MB,)，默认 100MB，单位 byte
        /// </summary>
        public int dbCacheLimit { get; set; }
        /// <summary>
        /// 设置数据库中数据丢弃规则。
        ///丢弃策略：discard丢弃新数据（默认）、discardOldest丢弃旧数据
        /// </summary>
        public DBCacheDiscard dbDiscardStrategy { get; set; }
        /// <summary>
        /// 对单个字段进行更改
        /// </summary>
        public Dictionary<string, object> dataModifier { get; set; }
        /// <summary>
        ///  对单条数据数据进行更改。
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> lineDataModifier { get; set; }

        /// <summary>
        /// Unity SDK 版本号
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
    ///  RUM 配置
    /// </summary>
    public class RUMConfig
    {
        /// <summary>
        /// Android  RUM AppId
        /// </summary>
        public string androidAppId { get; set; }
        /// <summary>
        /// iOS  RUM AppId
        /// </summary>
        public string iOSAppId { get; set; }

        /// <summary>
        /// 采集率的值范围为[0,1]，默认值为 1
        /// </summary>
        public float sampleRate { get; set; }

        /// <summary>
        /// 错误会话采样率的值范围为[0,1]，默认值为 0。未被采样命中的 Session ，命中 ERROR 采样，发生错误时，采集错误前 1 分钟数据
        /// </summary>
        public float sessionOnErrorSampleRate { get; set; }

        /// <summary>
        /// 添加 SDK 全局属性
        /// </summary>
        public Dictionary<string, string> globalContext { get; set; }
        /// <summary>
        /// 是否开启 Native Action 收集，默认 false
        /// </summary>
        public bool enableNativeUserAction { get; set; }

        /// <summary>
        /// 是否开启 Native View 收集，默认 false
        /// </summary>
        public bool enableNativeUserView { get; set; }
        /// <summary>
        /// 是否开启 Native Resource 请求，Android 支持 Okhttp，iOS 使用 NSURLSession，默认 false
        /// </summary>
        public bool enableNativeUserResource { get; set; }
        /// <summary>
        /// 是否采集请求目标域名地址的 IP。作用域：只影响 enableNativeUserResource 为 true 的默认采集。iOS：>= iOS 13 下支持。Android：单个 Okhttp 对相同域名存在 IP 缓存机制，相同 OkhttpClient，在连接服务端 IP 不发生变化的前提下，只会生成一次
        /// </summary>
        public bool enableResourceHostIP { get; set; }
        /// <summary>
        /// 是否采集 Native Java Crash、C/C++ Crash 
        /// </summary>
        public bool enableTrackNativeCrash { get; set; }
        /// <summary>
        /// 是否开启 `Native ANR` 监测，默认为 `false`
        /// </summary>
        public bool enableTrackNativeAppANR { get; set; }
        /// <summary>
        /// 是否进行 `Native Freeze` 自动追踪，默认为 `false`
        /// </summary>
        public bool enableTrackNativeFreeze { get; set; }
        /// <summary>
        /// 设置采集 `Native Freeze`卡顿的阈值，取值范围 [100,)，单位毫秒。iOS 默认 250ms，Android 默认 1000ms
        /// </summary>
        public int nativeFreezeDurationMs { get; set; }
        /// <summary>
        /// 错误监控补充类型：all、battery、 memory、 cpu
        /// </summary>
        public ErrorMonitorType extraMonitorTypeWithError { get; set; }
        /// <summary>
        /// 页面监控补充类型： all 、battery（仅Android支持)、 memory、cpu、fps
        /// </summary>
        public DeviceMetricsMonitorType deviceMonitorType { get; set; }
        /// <summary>
        /// normal(默认)、 frequent、rare
        /// </summary>
        public DetectFrequency detectFrequency { get; set; }
        /// <summary>
        /// 本地缓存最大 RUM 条目数量限制 [10_000,)，默认 100_000
        /// </summary>
        public int rumCacheLimitCount { get; set; }
        /// <summary>
        /// 丢弃策略：`discard`丢弃新数据（默认）、`discardOldest`丢弃旧数据
        /// </summary>
        public RUMCacheDiscard rumDiscardStrategy { get; set; }

    }

    /// <summary>
    ///  Trace 链路配置
    /// </summary>
    public class TraceConfig
    {
        /// <summary>
        /// 采集率的值范围为 [0,1]，默认值为 1
        /// </summary>
        public float sampleRate { get; set; }
        /// <summary>
        /// 链路类型：ddTrace（默认）、zipkinMultiHeader、zipkinSingleHeader、traceparent、skywalking、jaeger
        /// </summary>
        public TraceType traceType { get; set; }
        /// <summary>
        /// 是否与 RUM 数据关联，默认 false
        /// </summary>
        public bool enableLinkRumData { get; set; } = false;

        /// <summary>
        /// 是否开启自动添加 Trace header，Android 支持 Okhttp，iOS 使用 NSURLSession
        /// </summary>
        public bool enableNativeAutoTrace { get; set; } = false;

    }
    /// <summary>
    /// Log 配置
    /// </summary>
    public class LogConfig
    {
        /// <summary>
        /// 采集率的值范围为>= 0、<= 1，默认值为 1
        /// </summary>

        public float sampleRate { get; set; }
        /// <summary>
        /// 是否关联 RUM 数据
        /// </summary>
        public bool enableLinkRumData { get; set; }

        /// <summary>
        /// 是否开启自定义日志
        /// </summary>
        public bool enableCustomLog { get; set; }

        /// <summary>
        /// 日志丢弃策略：discard丢弃新数据（默认）、discardOldest丢弃旧数据
        /// </summary>
        public LogCacheDiscard discardStrategy { get; set; }
        /// <summary>
        /// 日志等级过滤，数组中需填写 日志等级：info提示、warning警告、error错误、critical、ok恢复，默认不过滤
        /// </summary>
        public List<LogLevel> logLevelFilters { get; set; }
        /// <summary>
        /// 添加 Log 全局属性
        /// </summary>
        public Dictionary<string, string> globalContext { get; set; }
        /// <summary>
        /// 本地缓存最大日志条目数量限制 [1000,)，日志越大，代表磁盘缓存压力越大，默认 5000
        /// </summary>
        public int logCacheLimitCount { get; set; }
    }

    /// <summary>
    /// 用户数据
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// 用户 id
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string userEmail { get; set; }
        /// <summary>
        /// 用户的额外信息
        /// </summary>
        public Dictionary<string, string> extra { get; set; }
    }

    /// <summary>
    ///  Resource 内容指标
    /// </summary>
    public class ResourceParams
    {
        /// <summary>
        /// 请求 url
        /// </summary>
        public string url { get; set; } = "";
        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string> requestHeader { get; set; }
        /// <summary>
        /// 响应头
        /// </summary>
        public Dictionary<string, string> responseHeader { get; set; }
        /// <summary>
        /// 响应 connection
        /// </summary>
        public string responseConnection { get; set; } = "";
        /// <summary>
        /// 响应 ContentType
        /// </summary>
        public string responseContentType { get; set; } = "";
        /// <summary>
        /// 响应 ContentEncoding
        /// </summary>
        public string responseContentEncoding { get; set; } = "";
        /// <summary>
        /// http 方法
        /// </summary>
        public string resourceMethod { get; set; } = "";
        /// <summary>
        /// 返回数据 body
        /// </summary>
        public string responseBody { get; set; } = "";
        /// <summary>
        /// 请求结果状态码
        /// </summary>
        public int resourceStatus { get; set; } = -1;

    }

    // /// <summary>
    // /// Resource 网络耗时指标
    // /// </summary>
    // public class NetStatus
    // {

    //     /// <summary>
    //     /// 请求任务开始时间
    //     /// </summary>
    //     public long callStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// tcp 连接时间
    //     /// </summary>
    //     public long tcpStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// tcp 结束时间
    //     /// </summary>
    //     public long tcpEndTime { get; set; } = -1L;
    //     /// <summary>
    //     /// dns 开始时间
    //     /// </summary>
    //     public long dnsStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// dns 结束时间
    //     /// </summary>
    //     public long dnsEndTime { get; set; } = -1L;
    //     /// <summary>
    //     /// 响应开始时间
    //     /// </summary>
    //     public long bodyStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// 响应结束时间
    //     /// </summary>
    //     public long bodyEndTime { get; set; } = -1L;
    //     /// <summary>
    //     /// ssl 开始时间
    //     /// </summary>
    //     public long sslStartTime { get; set; } = -1L;
    //     /// <summary>
    //     /// ssl 结束时间
    //     /// </summary>
    //     public long sslEndTime { get; set; } = -1L;

    // }

    /// <summary>
    /// 错误附加数据
    /// </summary>
    public enum ErrorMonitorType : int
    {
        All = -1,
        Battery = 1 << 1,
        Memory = 1 << 2,
        CPU = 1 << 3
    }

    /// <summary>
    /// 页面监控指标
    /// </summary>
    public enum DeviceMetricsMonitorType : int
    {
        All = -1,
        /// <summary>
        /// 仅仅支持 Android
        /// </summary>
        Battery = 1 << 1,
        Memory = 1 << 2,
        CPU = 1 << 3,
        FPS = 1 << 4
    }

    /// <summary>
    /// 扫描周期
    /// </summary>
    public enum DetectFrequency { Normal, Frequent, Rare }

    /// <summary>
    /// 链路类型
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
    /// 日志缓存丢弃策略
    /// </summary>
    public enum LogCacheDiscard { Discard, DiscardOldest }
    /// <summary>
    /// 数据缓存丢弃策略
    /// </summary>
    public enum DBCacheDiscard { Discard, DiscardOldest }
    /// <summary>
    /// RUM 缓存丢弃策略
    /// </summary>
    public enum RUMCacheDiscard { Discard, DiscardOldest }


    /// <summary>
    /// 日志等级
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
    /// Enum 转化为 String
    /// </summary>
    public class BridgeEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ErrorMonitorType || value is DeviceMetricsMonitorType)
            {
                writer.WriteValue(Convert.ToInt64(value));
            }
            else if (value is DetectFrequency || value is TraceType || value is LogCacheDiscard || value is LogLevel)
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
    /// 删除字典空 value
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
    /// Unity 桥接接口
    /// </summary>
    public class FTUnityBridge
    {
        public const string SDK_VERSION = "1.0.1-alpha.1";
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
        ///  iOS 桥接调用方法
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="json">json 格式参数</param>
        /// <returns></returns>
        [DllImport("__Internal")]
        private static extern IntPtr invokeMethod(string method, string json);

#endif

#if (UNITY_ANDROID && !UNITY_EDITOR)
         
        /// <summary>
        /// Android 桥接方法调用类
        /// </summary>
        /// 
        private const string ANDROID_PLUGIN_CLASS_NAME = "com.ft.sdk.unity.bridge.FTUnityBridge";

        private static AndroidJavaObject androidPlugin;

#endif

        /// <summary>
        /// 初始化SDK本地配置数据
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
        /// 设置 RUM 配置
        /// </summary>
        /// <param name="config"></param>
        public static void InitRUMConfig(RUMConfig config)
        {
            _InovkeMethod(KEY_METHOD_INIT_RUM_CONFIG, JsonConvert.SerializeObject(config, JSON_HANDLER));
        }

        /// <summary>
        /// 设置 log 配置
        /// </summary>
        /// <param name="config"></param>
        public static void InitLogConfig(LogConfig config)
        {
            _InovkeMethod(KEY_METHOD_INIT_LOG_CONFIG, JsonConvert.SerializeObject(config, JSON_HANDLER));
        }

        /// <summary>
        ///  设置 Trace 配置
        /// </summary>
        /// <param name="config"></param>
        public static void InitTraceConfig(TraceConfig config)
        {
            _InovkeMethod(KEY_METHOD_INIT_TRACE_CONFIG, JsonConvert.SerializeObject(config, JSON_HANDLER));
        }

        /// <summary>
        /// 绑定 RUM 用户信息
        /// </summary>
        /// <param name="userId">用户唯一id</param>
        public static void BindUserData(string userId)
        {
            BindUserData(new UserData()
            {
                userId = userId
            });
        }

        /// <summary>
        /// 绑定 RUM 用户信息
        /// </summary>
        /// <param name="userData"></param>
        public static void BindUserData(UserData userData)
        {
            _InovkeMethod(KEY_METHOD_BIND_USER_DATA, JsonConvert.SerializeObject(userData, JSON_HANDLER));
        }

        /// <summary>
        /// 解绑用户数据
        /// </summary>
        public static async Task UnBindUserdata()
        {
            await _InovkeMethodAsync(KEY_METHOD_UN_BIND_USER_DATA, "");
        }

        /// <summary>
        /// 添加自定义全局参数。作用于 RUM、Log 数据
        /// </summary>
        public static void AppendGlobalContext(Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_APPEND_GLOBAL_CONTEXT, JsonConvert.SerializeObject(property, JSON_HANDLER));
        }

        /// <summary>
        /// 添加自定义 RUM 全局参数。作用于 RUM 数据
        /// </summary>
        public static void AppendRUMGlobalContext(Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_APPEND_RUM_GLOBAL_CONTEXT, JsonConvert.SerializeObject(property, JSON_HANDLER));
        }

        /// <summary>
        /// 添加自定义 RUM、Log 全局参数。作用于 Log 数据
        /// </summary>
        public static void AppendLogGlobalContext(Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_APPEND_LOG_GLOBAL_CONTEXT, JsonConvert.SerializeObject(property, JSON_HANDLER));
        }

        /// <summary>
        ///  添加 Action 
        /// </summary>
        /// <param name="actionName">action 名称</param>
        /// <param name="actionType">action 类型</param>
        /// <param name="duartion">纳秒，持续时间</param>
        public static void AddAction(string actionName, string actionType, long duartion)
        {

            _InovkeMethod(KEY_METHOD_ADD_ACTION, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"actionName" , actionName},
                {"actionType",actionType},
                {"duration" , duartion}
            }, JSON_HANDLER));
        }

        /// <summary>
        ///  添加 Action 
        /// </summary>
        /// <param name="actionName"> action 名称</param>
        /// <param name="actionType"> action 类型</param>
        public static void StartAction(string actionName, string actionType)
        {
            StartAction(actionName, actionType, null);
        }

        /// <summary>
        /// 添加 Action
        /// </summary>
        /// <param name="actionName">action 名称</param>
        /// <param name="actionType">action 类型</param>
        /// <param name="property">附加属性参数</param>
        public static void StartAction(string actionName, string actionType, Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_START_ACTION, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"actionName" , actionName},
                {"actionType",actionType},
                {"property" , property}
            }.WithoutNullValues(), JSON_HANDLER));

        }

        /// <summary>
        /// 创建 View
        /// </summary>
        /// <param name="viewName"> 当前页面名称</param>
        /// <param name="loadTime">加载时间，纳秒</param>
        public static void CreateView(string viewName, long loadTime)
        {
            _InovkeMethod(KEY_METHOD_CREATE_VIEW, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"viewName" , viewName },
                {"loadTime" , loadTime },
            }, JSON_HANDLER));
        }

        /// <summary>
        ///  View 开始
        /// </summary>
        /// <param name="viewName">当前页面名称</param>
        public static void StartView(string viewName)
        {
            StartView(viewName, null);
        }

        /// <summary>
        /// View 开始
        /// </summary>
        /// <param name="viewName">当前页面名称</param>
        /// <param name="property">附加属性参数</param>
        public static void StartView(string viewName, Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_START_VIEW, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"viewName" , viewName },
                {"property" , property },
            }.WithoutNullValues(), JSON_HANDLER));
        }

        /// <summary>
        /// View 结束
        /// </summary>
        public static void StopView()
        {
            StopView(null);
        }

        /// <summary>
        /// View 结束
        /// </summary>
        /// <param name="property">附加属性参数</param>
        public static void StopView(Dictionary<string, object> property)
        {
            _InovkeMethod(KEY_METHOD_STOP_VIEW, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
               {"property" , property },
            }.WithoutNullValues(), JSON_HANDLER));
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static async Task AddError(string log, string message, Dictionary<string, object> property)
        {
            await AddError(log, message, DEFAULT_ERROR_TYPE, property);
        }


        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static async Task AddError(string log, string message)
        {
            await AddError(log, message, DEFAULT_ERROR_TYPE, null);
        }


        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="errorType">错误类型</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static async Task AddError(string log, string message, string errorType)
        {
            await AddError(log, message, errorType, null);
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="message">消息</param>
        /// <param name="errorType">错误类型</param>
        /// <param name="property">附加属性参数</param>
        /// <returns></returns>
        public static async Task AddError(string log, string message, string errorType,
            Dictionary<string, object> property)
        {
            string state = "run";
            await _InovkeMethodAsync(KEY_METHOD_ADD_ERROR, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"log", log },
                {"message" , message },
                {"errorType" , errorType },
                {"state" , state },
                {"property" , property }
            }, JSON_HANDLER));
        }

        /// <summary>
        /// 添加长耗时任务
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <param name="duration">持续时间，纳秒</param>
        /// <returns></returns>
        public static async Task AddLongTask(string log, long duration)
        {
            await AddLongTask(log, duration, null);

        }

        /// <summary>
        /// 添加长耗时任务
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <param name="duration">持续时间，纳秒</param>
        /// <param name="property">附加属性参数</param>
        /// <returns></returns>
        public static async Task AddLongTask(string log, long duration, Dictionary<string, object> property)
        {
            await _InovkeMethodAsync(KEY_METHOD_ADD_LONG_TASK, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"log" , log },
                {"duration" , duration},
                {"property" , property}
            }.WithoutNullValues(), JSON_HANDLER));

        }

        /// <summary>
        ///  resource 开始
        /// </summary>
        /// <param name="resourceId">资源 Id</param>
        /// <returns></returns>
        public static async Task StartResource(string resourceId)
        {
            await StartResource(resourceId, null);
        }

        /// <summary>
        /// resource 开始
        /// </summary>
        /// <param name="resourceId">资源 Id</param>
        /// <param name="property">附加属性参数</param>
        /// <returns></returns>
        public static async Task StartResource(string resourceId, Dictionary<string, object> property)
        {
            await _InovkeMethodAsync(KEY_METHOD_START_RESOURCE, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"resourceId" , resourceId},
                {"property" , property},
            }.WithoutNullValues(), JSON_HANDLER));
        }

        /// <summary>
        /// resource 结束
        /// </summary>
        /// <param name="resourceId">资源 Id</param>
        /// <returns></returns>
        public static async Task StopResource(string resourceId)
        {
            await StopResource(resourceId, null);
        }

        /// <summary>
        /// resource 结束
        /// </summary>
        /// <param name="resourceId">资源 Id</param>
        /// <param name="property">附加属性参数</param>
        public static async Task StopResource(string resourceId, Dictionary<string, object> property)
        {
            await _InovkeMethodAsync(KEY_METHOD_STOP_RESOURCE, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"resourceId" , resourceId},
                {"property" , property},
            }.WithoutNullValues(), JSON_HANDLER));
        }

        /// <summary>
        /// 添加网络传输内容和指标
        /// </summary>
        /// <param name="resourceId">资源 Id</param>
        /// <param name="resourceParams">数据传输内容</param>
        /// <param name="netStatus">网络指标数据</param>
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
        /// 添加日志
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <param name="level">日志等级 info，warning，error，critical，ok</param>
        /// <returns></returns>
        public static async Task AddLog(string log, LogLevel level)
        {
            await AddLog(log, level, null);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <param name="level">日志等级 info，warning，error，critical，ok</param>
        /// <param name="property">附加属性参数</param>
        /// <returns></returns>
        public static async Task AddLog(string log, LogLevel level, Dictionary<string, object> property)
        {
            await _InovkeMethodAsync(KEY_METHOD_ADD_LOG, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"log" , log},
                {"level" , level},
                {"property" , property},
            }.WithoutNullValues(), JSON_HANDLER));
        }
        /// <summary>
        /// 获取链路
        /// </summary>
        /// <param name="resourceId">资源 Id</param>
        /// <param name="url">url 地址</param>
        /// <returns>json 字符</returns>

        public static async Task<string> GetTraceHeader(string resourceId, string url)
        {
            return await _InovkeMethodAsync(KEY_METHOD_GET_TRACE_HEADER, JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                {"resourceId" , resourceId},
                {"url" , url},
            }, JSON_HANDLER));
        }

        /// <summary>
        /// 获取链路 Id
        /// </summary>
        /// <param name="url">url 地址</param>
        /// <returns>json 字符</returns>
        public static async Task<string> GetTraceHeaderWithUrl(string url)
        {
            return await GetTraceHeader(null, url);
        }


        /// <summary>
        /// 主动同步数据
        /// </summary>
        public static async void flushSyncData()
        {
            await _InovkeMethodAsync(KEY_METHOD_FLUSH_SYNC_DATA, "");
        }

        /// <summary>
        /// 清理未上报的缓存数据
        /// </summary>
        public static async void cleanAllData()
        {
            await _InovkeMethodAsync(KEY_METHOD_CLEAR_ALL_DATA, "");
        }


        /// <summary>
        /// SDK 释放
        /// </summary>
        public static void DeInit()
        {
            _InovkeMethod(KEY_METHOD_DE_INIT, "");
        }

        /// <summary>
        /// 异步跨平台传参
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
        /// 负责跨平台各个方法的传参数
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="json">json 格式参数</param>
        /// <returns></returns>
        private static string _InovkeMethod(string method, string json)
        {

            UnityEngine.Debug.Log(json);

#if (UNITY_IOS && !UNITY_EDITOR)
        IntPtr ptr = invokeMethod(method,json);
        return Marshal.PtrToStringAnsi(ptr);
#endif

            // 在Android上调用Android插件的方法
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