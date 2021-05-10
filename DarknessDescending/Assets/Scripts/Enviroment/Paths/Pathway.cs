using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour
{
    public bool[] openPaths = new bool[4]; // 0 Front 1 right 2 Back 3 left
    public Transform[] connectionPoints = new Transform[4];
    public int orientaion; // 0 original 1 rotate to the right 90// 2 rotate 180 3 rotate 270
    public bool isConnected;
    public Transform monsterSpawnPosition;
    //public List<GameObject> navObjects = new List<GameObject>();

    private bool hittingOtherPath = false;
    public List<GameObject> puzzlePoints = new List<GameObject>();
    


    public virtual List<int> FindOpens()
    {
        List<int> Opens = new List<int>();
        for (int i = 0; i <= 4; i++)
        {
            if (openPaths[i])
            {
                Opens.Add(i);
            }
        }
        return Opens;
    }
    public virtual bool IsOpen()
    {
        foreach(bool open in openPaths)
        {
            if (open)
            {
                return true;
            }
            continue;
        }
        return false;
    }
    public virtual void Rotate(int desiredOrientaion)
    {
        Debug.Log("SetAsPathway");
        return;
    }
    //Connection Point to Connect to from next placement
    public virtual int FindConnectPoint(int pointToConnectTo)
    {
        return -1;
    }
    public bool IsHitting()
    {
        return hittingOtherPath;
    }
    void OnTriggerStay(Collider other)
    {
       if(other.tag == "PathCollision" || other.tag == "Hub")
        {
            hittingOtherPath = true;
        } 
    }
}
