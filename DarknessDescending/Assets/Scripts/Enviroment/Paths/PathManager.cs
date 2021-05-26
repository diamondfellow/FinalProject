using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PathManager : NetworkBehaviour
{
    [ClientRpc]
    public void RpcSetConnPointOff(int connPointNumber)
    {
        GetComponent<Pathway>().connectionPoints[connPointNumber].gameObject.SetActive(false);

    }
}
