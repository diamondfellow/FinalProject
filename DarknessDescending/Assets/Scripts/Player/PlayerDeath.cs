using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerDeath : NetworkBehaviour
{
    // DEPACATED No USE
    public bool isDead = false;
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == ("Monster"))
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            isDead = true;
            RpcKillPlayer();
            
        }
    }
    [ClientRpc]
    private void RpcKillPlayer()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }
}
