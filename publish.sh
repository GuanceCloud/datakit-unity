#!/bin/bash

# 检查参数是否正确
if [ $# -ne 3 ]; then
    echo "Usage: $0 FT_PROD_ENV WORKSPACE_PATH UNITY_INSTALL_PATH"
    exit 1
fi


UNITY_PACKAGE_NAME="ft-sdk-unity.unitypackage"

#环境，例如 alpha
FT_PROD_ENV=$1

#执行项目位置
WORKSPACE_PATH=$2

#Unity 命令行工具地址
UNITY_INSTALL_PATH=$3

# 导出 unitypackage 
$UNITY_INSTALL_PATH -batchmode -projectPath ${WORKSPACE_PATH} -exportPackage Assets/Plugins $UNITY_PACKAGE_NAME -quit

# 上传至 OSS
OSS_TARGET_PATH=oss://zhuyun-static-files-production/ft-sdk-package/unitypackage/${FT_PROD_ENV}/${UNITY_PACKAGE_NAME}
~/ossutilmac64 cp -f "$UNITY_PACKAGE_NAME" "$OSS_TARGET_PATH"

# 删除导出 unitypackage
rm -f $UNITY_PACKAGE_NAME
