using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FTSDK.Unity.Bridge;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 4014
public class ClickEvent : MonoBehaviour
{
    // Start is called before the first frame update

    public void GoSecondScene()
    {
        SceneManager.LoadScene(1);
        FTUnityBridge.StartAction("click", "test", new Dictionary<string, object>(){
            {"action_property","property test"}
        });
        //  SceneManager.UnloadSceneAsync(0);
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene(0);
        // SceneManager.UnloadSceneAsync(1);
    }

    public void AddError()
    {
        FTUnityBridge.AddError("Custom Error Log", "Error Log");
    }

    public void AddAction()
    {
        FTUnityBridge.StartAction("AddAction", "click");
    }

    public void BinderUser()
    {
        FTUnityBridge.BindUserData("someone_user_id");
    }

    public void NetRequest()
    {
        String resourceId = "unique id";
        String url = "https://httpbin.org/status/200";
        Task.Run(async () =>
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string headers = await FTUnityBridge.GetTraceHeader(resourceId, url);

                    Dictionary<string, string> dic = JsonUtility.FromJson<Dictionary<String, String>>(headers);

                    if (dic != null)
                    {
                        foreach (KeyValuePair<string, string> kvp in dic)
                        {
                            string key = kvp.Key;
                            string value = kvp.Value;
                            client.DefaultRequestHeaders.Add(key, value);
                        }
                    }

                    FTUnityBridge.StartResource(resourceId, new Dictionary<string, object>{
                        {"resource_property","property test"}
                    });

                    HttpResponseMessage response = await client.GetAsync(url);

                    string responseData = await response.Content.ReadAsStringAsync();

                    FTUnityBridge.StopResource(resourceId);
                    ResourceParams resourceParams = new ResourceParams();
                    resourceParams.url = url;
                    resourceParams.requestHeader = client.DefaultRequestHeaders.ToDictionary(header => header.Key, header => string.Join(",", header.Value));
                    resourceParams.responseHeader = response.Headers.ToDictionary(header => header.Key, header => string.Join(",", header.Value));

                    resourceParams.resourceStatus = (int)response.StatusCode;
                    resourceParams.responseBody = responseData;
                    resourceParams.resourceMethod = "GET";
                    FTUnityBridge.AddResource(resourceId, resourceParams);

                }
                catch (HttpRequestException e)
                {
                    Debug.Log($"Network request failed: {e.Message}");
                }
            }
        });
    }

    public void DynamicGlobalContext()
    {
        FTUnityBridge.AppendGlobalContext(new Dictionary<string, object>()
        {
            {"ft_global_key","ft_global_value" },
        }
        );
        FTUnityBridge.AppendRUMGlobalContext(new Dictionary<string, object>(){
            { "ft_global_rum_key", "ft_global_rum_value" },
        });
        FTUnityBridge.AppendLogGlobalContext(new Dictionary<string, object>()
        {
            { "ft_global_log_key", "ft_global_log_value" },
        });
    }

    public void LongTask()
    {
        FTUnityBridge.AddLongTask("longtask", 1000000);
    }


}
#pragma warning restore 4014
