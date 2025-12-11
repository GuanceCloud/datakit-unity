package com.ft.sdk.unity.bridge;

import static com.ft.sdk.garble.utils.Constants.FT_LOG_DEFAULT_MEASUREMENT;

import android.util.Log;

import com.ft.sdk.DBCacheDiscard;
import com.ft.sdk.DataModifier;
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
import com.ft.sdk.LineDataModifier;
import com.ft.sdk.LogCacheDiscard;
import com.ft.sdk.RUMCacheDiscard;
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
import java.util.Map;

public class FTUnityBridge {

    private static final String TAG = "FTUnityBridge";
    private static final String METHOD_INSTALL = "Install";
    private static final String METHOD_DE_INIT = "DeInit";
    private static final String METHOD_BIND_USER_DATA = "BindUserData";
    private static final String METHOD_APPEND_GLOBAL_CONTEXT = "AppendGlobalContext";
    private static final String METHOD_APPEND_LOG_GLOBAL_CONTEXT = "AppendLogGlobalContext";
    private static final String METHOD_APPEND_RUM_GLOBAL_CONTEXT = "AppendRUMGlobalContext";
    private static final String METHOD_FLUSH_SYNC_DATA = "FlushSyncData";
    private static final String METHOD_FLUSH_CLEAR_ALL_DATA = "ClearAllData";
    private static final String METHOD_UN_BIND_USERDATA = "UnBindUserdata";
    private static final String METHOD_INIT_RUM_CONFIG = "InitRUMConfig";
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
     * Unity method conversion
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
            } else if (METHOD_APPEND_GLOBAL_CONTEXT.equals(method)) {
                appendGlobalContext(data);
            } else if (METHOD_APPEND_LOG_GLOBAL_CONTEXT.equals(method)) {
                appendLogGlobalContext(data);
            } else if (METHOD_APPEND_RUM_GLOBAL_CONTEXT.equals(method)) {
                appendRUMGlobalContext(data);
            } else if (METHOD_FLUSH_SYNC_DATA.equals(method)) {
                flushSyncData();
            } else if (METHOD_FLUSH_CLEAR_ALL_DATA.equals(method)) {
                clearAllData();
            }
        } catch (JSONException e) {
            LogUtils.e(TAG, Log.getStackTraceString(e));
        }

        return null;
    }

    /**
     * SDK initialization
     *
     * @param data
     */
    private static void install(JSONObject data) {
        HashMap<String, Object> map = convertJSONtoHashMap(data);
        String serverUrl = (String) map.get("serverUrl");
        String datakitUrl = (String) map.get("datakitUrl");
        if (datakitUrl == null) {
            //Compatible with old version
            datakitUrl = serverUrl;
        }
        String datawayUrl = (String) map.get("datawayUrl");
        String cliToken = (String) map.get("clientToken");
        Boolean debug = (Boolean) map.get("debug");
        Boolean autoSync = (Boolean) map.get("autoSync");
        Number syncPageSize = (Number) map.get("syncPageSize");
        Number syncSleepTime = (Number) map.get("syncSleepTime");
        Boolean enableDataIntegerCompatible = (Boolean) map.get("enableDataIntegerCompatible");
        Boolean compressIntakeRequests = (Boolean) map.get("compressIntakeRequests");
        String serviceName = (String) map.get("serviceName");
        JSONObject globalContextJson = (JSONObject) map.get("globalContext");
        Boolean enableLimitWithDbSize = (Boolean) map.get("enableLimitWithDbSize");
        Number dbCacheLimit = (Number) (map.get("dbCacheLimit"));
        Object dbDiscardStrategy = map.get("dbDiscardStrategy");
        JSONObject dataModifier = (JSONObject) map.get("dataModifier");
        JSONObject lineDataModifier = (JSONObject) map.get("lineDataModifier");

        FTSDKConfig sdkConfig = (datakitUrl != null)
                ? FTSDKConfig.builder(datakitUrl)
                : FTSDKConfig.builder(datawayUrl, cliToken);

        String envString = (String) map.get("env");
        if (envString != null) {
            sdkConfig.setEnv(envString);
        }
        if (debug != null) {
            sdkConfig.setDebug(debug);
        }
        if (serviceName != null) {
            sdkConfig.setServiceName(serviceName);
        }
        if (autoSync != null) {
            sdkConfig.setAutoSync(autoSync);
        }
        if (syncPageSize != null) {
            sdkConfig.setCustomSyncPageSize(syncPageSize.intValue());
        }
        if (syncSleepTime != null) {
            sdkConfig.setSyncSleepTime(syncSleepTime.intValue());
        }
        if (enableDataIntegerCompatible != null && enableDataIntegerCompatible) {
            sdkConfig.enableDataIntegerCompatible();
        }
        if (compressIntakeRequests != null && compressIntakeRequests) {
            sdkConfig.setCompressIntakeRequests(compressIntakeRequests);
        }
        if (globalContextJson != null) {
            Map<String, Object> globalContext = convertJSONtoHashMap(globalContextJson);

            for (Map.Entry<String, Object> entry : globalContext.entrySet()) {
                sdkConfig.addGlobalContext(entry.getKey(), entry.getValue().toString());
            }
        }
        if (enableLimitWithDbSize != null && enableLimitWithDbSize) {
            if (dbCacheLimit != null) {
                sdkConfig.enableLimitWithDbSize(dbCacheLimit.longValue());
            } else {
                sdkConfig.enableLimitWithDbSize();
            }
        }
        if (dbDiscardStrategy != null) {
            if (dbDiscardStrategy.equals("discardOldest")) {
                sdkConfig.setDbCacheDiscard(DBCacheDiscard.DISCARD_OLDEST);
            } else if (dbDiscardStrategy.equals("discard")) {
                sdkConfig.setDbCacheDiscard(DBCacheDiscard.DISCARD);
            }
        }

        if (dataModifier != null) {
            sdkConfig.setDataModifier(new DataModifier() {
                @Override
                public Object modify(String key, Object value) {
                    return dataModifier.opt(key);
                }
            });
        }
        if (lineDataModifier != null) {
            sdkConfig.setLineDataModifier(new LineDataModifier() {
                @Override
                public Map<String, Object> modify(String measurement, HashMap<String, Object> data) {
                    if (measurement.equals(FT_LOG_DEFAULT_MEASUREMENT)) {
                        return convertJSONtoHashMap(lineDataModifier.optJSONObject("log"));
                    } else {
                        return convertJSONtoHashMap(lineDataModifier.optJSONObject(measurement));
                    }
                }
            });
        }
        FTSdk.install(sdkConfig);
    }

    /**
     * SDK release
     *
     * @param data
     */
    private static void deInit(JSONObject data) {
        FTSdk.shutDown();
    }

    /**
     * Bind user data
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
     * Release user data
     *
     * @param data
     */
    private static void unBindUserdata(JSONObject data) {
        FTSdk.unbindRumUserData();
    }

    /**
     * Initialize RUM
     *
     * @param data
     */
    private static void initRUMConfig(JSONObject data) {
        Map<String, Object> map = convertJSONtoHashMap(data);
        JSONObject globalContextJson = (JSONObject) map.get("globalContext");
        String rumAppId = (String) map.get("androidAppId");
        Number sampleRate = (Number) map.get("sampleRate");
        Number sessionOnErrorSampleRate = (Number) map.get("sessionOnErrorSampleRate");
        Boolean enableNativeUserAction = (Boolean) map.get("enableNativeUserAction");
        Boolean enableNativeUserView = (Boolean) map.get("enableNativeUserView");
        Boolean enableNativeUserResource = (Boolean) map.get("enableNativeUserResource");
        Boolean enableResourceHostIP = (Boolean) map.get("enableResourceHostIP");
        Boolean enableTrackNativeCrash = (Boolean) map.get("enableTrackNativeCrash");
        Boolean enableTrackNativeAppANR = (Boolean) map.get("enableTrackNativeAppANR");
        Boolean enableTrackNativeFreeze = (Boolean) map.get("enableTrackNativeFreeze");
        Number nativeFreezeDurationMs = (Number) map.get("nativeFreezeDurationMs");
        Object errorType = map.get("extraMonitorTypeWithError");
        Object deviceType = map.get("deviceMonitorType");
        Object detectFrequencyStr = map.get("detectFrequency");
        Number rumCacheLimitCount = (Number) map.get("rumCacheLimitCount");
        Object rumDiscardStrategy = map.get("rumDiscardStrategy");
        FTRUMConfig rumConfig = new FTRUMConfig().setRumAppId(rumAppId);
        if (sampleRate != null) {
            rumConfig.setSamplingRate(sampleRate.floatValue());
        }
        if (sessionOnErrorSampleRate != null) {
            rumConfig.setSessionErrorSampleRate(sessionOnErrorSampleRate.floatValue());
        }
        if (enableNativeUserAction != null) {
            rumConfig.setEnableTraceUserAction(enableNativeUserAction);
        }
        if (enableNativeUserView != null) {
            rumConfig.setEnableTraceUserView(enableNativeUserView);
        }
        if (enableNativeUserResource != null) {
            rumConfig.setEnableTraceUserResource(enableNativeUserResource);
        }
        if (enableResourceHostIP != null) {
            rumConfig.setEnableResourceHostIP(enableResourceHostIP);
        }
        if (enableTrackNativeCrash != null) {
            rumConfig.setEnableTrackAppCrash(enableTrackNativeCrash);
        }
        if (enableTrackNativeFreeze != null) {
            if (nativeFreezeDurationMs != null) {
                rumConfig.setEnableTrackAppUIBlock(enableTrackNativeFreeze, nativeFreezeDurationMs.longValue());
            } else {
                rumConfig.setEnableTrackAppUIBlock(enableTrackNativeFreeze);
            }
        }
        if (enableTrackNativeAppANR != null) {
            rumConfig.setEnableTrackAppANR(enableTrackNativeAppANR);
        }
        if (errorType != null) {
            int errorMonitorType = ErrorMonitorType.NO_SET;
            if (errorType instanceof String) {
                if (errorType.equals("all")) {
                    errorMonitorType = ErrorMonitorType.ALL.getValue();
                } else if (errorType.equals("battery")) {
                    errorMonitorType = ErrorMonitorType.BATTERY.getValue();
                } else if (errorType.equals("memory")) {
                    errorMonitorType = ErrorMonitorType.MEMORY.getValue();
                } else if (errorType.equals("cpu")) {
                    errorMonitorType = ErrorMonitorType.CPU.getValue();
                }
            } else if (errorType instanceof JSONArray) {
                JSONArray errorTypeArr = (JSONArray) errorType;
                for (int i = 0; i < errorTypeArr.length(); i++) {
                    String errorTypeStr = errorTypeArr.optString(i);
                    if (errorTypeStr.equals("all")) {
                        errorMonitorType |= ErrorMonitorType.ALL.getValue();
                    } else if (errorTypeStr.equals("battery")) {
                        errorMonitorType |= ErrorMonitorType.BATTERY.getValue();
                    } else if (errorTypeStr.equals("memory")) {
                        errorMonitorType |= ErrorMonitorType.MEMORY.getValue();
                    } else if (errorTypeStr.equals("cpu")) {
                        errorMonitorType |= ErrorMonitorType.CPU.getValue();
                    }
                }
            }
            rumConfig.setExtraMonitorTypeWithError(errorMonitorType);
        }
        if (deviceType != null) {

            DetectFrequency detectFrequency = DetectFrequency.DEFAULT;
            if (detectFrequencyStr != null) {
//                if (detectFrequencyStr.equals("normal")) {
//                    detectFrequency = DetectFrequency.DEFAULT;
//                } else
                if (detectFrequencyStr.equals("frequent")) {
                    detectFrequency = DetectFrequency.FREQUENT;
                } else if (detectFrequencyStr.equals("rare")) {
                    detectFrequency = DetectFrequency.RARE;
                }
            }

            int deviceMonitorType = DeviceMetricsMonitorType.NO_SET;
            if (deviceType instanceof String) {
                if (deviceType.equals("all")) {
                    deviceMonitorType = DeviceMetricsMonitorType.ALL.getValue();
                } else if (deviceType.equals("battery")) {
                    deviceMonitorType = DeviceMetricsMonitorType.BATTERY.getValue();
                } else if (deviceType.equals("memory")) {
                    deviceMonitorType = DeviceMetricsMonitorType.MEMORY.getValue();
                } else if (deviceType.equals("cpu")) {
                    deviceMonitorType = DeviceMetricsMonitorType.CPU.getValue();
                } else if (deviceType.equals("fps")) {
                    deviceMonitorType = DeviceMetricsMonitorType.FPS.getValue();
                }
            } else if (deviceType instanceof JSONArray) {
                JSONArray deviceTypeArr = (JSONArray) deviceType;
                for (int i = 0; i < deviceTypeArr.length(); i++) {
                    String deviceTypeStr = deviceTypeArr.optString(i);
                    if (deviceTypeStr.equals("all")) {
                        deviceMonitorType |= DeviceMetricsMonitorType.ALL.getValue();
                    } else if (deviceTypeStr.equals("battery")) {
                        deviceMonitorType |= DeviceMetricsMonitorType.BATTERY.getValue();
                    } else if (deviceTypeStr.equals("memory")) {
                        deviceMonitorType |= DeviceMetricsMonitorType.MEMORY.getValue();
                    } else if (deviceTypeStr.equals("cpu")) {
                        deviceMonitorType |= DeviceMetricsMonitorType.CPU.getValue();
                    } else if (deviceTypeStr.equals("fps")) {
                        deviceMonitorType |= DeviceMetricsMonitorType.FPS.getValue();
                    }
                }
            }
            rumConfig.setDeviceMetricsMonitorType(deviceMonitorType, detectFrequency);
        }

        if (globalContextJson != null) {
            Map<String, Object> globalContext = convertJSONtoHashMap(globalContextJson);
            for (Map.Entry<String, Object> entry : globalContext.entrySet()) {
                rumConfig.addGlobalContext(entry.getKey(), entry.getValue().toString());
            }
        }
        if (rumCacheLimitCount != null) {
            rumConfig.setRumCacheLimitCount(rumCacheLimitCount.intValue());
        }
        if (rumDiscardStrategy != null) {
            if (rumDiscardStrategy.equals("discardOldest")) {
                rumConfig.setRumCacheDiscardStrategy(RUMCacheDiscard.DISCARD_OLDEST);
            } else if (rumDiscardStrategy.equals("discard")) {
                rumConfig.setRumCacheDiscardStrategy(RUMCacheDiscard.DISCARD);
            }
        }
        FTSdk.initRUMWithConfig(rumConfig);
    }

    /**
     * Create View
     *
     * @param data
     */
    private static void createView(JSONObject data) {
        String viewName = data.optString("viewName");
        long loadTime = data.optLong("loadTime");
        FTRUMGlobalManager.get().onCreateView(viewName, loadTime);
    }

    /**
     * View start
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
     * View end
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
     * Add Action
     */
    private static void addAction(JSONObject data) {
        String actionName = data.optString("actionName");
        String actionType = data.optString("actionType");
        JSONObject property = data.optJSONObject("property");
        HashMap<String, Object> params = convertJSONtoHashMap(property);
        long duration = data.optLong("duration");
        FTRUMGlobalManager.get().addAction(actionName, actionType, duration, params);
    }

    /**
     * Action start
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
     * Resource start
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
     * Resource end
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
     * Add Resource data
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

//        JSONObject netStatus = data.optJSONObject("netStatus");
        NetStatusBean netStatusBean = new NetStatusBean();
//        if (netStatus != null) {
//            netStatusBean.callStartTime = netStatus.optLong("callStartTime");
//            netStatusBean.dnsStartTime = netStatus.optLong("dnsStartTime");
//            netStatusBean.dnsEndTime = netStatus.optLong("dnsEndTime");
//            netStatusBean.headerStartTime = netStatus.optLong("headerStartTime");
//            netStatusBean.headerEndTime = netStatus.optLong("headerEndTime");
//            netStatusBean.bodyStartTime = netStatus.optLong("bodyStartTime");
//            netStatusBean.bodyEndTime = netStatus.optLong("bodyEndTime");
//            netStatusBean.sslStartTime = netStatus.optLong("sslStartTime");
//            netStatusBean.sslEndTime = netStatus.optLong("sslEndTime");
//            netStatusBean.tcpStartTime = netStatus.optLong("tcpStartTime");
//            netStatusBean.tcpEndTime = netStatus.optLong("tcpEndTime");
//        }

        FTRUMGlobalManager.get().addResource(key, params, netStatusBean);
    }

    /**
     * Add Error data
     *
     * @param data
     */
    private static void addError(JSONObject data) {
        String message = data.optString("message");
        String stack = data.optString("log");
        String errorType = data.optString("errorType");
        String state = data.optString("state");
        JSONObject property = data.optJSONObject("property");
        HashMap<String, Object> params = convertJSONtoHashMap(property);
        AppState appState = AppState.UNKNOWN;
        if (state != null) {
            appState = AppState.getValueFrom(state);
        }
        FTRUMGlobalManager.get().addError(stack, message, errorType, appState, params);
    }

    /**
     * Add LongTask data
     *
     * @param data
     */
    private static void addLongTask(JSONObject data) {
        String message = data.optString("log");
        long duration = data.optLong("duration");
        FTRUMGlobalManager.get().addLongTask(message, duration);
    }


    /**
     * Initialize Log configuration
     *
     * @param data
     */
    private static void initLogConfig(JSONObject data) {
        Map<String, Object> map = convertJSONtoHashMap(data);
        JSONObject globalContextJson = (JSONObject) map.get("globalContext");
        String discardStrategy = (String) (map.get("discardStrategy"));
        Number sampleRate = (Number) map.get("sampleRate");
        JSONArray logTypeReadArr = (JSONArray) map.get("logLevelFilters");
        Boolean enableLinkRumData = (Boolean) map.get("enableLinkRumData");
        Boolean enableCustomLog = (Boolean) map.get("enableCustomLog");
        Number logCacheLimitCount = (Number) map.get("logCacheLimitCount");

        FTLoggerConfig logConfig = new FTLoggerConfig();

        if (enableCustomLog != null) {
            logConfig.setEnableCustomLog(enableCustomLog);
        }
        if (sampleRate != null) {
            logConfig.setSamplingRate(sampleRate.floatValue());
        }

        if (discardStrategy != null) {
            if (discardStrategy.equals("discardOldest")) {
                logConfig.setLogCacheDiscardStrategy(LogCacheDiscard.DISCARD_OLDEST);
            } else if (discardStrategy.equals("discard")) {
                logConfig.setLogCacheDiscardStrategy(LogCacheDiscard.DISCARD);
            }
        }

        if (logTypeReadArr != null) {
            Status[] statuses = new Status[logTypeReadArr.length()];
            for (int i = 0; i < logTypeReadArr.length(); i++) {
                Status logStatus = null;
                for (Status value : Status.values()) {
                    if (value.name.equals(logTypeReadArr.optString(i))) {
                        logStatus = value;
                        break;
                    }
                }
                statuses[i] = logStatus;
            }

            logConfig.setLogLevelFilters(statuses);

        }

        if (enableLinkRumData != null) {
            logConfig.setEnableLinkRumData(enableLinkRumData);
        }

        if (enableCustomLog != null) {
            logConfig.setEnableCustomLog(enableCustomLog);
        }

        if (globalContextJson != null) {
            Map<String, Object> globalContext = convertJSONtoHashMap(globalContextJson);
            for (Map.Entry<String, Object> entry : globalContext.entrySet()) {
                logConfig.addGlobalContext(entry.getKey(), entry.getValue().toString());
            }
        }

        if (logCacheLimitCount != null) {
            logConfig.setLogCacheLimitCount(logCacheLimitCount.intValue());
        }

        FTSdk.initLogWithConfig(logConfig);
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
     * Initialize Trace configuration
     *
     * @param data
     */
    private static void initTraceConfig(JSONObject data) {
        Map<String, Object> map = convertJSONtoHashMap(data);
        Number sampleRate = (Number) map.get("sampleRate");
        Object traceType = map.get("traceType");
        Boolean enableLinkRUMData = (Boolean) map.get("enableLinkRUMData");
        Boolean enableNativeAutoTrace = (Boolean) map.get("enableNativeAutoTrace");

        FTTraceConfig traceConfig = new FTTraceConfig();
        if (sampleRate != null) {
            traceConfig.setSamplingRate(sampleRate.floatValue());
        }

        if (traceType != null) {
            if (traceType.equals("ddTrace")) {
                traceConfig.setTraceType(TraceType.DDTRACE);
            } else if (traceType.equals("zipkinMultiHeader")) {
                traceConfig.setTraceType(TraceType.ZIPKIN_MULTI_HEADER);
            } else if (traceType.equals("zipkinSingleHeader")) {
                traceConfig.setTraceType(TraceType.ZIPKIN_SINGLE_HEADER);
            } else if (traceType.equals("traceparent")) {
                traceConfig.setTraceType(TraceType.TRACEPARENT);
            } else if (traceType.equals("skywalking")) {
                traceConfig.setTraceType(TraceType.SKYWALKING);
            } else if (traceType.equals("jaeger")) {
                traceConfig.setTraceType(TraceType.JAEGER);
            }
        }

        if (enableLinkRUMData != null) {
            traceConfig.setEnableLinkRUMData(enableLinkRUMData);
        }

        if (enableNativeAutoTrace != null) {
            traceConfig.setEnableAutoTrace(enableNativeAutoTrace);
        }

        FTSdk.initTraceWithConfig(traceConfig);
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
     * Dynamically set global tag
     */
    private static void appendGlobalContext(JSONObject extra) {
        if (extra != null) {
            FTSdk.appendGlobalContext(convertJSONtoHashMap(extra));
        }
    }


    /**
     * Dynamically set log global tag
     */
    private static void appendLogGlobalContext(JSONObject extra) {
        if (extra != null) {
            FTSdk.appendLogGlobalContext(convertJSONtoHashMap(extra));
        }
    }

    /**
     * Dynamically set RUM global tag
     */
    private static void appendRUMGlobalContext(JSONObject extra) {
        if (extra != null) {
            FTSdk.appendRUMGlobalContext(convertJSONtoHashMap(extra));
        }
    }

    /**
     * Perform cache data synchronization
     */
    private static void flushSyncData() {
        FTSdk.flushSyncData();

    }

    /**
     * Clear SDK data
     */
    private static void clearAllData() {
        FTSdk.clearAllData();
    }

    /**
     * Convert JSON to hashMap
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
