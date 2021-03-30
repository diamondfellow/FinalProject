using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour
{
    public bool[] openPaths = new bool[4]; // 0 up 1 right 2 down 3 left
    public Vector3[] connectionPoints = new Vector3[4];
    public int orientaion; // 0 original 1 rotate to the right 90// 2 rotate 180 3 rotate 270
    public bool isConnected;
    public virtual void Rotate()
    {

    }
    public virtual int FindUp()
    {
        return -1;
    }
    public virtual int FindRight()
    {
        return -1;
    }
    public virtual int FindDown()
    {
        return -1;
    }
    public virtual int FindLeft()
    {
        return -1;
    }
}
