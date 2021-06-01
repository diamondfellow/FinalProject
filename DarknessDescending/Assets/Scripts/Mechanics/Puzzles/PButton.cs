using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PButton : Puzzle
{
    public Color onColor;
    [SerializeField] private GameObject lightObject;
    [Server]
    public override void Interacted()
    {
        base.Interacted();
        GameManager.gameMan.RpcPlaySound(gameObject, "ButtonClick");
        ButtonPressed();
        RpcChangeButtonColor();  
    }
    [ClientRpc]
    public void RpcChangeButtonColor()
    {
        lightObject.GetComponent<MeshRenderer>().material.color = onColor;
    }
    [Server]
    public void ButtonPressed()
    {
        GameManager.gameMan.PuzzleComplete(1);
    }
}
