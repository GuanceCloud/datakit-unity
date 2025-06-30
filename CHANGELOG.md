# 1.1.0-alpha.1
* 适配 Android ft-sdk 1.6.11,ft-native 1.1.1 ，iOS 1.5.16
* 支持通过公网 dataway 上报采集数据
* 添加 SDKConfig.dataModifier、SDKConfig.dataModifier 支持数据写入替换，支持数据脱敏
* RUMConfig.sessionErrorSampleRate 支持错误采样，在未被 sampeRate 采样时， 在发生错误时可以对 1 分钟前的 rum 的数据进行取样采集
* 支持通过 SDKConfig.autoSync 关闭自动同步，并使用 FTUnityBridge.flushSyncData() 自行同步数据
* 支持通过 SDKConfig.syncPageSize 控制同步数据条目，SDKConfig.syncSleepTime 控制数据同步的间隔时间
* 支持通过 SDKConfig.enableLimitWithDbSize 开始缓存限制，使用 SDKConfig.dbCacheLimit 限制总缓存大小，开启之后 LogConfig.logCacheLimitCount 及 RUMConfig.rumCacheLimitCount 将失效
* 支持通过 RUMConfig.rumCacheLimitCount 限制 RUM 数据缓存条目数上限，默认 100_000。
* 支持通过 LogConfig.logCacheLimitCount 闲置 Log 数据缓存条目数上限，默认 5000.
* 支持通过 SDKConfig.compressIntakeRequests 开启 defalte 数据同步压缩
* 支持通过 FTUnityBridge.appendGlobalContext(globalContext)FTUnityBridge.appendRUMGlobalContext(globalContext)、 FTUnityBridge.appendLogGlobalContext(globalContext)添加动态属性
* 支持通过 FTUnityBridge.clearAllData() 清理未上报缓存数据
* 支持通过 RUMConfig.enableTrackNativeCrash, RUMConfig.enableTrackNativeAppANR, RUMConfig.enableTrackNativeFreeze, 来兼容原生应用的崩溃、卡顿等问题。

---
# 1.0.0-alpha.2
* 清理 SDK 无用引用

---

# 1.0.0-alpha.1
* 适配 Android ft-sdk 1.3.16-beta03, iOS 1.4.7-alpha.1
* 添加手段 Trare , RUM , Log 基础接口 