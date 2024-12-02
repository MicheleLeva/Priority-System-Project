using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XRDebugLog : MonoBehaviour {
    public Text display;

    private void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string condition, string stacktrace, LogType type) {
        display.text += $"\n{condition}";
    }
}