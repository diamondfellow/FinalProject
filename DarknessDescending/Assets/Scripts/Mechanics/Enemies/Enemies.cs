using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Enemies : NetworkBehaviour
{
    public NavMeshAgent navAgent;
    public float moveSpeed;
    public float turnSpeed;
    [Server]
    public void Awake()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
    }
    [Server]
    public virtual void Move(Vector3 movePosition)
    {
        navAgent.SetDestination(movePosition);
    }
    

}
