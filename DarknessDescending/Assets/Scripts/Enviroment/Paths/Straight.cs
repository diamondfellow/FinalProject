using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Straight : Pathway
{
    void Awake()
    {
        openPaths[0] = true;
        openPaths[2] = true;
    }

    // Update is called once per frame
    public override int FindConnectPoint(int pointToConnectTo)
    {
        switch (pointToConnectTo)
        {
            case 0:
                Rotate(0);
                break;
            case 1:
                Rotate(1);
                break;
            case 2:
                Rotate(2);
                break;
            case 3:
                Rotate(3);
                break;
            default:
                Debug.Log("Out of Array Connection Point");
                return -1;
        }
        //gameObject.transform.
        return -1;
    }
    public override void Rotate(int desiredOrientaion)
    {
        Vector3 transfer = transform.eulerAngles;
        switch (desiredOrientaion)
        {
            case 0:
                transfer = new Vector3(transfer.z, 0, transfer.z);
                orientaion = 0;
                switch (orientaion)
                {
                    case 1:
                        connectionPoints[0] = connectionPoints[1];
                        connectionPoints[2] = connectionPoints[3];

                        connectionPoints[1] = null;
                        connectionPoints[3] = null;
                        break;
                    case 2:
                        connectionPoints[0] = connectionPoints[1];
                        connectionPoints[2] = connectionPoints[3];

                        connectionPoints[1] = null;
                        connectionPoints[3] = null;
                        break;
                    case 3:
                        connectionPoints[0] = connectionPoints[1];
                        connectionPoints[2] = connectionPoints[3];

                        connectionPoints[1] = null;
                        connectionPoints[3] = null;
                        break;
                }
                break;
            case 1:
                transfer = new Vector3(transfer.z, 90, transfer.z);
                orientaion = 1;
                switch (orientaion)
                {

                }
                break;
            case 2:
                transfer = new Vector3(transfer.z, 180, transfer.z);
                orientaion = 2;
                switch (orientaion)
                {

                }
                break;
            case 3:
                transfer = new Vector3(transfer.z, 270, transfer.z);
                orientaion = 3;
                switch (orientaion)
                {

                }
                break;
        }
        transform.rotation = Quaternion.Euler(transfer);
    }




}

