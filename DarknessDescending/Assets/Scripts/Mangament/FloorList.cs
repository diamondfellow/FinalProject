using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorList : MonoBehaviour
{
    public static FloorList floorList;

    public List<GameObject> DefaultFloorObjects = new List<GameObject>();

    public  int numberOfFloorTypes = 1;
    public void Awake()
    {
        floorList = this;
    }
    public GameObject RandomPathway(int floortype)
    {
        switch (floortype)
        {
            case 1:
                return DefaultFloorObjects[Random.Range(0, (DefaultFloorObjects.Count))];
            case 2:
                return DefaultFloorObjects[Random.Range(0, (DefaultFloorObjects.Count))];
        }
        return null;
    }
}
