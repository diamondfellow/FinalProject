using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Button : Puzzle
{
    public Color onColor;
    [Server]
    public override void Interacted()
    {
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
