using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Pathway UpStart;
    [SerializeField] private Pathway DownStart;
    [SerializeField] private Pathway RightStart;
    [SerializeField] private Pathway LeftStart;
    [SerializeField] private float gameScaling = .1f; //Amount per stage increase in percent .1 - 1;
    public int NumberofPlayers;

    
    private int puzzlesToBeSolved;
    private int currentStageNumber = 0;
    private bool isSingleplayer = false;
    private int stageFloorType;
    private List<Pathway> allStagePathways = new List<Pathway>();
    private float gameScale;
    [ServerCallback]
    public void Awake()
    {
        if( SceneManager.GetActiveScene().name == "Singleplayer")
        {
            isSingleplayer = true;
        }
        else
        {
            isSingleplayer = false;
        }
    }
    [ServerCallback]
    public void Start()
    {
        StartGame();
    }
    #region MultStartcode
    [Server]
    public void StartGame()
    {
        gameScale = 1;
        gameScaling += gameScaling * NumberofPlayers;
        currentStageNumber = 0;
        BuildMap();
    }
    public void StartStage()
    {
        // allows players to see and move
        currentStageNumber += 1;
    }
    public void StartNewFloor()
    {
        gameScale += gameScaling;
    }
    [ClientRpc]
    public void PuzzleComplete(int puzzlesCompleted)
    {
        puzzlesToBeSolved += puzzlesCompleted;
    }
    #endregion
    #region StageBuild
    [Server]
    public void BuildMap()
    {
        int partsPerSection = 5;
        partsPerSection = Mathf.CeilToInt(partsPerSection * gameScale);
        stageFloorType = Random.Range(0, (FloorList.floorList.numberOfFloorTypes + 1));
        for (int o = 0; o <= 3; o++)
        {
            for (int i = 0; i <= partsPerSection; i++)
            {
                switch (o)
                {
                    case 0:
                        PlaceFloorObjects(partsPerSection, UpStart);
                        break;
                    case 1:
                        PlaceFloorObjects(partsPerSection, RightStart);
                        break;
                    case 2:
                        PlaceFloorObjects(partsPerSection, DownStart);
                        break;
                    case 3:
                        PlaceFloorObjects(partsPerSection, LeftStart);
                        break;
                }
            }
        }
        StartStage();
    }
    [Server]
    public void PlaceFloorObjects(int partsPerSection, Pathway pathComponent)
    {
        List<Pathway> currentSectionPathways = new List<Pathway>();

        currentSectionPathways.Add(pathComponent);
        for (int i = 0; i < partsPerSection; i++)
        {
            Pathway nextPlacement = currentSectionPathways[Random.Range(0, (currentSectionPathways.Count + 1))];
            GameObject NewestObject = Instantiate(FloorList.floorList.RandomFloorType(stageFloorType), nextPlacement.transform.position, Quaternion.identity);
            // attatch to nextPlacement
            if (NewestObject.GetComponent<Pathway>().IsHitting())
            {
                PlaceEndCap(nextPlacement, -1);
                currentSectionPathways.Remove(nextPlacement);
                Destroy(NewestObject);
            }
            else
            {
                currentSectionPathways.Add(NewestObject.GetComponent<Pathway>());
                allStagePathways.Add(NewestObject.GetComponent<Pathway>());
            }
            if (!nextPlacement.IsOpen())
            {
                currentSectionPathways.Remove(nextPlacement);
            }

        }
        foreach (Pathway openPath in currentSectionPathways)
        {
            List<int> transfer = openPath.FindOpens();
            for (int i = 0; i < transfer.Count; i++)
            {
                PlaceEndCap(openPath, transfer[i]);
            }
        }
        PlacePuzzles();
    }
    [Server]
    public void PlaceEndCap(Pathway EndPath, int direction)
    {
        GameObject EndCap =Instantiate(FloorList.floorList.EndCap(stageFloorType), new Vector3(-1000, -1000, -1000), Quaternion.identity);
        NetworkServer.Spawn(EndCap);
        // Attatch to EndPath
    }
    #endregion
    #region PuzzlePlace
    [Server]
    public void PlacePuzzles()
    {
        puzzlesToBeSolved = Mathf.CeilToInt(NumberofPlayers * gameScaling);
        List<Pathway> puzzlePathways = allStagePathways;
        for (int i = 0; i < puzzlesToBeSolved; i++)
        {

            Transform randomPathway = puzzlePathways[Random.Range(0, allStagePathways.Count + 1)].transform;
            GameObject puzzle = Instantiate(PuzzleList.puzzleList.EasyPuzzles
                [Random.Range(0, PuzzleList.puzzleList.EasyPuzzles.Count + 1)].gameObject, randomPathway);
            NetworkServer.Spawn(puzzle);
            puzzle.transform.position += randomPathway.GetComponent<Pathway>().puzzlePoints
                [Random.Range(0, randomPathway.GetComponent<Pathway>().puzzlePoints.Count + 1)];
        }
    }

    // For extraPuzzles
    float FourProb;
    float threeProb;
    float twoProb;
    float HardProb;
    float MediumProb;
    float EasyProb;
    public void PuzzleProbabiltys()
    {
        FourProb = 0;
        threeProb = 0;
        twoProb = 0;
        HardProb = 0;
        MediumProb = 0;
        EasyProb = 0;
        if (NumberofPlayers == 4)
        {
            FourProb = (.01f * gameScale);
            threeProb = (.03f * gameScale);
            twoProb = (.05f * gameScale);
        }
        else if (NumberofPlayers == 3)
            {
            threeProb = (.01f * gameScale);
            twoProb = (.01f * gameScale);
        }
        else if (NumberofPlayers == 3)
        {
            FourProb = (.01f * gameScale);
        }
        else
        {

        }
    }
    #endregion
    // \/ Deprecated
    #region SinglePlayer

    [Server]
    public void SinBuildMap()
    {
        int partsPerSection = 5;
        partsPerSection = Mathf.CeilToInt(partsPerSection * gameScale);
        stageFloorType = Random.Range(0, (FloorList.floorList.numberOfFloorTypes + 1));
        for (int o = 0; o <= 3; o++)
        {
            for (int i = 0; i <= partsPerSection; i++)
            {
                switch (o)
                {
                    case 0:
                        PlaceFloorObjects(partsPerSection, UpStart);
                        break;
                    case 1:
                        PlaceFloorObjects(partsPerSection, RightStart);
                        break;
                    case 2:
                        PlaceFloorObjects(partsPerSection, DownStart);
                        break;
                    case 3:
                        PlaceFloorObjects(partsPerSection, LeftStart);
                        break;
                }
            }
        }
        StartStage();
    }
    public void SinPlaceFloorObjects(int partsPerSection, Pathway pathComponent)
    {
        List<Pathway> currentSectionPathways = new List<Pathway>();

        currentSectionPathways.Add(pathComponent);
        for (int i = 0; i < partsPerSection; i++)
        {
            Pathway nextPlacement = currentSectionPathways[Random.Range(0, (currentSectionPathways.Count + 1))];
            GameObject NewestObject = Instantiate(FloorList.floorList.RandomFloorType(stageFloorType), nextPlacement.transform.position, Quaternion.identity);
            // attatch to nextPlacement
            if (NewestObject.GetComponent<Pathway>().IsHitting())
            {
                PlaceEndCap(nextPlacement, -1);
                currentSectionPathways.Remove(nextPlacement);
                Destroy(NewestObject);
            }
            else
            {
                currentSectionPathways.Add(NewestObject.GetComponent<Pathway>());
                allStagePathways.Add(NewestObject.GetComponent<Pathway>());
            }
            if (!nextPlacement.IsOpen())
            {
                currentSectionPathways.Remove(nextPlacement);
            }

        }
        foreach (Pathway openPath in currentSectionPathways)
        {
            List<int> transfer = openPath.FindOpens();
            for (int i = 0; i < transfer.Count; i++)
            {
                PlaceEndCap(openPath, transfer[i]);
            }
        }
        PlacePuzzles();
    }
    public void SinPlaceEndCap(Pathway EndPath, int direction)
    {
        Instantiate(FloorList.floorList.EndCap(stageFloorType), new Vector3(-1000, -1000, -1000), Quaternion.identity);
        // Attatch to EndPath
    }
    #endregion 
}
