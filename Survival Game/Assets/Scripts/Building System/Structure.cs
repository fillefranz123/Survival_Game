using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Structure : MonoBehaviour
{

    public enum Structures {none, floor, wall, roof, ramp };
    public Structures structure = Structures.floor;
    public SnapPoint[] snapPoints;
    public bool canBePlacedOnGround = false;

    public Vector3 placementOffset = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public struct SnapPoint
{
    public GameObject snapPoint;
    public SnapGroup[] snapGroups;
}

[Serializable]
public struct SnapGroup
{
    public Structure.Structures[] compatibleStructures;
    public bool isOccupied;
}
