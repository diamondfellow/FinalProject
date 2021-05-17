using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PButton : Puzzle
{
    public Color onColor;
    [Server]
    public override void Interacted()
    {
        GameManager.gameMan.RpcPlaySound(gameObject, "ButtonClick");
        base.Interacted();
        ButtonPressed();
        ChangeButtonColor();
    }
    [ClientRpc]
    public void ChangeButtonColor()
    {
        gameObject.GetComponent<Material>().color = onColor;
    }
    [Server]
    private void ButtonPressed()
    {
        gameMangaer.PuzzleComplete(1);
    }

   
}
