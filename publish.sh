#!/bin/bash

# Check if parameters are correct
if [ $# -ne 3 ]; then
    echo "Usage: $0 FT_PROD_ENV WORKSPACE_PATH UNITY_INSTALL_PATH"
    exit 1
fi


UNITY_PACKAGE_NAME="ft-sdk-unity.unitypackage"

# Environment, e.g. alpha
FT_PROD_ENV=$1

# Project execution location
WORKSPACE_PATH=$2

# Unity command line tool path
UNITY_INSTALL_PATH=$3

# Export unitypackage 
$UNITY_INSTALL_PATH -batchmode -projectPath ${WORKSPACE_PATH} -exportPackage Assets/Plugins Assets/FTSDK $UNITY_PACKAGE_NAME -quit

# Upload to OSS
OSS_TARGET_PATH=oss://zhuyun-static-files-production/ft-sdk-package/unitypackage/${FT_PROD_ENV}/${UNITY_PACKAGE_NAME}
~/ossutilmac64 cp -f "$UNITY_PACKAGE_NAME" "$OSS_TARGET_PATH"

# Delete exported unitypackage
rm -f $UNITY_PACKAGE_NAME
