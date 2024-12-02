using Network.Objects;
using PathCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Follower : MonoBehaviour {
    public PathCreator pathCreator;
    public float speed = 5;

    private float _distanceTravelled;

    public bool waitForCompleteSceneLoading;

    private float waitTimer;
    private float waitTime = 120f;

    private int _steps = 120;
    [SerializeField]
    private Vector3[] _segments = new Vector3[120];
    [SerializeField]
    private Vector3[] _rotations = new Vector3[120];
    [SerializeField]
    private Vector3[] _localRotations = new Vector3[120];
    private float _stepLength;
    private int _currentStep = 0;
    public event Action OnStepMade;
    public event Action OnPathCompleted;
    private bool pathCompleted = false;
    
    private bool _stop;

    public void Start()
    {
        var moveTestData = Resources.Load<TextAsset>("moveTestData");
        string dataString = moveTestData.text;
        List<string> data = dataString.Replace("(", string.Empty).Replace(")", string.Empty).Trim().Split(';').ToList();
        //Debug.Log($"data is {data[0]}, datacount is {data.Count}");
        for(int i = 0; i < 120; i++)
        {
            string[] segment = data[i*3].Split(',');
            string[] rotation = data[i * 3 + 1].Split(',');
            string[] localRotation = data[i * 3 + 2].Split(',');

            System.Globalization.CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            if (culture.NumberFormat.NumberDecimalSeparator == ",")
            {
                for (int k = 0; k < 3; k++)
                {
                    segment[k] = segment[k].Replace('.', ',');
                    rotation[k] = rotation[k].Replace('.', ',');
                    localRotation[k] = localRotation[k].Replace('.', ',');
                }
            }

            //Debug.Log($"segment {i} is {segment[0]}");
            _segments[i] = new Vector3(float.Parse(segment[0]), float.Parse(segment[1]), float.Parse(segment[2]));
            _rotations[i] = new Vector3(float.Parse(rotation[0]), float.Parse(rotation[1]), float.Parse(rotation[2]));
            _localRotations[i] = new Vector3(float.Parse(localRotation[0]), float.Parse(localRotation[1]), float.Parse(localRotation[2]));
        }

        if (!waitForCompleteSceneLoading)
            waitTime = 3;

        //Debug.Log($"Path length = {pathCreator.path.length}, step length = {_stepLength}");
        waitTimer = waitTime;

    }

    void Update() {
        if (NetworkManager.Singleton == null
            || !NetworkManager.Singleton.IsClient
            || !NetworkManager.Singleton.IsConnectedClient
            || !Logger.Move
            || pathCompleted) return;
        
        if (_distanceTravelled > pathCreator.path.length || _currentStep >= _steps - 1)
        {
            pathCompleted = true;
            OnPathCompleted?.Invoke();
            return;
        }

        if (waitTimer >= 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        if (_stop)
            return;

        var t = transform;
        if (Vector3.Distance(t.position, _segments[_currentStep]) <= 1f && Logger.instance.clientScreen)
        {
            StartCoroutine(StepMade());
            //Debug.Log($"New step made, current step = {_currentStep}");
            _stop = true;
        }

        if (_stop)
            return;

        _distanceTravelled += speed * Time.deltaTime;
        
        t.position = pathCreator.path.GetPointAtDistance(_distanceTravelled); 
        t.rotation = pathCreator.path.GetRotationAtDistance(_distanceTravelled);
        t.localRotation = Quaternion.Euler(t.localRotation.eulerAngles + new Vector3(0, 0, 90));

        /*
        //Call on first frame to get initial perspective
        if (_distanceSinceLastStep <= 0 && _currentStep == 0)
            OnStepMade?.Invoke();

        //Call at all the other perspectives
        if (_distanceSinceLastStep >= _stepLength)
        {
            _distanceSinceLastStep = 0;
            _currentStep++;
            if (_currentStep <= _steps)
            {
                OnStepMade?.Invoke();
            }
        }
        */
    }

    private IEnumerator StepMade()
    {
        for (int i = 0; i < 3; i++) yield return null;

        Quaternion currentRotation = transform.rotation;
        Quaternion currentLocalRotation = transform.localRotation;
        Vector3 currentPosition = transform.position;

        transform.position = _segments[_currentStep];
        transform.rotation = Quaternion.Euler(_rotations[_currentStep]);
        transform.localRotation = Quaternion.Euler(_localRotations[_currentStep]);
        //Debug.Log($"_rotations of step {_currentStep} is {_rotations[_currentStep]}, localRot is {_localRotations[_currentStep]} ");
        OnStepMade?.Invoke();

        for (int i = 0; i < 3; i++) yield return null;

        //Debug.Log($"resetting to position: currentStep = {_currentStep}, frame count = {Time.frameCount}");
        _currentStep++;
        transform.position = currentPosition;
        transform.rotation = currentRotation;
        transform.localRotation = currentLocalRotation;
        _stop = false;
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    public void SaveStepPositions()
    {
        _stepLength = pathCreator.path.length / _steps;
        string toText = "";
        for (int i = 0; i < _steps; i++)
        {
            Vector3 point = pathCreator.path.GetPointAtDistance(_stepLength * i);
            point.x = (float)Math.Round(point.x, 2);  
            point.y = (float)Math.Round(point.y, 2);  
            point.z = (float)Math.Round(point.z, 2);
            _segments[i] = point;

            _rotations[i] = pathCreator.path.GetRotationAtDistance(_stepLength * i).eulerAngles;
            _rotations[i] = new Vector3((int)_rotations[i].x, (int)_rotations[i].y, (int)_rotations[i].z);
            _localRotations[i] = Quaternion.Euler(_rotations[i] + new Vector3(0, 0, 90)).eulerAngles;
            _localRotations[i] = new Vector3((int)_localRotations[i].x, (int)_localRotations[i].y, (int)_localRotations[i].z);

            toText += _segments[i].ToString() + ";" + _rotations[i] + ";" + _localRotations[i] + ";"; 
        }

        File.Delete(Application.dataPath + "/Resources/moveTestData.txt");
        File.WriteAllText(Application.dataPath + "/Resources/moveTestData.txt", toText);
        AssetDatabase.Refresh();
    }
#endif
}