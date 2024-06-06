using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Network.Objects;
using Network.SpawnUpdater;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class Logger : MonoBehaviour {
    private static string _dir;
    private static string _logFpsFile;
    private static string _logObjFile;
    private UnityTransport _transport;

    public float fpsLogDelay = 0.5F;
    public float screenDelay = 0.5F;

    public bool clientFps;
    public bool serverFps;
    public bool clientScreen;
    public bool objTimes;
    public bool move;

    public static bool Move;
    private static Transform _transform;
    public static readonly Dictionary<int, float> ObjDelays = new();

    public static bool ObjTimes => _instance.objTimes;
    private static Logger _instance;
    public static string ObjTimesStr;

    private void Start() {
        Move = move;
        _instance = this;
        _transform = Camera.main!.transform;
        
        try {
            Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(120);
            // StartCoroutine(Startup.StartNetwork("client"));
        }
        catch {
            Debug.Log("No XR Device");
            Screen.SetResolution(1094, 484, false);
            FindObjectOfType<XRUIInputModule>().enabled = false;
            var l = GameObject.Find("LeftHand Controller");
            l.GetComponent<XRBaseController>().enabled = false;
            l.transform.position += new Vector3(0, -10, 0);
            var r = GameObject.Find("RightHand Controller");
            r.GetComponent<XRBaseController>().enabled = false;
            r.transform.position += new Vector3(0, -10, 0);
        }
        
        _dir = Application.persistentDataPath + "/LOGS";
        Directory.CreateDirectory(_dir);

        if (serverFps) {
            _transport = (UnityTransport) NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            //Directory.CreateDirectory(_dir);
            _logFpsFile = "log_fps.csv";
            CreateFile("log_fps.csv");

            NetworkManager.Singleton.OnServerStarted += () =>
                NetworkManager.Singleton.OnClientConnectedCallback += _ =>
                    StartCoroutine(LogLoopServer());
        }

        if (clientFps)
            StartCoroutine(LogLoopClient());

        if (objTimes)
            CreateFile("/obj_times.csv");

        if (clientScreen) {
            Directory.CreateDirectory($"{_dir}/SCREEN/");
            var di = new DirectoryInfo($"{_dir}/SCREEN/");
            foreach (var file in di.GetFiles()) {
                file.Delete();
            }

            //If the character is moving screenshots will be taken through events callbacks, see Follower script
            if (!move)
                StartCoroutine(ScreenLoopClient());
            else
                FindObjectOfType<Follower>().OnStepMade += TakeScreenshot;
        }
    }

    public static bool IsCurrentAppInstanceVR()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }
        return false;
    }

    private static int GetVisibleObjs() {
        return FindObjectsOfType<NetObject>()
            .Count(no =>
                no.GetComponent<Renderer>().isVisible);
    }

    private int GetObjOfPriority(int p) {
        return FindObjectsOfType<NetObject>()
            .Count(o => o.priority == p);
    }

    private IEnumerator ScreenLoopClient() {
        var i = 0;
        while (true) {
            if (NetworkManager.Singleton.IsServer) yield break;
            if (NetworkManager.Singleton.IsClient) {
                // SaveCameraView(Camera.main, $"/SCREEN/screen_{GetTime()}.png");

                if (IsCurrentAppInstanceVR())
                    {
                        ScreenCapture.CaptureScreenshot($"LOGS/SCREEN/screen_{GetTime()}.png");
                    }            
                else
                    ScreenCapture.CaptureScreenshot($"{_dir}/SCREEN/screen_{GetTime()}.png");

                //Debug.Log($"{_dir}/SCREEN/screen_{GetTime()}.png");
                yield return new WaitForSeconds(screenDelay);
                i++;                
            }
            else {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void TakeScreenshot()
    {
        if (NetworkManager.Singleton.IsServer) return;
        if (NetworkManager.Singleton.IsClient)
        {
            // SaveCameraView(Camera.main, $"/SCREEN/screen_{GetTime()}.png");

            if (IsCurrentAppInstanceVR())
            {
                //Debug.Log("Enable VR");
                ScreenCapture.CaptureScreenshot($"LOGS/SCREEN/screen_{GetTime()}.png");
            }
            else
                ScreenCapture.CaptureScreenshot($"{_dir}/SCREEN/screen_{GetTime()}.png");

            Debug.Log($"{_dir}/SCREEN/screen_{GetTime()}.png");
        }
    }

    private float _fpsSum;
    private int _fpsCount;
    private int _totCount;

    private IEnumerator LogLoopServer() {
        while (true) {
            if (Time.fixedTime / (fpsLogDelay * _totCount) >= 1) {
                // Debug.LogError($"FPS: {GetFPS()} ~ {_fpsSum / _fpsCount}");
                AppendText($"{GetTime()}, {_fpsSum / _fpsCount}, {GetRtt()}\n", _logFpsFile);
                _fpsSum = 0;
                _fpsCount = 0;
                _totCount++;
            }
            else {
                _fpsSum += GetFPS();
                _fpsCount++;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator LogLoopClient() {
        while (true) {
            if (NetworkManager.Singleton.IsServer) yield break;
            if (NetworkManager.Singleton.IsClient) {
                if (Time.fixedTime / (fpsLogDelay * _totCount) >= 1) {
                    AppendText($"{GetTime()}, {_fpsSum / _fpsCount}\n", _logFpsFile);
                    _fpsSum = 0;
                    _fpsCount = 0;
                    _totCount++;
                }
                else {
                    _fpsSum += GetFPS();
                    _fpsCount++;
                }
            }
            else {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public static void LogObj(GameObject target) {
        var distance = (target.transform.position - _transform.position).magnitude;
        AppendText($"{GetTime()}, {target.name}, {distance}, {target.GetComponent<NetObject>().priority}\n",
            _logObjFile);
    }

    public static string GetTime() {
        return Time.time.ToString("f3");
        // return DateTime.Now.ToString("HH:mm:ss:fff");
    }

    private static float GetFPS() {
        return 1.0f / Time.deltaTime;
    }


    private string GetRtt() {
        var clients = NetworkManager
            .Singleton
            .ConnectedClientsIds;

        return clients.Count > 0
            ? _transport
                .GetCurrentRtt(clients[0])
                .ToString()
            : null;
    }


    public static void SaveFiles() {
        if (ObjTimes) {
            AppendText(ObjTimesStr, "obj_times.csv");
            Debug.LogError("Saved Times.");
        }
    }

    void SaveCameraView(Camera cam, string filename) {
        var screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        cam.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        cam.Render();
        var renderedTexture = new Texture2D(Screen.width, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;
        var byteArray = renderedTexture.EncodeToPNG();
        File.WriteAllBytes(_dir + filename, byteArray);
    }

    // ------------------------------------------------------------------
    // FILES
    // ------------------------------------------------------------------

    private static void CreateFile(string path) {
        File.WriteAllText(_dir + path, "");
    }

    public static void AppendText(string text, string path) {
        File.AppendAllTextAsync(_dir + path, text);
    }


// ------------------------------------------------------------------
// EDITOR
// ------------------------------------------------------------------

    public GameObject cube;

    [ContextMenu("Spawn Cubes")]
    public void SpawnCubes() {
        for (var i = -100; i <= 100; i += 10) {
            for (var j = -100; j <= 100; j += 10) {
                var x = Instantiate(cube);
                x.name = $"Cube({i},{j})";
                x.transform.position = new Vector3(i, 0, j);
                x.GetComponent<NetObject>().priority = 1;

                var y = Instantiate(cube);
                y.name = $"Cube2({i},{j})";
                y.transform.position = new Vector3(i - 5, 1, j - 5);
                y.transform.localScale = new Vector3(3, 3, 3);
                y.GetComponent<NetObject>().priority = 0;
            }
        }
    }
}