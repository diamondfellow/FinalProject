using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleList : MonoBehaviour
{
    public static PuzzleList puzzleList;
    public void Awake()
    {
        puzzleList = this;
    }
    public List<Puzzle> EasyPuzzles = new List<Puzzle>();
    List<Puzzle> MediumPuzzles = new List<Puzzle>();
    List<Puzzle> HardPuzzles = new List<Puzzle>();

    //Number is number of players
    List<Puzzle> TwoMediumPuzzles = new List<Puzzle>();
    List<Puzzle> ThreeHardPuzzles = new List<Puzzle>();
    List<Puzzle> FourHardPuzzles = new List<Puzzle>();
}
