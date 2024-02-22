package com.ft.sdk.unity.bridge;

import android.util.Log;

import com.ft.sdk.DetectFrequency;
import com.ft.sdk.DeviceMetricsMonitorType;
import com.ft.sdk.ErrorMonitorType;
import com.ft.sdk.FTLogger;
import com.ft.sdk.FTLoggerConfig;
import com.ft.sdk.FTRUMConfig;
import com.ft.sdk.FTRUMGlobalManager;
import com.ft.sdk.FTSDKConfig;
import com.ft.sdk.FTSdk;
import com.ft.sdk.FTTraceConfig;
import com.ft.sdk.FTTraceManager;
import com.ft.sdk.LogCacheDiscard;
import com.ft.sdk.TraceType;
import com.ft.sdk.garble.bean.AppState;
import com.ft.sdk.garble.bean.NetStatusBean;
import com.ft.sdk.garble.bean.ResourceParams;
import com.ft.sdk.garble.bean.Status;
import com.ft.sdk.garble.bean.UserData;
import com.ft.sdk.garble.utils.LogUtils;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Iterator;

public class FTUnityBridge {

    private static final String TAG = "FTUnityBridge";
    private static final String METHOD_INSTALL = "Install";
    private static final String METHOD_DE_INIT = "DeInit";
    private static final String METHOD_BIND_USER_DATA = "BindUserData";
    private static final String METHOD_UN_BIND_USERDATA = "UnBindUserdata";
    public static final String METHOD_INIT_RUM_CONFIG = "InitRUMConfig";
    private static final String METHOD_CREATE_VIEW = "CreateView";
    private static final String METHOD_START_VIEW = "StartView";
    private static final String METHOD_STOP_VIEW = "StopView";
    private static final String METHOD_START_ACTION = "StartAction";
    private static final String METHOD_ADD_ACTION = "AddAction";
    private static final String METHOD_START_RESOURCE = "StartResource";
    private static final String METHOD_STOP_RESOURCE = "StopResource";
    private static final String METHOD_ADD_RESOURCE = "AddResource";
    private static final String METHOD_ADD_ERROR = "AddError";
    private static final String METHOD_ADD_LONG_TASK = "AddLongTask";
    private static final String METHOD_INIT_LOG_CONFIG = "InitLogConfig";
    private static final String METHOD_ADD_LOG = "AddLog";
    private static final String METHOD_INIT_TRACE_CONFIG = "InitTraceConfig";
    private static final String METHOD_INIT_GET_TRACE_HEADER = "GetTraceHeader";


    /**
     * Unity 方法转化
     *
     * @param method
     * @param json
     * @return
     */
    public static String invokeMethod(String method, String json) {
        LogUtils.i(TAG, method + "," + json);
        JSONObject data = null;
        try {
            data = new JSONObject(json);
            if (METHOD_INSTALL.equals(method)) {
                install(data);
            } else if (METHOD_DE_INIT.equals(method)) {
                deInit(data);
            } else if (METHOD_BIND_USER_DATA.equals(method)) {
                bindUserData(data);
            } else if (METHOD_UN_BIND_USERDATA.equals(method)) {
                unBindUserdata(data);
            } else if (METHOD_INIT_RUM_CONFIG.equals(method)) {
                initRUMConfig(data);
            } else if (METHOD_CREATE_VIEW.equals(method)) {
                createView(data);
            } else if (METHOD_START_VIEW.equals(method)) {
                startView(data);
            } else if (METHOD_STOP_VIEW.equals(method)) {
                stopView(data);
            } else if (METHOD_START_ACTION.equals(method)) {
                startAction(data);
            } else if (METHOD_ADD_ACTION.equals(method)) {
                addAction(data);
            } else if (METHOD_START_RESOURCE.equals(method)) {
                startResource(data);
            } else if (METHOD_STOP_RESOURCE.equals(method)) {
                stopResource(data);
            } else if (METHOD_ADD_RESOURCE.equals(method)) {
                addResource(data);
            } else if (METHOD_ADD_ERROR.equals(method)) {
                addError(data);
            } else if (METHOD_ADD_LONG_TASK.equals(method)) {
                addLongTask(data);
            } else if (METHOD_INIT_LOG_CONFIG.equals(method)) {
                initLogConfig(data);
            } else if (METHOD_ADD_LOG.equals(method)) {
                addLog(data);
            } else if (METHOD_INIT_TRACE_CONFIG.equals(method)) {
                initTraceConfig(data);
            } else if (METHOD_INIT_GET_TRACE_HEADER.equals(method)) {
                return getTraceHeader(data);
            }
        } catch (JSONException e) {
            LogUtils.e(TAG, Log.getStackTraceString(e));
        }

        return null;
    }

