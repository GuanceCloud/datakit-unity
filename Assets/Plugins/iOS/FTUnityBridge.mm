//
//  FTUnityBridge.m
//  FTUnityBridge
//
//  Created by hulilei on 2023/9/4.
//

#import <Foundation/Foundation.h>
#import <FTMobileSDK/FTMobileAgent.h>
#import <FTMobileSDK/FTMobileConfig+Private.h>
#import <FTMobileSDK/FTConstants.h>

/// Convert c string to oc string
/// - Parameter string: c string
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}
/// String copy
/// - Parameter string: string
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
        // Remove NSNull from NSDictionary, then save to dictionary
        if (![value isKindOfClass:NSNull.class]) {
            [mdic setValue:value forKey:strKey];
        }
    }
    return mdic;
}
/// Convert json string to dictionary
/// - Parameter jsonString: json string
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
/// SDK initialization configuration
/// - Parameter json: configuration items
/// serverUrl: datakit installation address URL
/// env: data upload environment, default prod
/// serviceName: application service name, default android df_rum_android, iOS df_rum_ios
/// debug: whether to enable Debug mode
/// globalContext: custom global parameters
void install(const char* json){
    NSDictionary *params = JsonStringToDict(json);
    if(params == nil){
        return;
    }
    NSString *serverUrl = [params valueForKey:@"serverUrl"];
    NSString *datakitUrl = [params valueForKey:@"datakitUrl"];
    datakitUrl = datakitUrl ?:serverUrl;
    NSString *dataWayUrl = [params valueForKey:@"datawayUrl"];
    NSString *clientToken = [params valueForKey:@"clientToken"];
    FTMobileConfig *config;
    if(dataWayUrl && dataWayUrl.length>0 && clientToken && clientToken.length>0){
        config = [[FTMobileConfig alloc]initWithDatawayUrl:dataWayUrl clientToken:clientToken];
    }else if(datakitUrl && datakitUrl.length>0){
        config = [[FTMobileConfig alloc]initWithDatakitUrl:datakitUrl];
    }else{
        return;
    }
    if([params.allKeys containsObject:@"debug"]){
        NSNumber *debug = params[@"debug"];
        config.enableSDKDebugLog = [debug boolValue];
    }
    if ([params.allKeys containsObject:@"serviceName"]) {
        config.service = params[@"serviceName"];
    }
    if([params.allKeys containsObject:@"env"]){
        id env = params[@"env"];
        if([env isKindOfClass:NSString.class]){
            config.env = env;
        }
    }
    if ([params.allKeys containsObject:@"autoSync"]) {
        config.autoSync = [params[@"autoSync"] boolValue];
    }
    if ([params.allKeys containsObject:@"syncPageSize"]) {
        config.syncPageSize = [params[@"syncPageSize"] integerValue];
    }
    if ([params.allKeys containsObject:@"syncSleepTime"]) {
        config.syncSleepTime = [params[@"syncSleepTime"] integerValue];
    }
    if ([params.allKeys containsObject:@"enableDataIntegerCompatible"]) {
        config.enableDataIntegerCompatible = [params[@"enableDataIntegerCompatible"] boolValue];
    }
    if ([params.allKeys containsObject:@"compressIntakeRequests"]) {
        config.compressIntakeRequests = [params[@"compressIntakeRequests"] boolValue];
    }
    if ([params.allKeys containsObject:@"dbDiscardStrategy"]){
        NSString *type = params[@"dbDiscardStrategy"];
        if([type isEqualToString:@"discard"]){
            config.dbDiscardType = FTDBDiscard;
        }else if([type isEqualToString:@"discardOldest"]){
            config.dbDiscardType = FTDBDiscardOldest;
        }
    }
    if ([params.allKeys containsObject:@"enableLimitWithDbSize"]){
        config.enableLimitWithDbSize = [params[@"enableLimitWithDbSize"] boolValue];
    }
    if ([params.allKeys containsObject:@"dbCacheLimit"]){
        config.dbCacheLimit = [params[@"dbCacheLimit"] longLongValue];
    }
    if ([params.allKeys containsObject:@"globalContext"]) {
        NSDictionary *globalContext = [params valueForKey:@"globalContext"];
        config.globalContext = globalContext;
    }
    
    if ([params.allKeys containsObject:@"dataModifier"]) {
        NSDictionary *dataModifierDict = [params valueForKey:@"dataModifier"];
        config.dataModifier = ^id _Nullable(NSString * _Nonnull key, id  _Nonnull value) {
            if ([dataModifierDict.allKeys containsObject:key]) {
                return dataModifierDict[key];
            }
            return value;
        };
    }
    
    if ([params.allKeys containsObject:@"lineDataModifier"]) {
        NSDictionary *lineDataModifierDict = [params valueForKey:@"lineDataModifier"];
        config.lineDataModifier = ^NSDictionary<NSString *,id> * _Nullable(NSString * _Nonnull measurement, NSDictionary<NSString *,id> * _Nonnull data) {
            if ([measurement isEqualToString:FT_LOGGER_SOURCE] || [measurement isEqualToString:FT_LOGGER_TVOS_SOURCE]) {
                return [lineDataModifierDict valueForKey:@"log"];
            }else{
                return [lineDataModifierDict valueForKey:measurement];
            }
        };
    }

    if ([params.allKeys containsObject:@"serviceName"]) {
        config.service = params[@"serviceName"];
    }

    [FTMobileAgent startWithConfigOptions:config];
}
/// SDK shutdown
void deInit(){
    [FTMobileAgent shutDown];
}
#pragma mark ========== Bind/Unbind User ==========

