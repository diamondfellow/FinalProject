using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathway : MonoBehaviour
{
    public ConnectionPoint[] connectionPoints;
    public MeshCollider meshCollider;

    [SerializeField] Bounds PathBounds
    {
        get { return meshCollider.bounds; }
    }
}
