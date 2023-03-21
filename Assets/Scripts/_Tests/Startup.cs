using System.Collections;
using Network;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Application = UnityEngine.Application;

public class Startup : MonoBehaviour {
    public static int ShowOnlyPriority = -1;

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
            StartCoroutine(KillAfter(time));
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

    public static string GetArg(string name) {
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++) {
            if (args[i] == name && args.Length > i + 1) {
                return args[i + 1];
            }
        }

        return null;
    }
}