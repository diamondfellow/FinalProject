using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Enemies : NetworkBehaviour
{
    public NavMeshAgent navAgent;
    [ServerCallback]
    public void Mop()
    {

    }
}
