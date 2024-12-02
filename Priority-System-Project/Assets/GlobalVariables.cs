using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance;

    public int highestAssignedPriority = 1;
    public int lowestAssignedPriority = 0;

    public bool seeFrustum = false;
    public bool seePriorities = false;

    private void Awake()
    {
        Instance = this;
    }

}
