using System.Collections;
using Network;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Utils;
using Application = UnityEngine.Application;

public class Startup : MonoBehaviour {
    public static int ShowOnlyPriority = -1;

    public TMPro.TMP_Dropdown priorityTypeDropdown;

    private void Start() {
        try {
            var mode = GetArg("-mode");
            StartCoroutine(StartNetwork(mode));
        }
        catch {
            Debug.Log("[Settings] no app mode");
        }

        try {
            var time = int.Parse(GetArg("-time"));
            if (!Logger.Move)
                StartCoroutine(KillAfter(time));
            else
                FindObjectOfType<Follower>().OnPathCompleted += KillApp;
        }
        catch {
            Debug.Log("[Settings] no timer");
        }

        try {
            var showPriority = int.Parse(GetArg("-show"));
            ShowOnlyPriority = showPriority;
        }
        catch {
            Debug.Log("[Settings] no show priority");
        }

        try {
            Prefs.Singleton.aoi = GetArg("-no-priority") == null;
        }
        catch {
            Debug.Log("[Settings] error");
        }

        try {
            var delay = int.Parse(GetArg("-delay"));
            Prefs.Singleton.sendDelay = delay;
        }
        catch {
            Debug.Log("[Settings] no delay: default 0.06s");
        }

        try
        {
            string priorityType = GetArg("-priorityType");
            if (priorityType.Equals("aoi"))
            {
                Prefs.Singleton.PriorityTypeValueChanged(0);
                priorityTypeDropdown.value = 0;
            }
            else
            {
                Prefs.Singleton.PriorityTypeValueChanged(1);
                priorityTypeDropdown.value = 1;
            }
                
        }
        catch
        {
            Debug.Log("[Settings] Priority type not set from args");
        }

        try
        {
            string getLocalIp = GetArg("-SetLocalIP");
            FindObjectOfType<NetworkUI>().SetIP(NetworkUI.GetLocalIPAddress());
        }
        catch 
        {
            Debug.Log("[Settings] Using default ip");
        }

        try
        {
            double d1 = double.Parse(GetArg("-D1"));
            double d2 = double.Parse(GetArg("-D2"));
            double d3 = double.Parse(GetArg("-D3"));

            Priority.SetWeights(d1, d2, d3);
        }
        catch
        {
            Debug.Log("[Settings] Using default weights for priority calculations");
        }

    }

    public static IEnumerator StartNetwork(string mode) {
        yield return new WaitForSeconds(1);
        if (mode == "server")
            FindObjectOfType<NetworkUI>().StartServer();
        if (mode == "client")
            FindObjectOfType<NetworkUI>().StartClient();
    }

    public static IEnumerator KillAfter(int time) {
        yield return new WaitForSeconds(time);
        Logger.SaveFiles();
        Application.Quit();
    }

    public void KillApp()
    {
        Logger.SaveFiles();
        Application.Quit();
    }

    public static string GetArg(string name) {
        string[] args;

        if (Logger.IsCurrentAppInstanceVR())
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            args = intent.Call<string>("getDataString").Split(',');
        }
        else
            args = System.Environment.GetCommandLineArgs();

        for (var i = 0; i < args.Length; i++) {
            if (args[i] == name && args.Length > i + 1) {
                return args[i + 1];
            }
        }

        return null;

    }
}