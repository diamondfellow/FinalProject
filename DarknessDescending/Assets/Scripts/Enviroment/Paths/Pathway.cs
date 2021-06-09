using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pathway : NetworkBehaviour
{
    public ConnectionPoint[] connectionPoints;
    public List<Transform> puzzlePoints = new List<Transform>();
    public Transform monsterSpawn;
    public BoxCollider pathCollider;
    public MeshCollider meshCollider;
    public bool isColliding = false;
    public LayerMask pathLayer;

    public Bounds PathBounds
    {
        get { return meshCollider.bounds; }
    }
    [ServerCallback]
    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 8)
        {
            Debug.Log("isonlayer");
            
              
                
            
            
            
                Debug.Log("Iscoll");
                isColliding = true;
            
        }
        return;
    }
}
