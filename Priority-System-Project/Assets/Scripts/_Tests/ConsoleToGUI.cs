using TMPro;
using UnityEngine;

public class ConsoleToGUI : MonoBehaviour {
    //#if !UNITY_EDITOR
    static string myLog = "";
    private string output;
    private string stack;

    /// <summary>
    /// Is the GUI active?
    /// </summary>
    private bool openGUI = true;

    private bool assignedToClient;

    public TextMeshProUGUI consoleLog;
    public Canvas clientCanvas;
    public GameObject scrollView;

    void OnEnable() {
        Application.logMessageReceived += Log;
    }

    void OnDisable() {
        Application.logMessageReceived -= Log;
    }

    public void switchGUI() { openGUI = !openGUI; }

    public void Log(string logString, string stackTrace, LogType type) {
        output = logString;
        stack = stackTrace;
        myLog = output + "\n\n" + myLog;
        if (myLog.Length > 5000) {
            myLog = myLog.Substring(0, 4000);
        }
    }

    private void Update()
    {
        if (!assignedToClient)
        {
            GameObject networkPlayer = GameObject.Find("Network Player");
            if(networkPlayer != null)
            {
                clientCanvas.worldCamera = networkPlayer.GetComponentInChildren<Camera>();
                networkPlayer.transform.SetParent(gameObject.transform, false);
                transform.localPosition = new Vector3(0, 0, 10f);
                assignedToClient = true;
            }  
        }

        if (openGUI)
        {
            consoleLog.text = myLog;
        }
    }

    public void ToggleDebugLog()
    {
        scrollView.SetActive(!scrollView.activeSelf);
    }

    void OnGUI() {
        //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
        /*
        if(openGUI)
        {
            myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), myLog);
        }
        */
    }
    //#endif
}