/// Bind user
/// - Parameter json: parameters
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
/// Unbind user
void unbindUserdata(){
    [[FTMobileAgent sharedInstance] unbindUser];
}

void appendGlobalContext(const char* json){
    NSDictionary *context = JsonStringToDict(json);
    if(context == nil){
        return;
    }
    [FTMobileAgent appendGlobalContext:context];
}

void appendRUMGlobalContext(const char* json){
    NSDictionary *context = JsonStringToDict(json);
    if(context == nil){
        return;
    }
    [FTMobileAgent appendRUMGlobalContext:context];
}

void appendLogGlobalContext(const char* json){
    NSDictionary *context = JsonStringToDict(json);
    if(context == nil){
        return;
    }
    [FTMobileAgent appendLogGlobalContext:context];
}

void flushSyncData(){
    [[FTMobileAgent sharedInstance] flushSyncData];
}

void clearAllData(){
    [FTMobileAgent clearAllData];
}

#pragma mark ========== RUM ==========
/// Initialize RUM configuration
/// - Parameter rumConfigJson: configuration items
/// iOSAppId: appId
/// sampleRate: sampling rate
/// sessionOnErrorSampleRate
/// enableNativeUserResource: whether to perform `Native Resource` automatic tracking
/// enableNativeUserAction: whether to perform `Native Action` tracking, including cold and hot startup
/// enableNativeUserView: whether to perform `Native View` automatic tracking
/// extraMonitorTypeWithError: error monitoring supplement types
/// deviceMonitorType: page monitoring supplement types
/// detectFrequency: page monitoring frequency
/// globalContext: custom RUM global parameters
void initRUMConfig(const char* rumConfigJson){
    NSDictionary *params = JsonStringToDict(rumConfigJson);
    if(params == nil){
        return;
    }
   NSString *rumAppId = [params objectForKey:@"iOSAppId"];
    FTRumConfig *rumConfig = [[FTRumConfig alloc]initWithAppid:rumAppId];
    if ([params.allKeys containsObject:@"sampleRate"]) {
        rumConfig.samplerate = [params[@"sampleRate"] doubleValue] * 100;
    }
    if ([params.allKeys containsObject:@"sessionOnErrorSampleRate"]) {
        rumConfig.sessionOnErrorSampleRate  = [params[@"sessionOnErrorSampleRate"] doubleValue] * 100;
    }
    if ([params.allKeys containsObject:@"enableNativeUserAction"]) {
        rumConfig.enableTraceUserAction = params[@"enableNativeUserAction"];
    }
    if ([params.allKeys containsObject:@"enableNativeUserView"]) {
        rumConfig.enableTraceUserView = params[@"enableNativeUserView"];
    }
    if ([params.allKeys containsObject:@"enableNativeUserResource"]) {
        rumConfig.enableTraceUserResource = params[@"enableNativeUserResource"];
    }
    if ([params.allKeys containsObject:@"extraMonitorTypeWithError"]) {
        id type = params[@"extraMonitorTypeWithError"];
        if([type isKindOfClass:NSString.class]){
            //all, battery, memory, cpu
            if([type isEqualToString:@"all"]){
                rumConfig.errorMonitorType = FTErrorMonitorAll;
            }else if ([type isEqualToString:@"memory"]){
                rumConfig.errorMonitorType = FTErrorMonitorMemory;
            }else if ([type isEqualToString:@"cpu"]){
                rumConfig.errorMonitorType = FTErrorMonitorCpu;
            }else if ([type isEqualToString:@"battery"]){
                rumConfig.errorMonitorType = FTErrorMonitorBattery;
            }
        }else if ([type isKindOfClass:NSArray.class]){
            NSArray *typeAry = type;
            NSEnumerator *enumerator =typeAry.objectEnumerator;
            NSString *typeStr;
            while ((typeStr = enumerator.nextObject) != nil) {
                if([typeStr isEqualToString:@"all"]){
                    rumConfig.errorMonitorType = FTErrorMonitorAll;
                    break;
                }else if ([typeStr isEqualToString:@"memory"]){
                    rumConfig.errorMonitorType = rumConfig.errorMonitorType|FTErrorMonitorMemory;
                }else if ([typeStr isEqualToString:@"cpu"]){
                    rumConfig.errorMonitorType = rumConfig.errorMonitorType|FTErrorMonitorCpu;
                }else if ([typeStr isEqualToString:@"battery"]){
                    rumConfig.errorMonitorType = rumConfig.errorMonitorType|FTErrorMonitorBattery;
                }
            }
        }
    }
    if ([params.allKeys containsObject:@"deviceMonitorType"]){
        id type = params[@"deviceMonitorType"];
        //all, memory, cpu, fps
        if ([type isKindOfClass:NSString.class]){
            if([type isEqualToString:@"all"]){
                rumConfig.deviceMetricsMonitorType = FTDeviceMetricsMonitorAll;
            }else if ([type isEqualToString:@"memory"]){
                rumConfig.deviceMetricsMonitorType = FTDeviceMetricsMonitorMemory;
            }else if ([type isEqualToString:@"cpu"]){
                rumConfig.deviceMetricsMonitorType = FTDeviceMetricsMonitorCpu;
            }else if ([type isEqualToString:@"fps"]){
                rumConfig.deviceMetricsMonitorType = FTDeviceMetricsMonitorFps;
            }
        }else if([type isKindOfClass:NSArray.class]){
            NSArray *typeAry = type;
            NSEnumerator *enumerator = typeAry.objectEnumerator;
            NSString *typeStr;
            while ((typeStr = enumerator.nextObject)!=nil) {
                if([typeStr isEqualToString:@"all"]){
                    rumConfig.deviceMetricsMonitorType = FTDeviceMetricsMonitorAll;
                    break;
                }else if ([typeStr isEqualToString:@"memory"]){
                    rumConfig.deviceMetricsMonitorType = rumConfig.deviceMetricsMonitorType|FTDeviceMetricsMonitorMemory;
                }else if ([typeStr isEqualToString:@"cpu"]){
                    rumConfig.deviceMetricsMonitorType = rumConfig.deviceMetricsMonitorType|FTDeviceMetricsMonitorCpu;
                }else if ([typeStr isEqualToString:@"fps"]){
                    rumConfig.deviceMetricsMonitorType = rumConfig.deviceMetricsMonitorType|FTDeviceMetricsMonitorFps;
                }
            }
        }
    }
    if([params.allKeys containsObject:@"detectFrequency"]){
        //normal, frequent, rare
        NSString *type = params[@"detectFrequency"];
        if([type isEqualToString:@"normal"]){
            rumConfig.monitorFrequency = FTMonitorFrequencyDefault;
        }else if ([type isEqualToString:@"frequent"]){
            rumConfig.monitorFrequency = FTMonitorFrequencyFrequent;
        }else if ([type isEqualToString:@"rare"]){
            rumConfig.monitorFrequency = FTMonitorFrequencyRare;
        }
    }
    if ([params.allKeys containsObject:@"enableResourceHostIP"]) {
        rumConfig.enableResourceHostIP = [params[@"enableResourceHostIP"] boolValue];
    }
    if ([params.allKeys containsObject:@"enableTrackNativeCrash"]){
      rumConfig.enableTrackAppCrash = [params[@"enableTrackNativeCrash"] boolValue];
    }
    if ([params.allKeys containsObject:@"enableTrackNativeAppANR"]){
      rumConfig.enableTrackAppANR = [params[@"enableTrackNativeAppANR"] boolValue];
    }
    if ([params.allKeys containsObject:@"enableTrackNativeFreeze"]){
      rumConfig.enableTrackAppFreeze = [params[@"enableTrackNativeFreeze"] boolValue];
    }
    if ([params.allKeys containsObject:@"nativeFreezeDurationMs"]){
        rumConfig.freezeDurationMs = [params[@"nativeFreezeDurationMs"] integerValue];
    }
    if ([params.allKeys containsObject:@"rumDiscardStrategy"]) {
        NSString *type = params[@"rumDiscardStrategy"];
        if([type isEqualToString:@"discard"]){
            rumConfig.rumDiscardType = FTRUMDiscard;
        }else if ([type isEqualToString:@"discardOldest"]){
            rumConfig.rumDiscardType = FTRUMDiscardOldest;
        }
    }
    if ([params.allKeys containsObject:@"rumCacheLimitCount"]) {
        rumConfig.rumCacheLimitCount = [params[@"rumCacheLimitCount"] integerValue];
    }
    if ([params.allKeys containsObject:@"globalContext"]) {
        rumConfig.globalContext = params[@"globalContext"];
    }
    [[FTMobileAgent sharedInstance] startRumWithConfigOptions:rumConfig];
}
/// Add Action event
/// - Parameter json: parameters
/// actionName: event name
/// actionType: event type
/// property: event context (optional)
void startAction(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *actionName = [configDict objectForKey:@"actionName"];
    NSString *actionType = [configDict objectForKey:@"actionType"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    if(actionName && actionType){
        [FTExternalDataManager.sharedManager startAction:actionName actionType:actionType property:property];
    }
}
/// Create page
/// - Parameter json: parameters
/// viewName: page name
/// loadTime: load time, nanoseconds
void createView(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *viewName = [configDict objectForKey:@"viewName"];
    NSNumber *loadTime = [configDict objectForKey:@"loadTime"];
    [FTExternalDataManager.sharedManager onCreateView:viewName loadTime:loadTime];
}

/// Enter page
/// - Parameter json: parameters
/// viewName: page name
/// property: event context (optional)
void startView(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *viewName = [configDict objectForKey:@"viewName"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    [FTExternalDataManager.sharedManager startViewWithName:viewName property:property];
}

/// Leave page
/// - Parameter json: parameters
/// property: event context (optional)
void stopView(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict != nil){
        NSDictionary *property = [configDict objectForKey:@"property"];
        [FTExternalDataManager.sharedManager stopViewWithProperty:property];
    }else{
        [FTExternalDataManager.sharedManager stopView];
    }
}

/// HTTP request start
/// - Parameter json: parameters
/// resourceId: request unique identifier
/// property: event context (optional)
void startResource(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *resourceId = [configDict objectForKey:@"resourceId"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    [FTExternalDataManager.sharedManager startResourceWithKey:resourceId property:property];
}

/// HTTP request end
/// - Parameter json: parameters
/// resourceId: request unique identifier
/// property: event context (optional)
void stopResource(const char* json){
    NSDictionary *configDict = JsonStringToDict(json);
    if(configDict == nil){
        return;
    }
    NSString *resourceId = [configDict objectForKey:@"resourceId"];
    NSDictionary *property = [configDict objectForKey:@"property"];
    [FTExternalDataManager.sharedManager stopResourceWithKey:resourceId property:property];
}
/// Add network transmission content
/// - Parameter json: parameters
/// resourceId: request unique identifier
/// params: request performance data
///       * url
///       * requestHeader
///       * responseHeader
///       * resourceMethod
///       * responseBody
///       * resourceStatus
/// netStatusBean: request related data
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
//    NSDictionary *netStatus = [configDict objectForKey:@"netStatus"];
    
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
//    if(netStatus){
//        long tcpStartTime = [[netStatus objectForKey:@"tcpStartTime"] longValue];
//        long tcpEndTime = [[netStatus objectForKey:@"tcpEndTime"] longValue];
//        NSNumber  *tcpTime = @(tcpEndTime - tcpStartTime);
//        
//        long dnsEndTime = [[netStatus objectForKey:@"dnsEndTime"] longValue];
//        long dnsStartTime = [[netStatus objectForKey:@"dnsStartTime"] longValue];
//        NSNumber * dnsTime = @(dnsEndTime - dnsStartTime);
//        
//        long sslEndTime = [[netStatus objectForKey:@"sslEndTime"] longValue];
//        long sslStartTime = [[netStatus objectForKey:@"sslStartTime"] longValue];
//        NSNumber * sslTime = @(sslEndTime - sslStartTime);
//        
//        long bodyEndTime = [[netStatus objectForKey:@"bodyEndTime"] longValue];
//        long bodyStartTime = [[netStatus objectForKey:@"bodyStartTime"] longValue];
//        long callStartTime = [[netStatus objectForKey:@"callStartTime"] longValue];
//        long headerStartTime = [[netStatus objectForKey:@"headerStartTime"] longValue];
//        long headerEndTime = [[netStatus objectForKey:@"headerEndTime"] longValue];
//        
//    
//        
//        NSNumber * ttfb = @(bodyStartTime - callStartTime);
//        NSNumber *transTime = @(bodyEndTime - bodyStartTime);
//        
//
//        NSNumber * duration = @(bodyEndTime - callStartTime);
//        
//        metrics.resource_tcp = tcpTime;
//        metrics.resource_dns = dnsTime;
//        metrics.resource_ssl = sslTime;
//        metrics.resource_trans = transTime;
//        metrics.resource_ttfb = ttfb;
//        metrics.duration = duration;
//    }
    [FTExternalDataManager.sharedManager addResourceWithKey:resourceId metrics:metrics content:content];
}

/// Add error event
/// - Parameter json: parameters
/// log: error log
/// message: error message
/// errorType: error type
/// state: program running state (run, startup, unknown)
/// property: event context (optional)
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
/// Add longtask
/// - Parameter json: parameters
/// log: stuttering log
/// duration: stuttering duration
/// property: event context (optional)
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
/// Initialize Log
/// - Parameter logConfigJson: configuration items
/// sampleRate: sampling rate
/// enableCustomLog: whether to enable custom logs
/// enableLinkRumData: whether to associate with RUM
/// logLevelFilters: log level filtering
/// discardStrategy: log discard strategy
/// globalContext: custom log global parameters
void initLogConfig(const char* logConfigJson){
    NSDictionary *params = JsonStringToDict(logConfigJson);
    if(params == nil){
        return;
    }
    FTLoggerConfig *config = [[FTLoggerConfig alloc]init];
    if ([params.allKeys containsObject:@"sampleRate"]) {
        config.samplerate =[params[@"sampleRate"] doubleValue] * 100;
    }
    if ([params.allKeys containsObject:@"enableLinkRumData"]) {
        config.enableLinkRumData = params[@"enableLinkRumData"];
    }
    if ([params.allKeys containsObject:@"enableCustomLog"]) {
        config.enableCustomLog = params[@"enableCustomLog"];
    }
    if ([params.allKeys containsObject:@"discardStrategy"]) {
        NSString *type = params[@"discardStrategy"];
        //`discard` discard new data (default), `discardOldest`
        if([type isEqualToString:@"discardOldest"]){
            config.discardType = FTDiscardOldest;
        }else{
            config.discardType = FTDiscard;
        }
    }
    if ([params.allKeys containsObject:@"logLevelFilters"]) {
        NSArray *filters = params[@"logLevelFilters"];
        if(filters.count>0){
            NSEnumerator *enumerator = filters.objectEnumerator;
            NSString *level;
            NSMutableArray *logLevelFilters = [NSMutableArray new];
            while ((level = enumerator.nextObject)) {
                //`info` prompt, `warning` warning, `error` error, `critical`, `ok` recovery
                if([level isEqualToString:@"info"]){
                    [logLevelFilters addObject:@(FTStatusInfo)];
                }else if([level isEqualToString:@"warning"]){
                    [logLevelFilters addObject:@(FTStatusWarning)];
                }else if([level isEqualToString:@"error"]){
                    [logLevelFilters addObject:@(FTStatusError)];
                }else if([level isEqualToString:@"critical"]){
                    [logLevelFilters addObject:@(FTStatusCritical)];
                }else if([level isEqualToString:@"ok"]){
                    [logLevelFilters addObject:@(FTStatusOk)];
                }
            }
            config.logLevelFilter =  logLevelFilters;
        }
    }
    if ([params.allKeys containsObject:@"logCacheLimitCount"]) {
        config.logCacheLimitCount = [params[@"logCacheLimitCount"] integerValue];
    }
    if([params.allKeys containsObject:@"globalContext"]){
        config.globalContext = [params objectForKey:@"globalContext"];
    }
    [[FTMobileAgent sharedInstance] startLoggerWithConfigOptions:config];
}

/// Log printing
/// - Parameter json: parameters
/// log: log content
/// level: log level
/// property: event context (optional)
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

/// Initialize Trace
/// - Parameter traceConfigJson: configuration items
/// sampleRate: sampling rate
/// traceType: link type
/// enableLinkRUMData: whether to associate with `RUM` data
/// enableAutoTrace: whether to enable native network automatic tracking
void initTraceConfig(const char* traceConfigJson){
    NSDictionary *params = JsonStringToDict(traceConfigJson);
    if(params == nil){
        return;
    }
   FTTraceConfig *trace = [[FTTraceConfig alloc]init];
    if ([params.allKeys containsObject:@"sampleRate"]) {
        trace.samplerate =[params[@"sampleRate"] doubleValue] * 100;
    }
    if ([params.allKeys containsObject:@"traceType"]) {
        NSString *type =  params[@"traceType"];
        //`ddTrace` (default), `zipkinMultiHeader`, `zipkinSingleHeader`, `traceparent`, `skywalking`, `jaeger`
        if([type isEqualToString:@"ddTrace"]){
            trace.networkTraceType = FTNetworkTraceTypeDDtrace;
        }else if ([type isEqualToString:@"zipkinMultiHeader"]){
            trace.networkTraceType = FTNetworkTraceTypeZipkinMultiHeader;
        }else if ([type isEqualToString:@"zipkinSingleHeader"]){
            trace.networkTraceType = FTNetworkTraceTypeZipkinSingleHeader;
        }else if ([type isEqualToString:@"traceparent"]){
            trace.networkTraceType = FTNetworkTraceTypeTraceparent;
        }else if ([type isEqualToString:@"skywalking"]){
            trace.networkTraceType = FTNetworkTraceTypeSkywalking;
        }else if ([type isEqualToString:@"jaeger"]){
            trace.networkTraceType = FTNetworkTraceTypeJaeger;
        }
    }
    if ([params.allKeys containsObject:@"enableLinkRumData"]) {
        trace.enableLinkRumData = params[@"enableLinkRumData"];
    }
    if ([params.allKeys containsObject:@"enableNativeAutoTrace"]) {
        trace.enableAutoTrace = params[@"enableNativeAutoTrace"];
    }
    [[FTMobileAgent sharedInstance] startTraceWithConfigOptions:trace];
}
/// Get trace headers that need to be added
/// - Parameter json: parameters
/// resourceId: request unique identifier
/// url: request URL
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
    }else if ([methodStr isEqualToString:@"AppendGlobalContext"]){
         appendGlobalContext(json);
    }else if ([methodStr isEqualToString:@"AppendLogGlobalContext"]){
         appendLogGlobalContext(json);
    }else if ([methodStr isEqualToString:@"AppendRUMGlobalContext"]){
         appendRUMGlobalContext(json);
    }else if ([methodStr isEqualToString:@"FlushSyncData"]){
        flushSyncData();
    }else if ([methodStr isEqualToString:@"ClearAllData"]){
        clearAllData();
    }
    return nil;
}
}

