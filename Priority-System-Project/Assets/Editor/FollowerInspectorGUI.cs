using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Follower))]
// ^ This is the script we are making a custom editor for.
public class FollowerInspectorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        //Called whenever the inspector is drawn for this object.
        DrawDefaultInspector();
        //This draws the default screen.  You don't need this if you want
        //to Start from Scratch, but I use this when I'm just adding a button or
        //some small addition and don't feel like recreating the whole inspector.

        Follower follower = (Follower)target;

        if (GUILayout.Button("Calculate Steps"))
        {
            follower.SaveStepPositions();
        }
    }
}

