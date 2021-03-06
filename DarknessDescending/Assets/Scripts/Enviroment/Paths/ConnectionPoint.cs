using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);
        Ray ray2 = new Ray(transform.position, transform.rotation * Vector3.right);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(ray2);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }
}