    /**
     * SDK 初始化
     *
     * @param data
     */
    private static void install(JSONObject data) {
        String datakitUrl = data.optString("datakitUrl", null);
        if (datakitUrl == null || datakitUrl.isEmpty()) {
            datakitUrl = data.optString("serverUrl", null);
        }
        String datawayUrl = data.optString("datawayUrl", null);
        String cliToken = data.optString("cliToken", null);
        if (datakitUrl != null || (datawayUrl != null && cliToken != null)) {
            FTSDKConfig config = datakitUrl != null ? FTSDKConfig.builder(datakitUrl) : FTSDKConfig.builder(datawayUrl, cliToken);
            config.setDebug(data.optBoolean("debug", false));
            String env = data.optString("env", null);
            if (env != null) {
                config.setEnv(env);
            }

            JSONObject globalContext = data.optJSONObject("globalContext");
            if (globalContext != null) {
                for (Iterator<String> it = globalContext.keys(); it.hasNext(); ) {
                    String key = it.next();
                    config.addGlobalContext(key, globalContext.optString(key));
                }
            }

            String service = data.optString("serviceName", null);
            if (service != null) {
                config.setServiceName(service);
            }
            FTSdk.install(config);
        }
    }

    /**
     * SDK 释放
     *
     * @param data
     */
    private static void deInit(JSONObject data) {
        FTSdk.shutDown();
    }

    /**
     * 绑定用户数据
     *
     * @param data
     */
    private static void bindUserData(JSONObject data) {
        UserData userData = new UserData();
        String userId = data.optString("userId", null);
        String userName = data.optString("userName", null);
        String userEmail = data.optString("userEmail", null);
        JSONObject extra = data.optJSONObject("extra");
        if (userId != null) {
            userData.setId(userId);
        }
        if (userName != null) {
            userData.setName(userName);
        }
        if (userEmail != null) {
            userData.setEmail(userEmail);
        }
        if (extra != null) {
            HashMap<String, String> hashMap = convertJSONtoHashMap(extra);
            userData.setExts(hashMap);
        }

        FTSdk.bindRumUserData(userData);
    }

    /**
     * 释放用户数据
     *
     * @param data
     */
    private static void unBindUserdata(JSONObject data) {
        FTSdk.unbindRumUserData();
    }

    /**
     * 初始化 RUM
     *
     * @param data
     */
    private static void initRUMConfig(JSONObject data) {
        FTRUMConfig rumConfig = new FTRUMConfig();
        String appId = data.optString("androidAppId", null);
        rumConfig.setRumAppId(appId);
        String sampleRate = data.optString("sampleRate", null);
        if (sampleRate != null) {
            rumConfig.setSamplingRate(Float.valueOf(sampleRate));
        }
        boolean enableNativeUserAction = data.optBoolean("enableNativeUserAction");
        rumConfig.setEnableTraceUserAction(enableNativeUserAction);

        boolean enableNativeUserView = data.optBoolean("enableNativeUserView");
        rumConfig.setEnableTraceUserAction(enableNativeUserView);

        boolean enableNativeUserResource = data.optBoolean("enableNativeUserResource");
        rumConfig.setEnableTraceUserAction(enableNativeUserResource);

        int errorMonitorType = data.optInt("extraMonitorTypeWithError", ErrorMonitorType.NO_SET);
        rumConfig.setExtraMonitorTypeWithError(errorMonitorType);

        Object deviceType = data.opt("deviceMonitorType");
        if (deviceType != null) {
            String detectFrequencyStr = data.optString("detectFrequency", null);
            DetectFrequency detectFrequency = DetectFrequency.DEFAULT;
            if (detectFrequencyStr != null) {
                if (detectFrequencyStr.equals("frequent")) {
                    detectFrequency = DetectFrequency.FREQUENT;
                } else if (detectFrequencyStr.equals("rare")) {
                    detectFrequency = DetectFrequency.RARE;
                }
            }
            int deviceMonitorType = data.optInt("deviceMonitorType", DeviceMetricsMonitorType.NO_SET);
            rumConfig.setDeviceMetricsMonitorType(deviceMonitorType, detectFrequency);
        }

        JSONObject globalContext = data.optJSONObject("globalContext");
        if (globalContext != null) {
            for (Iterator<String> it = globalContext.keys(); it.hasNext(); ) {
                String key = it.next();
                rumConfig.addGlobalContext(key, globalContext.optString(key));
            }
        }

        FTSdk.initRUMWithConfig(rumConfig);
    }

    /**
     * 创建 View
     *
     * @param data
     */
    private static void createView(JSONObject data) {
        String viewName = data.optString("viewName");
        long loadTime = data.optLong("loadTime");
        FTRUMGlobalManager.get().onCreateView(viewName, loadTime);
    }

