using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Interactables : NetworkBehaviour
{
    [Server]
    public virtual void Interacted()
    {
    }
}
