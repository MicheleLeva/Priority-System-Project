using PathCreation;
using Unity.Netcode;
using UnityEngine;

public class Follower : MonoBehaviour {
    public PathCreator pathCreator;
    public float speed = 5;

    private float _distanceTravelled;

    void Update() {
        if (NetworkManager.Singleton == null
            || !NetworkManager.Singleton.IsClient
            || !NetworkManager.Singleton.IsConnectedClient
            || !Logger.Move
            || _distanceTravelled > pathCreator.path.length) return;

        _distanceTravelled += speed * Time.deltaTime;
        var t = transform;
        t.position = pathCreator.path.GetPointAtDistance(_distanceTravelled);
        t.rotation = pathCreator.path.GetRotationAtDistance(_distanceTravelled);
        t.localRotation = Quaternion.Euler(t.localRotation.eulerAngles + new Vector3(0, 0, 90));
    }
}