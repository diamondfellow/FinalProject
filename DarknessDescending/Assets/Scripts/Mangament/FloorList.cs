using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorList : MonoBehaviour
{
    public static FloorList floorList;
    

    public List<GameObject> DebugFloorObjects = new List<GameObject>();



    public int numberOfFloorTypes = 1;
    public void Awake()
    {
        floorList = this;
    }
    public GameObject RandomFloorType(int floortype)
    {
        switch (floortype)
        {
            case 1:
                return DebugFloorObjects[Random.Range(0, (DebugFloorObjects.Count))];
            case 2:
                return DebugFloorObjects[Random.Range(0, (DebugFloorObjects.Count))];
        }
        return null;
    }
    public GameObject EndCap(int floorType)
    {
        switch (floorType)
        {
            case 1:
                return DebugFloorObjects[DebugFloorObjects.Count];
            case 2:
                return DebugFloorObjects[DebugFloorObjects.Count];
        }
        return null;
    }
}
