using Network.Objects;
using PathCreation;
using System;
using Unity.Netcode;
using UnityEngine;

public class Follower : MonoBehaviour {
    public PathCreator pathCreator;
    public float speed = 5;

    private float _distanceTravelled;

    public bool waitForCompleteSceneLoading;
    private float waitTimer;
    private float waitTime = 120f;

    private int _steps = 60;
    private float _stepLength;
    private float _distanceSinceLastStep;
    private int _currentStep = 0;
    public event Action OnStepMade;
    public event Action OnPathCompleted;
    private bool pathCompleted = false;

    private float yPosition;

    public void Start()
    {
        _stepLength = pathCreator.path.length / _steps;
        Debug.Log($"Path length = {pathCreator.path.length}, step length = {_stepLength}");
        _distanceSinceLastStep = 0;
        yPosition = transform.position.y;
        Debug.Log($"yPosition is {yPosition}");
        waitTimer = waitTime;
    }

    void Update() {
        if (NetworkManager.Singleton == null
            || !NetworkManager.Singleton.IsClient
            || !NetworkManager.Singleton.IsConnectedClient
            || !Logger.Move
            || pathCompleted) return;
        
        if (_distanceTravelled > pathCreator.path.length)
        {
            pathCompleted = true;
            OnPathCompleted?.Invoke();
        }

        if (waitForCompleteSceneLoading)
        {
            if (waitTimer >= 0)
            {
                waitTimer -= Time.deltaTime;
                return;
            }
        }

        _distanceTravelled += speed * Time.deltaTime;
        _distanceSinceLastStep += speed * Time.deltaTime;
        var t = transform;
        t.position = pathCreator.path.GetPointAtDistance(_distanceTravelled); 
        t.rotation = pathCreator.path.GetRotationAtDistance(_distanceTravelled);
        //if we have disabled the trackedposedriver we set the vertical offset of the player camera from the floor to the one recored on Start
        /*if (!FindObjectOfType<UnityEngine.InputSystem.XR.TrackedPoseDriver>().enabled)
            t.position = new Vector3(t.position.x, yPosition, t.position.z);*/
        t.localRotation = Quaternion.Euler(t.localRotation.eulerAngles + new Vector3(0, 0, 90));

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
    }
}