# 1.1.0-alpha.2
* Added bridgeContext feature
* Fixed SDK Config initialization error

# 1.1.0-alpha.1
* Compatible with Android ft-sdk 1.6.11, ft-native 1.1.1, iOS 1.5.16
* Support reporting collected data through public network dataway
* Added SDKConfig.dataModifier, SDKConfig.dataModifier support for data write replacement and data desensitization
* RUMConfig.sessionErrorSampleRate supports error sampling. When not sampled by sampleRate, rum data from 1 minute ago can be sampled when an error occurs
* Support disabling automatic synchronization through SDKConfig.autoSync and using FTUnityBridge.flushSyncData() to synchronize data manually
* Support controlling synchronization data entries through SDKConfig.syncPageSize and controlling data synchronization interval through SDKConfig.syncSleepTime
* Support enabling cache limits through SDKConfig.enableLimitWithDbSize, using SDKConfig.dbCacheLimit to limit total cache size. After enabling, LogConfig.logCacheLimitCount and RUMConfig.rumCacheLimitCount will be invalid
* Support limiting RUM data cache entry count through RUMConfig.rumCacheLimitCount, default 100_000
* Support limiting Log data cache entry count through LogConfig.logCacheLimitCount, default 5000
* Support enabling deflate data synchronization compression through SDKConfig.compressIntakeRequests
* Support adding dynamic attributes through FTUnityBridge.appendGlobalContext(globalContext), FTUnityBridge.appendRUMGlobalContext(globalContext), FTUnityBridge.appendLogGlobalContext(globalContext)
* Support clearing unreported cached data through FTUnityBridge.clearAllData()
* Support compatibility with native application crashes, freezes and other issues through RUMConfig.enableTrackNativeCrash, RUMConfig.enableTrackNativeAppANR, RUMConfig.enableTrackNativeFreeze

---
# 1.0.0-alpha.2
* Clean up unused SDK references

---

# 1.0.0-alpha.1
* Compatible with Android ft-sdk 1.3.16-beta03, iOS 1.4.7-alpha.1
* Added basic interfaces for Trace, RUM, Log 