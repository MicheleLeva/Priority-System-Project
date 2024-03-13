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

    private int _steps = 60;
    private float _stepLength;
    private float _distanceSinceLastStep;
    private int _currentStep = 0;
    public event Action OnStepMade;
    public event Action OnPathCompleted;
    private bool pathCompleted = false;

    public void Start()
    {
        _stepLength = pathCreator.path.length / _steps;
        Debug.Log($"Path length = {pathCreator.path.length}, step length = {_stepLength}");
        _distanceSinceLastStep = 0;
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

        if (waitForCompleteSceneLoading && FindObjectsOfType<NetObject>().Length < 490) return;

        _distanceTravelled += speed * Time.deltaTime;
        _distanceSinceLastStep += speed * Time.deltaTime;
        var t = transform;
        t.position = pathCreator.path.GetPointAtDistance(_distanceTravelled);
        t.rotation = pathCreator.path.GetRotationAtDistance(_distanceTravelled);
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