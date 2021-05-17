using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Mop : Enemies
{
    [SerializeField] private float moveTime;
    [SerializeField] private float moveRadius = 2;
    private float timer;
    private float randomMoveTime;
    private void Start()
    {
        randomMoveTime = Random.Range(moveTime - 2, moveTime + 2);
    }
    [ServerCallback]
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > randomMoveTime)
        {
            DetermineNextPosition();
            timer = 0;
            randomMoveTime = Random.Range(moveTime - 2, moveTime + 2);
        }
        if (navAgent.hasPath && !GetComponent<AudioSource>().isPlaying)
        {
            GameManager.gameMan.RpcPlaySound(gameObject, "MopMove");
        }
        else if (!navAgent.hasPath)
        {
            GameManager.gameMan.RpcStopSound(gameObject);
        }
    }
    [Server]
    private void DetermineNextPosition()
    {
        Vector3 randomDirection = Vector3.zero;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, moveRadius, 1);
        Move(hit.position);
    }
    
}
