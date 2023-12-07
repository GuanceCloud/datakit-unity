//
//  FTUnityBridge.m
//  FTUnityBridge
//
//  Created by hulilei on 2023/9/4.
//

#import <Foundation/Foundation.h>
#import <FTMobileSDK/FTMobileAgent.h>

/// c 字符串 转换 oc 字符串
/// - Parameter string: c 字符串
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}
/// 字符串拷贝
/// - Parameter string: 字符串
char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}
NSDictionary *RemoveNull(NSDictionary *dict){
    NSMutableDictionary *mdic = [NSMutableDictionary dictionary];
    for (NSString *strKey in dict.allKeys) {
        NSValue *value = dict[strKey];
        // 删除NSDictionary中的NSNull，再保存进字典
        if (![value isKindOfClass:NSNull.class]) {
            [mdic setValue:value forKey:strKey];
        }
    }
    return mdic;
}
/// json 字符串转字典
/// - Parameter jsonString: json 字符串
NSDictionary* JsonStringToDict(const char* jsonString){
    if (jsonString){
        NSData *jsonData = [[NSString stringWithUTF8String: jsonString] dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingMutableContainers error:nil];
        return RemoveNull(dictionary);
    }
    return nil;
}

extern "C"{

#pragma mark ========== SDK INIT/DeInit ==========
/// SDK 初始化配置
/// - Parameter json: 配置项
/// serverUrl：datakit 安装地址 URL 地址
/// env：数据上传环境，默认 prod
/// serviceName：应用服务名，默认 android df_rum_android, iOS df_rum_ios
/// debug: 是否开启 Debug 模式
/// globalContext: 自定义全局参数
void install(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *serverUrl = [configDict objectForKey:@"serverUrl"];
    NSString *envType = [configDict objectForKey:@"env"];
    NSString *serviceName = [configDict objectForKey:@"serviceName"];
    NSNumber *debug = [configDict objectForKey:@"debug"];
    NSDictionary *globalContext = [configDict objectForKey:@"globalContext"];
    
    FTMobileConfig *config = [[FTMobileConfig alloc]initWithMetricsUrl:serverUrl];
    config.env = envType;
    config.service = serviceName;
    config.enableSDKDebugLog = [debug boolValue];
    config.globalContext = globalContext;
    [FTMobileAgent startWithConfigOptions:config];
}
/// SDK 关闭
void deInit(){
    [[FTMobileAgent sharedInstance] shutDown];
}
#pragma mark ========== Bind/Unbind User ==========

/// 绑定用户
/// - Parameter json: 参数
/// userId:
/// userName:
/// userEmail:
/// extra:
void bindUserData(const char* json){
    NSDictionary *user = JsonStringToDict(json);
    if(user == nil){
        return;
    }
    NSString *userId = [user objectForKey:@"userId"];
    NSString *userName = [user objectForKey:@"userName"];
    NSString *userEmail = [user objectForKey:@"userEmail"];
    NSDictionary *extra = [user objectForKey:@"extra"];
    [[FTMobileAgent sharedInstance] bindUserWithUserID:userId userName:userName userEmail:userEmail extra:extra];
}
/// 解绑用户
void unbindUserdata(){
    [[FTMobileAgent sharedInstance] unbindUser];
}

#pragma mark ========== RUM ==========
/// 初始化 RUM 配置
/// - Parameter rumConfigJson: 配置项
/// iOSAppId：appId
/// sampleRate：采样率
/// enableNativeUserResource：是否进行 `Native Resource` 自动追踪
/// enableNativeUserAction：是否进行 `Native Action` 追踪，包括冷热启动
/// enableNativeUserView：是否进行 `Native View` 自动追踪
/// extraMonitorTypeWithError：错误监控补充类型
/// deviceMonitorType: 页面监控补充类型
/// detectFrequency: 页面监控频率
/// globalContext: 自定义 RUM 全局参数
void initRUMConfig(const char* rumConfigJson){
    NSDictionary *configDict = JsonStringToDict(rumConfigJson);
    if(configDict == nil){
        return;
    }
    NSString *rumAppId = [configDict objectForKey:@"iOSAppId"];
    NSNumber *sampleRate = [configDict objectForKey:@"sampleRate"];
    NSNumber *enableTraceUserResource = [configDict objectForKey:@"enableNativeUserResource"];
    NSNumber *enableTraceUserAction = [configDict objectForKey:@"enableNativeUserAction"];
    NSNumber *enableNativeUserView = [configDict objectForKey:@"enableNativeUserView"];
    NSNumber *extraMonitorTypeWithError = [configDict objectForKey:@"extraMonitorTypeWithError"];
    NSNumber *deviceMonitorType = [configDict objectForKey:@"deviceMonitorType"];
    NSString *detectFrequency = [configDict objectForKey:@"detectFrequency"];
    NSDictionary *globalContext = [configDict objectForKey:@"globalContext"];
    FTRumConfig *rumConfig = [[FTRumConfig alloc]initWithAppid:rumAppId];
    if(sampleRate){
        rumConfig.samplerate = [sampleRate floatValue] * 100;
    }
    if(extraMonitorTypeWithError){
        rumConfig.errorMonitorType = (FTErrorMonitorType)[extraMonitorTypeWithError intValue];
    }
    if(deviceMonitorType){
        rumConfig.deviceMetricsMonitorType = (FTDeviceMetricsMonitorType)[deviceMonitorType intValue];
    }
    if(detectFrequency){
        if ([detectFrequency isEqualToString:@"normal"]) {
            rumConfig.monitorFrequency = FTMonitorFrequencyDefault;
        }else if ([detectFrequency isEqualToString:@"frequent"]){
            rumConfig.monitorFrequency = FTMonitorFrequencyFrequent;
        }else if ([detectFrequency isEqualToString:@"rare"]){
            rumConfig.monitorFrequency = FTMonitorFrequencyRare;
        }
    }
    rumConfig.enableTraceUserResource = [enableTraceUserResource boolValue];
    rumConfig.enableTraceUserAction = [enableTraceUserAction boolValue];
    rumConfig.enableTraceUserView = [enableNativeUserView boolValue];
    rumConfig.globalContext = globalContext;
    [[FTMobileAgent sharedInstance] startRumWithConfigOptions:rumConfig];
}
/// 添加 Action 事件
/// - Parameter json: 参数
/// actionName: 事件名称
/// actionType: 事件类型
/// property: 事件上下文(可选)
void startAction(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *actionName = [configDict objectForKey:@"actionName"];
    NSString *actionType = [configDict objectForKey:@"actionType"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    if(actionName && actionType){
        [FTExternalDataManager.sharedManager addActionName:actionName actionType:actionType property:property];
    }
}
/// 创建页面
/// - Parameter json: 参数
/// viewName：页面名称
/// loadTime: 加载时间，纳秒
void createView(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *viewName = [configDict objectForKey:@"viewName"];
    NSNumber *loadTime = [configDict objectForKey:@"loadTime"];
    [FTExternalDataManager.sharedManager onCreateView:viewName loadTime:loadTime];
}

/// 进入页面
/// - Parameter json: 参数
/// viewName：页面名称
/// property：事件上下文(可选)
void startView(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *viewName = [configDict objectForKey:@"viewName"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    [FTExternalDataManager.sharedManager startViewWithName:viewName property:property];
}

/// 离开页面
/// - Parameter json: 参数
/// property：事件上下文(可选)
void stopView(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict != nil){
        NSDictionary *property = [configDict objectForKey:@"property"];
        [FTExternalDataManager.sharedManager stopViewWithProperty:property];
    }else{
        [FTExternalDataManager.sharedManager stopView];
    }
}

/// HTTP 请求开始
/// - Parameter json: 参数
/// resourceId: 请求唯一标识
/// property: 事件上下文(可选)
void startResource(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *resourceId = [configDict objectForKey:@"resourceId"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    [FTExternalDataManager.sharedManager startResourceWithKey:resourceId property:property];
}

/// HTTP 请求结束
/// - Parameter json: 参数
/// resourceId: 请求唯一标识
/// property: 事件上下文(可选)
void stopResource(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *resourceId = [configDict objectForKey:@"resourceId"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    [FTExternalDataManager.sharedManager stopResourceWithKey:resourceId property:property];
}
/// 添加网络传输内容
/// - Parameter json: 参数
/// resourceId: 请求唯一标识
/// params：请求性能数据
///       * url
///       * requestHeader
///       * responseHeader
///       * resourceMethod
///       * responseBody
///       * resourceStatus
/// netStatusBean：请求相关数据
///       * tcpTime: tcpEndTime-tcpStartTime
///       * dnsTime: dnsEndTime-dnsStartTime
///       * sslTime: sslEndTime-sslStartTime
///       * ttfbTime: responseStartTime - requestStartTime
///       * transTime: responseEndTime - responseStartTime
///       * duration: responseEndTime - fetchStartTime
void addResource(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *resourceId = [configDict objectForKey:@"resourceId"];
    NSDictionary *params = [configDict objectForKey:@"resourceParams"];
    NSDictionary *netStatus = [configDict objectForKey:@"netStatus"];
    
    FTResourceContentModel *content = [[FTResourceContentModel alloc]init];
    id requestHeader =  [params objectForKey:@"requestHeader"];
    if([requestHeader isKindOfClass:NSDictionary.class]){
        content.requestHeader = requestHeader;
    }
    id responseHeader =  [params objectForKey:@"responseHeader"];
    if([responseHeader isKindOfClass:[NSDictionary class]]){
        content.responseHeader = responseHeader;
    }
    content.httpMethod = [params objectForKey:@"resourceMethod"];
    content.responseBody = [params objectForKey:@"responseBody"];
    content.url = [NSURL URLWithString:[params objectForKey:@"url"]];
    content.httpStatusCode = [[params objectForKey:@"resourceStatus"] integerValue];
    FTResourceMetricsModel *metrics = [[FTResourceMetricsModel alloc]init];
    if(netStatus){
        long tcpStartTime = [[netStatus objectForKey:@"tcpStartTime"] longValue];
        long tcpEndTime = [[netStatus objectForKey:@"tcpEndTime"] longValue];
        NSNumber  *tcpTime = @(tcpEndTime - tcpStartTime);
        
        long dnsEndTime = [[netStatus objectForKey:@"dnsEndTime"] longValue];
        long dnsStartTime = [[netStatus objectForKey:@"dnsStartTime"] longValue];
        NSNumber * dnsTime = @(dnsEndTime - dnsStartTime);
        
        long sslEndTime = [[netStatus objectForKey:@"sslEndTime"] longValue];
        long sslStartTime = [[netStatus objectForKey:@"sslStartTime"] longValue];
        NSNumber * sslTime = @(sslEndTime - sslStartTime);
        
        long responseEndTime = [[netStatus objectForKey:@"responseEndTime"] longValue];
        long responseStartTime = [[netStatus objectForKey:@"responseStartTime"] longValue];
        long requestStartTime = [[netStatus objectForKey:@"requestStartTime"] longValue];
        NSNumber * ttfb = @(responseStartTime - requestStartTime);
        NSNumber *transTime = @(responseEndTime - responseStartTime);
        
        long fetchStartTime = [[netStatus objectForKey:@"fetchStartTime"] longValue];
        NSNumber * duration = @(responseEndTime - fetchStartTime);
        
        metrics.resource_tcp = tcpTime;
        metrics.resource_dns = dnsTime;
        metrics.resource_ssl = sslTime;
        metrics.resource_trans = transTime;
        metrics.resource_ttfb = ttfb;
        metrics.duration = duration;
    }
    [FTExternalDataManager.sharedManager addResourceWithKey:resourceId metrics:metrics content:content];
}

/// 添加错误事件
/// - Parameter json: 参数
/// log：错误日志
/// message：错误信息
/// errorType：错误类型
/// state：程序运行状态（run、startup、unknown）
/// property: 事件上下文(可选)
void addError(const char*json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *log = [configDict objectForKey:@"log"];
    NSString *message = [configDict objectForKey:@"message"];
    NSString *errorType = [configDict objectForKey:@"errorType"];
    NSString *state = [configDict objectForKey:@"state"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    if(state){
        FTAppState appState;
        if([state isEqualToString:@"run"]){
            appState = FTAppStateRun;
        }else if([state isEqualToString:@"startup"]){
            appState = FTAppStateStartUp;
        }else{
            appState = FTAppStateUnknown;
        }
        [FTExternalDataManager.sharedManager addErrorWithType:errorType state:appState  message:message stack:log property:property];
    }else{
        [FTExternalDataManager.sharedManager addErrorWithType:errorType message:message stack:log property:property];
    }
}
/// 添加 longtask
/// - Parameter json: 参数
/// log：卡顿日志
/// duration：卡顿时长
/// property: 事件上下文(可选)
void addLongTask(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *log = [configDict objectForKey:@"log"];
    NSNumber *duration = [configDict objectForKey:@"duration"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    [FTExternalDataManager.sharedManager addLongTaskWithStack:log duration:duration property:property];
}
#pragma mark ========== Log ==========
/// 初始化 Log
/// - Parameter logConfigJson: 配置项
/// sampleRate：采样率
/// enableCustomLog：是否开启自定义日志
/// enableLinkRumData：是否与 RUM 关联
/// logLevelFilters：日志等级过滤
/// discardStrategy：日志丢弃策略
/// globalContext：自定义日志全局参数
void initLogConfig(const char* logConfigJson){
    NSDictionary *configDict = JsonStringToDict(logConfigJson);
    if(configDict == nil){
        return;
    }
    BOOL enableCustomLog = [[configDict objectForKey:@"enableCustomLog"] boolValue];
    BOOL enableLinkRumData = [[configDict objectForKey:@"enableLinkRumData"] boolValue];
    NSNumber *sampleRate = [configDict objectForKey:@"sampleRate"];
    NSArray *logLevelFilters = [configDict objectForKey:@"logLevelFilters"];
    NSString *discardStrategy = [configDict objectForKey:@"discardStrategy"];
    NSDictionary *globalContext = [configDict objectForKey:@"globalContext"];
    FTLoggerConfig *loggerConfig = [[FTLoggerConfig alloc]init];
    if(sampleRate){
        loggerConfig.samplerate = [sampleRate floatValue] * 100;
    }
    if(discardStrategy){
        if([discardStrategy isEqualToString:@"discard"]){
            loggerConfig.discardType = FTDiscard;
        }else if ([discardStrategy isEqualToString:@"discardOldest"]){
            loggerConfig.discardType = FTDiscardOldest;
        }
    }
    if(logLevelFilters&&logLevelFilters.count>0){
        NSMutableArray *logLevels = [NSMutableArray new];
        for (NSString *level in logLevelFilters) {
            if([level isEqualToString:@"info"]){
                [logLevels addObject:@(FTStatusInfo)];
            }else if ([level isEqualToString:@"warning"]){
                [logLevels addObject:@(FTStatusWarning)];
            }else if ([level isEqualToString:@"error"]){
                [logLevels addObject:@(FTStatusError)];
            }else if ([level isEqualToString:@"critical"]){
                [logLevels addObject:@(FTStatusCritical)];
            }else if ([level isEqualToString:@"ok"]){
                [logLevels addObject:@(FTStatusOk)];
            }
        }
        loggerConfig.logLevelFilter = logLevels;
    }
    loggerConfig.enableCustomLog = enableCustomLog;
    loggerConfig.enableLinkRumData = enableLinkRumData;
    loggerConfig.globalContext = globalContext;
    [[FTMobileAgent sharedInstance] startLoggerWithConfigOptions:loggerConfig];
}

/// 日志打印
/// - Parameter json: 参数
/// log：日志内容
/// level：日志等级
/// property：事件上下文(可选)
void addLog(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *status = [configDict objectForKey:@"level"];
    NSString *content = [configDict objectForKey:@"log"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    if([status isEqualToString:@"info"]){
        [[FTLogger sharedInstance] info:content property:property];
    }else if ([status isEqualToString:@"warning"]){
        [[FTLogger sharedInstance] warning:content property:property];
    }else if ([status isEqualToString:@"error"]){
        [[FTLogger sharedInstance] error:content property:property];
    }else if ([status isEqualToString:@"critical"]){
        [[FTLogger sharedInstance] critical:content property:property];
    }else if ([status isEqualToString:@"ok"]){
        [[FTLogger sharedInstance] ok:content property:property];
    }else{
        [[FTLogger sharedInstance] info:content property:property];
    }
}
#pragma mark ========== Trace ==========

/// 初始化 Trace
/// - Parameter traceConfigJson: 配置项
/// sampleRate：采样率
/// traceType：链路类型
/// enableLinkRUMData：是否与 `RUM` 数据关联
/// enableAutoTrace：是否开启原生网络自动追踪
void initTraceConfig(const char* traceConfigJson){
    NSDictionary *configDict = JsonStringToDict(traceConfigJson);
    if(configDict == nil){
        return;
    }
    NSString *traceType = [configDict objectForKey:@"traceType"];
    BOOL enableLinkRumData = [[configDict objectForKey:@"enableLinkRumData"] boolValue];
    NSNumber *sampleRate = [configDict objectForKey:@"sampleRate"];
    NSNumber *enableAutoTrace = [configDict objectForKey:@"enableAutoTrace"];
    FTTraceConfig *tracrConfig = [[FTTraceConfig alloc]init];
    if(traceType!=nil && traceType.length>0){
        if([traceType isEqualToString:@"ddtrace"]){
            tracrConfig.networkTraceType = FTNetworkTraceTypeDDtrace;
        }else if ([traceType isEqualToString:@"zipkinMulti"]){
            tracrConfig.networkTraceType = FTNetworkTraceTypeZipkinMultiHeader;
        }else if ([traceType isEqualToString:@"zipkinSingle"]){
            tracrConfig.networkTraceType = FTNetworkTraceTypeZipkinSingleHeader;
        }else if ([traceType isEqualToString:@"jaeger"]){
            tracrConfig.networkTraceType = FTNetworkTraceTypeJaeger;
        }else if ([traceType isEqualToString:@"skywalking"]){
            tracrConfig.networkTraceType = FTNetworkTraceTypeSkywalking;
        }else if ([traceType isEqualToString:@"traceParent"]){
            tracrConfig.networkTraceType = FTNetworkTraceTypeTraceparent;
        }
    }
    if(sampleRate){
        tracrConfig.samplerate = [sampleRate floatValue] * 100;
    }
    tracrConfig.enableAutoTrace = [enableAutoTrace boolValue];
    tracrConfig.enableLinkRumData = enableLinkRumData;
    [[FTMobileAgent sharedInstance] startTraceWithConfigOptions:tracrConfig];
}
/// 获取 trace 需要添加的请求头
/// - Parameter json: 参数
/// resourceId：请求唯一标识
/// url：请求 URL
const char* getTraceHeader(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return nil;
    }
    NSString *resourceId = [configDict objectForKey:@"resourceId"];
    NSString *url = [configDict objectForKey:@"url"];
    NSDictionary *traceHeader = [[FTExternalDataManager sharedManager] getTraceHeaderWithKey:resourceId url: [NSURL URLWithString:url]];
    if(traceHeader){
        NSError *error;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:traceHeader options:NSJSONWritingPrettyPrinted error:&error];
        if (!error) {
            NSString *headerStr = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
            return MakeStringCopy([headerStr UTF8String]);
        }
    }
    return nil;
}

const char* invokeMethod(const char* method,const char* json){
    NSString *methodStr = CreateNSString(method);
    if([methodStr isEqualToString:@"Install"]){
        install(json);
    }else if([methodStr isEqualToString:@"DeInit"]){
        deInit();
    }else if([methodStr isEqualToString:@"BindUserData"]){
        bindUserData(json);
    }else if([methodStr isEqualToString:@"UnbindUserdata"]){
        unbindUserdata();
    }else if([methodStr isEqualToString:@"InitRUMConfig"]){
        initRUMConfig(json);
    }else if ([methodStr isEqualToString:@"StartAction"]){
        startAction(json);
    }else if ([methodStr isEqualToString:@"CreateView"]){
        createView(json);
    }else if([methodStr isEqualToString:@"StartView"]){
        startView(json);
    }else if ([methodStr isEqualToString:@"StopView"]){
        stopView(json);
    }else if ([methodStr isEqualToString:@"StartResource"]){
        startResource(json);
    }else if ([methodStr isEqualToString:@"StopResource"]){
        stopResource(json);
    }else if ([methodStr isEqualToString:@"AddResource"]){
        addResource(json);
    }else if ([methodStr isEqualToString:@"AddError"]){
        addError(json);
    }else if ([methodStr isEqualToString:@"AddLongTask"]){
        addLongTask(json);
    }else if ([methodStr isEqualToString:@"InitLogConfig"]){
        initLogConfig(json);
    }else if ([methodStr isEqualToString:@"AddLog"]){
        addLog(json);
    }else if ([methodStr isEqualToString:@"InitTraceConfig"]){
        initTraceConfig(json);
    }else if ([methodStr isEqualToString:@"GetTraceHeader"]){
        return getTraceHeader(json);
    }
    return nil;
}
}

