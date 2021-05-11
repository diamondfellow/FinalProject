using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class GameManager : NetworkBehaviour
{
    public static GameManager gameMan;

    public int NumberofPlayers;
    

    [SerializeField] private GameUI gameUI;
    [SerializeField] private Pathway FrontStart;
    [SerializeField] private Pathway BackStart;
    [SerializeField] private Pathway RightStart;
    [SerializeField] private Pathway LeftStart;
    [SerializeField] private Transform[] spawnPositions = new Transform[4];
    [SerializeField] private GameObject[] doorButtons = new GameObject[4];
    [SerializeField] private NavMeshSurface hubFloor;

    [SerializeField] private float gameScaling = .1f; //Amount per stage increase in percent .1 - 1;
    [SerializeField] private float stageEndTimer = 20f;


    private LayerMask roomLayerMask;
    private float timer;
    private bool stageEnding = false;
    private int puzzlesToBeSolved = 0;
    private int puzzlesSolved = 0;
    private int currentStageNumber = 0;
    private int stageFloorType;
    private List<Pathway> allStagePathways = new List<Pathway>();
    private List<ConnectionPoint> sectionConnectionPoints = new List<ConnectionPoint>();
    private float gameScale;

    [ServerCallback]
    public void Awake()
    {
        gameMan = this;
        gameScale = 1;
        timer = stageEndTimer;
    }
    [ServerCallback]
    public void Start()
    {
        hubFloor.collectObjects = CollectObjects.Children;
        StartGame();
    }
    [ServerCallback]
    public void Update()
    {
        //NumberofPlayers = 
        if (stageEnding)
        {
            timer -= Time.deltaTime;
            RpcEndingUpdate(timer);
            if(timer < 0)
            {
                foreach(GameObject doorButton in doorButtons)
                {
                    GetComponent<DoorButton>().Close();
                }
                StartNewStage();
                stageEnding = false;
                timer = stageEndTimer;
            }
        }
    }

    [Server]
    public void PuzzleComplete(int puzzlesCompleted)
    {
        puzzlesSolved += puzzlesCompleted;
        if (puzzlesSolved >= puzzlesToBeSolved)
        {
            stageEnding = true;
            RpcEndingUpdate(timer);
        }
        RpcUpdatePuzzleUI(puzzlesSolved, puzzlesToBeSolved);
    }
    [ClientRpc]
    private void RpcUpdatePuzzleUI(int puzzles, int totalPuzzles)
    {
        gameUI.puzzleText.text = ("Puzzles Completed: " + puzzles + "/" + totalPuzzles);
    }
    [ClientRpc]
    private void RpcEndingUpdate(float time)
    {
        gameUI.endingText.enabled = true;
        gameUI.endingText.text = ("GET BACK TO THE CENTER \n TIME LEFT: " + Mathf.Round(time));
    }
    [ClientRpc]
    private void RpcStageUpdate()
    {
        gameUI.stageText.text = ("Stage: " + currentStageNumber);
    }



    #region StageStartingOrEndingCode
    // Ran Once when GAme first Loaded 0
    [Server]
    public void StartGame()
    {
        gameScale = 1;
        gameScale += gameScaling;
        currentStageNumber = 0;
        StartCoroutine(nameof(BuildMap));
    }


    // Ran at end of stage building 5
    [Server]
    public void StartStage()
    {
        foreach (NetworkConnection conn in NetworkMan.Players)
        {
            TgtAllowPlayer(conn);
        }
        currentStageNumber += 1;
        RpcStageUpdate();
    }

    //Ran When Previous Stage Ended 6
    [Server]
    public void StartNewStage()
    {
        int ii = 0;
        foreach(NetworkConnection plyrConn in NetworkMan.Players)
        {
            TgtAllowPlayer(plyrConn);
            TgtPlayerMoveSpawn(plyrConn, ii);
            ii++;
        }
        gameScale += gameScaling;
        foreach(Pathway pathway in allStagePathways)
        {
            Destroy(pathway.gameObject);
            NetworkServer.Destroy(pathway.gameObject);
        }
        allStagePathways.Clear();
        StartCoroutine(nameof(BuildMap));
    }

    [TargetRpc]
    private void TgtAllowPlayer(NetworkConnection player)
    {
        player.identity.gameObject.GetComponent<Player>().SeeMove();
    }
    [TargetRpc]
    private void TgtPlayerMoveSpawn(NetworkConnection conn, int playerNumber)
    {

         conn.identity.gameObject.transform.position = spawnPositions[playerNumber].position;
            

        
    }
    #endregion

    #region BuildingOfStage
    //Place Floor Objects for Every Object 1
    [Server]
    IEnumerator BuildMap()
    {
        WaitForSeconds startup = new WaitForSeconds(1);
        WaitForFixedUpdate interval = new WaitForFixedUpdate();

        yield return startup;


        int partsPerSection = 3;
        partsPerSection = Mathf.CeilToInt(partsPerSection * gameScale * NumberofPlayers);
        stageFloorType = Random.Range(0, (FloorList.floorList.numberOfFloorTypes));
        for (int o = 0; o < 3; o++)
        {
              sectionConnectionPoints = new List<ConnectionPoint>();
              switch (o)
              {
                    case 0:                   
                    for (int i1 = 0; i1 < partsPerSection; i1++)
                    {
                        AddConnectionPoints(FrontStart);
                        PlaceFloorObject();
                        yield return interval;
                    }
                    break;
                    case 1:
                    for (int i1 = 0; i1 < partsPerSection; i1++)
                    {
                    }
                    break;
                    case 2:
                    for (int i1 = 0; i1 < partsPerSection; i1++)
                    {

                    }
                    break;
                    case 3:
                    for (int i1 = 0; i1 < partsPerSection; i1++)
                    {

                    }
                    break;
              }          
        }
        StartStage();
    }
    //Picks random object to place and snaps it to random open space. 2
    [Server]
    public void PlaceFloorObject()
    {
        if(sectionConnectionPoints.Count == 0)
        {
            return;
        }
        ConnectionPoint nextPointToPlace = sectionConnectionPoints[Random.Range(0, sectionConnectionPoints.Count)];
        GameObject newPath = Instantiate(FloorList.floorList.RandomFloorObject(stageFloorType), hubFloor.gameObject.transform);
        NetworkServer.Spawn(newPath);

        ConnectionPoint newPathConnPoint = newPath.GetComponent<Pathway>().connectionPoints
            [Random.Range(0, newPath.GetComponent<Pathway>().connectionPoints.Length)];

        PositionNewPath(newPath, newPathConnPoint ,nextPointToPlace);
        if (!NewPathOverlap())
        {
            sectionConnectionPoints.Remove(nextPointToPlace);
            PlaceFloorObject();
            return;
        }
        allStagePathways.Add(newPath.GetComponent<Pathway>());

        nextPointToPlace.gameObject.SetActive(false);
        sectionConnectionPoints.Remove(nextPointToPlace);

        newPathConnPoint.gameObject.SetActive(false);
        sectionConnectionPoints.Remove(newPathConnPoint);
    }
    [Server]
    bool NewPathOverlap()
    {
        return false;
    }
    [Server]
    private void PositionNewPath(GameObject newPath, ConnectionPoint currentPoint, ConnectionPoint pointToPlace)
    {
        newPath.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Vector3 pointToPlaceEuler = pointToPlace.transform.eulerAngles;
        Vector3 currentPointEuler = currentPoint.transform.eulerAngles;
        float deltaAngle = Mathf.DeltaAngle(currentPointEuler, pointToPlaceEuler);
        Quaternion currentPathTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
        newPath.transform.rotation = currentPathTargetRotation * Quaternion.Euler(0, 180f, 0);
    }
    [Server]
    private void AddConnectionPoints(Pathway pathway)
    {
        foreach(ConnectionPoint connpoint in pathway.connectionPoints)
        {
            sectionConnectionPoints.Add(connpoint);
        }
    }
    [Server]
    public void PlacePuzzles()
    {
        
        puzzlesToBeSolved = 3 + (NumberofPlayers * currentStageNumber);
        List<Pathway> puzzlePathways = allStagePathways;
        for (int i = 0; i < puzzlesToBeSolved; i++)
        {

            Transform randomPathway = puzzlePathways[Random.Range(0, allStagePathways.Count + 1)].transform;
           // Transform randomPuzzlePoint = randomPathway.GetComponent<Pathway>().puzzlePoints
             //   [Random.Range(0, randomPathway.GetComponent<Pathway>().puzzlePoints.Count + 1)].transform;
            //GameObject puzzle = Instantiate(PuzzleList.puzzleList.EasyPuzzles
              //  [Random.Range(0, PuzzleList.puzzleList.EasyPuzzles.Count + 1)].gameObject, randomPuzzlePoint);
            //NetworkServer.Spawn(puzzle);
            //puzzle.transform.position = Vector3.zero;
            //puzzle.transform.rotation = randomPuzzlePoint.rotation;

        }
        hubFloor.BuildNavMesh();
        MonsterSpawn();
    }
    // Spawns in Monsters 5
    [Server]
    public void MonsterSpawn()
    {
        int amountOfMonster = 1 + Mathf.FloorToInt((currentStageNumber * NumberofPlayers) / 3);
        for (int i = 0; i < amountOfMonster; i++)
        {
            GameObject monsterSpawn = MonsterList.monsterList.Monsters[Random.Range(0, MonsterList.monsterList.Monsters.Count)];
            //GameObject spawnedMonster = Instantiate(monsterSpawn, allStagePathways[Random.Range(0, allStagePathways.Count + 1)].monsterSpawnPosition, Quaternion.identity);
            //NetworkServer.Spawn(spawnedMonster);
        }
        StartStage();
    }

    #endregion

    //If more then one puzzle
    #region PuzzlePlace


    /* For extraPuzzles
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
    */
    #endregion
}
