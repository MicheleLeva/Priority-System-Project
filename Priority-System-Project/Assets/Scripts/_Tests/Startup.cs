using System;
using System.Collections;
using Network;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Utils;
using Application = UnityEngine.Application;

public class Startup : MonoBehaviour {
    public static int ShowOnlyPriority = -1;

    public TMPro.TMP_Dropdown priorityTypeDropdown;

    public static bool startupComplete = false;

    public TrackedPoseDriver trackedPoseDriver;
    public GameObject leftController;
    public GameObject rightController;

    private void Start() {

#if UNITY_EDITOR
        Debug.Log("[Settings] Editor: Enabling Device Simulator!");

            XRDeviceSimulator deviceSimulator = FindObjectOfType<XRDeviceSimulator>();
            if (deviceSimulator != null)
            {
                deviceSimulator.enabled = true;
            }
#endif

        try
        {
            var mode = GetArg("-mode");
            //if we are testing the client we are disabling the hands for the screenshot
            if (mode.Equals("client"))
            {
                FindObjectOfType<XRUIInputModule>().enabled = false;
                var l = GameObject.Find("LeftHand Controller");
                l.GetComponent<XRBaseController>().enabled = false;
                l.transform.position += new Vector3(0, -10, 0);
                var r = GameObject.Find("RightHand Controller");
                r.GetComponent<XRBaseController>().enabled = false;
                r.transform.position += new Vector3(0, -10, 0);
            }

            StartCoroutine(StartNetwork(mode));
            Debug.Log($"[Settings] Setting mode {mode}");
        }
        catch {
            //if we are not testing we are enabling the TrackedPoseDriver and the controllers
            //trackedPoseDriver.enabled = true;
            Debug.Log("[Settings] No app mode");
        }

        try {
            var time = int.Parse(GetArg("-time"));
            if (!Logger.Move)
            {
                StartCoroutine(KillAfter(time));
                Debug.Log($"[Settings] Setting auto kill after {time} seconds");
            }
            else
            {
                FindObjectOfType<Follower>().OnPathCompleted += KillApp;
                Debug.Log($"[Settings] Setting auto kill after completion of path");
            }
        }
        catch {
            Debug.Log("[Settings] no timer");
        }

        try {
            var showPriority = int.Parse(GetArg("-show"));
            ShowOnlyPriority = showPriority;
            Debug.Log($"[Settings] Show Priority");
        }
        catch {
            Debug.Log("[Settings] no show priority");
        }

        try {
            Prefs.Singleton.aoi = GetArg("-no-priority") == null;
            Debug.Log($"[Settings] Priority set");
        }
        catch {
            Debug.Log("[Settings] error");
        }

        try {
            var delay = int.Parse(GetArg("-delay"));
            Prefs.Singleton.sendDelay = delay;
            Debug.Log($"[Settings] Delay set: {delay}s");
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
            Debug.Log($"[Settings] Priority type set: {priorityType}");

        }
        catch
        {
            Debug.Log("[Settings] Priority type not set from args");
        }

        try
        {
            var setIp = GetArg("-SetIP");
            if (setIp != null)
            {
                if (setIp.Equals("local"))
                {
                    FindObjectOfType<NetworkUI>().SetIP(NetworkUI.GetLocalIPAddress());
                    Debug.Log($"[Settings] Local ip set");
                } else
                {
                    FindObjectOfType<NetworkUI>().SetIP(setIp);
                    Debug.Log($"[Settings] Ip set from parameters: {setIp}");
                }
            }
            else
                Debug.Log("[Settings] Using default ip");
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
            Debug.Log($"[Settings] Weights for priority calculations set: {d1}, {d2}, {d3}");
        }
        catch
        {
            Debug.Log("[Settings] Using default weights for priority calculations");
        }

        try
        {
            bool fullScene = GetArg("-FullScene") != null;
            if (fullScene)
            {
                FindObjectOfType<Follower>().waitForCompleteSceneLoading = true;
                Debug.Log($"[Settings] Wait for complete scene loading");
            }
            else
                Debug.Log("[Settings] Standard scene loading");
        }
        catch
        {
            Debug.Log("[Settings] Standard scene loading");
        }

        startupComplete = true;
    }

    public IEnumerator StartNetwork(string mode) {
        yield return new WaitForSeconds(1);
        if (mode == "server")
            FindObjectOfType<NetworkUI>().StartServer();
        if (mode == "client")
        {
            Camera.main.transform.rotation = Quaternion.identity;
            Camera.main.transform.position = new Vector3(-0.3480972f, 1.12f, -5.517212f);
            trackedPoseDriver.enabled = false;
            FindObjectOfType<NetworkUI>().StartClient();
        } 
    }

    public static IEnumerator KillAfter(int time) {
        yield return new WaitForSeconds(time);
        Logger.SaveFiles();
        Application.Quit();
    }

    public void KillApp()
    {
        Debug.Log("Closing app");

        Logger.SaveFiles();

        if (Logger.IsCurrentAppInstanceVR())
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("finish");
        }
        else
        {
            Application.Quit();
        }
    }

    public static string GetArg(string name) {
        string[] args;

        if (Logger.IsCurrentAppInstanceVR())
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            //AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("parseIntent");

            //Debug.Log("intent URI: " + intent.Call<string>("toUri", 0).ToString());

            //args = intent.Call<string>("getDataString").Split(',');

            string parsedString = intent.Call<string>("getStringExtra", "unity");
            args = parsedString.Split('/');

            string argsToPrint = "";
            foreach(string s in args)
            {
                argsToPrint += s + ", ";
            }
            Debug.Log("args = " + argsToPrint);

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