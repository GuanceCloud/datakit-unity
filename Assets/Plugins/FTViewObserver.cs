using System;
using FTSDK.Unity.Bridge;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FTSDK.Unity
{
#pragma warning disable 4014
    public class FTViewObserver : MonoBehaviour
    {
        String currentViewName = "";
        void Awake()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            currentViewName = currentScene.name;
        }

        void OnEnable()
        {
            FTUnityBridge.StartView(currentViewName);
        }

        void OnDisable()
        {
            FTUnityBridge.StopView();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                FTUnityBridge.StopView();
            }
            else
            {
                FTUnityBridge.StartView(currentViewName);
            }
        }

    }
#pragma warning restore 4014


}
