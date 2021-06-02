using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Pathway : MonoBehaviour
{
    public ConnectionPoint[] connectionPoints;
    public List<Transform> puzzlePoints = new List<Transform>();
    public Transform monsterSpawn;
    public MeshCollider meshCollider;

    public Bounds PathBounds
    {
        get { return meshCollider.bounds; }
    }
}