    /**
     * View 开始
     *
     * @param data
     */
    private static void startView(JSONObject data) {
        String viewName = data.optString("viewName");
        JSONObject property = data.optJSONObject("property");
        HashMap<String, Object> params = convertJSONtoHashMap(property);
        FTRUMGlobalManager.get().startView(viewName, params);
    }

    /**
     * View 结束
     *
     * @param data
     */
    private static void stopView(JSONObject data) {
        if (data != null) {
            JSONObject property = data.optJSONObject("property");
            HashMap<String, Object> params = convertJSONtoHashMap(property);
            FTRUMGlobalManager.get().stopView(params);
        } else {
            FTRUMGlobalManager.get().stopView();
        }
    }

    /**
     * 添加 Action
     */
    private static void addAction(JSONObject data) {
        String actionName = data.optString("actionName");
        String actionType = data.optString("actionType");
        long duration = data.optLong("duration");
        FTRUMGlobalManager.get().addAction(actionName, actionType, duration);
    }

    /**
     * Action 开始
     *
     * @param data
     */
    private static void startAction(JSONObject data) {
        String actionName = data.optString("actionName");
        String actionType = data.optString("actionType");
        JSONObject property = data.optJSONObject("property");
        HashMap<String, Object> params = convertJSONtoHashMap(property);
        FTRUMGlobalManager.get().startAction(actionName, actionType, params);
    }

    /**
     * Resource 开始
     *
     * @param data
     */
    private static void startResource(JSONObject data) {
        String key = data.optString("resourceId");
        JSONObject property = data.optJSONObject("property");
        HashMap<String, Object> params = convertJSONtoHashMap(property);
        FTRUMGlobalManager.get().startResource(key, params);
    }

    /**
     * Resource 结束
     *
     * @param data
     */
    private static void stopResource(JSONObject data) {
        String key = data.optString("resourceId");
        JSONObject property = data.optJSONObject("property");
        HashMap<String, Object> params = convertJSONtoHashMap(property);
        FTRUMGlobalManager.get().stopResource(key);
    }

    /**
     * 添加 Resource 数据
     *
     * @param data
     */
    private static void addResource(JSONObject data) {
        String key = data.optString("resourceId");
        JSONObject resourceParams = data.optJSONObject("resourceParams");
        ResourceParams params = new ResourceParams();
        if (resourceParams != null) {
            params.url = resourceParams.optString("url");
            params.resourceMethod = resourceParams.optString("resourceMethod");
            params.requestHeader = resourceParams.optString("requestHeader");
            params.responseHeader = resourceParams.optString("responseHeader");
            params.responseBody = resourceParams.optString("responseBody");
            params.resourceStatus = resourceParams.optInt("resourceStatus");
            params.responseConnection = resourceParams.optString("responseConnection");
            params.responseContentEncoding = resourceParams.optString("responseContentEncoding");
            params.responseContentType = resourceParams.optString("responseContentType");
        }

        JSONObject netStatus = data.optJSONObject("netStatus");
        NetStatusBean netStatusBean = new NetStatusBean();
        if (netStatus != null) {
            netStatusBean.fetchStartTime = netStatus.optLong("fetchStartTime");
            netStatusBean.dnsStartTime = netStatus.optLong("dnsStartTime");
            netStatusBean.dnsEndTime = netStatus.optLong("dnsEndTime");
            netStatusBean.responseStartTime = netStatus.optLong("responseStartTime");
            netStatusBean.responseEndTime = netStatus.optLong("responseEndTime");
            netStatusBean.sslStartTime = netStatus.optLong("sslStartTime");
            netStatusBean.sslEndTime = netStatus.optLong("sslEndTime");
            netStatusBean.tcpStartTime = netStatus.optLong("tcpStartTime");
            netStatusBean.tcpEndTime = netStatus.optLong("tcpEndTime");
        }

        FTRUMGlobalManager.get().addResource(key, params, netStatusBean);
    }

    /**
     * 添加 Error 数据
     *
     * @param data
     */
    private static void addError(JSONObject data) {
        String message = data.optString("log");
        String stack = data.optString("message");
        String errorType = data.optString("errorType");
        String state = data.optString("state");
        JSONObject property = data.optJSONObject("property");
        HashMap<String, Object> params = convertJSONtoHashMap(property);
        FTRUMGlobalManager.get().addError(message, stack,
                errorType, AppState.getValueFrom(state), params);
    }

    /**
     * 添加 LongTask 数据
     *
     * @param data
     */
    private static void addLongTask(JSONObject data) {
        String message = data.optString("log");
        long duration = data.optLong("duration");
        FTRUMGlobalManager.get().addLongTask(message, duration);
    }


