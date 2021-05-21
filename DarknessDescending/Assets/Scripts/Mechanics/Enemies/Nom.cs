using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class Nom : Enemies
{
    [SerializeField] private float moveTime;
    private Transform currentPoint;
    private float timer;
    private bool isPointClosed;
    [Server]
    private void Update()
    {
        if (!navAgent.isOnNavMesh) { return; }
        if (!navAgent.hasPath) { timer += Time.deltaTime; }

        if(timer > moveTime)
        {
            FindConnectionPoint();
            timer = 0;
        }
    }
    [Server]
    private void FindConnectionPoint()
    {
        Pathway randomPathway = GameManager.gameMan.allStagePathways[Random.Range(0, GameManager.gameMan.allStagePathways.Count)];
        ConnectionPoint randomPoint = randomPathway.connectionPoints[Random.Range(0,randomPathway.connectionPoints.Length)];
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint.transform.position, out hit, 2, 1);
        Vector3 movePosition = hit.position;
        Debug.Log(movePosition);
        Move(movePosition);
    }
}
