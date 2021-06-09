using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Mop : Enemies
{
    [SerializeField] private float moveTime;
    [SerializeField] private float moveRadius = 2;
    [SerializeField] private SphereCollider playerDetect;
    private float timer;
    private float randomMoveTime;
    private bool playerRange = false;
    private List<GameObject> playersInRange = new List<GameObject>();
    private GameObject closestPlayer;
    private void Start()
    {
        randomMoveTime = Random.Range(moveTime - 2, moveTime + 2);
    }
    [ServerCallback]
    void Update()
    {
        if (!playerRange)
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
        else if (playerRange)
        {
            float closestplayerDistance = -10000;
            foreach(GameObject player in playersInRange)
            {
                if(closestplayerDistance == -10000)
                {
                    closestplayerDistance = Mathf.Abs(Vector3.Distance(transform.position, player.transform.position));
                    closestPlayer = player;
                }
                else if(closestplayerDistance > Mathf.Abs(Vector3.Distance(transform.position, player.transform.position)))
                {
                    closestPlayer = player;
                    closestplayerDistance = Mathf.Abs(Vector3.Distance(transform.position, player.transform.position));
                }
            }
            ChasePlayer();
        }
    }
    [Server]
    private void ChasePlayer()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(closestPlayer.transform.position, out hit, playerDetect.radius * 2, 1);
        Move(hit.position); 
    }
    [Server]
    private void DetermineNextPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, moveRadius, 1);
        Move(hit.position);
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playersInRange.Add(other.gameObject);
            playerRange = true;
        }
    }
    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playersInRange.Remove(other.gameObject);
        }
        if(playersInRange.Count == 0)
        {
            playerRange = false;
        }
    }

}
