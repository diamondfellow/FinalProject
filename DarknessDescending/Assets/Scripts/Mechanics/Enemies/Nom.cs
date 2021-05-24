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
    private List<GameObject> playersNear = new List<GameObject>();

    private float playerDistance = 1000f;
    private GameObject playerHolder;
    [Server]
    private void Update()
    {
        if (!navAgent.isOnNavMesh) { return; }
        if (!navAgent.hasPath) { timer += Time.deltaTime; }
        /*
        if (!navAgent.hasPath)
        {
            if(playersNear.Count == 0)
            {
                playerDistance = 1000f;
                return;
            }
            else
            {
                foreach(GameObject player in playersNear)
                {
                    float check = Vector3.Distance(transform.position, player.transform.position);
                    if(check < playerDistance)
                    {
                        playerHolder = player;
                        playerDistance = check;
                    }
                }
                transform.rotation = Quaternion.Euler(Vector3.RotateTowards(transform.rotation.eulerAngles, playerHolder.transform.position, 2 * Mathf.PI, 80));
            }
        }
        */
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
    /*
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playersNear.Add(other.gameObject);
        }
    }
    [ServerCallback]
    private void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playersNear.Remove(collision.gameObject);
        }
    }
    */
}