    /**
     * 初始化 Log 配置
     *
     * @param data
     */
    private static void initLogConfig(JSONObject data) {
        FTLoggerConfig config = new FTLoggerConfig();

        String sampleRate = data.optString("sampleRate", null);
        if (sampleRate != null) {
            config.setSamplingRate(Float.valueOf(sampleRate));
        }
        boolean enableLinkRumData = data.optBoolean("enableLinkRumData");
        config.setEnableLinkRumData(enableLinkRumData);

        boolean enableCustomLog = data.optBoolean("enableCustomLog");
        config.setEnableCustomLog(enableCustomLog);

        String discardStrategy = data.optString("discardStrategy", null);
        if (discardStrategy != null) {
            if (discardStrategy.equals("discardOldest")) {
                config.setLogCacheDiscardStrategy(LogCacheDiscard.DISCARD_OLDEST);
            } else if (discardStrategy.equals("discard")) {
                config.setLogCacheDiscardStrategy(LogCacheDiscard.DISCARD);
            }

        }
        JSONArray logLevelFilters = data.optJSONArray("logLevelFilters");
        if (logLevelFilters != null) {
            Status[] statuses = new Status[logLevelFilters.length()];
            for (int i = 0; i < logLevelFilters.length(); i++) {
                Status logStatus = null;
                for (Status value : Status.values()) {
                    if (value.name.equals(logLevelFilters.optString(i))) {
                        logStatus = value;
                        break;
                    }
                }
                statuses[i] = logStatus;
            }

            config.setLogLevelFilters(statuses);

        }

        JSONObject globalContext = data.optJSONObject("globalContext");
        if (globalContext != null) {
            for (Iterator<String> it = globalContext.keys(); it.hasNext(); ) {
                String key = it.next();
                config.addGlobalContext(key, globalContext.optString(key));
            }
        }

        FTSdk.initLogWithConfig(config);
    }

    private static void addLog(JSONObject data) {
        String content = data.optString("log", null);
        String status = data.optString("level", null);
        JSONObject property = data.optJSONObject("property");
        if (content != null && status != null) {
            HashMap<String, Object> propertyMap = convertJSONtoHashMap(property);
            Status logStatus = null;
            for (Status value : Status.values()) {
                if (value.name.equals(status)) {
                    logStatus = value;
                    break;
                }
            }
            FTLogger.getInstance().logBackground(content, logStatus, propertyMap);
        }
    }

    /**
     * 初始化 Trace 配置
     *
     * @param data
     */
    private static void initTraceConfig(JSONObject data) {
        FTTraceConfig config = new FTTraceConfig();
        String sampleRate = data.optString("sampleRate", null);
        if (sampleRate != null) {
            config.setSamplingRate(Float.valueOf(sampleRate));
        }

        String traceType = data.optString("traceType", null);
        if (traceType != null) {
            if (traceType.equals("ddtrace")) {
                config.setTraceType(TraceType.DDTRACE);
            } else if (traceType.equals("zipkinMulti")) {
                config.setTraceType(TraceType.ZIPKIN_MULTI_HEADER);
            } else if (traceType.equals("zipkinSingle")) {
                config.setTraceType(TraceType.ZIPKIN_SINGLE_HEADER);
            } else if (traceType.equals("traceparent")) {
                config.setTraceType(TraceType.TRACEPARENT);
            } else if (traceType.equals("skywalking")) {
                config.setTraceType(TraceType.SKYWALKING);
            } else if (traceType.equals("jaeger")) {
                config.setTraceType(TraceType.JAEGER);
            }
        }

        Boolean enableLinkRumData = data.optBoolean("enableLinkRumData");
        if (enableLinkRumData != null) {
            config.setEnableLinkRUMData(enableLinkRumData);
        }
        Boolean enableAutoTrace = data.optBoolean("enableAutoTrace");
        if (enableAutoTrace != null) {
            config.setEnableAutoTrace(enableAutoTrace);
        }
        FTSdk.initTraceWithConfig(config);
    }

    private static String getTraceHeader(JSONObject data) {

        String key = data.optString("resourceId", null);
        String url = data.optString("url", null);
        if (key != null && url != null) {
            JSONObject result = new JSONObject();
            HashMap<String, String> headerMap = null;
            try {
                headerMap = FTTraceManager.get().getTraceHeader(key, url);
                for (String headerKey : headerMap.keySet()) {
                    result.put(headerKey, headerMap.get(headerKey));
                }
            } catch (Exception e) {
                LogUtils.e(TAG, Log.getStackTraceString(e));
            }
            return result.toString();
        }
        return null;

    }


    /**
     * 转化 JSON 为 hashMap
     *
     * @param property
     * @param <T>
     * @return
     */
    private static <T> HashMap<String, T> convertJSONtoHashMap(JSONObject property) {
        if (property != null) {
            HashMap<String, T> params = new HashMap<>();
            for (Iterator<String> it = property.keys(); it.hasNext(); ) {
                String key = it.next();
                params.put(key, (T) property.opt(key));
            }
            return params;
        }
        return null;
    }
}
