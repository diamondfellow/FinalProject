using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class GameManager : NetworkBehaviour
{
    public static GameManager gameMan;

    [HideInInspector]
    public int NumberofPlayers;
    public bool endingDoors;

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

    private LayerMask pathLayerMask;
    private LayerMask playerMask;
    private float timer;
    private float noiseTimer;
    private bool stageEnding = false;
    private int puzzlesToBeSolved = 0;
    private int puzzlesSolved = 0;
    private int currentStageNumber = 0;
    private int stageFloorType;
    private List<Pathway> allStagePathways = new List<Pathway>();
    private List<GameObject> allStagePuzzles = new List<GameObject>();
    private List<GameObject> allStageMonsters = new List<GameObject>();
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
        noiseTimer += Time.deltaTime;
        int check = Random.Range(Mathf.FloorToInt(noiseTimer), 101);
        if(check == 100)
        {

        }
        //NumberofPlayers = 
        if (stageEnding)
        {
            timer -= Time.deltaTime;
            RpcEndingUpdate(timer);
            if(timer < 0)
            {
                endingDoors = true;
                foreach(GameObject doorButton in doorButtons)
                {
                    doorButton.GetComponent<DoorButton>().StartCoroutine("Close");
                }
            }
        }
    }
    [Server]
    public void DoorsClosed()
    {
        CheckForPlayers();
        endingDoors = false;
        StartNewStage();
        stageEnding = false;
        timer = stageEndTimer;
    }

    [Server]
    public void PuzzleComplete(int puzzlesCompleted)
    {
        puzzlesSolved += puzzlesCompleted;
        if (puzzlesSolved >= puzzlesToBeSolved)
        {
            Debug.Log("puzzle complete");
            stageEnding = true;
            foreach(NetworkConnection conn in NetworkMan.Players)
            {
                RpcPlaySound(conn.identity.gameObject, "EndStageAlert");
            }
            RpcEndingUpdate(timer);
        }
        RpcUpdatePuzzleUI(puzzlesSolved, puzzlesToBeSolved);
    }

    #region Sound
    [ClientRpc]
    public void RpcStopSound(GameObject soundObject)
    {
        soundObject.GetComponent<AudioSource>().Stop();
    }
    [Server]
    private void RandomScaryNoise()
    {
        int playerNumber = Random.Range(0, NetworkMan.Players.Count);
        NetworkConnection player = NetworkMan.Players[playerNumber];
        string noiseID = "ScaryNoise0" + Random.Range(1, 5);
        TgtPlayPrivateSound(player, noiseID);
    }
    [TargetRpc]
    public void TgtPlayPrivateSound(NetworkConnection conn, string soundID)
    {
        AudioSource source = conn.identity.gameObject.GetComponent<AudioSource>();
        source.clip = SoundList.soundList.GetAudioClip(soundID);
        source.Play();
    }
    [ClientRpc]
    public void RpcPlaySound(GameObject soundObject, string soundID)
    {
        AudioSource source = soundObject.GetComponent<AudioSource>();
        source.clip = SoundList.soundList.GetAudioClip(soundID);
        source.Play();
    }
    #endregion
    #region Rpcs
    [ClientRpc]
    private void RpcUpdatePuzzleUI(int puzzles, int totalPuzzles)
    {
        gameUI.puzzleText.text = ("Puzzles Completed: " + puzzles + "/" + totalPuzzles);
    }
    [ClientRpc]
    private void RpcEndingUpdate(float time)
    {
        gameUI.endingText.text = ("GET  BACK  TO  THE  CENTER \n TIME  LEFT:  " + Mathf.Round(time));
        gameUI.endingText.enabled = true;
    }
    [ClientRpc]
    private void RpcStageUpdate()
    {
        gameUI.stageText.text = ("Stage: " + currentStageNumber);
    }
    #endregion
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
        RpcUpdatePuzzleUI(puzzlesSolved, puzzlesToBeSolved);
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
        foreach(GameObject puzzle in allStagePuzzles)
        {
            Destroy(puzzle);
            NetworkServer.Destroy(puzzle);
        }
        foreach(GameObject monster in allStageMonsters)
        {
            Destroy(monster);
            NetworkServer.Destroy(monster);
        }
        puzzlesSolved = 0;
        allStageMonsters.Clear();
        allStagePuzzles.Clear();
        allStagePathways.Clear();
        StartCoroutine(nameof(BuildMap));
    }
    [Server]
    private void CheckForPlayers()
    {
        Bounds bounds = hubFloor.gameObject.GetComponent<BoxCollider>().bounds;
        Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, hubFloor.transform.rotation, playerMask);
        List<NetworkConnection> tempList = NetworkMan.Players;
        if (colliders.Length > 0)
        {
            foreach (Collider c in colliders)
            {
                foreach (NetworkConnection conn in tempList)
                {
                    if (c.gameObject == conn.identity.gameObject)
                    {
                        tempList.Remove(conn);
                    }
                }
            }
        }
        foreach(NetworkConnection conn in tempList)
        {
            conn.identity.gameObject.GetComponent<Player>().RpcKillPlayer();
        }
    }
    [TargetRpc]
    private void TgtAllowPlayer(NetworkConnection player)
    {
        player.identity.gameObject.GetComponent<Player>().SeeMove();
    }
    [TargetRpc]
    private void TgtPlayerMoveSpawn(NetworkConnection conn, int playerNumber)
    {
       // if (conn.identity.gameObject.GetComponent<Player>().spectate) { return; } //Spectating players wont go to spawn on new stage
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
        stageFloorType = Random.Range(1, (FloorList.floorList.numberOfFloorTypes + 1));
        Debug.Log(partsPerSection);
         for (int i = 0; i <= 3; i++)
         {
               sectionConnectionPoints = new List<ConnectionPoint>();
               switch (i)
               {
                     case 0:                   
                     for (int i1 = 0; i1 < partsPerSection; i1++)
                     {
                        if(i1 == 0) { AddConnectionPoints(FrontStart); }
                         PlaceFloorObject();
                         yield return interval;
                     }
                     break;
                     case 1:
                     for (int i1 = 0; i1 < partsPerSection; i1++)
                     {
                        if (i1 == 0) { AddConnectionPoints(RightStart); }
                        PlaceFloorObject();
                         yield return interval;
                     }
                     break;
                     case 2:
                     for (int i1 = 0; i1 < partsPerSection; i1++)
                     {
                        if (i1 == 0) { AddConnectionPoints(LeftStart); }
                        PlaceFloorObject();
                         yield return interval;

                     }
                     break;
                     case 3:
                     for (int i1 = 0; i1 < partsPerSection; i1++)
                     {
                        if (i1 == 0) { AddConnectionPoints(BackStart); }
                        PlaceFloorObject();
                         yield return interval;
                     }
                     break;
               }          
         }
         PlacePuzzles();
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
        newPath.transform.localPosition = Vector3.zero;
        Vector3 transfer = newPath.transform.localRotation.eulerAngles;
        transfer = new Vector3(transfer.x, 0, 0);
        newPath.transform.localRotation = Quaternion.Euler(transfer);

        ConnectionPoint newPathConnPoint = newPath.GetComponent<Pathway>().connectionPoints
            [Random.Range(0, newPath.GetComponent<Pathway>().connectionPoints.Length)];
        

        PositionNewPath(newPath, newPathConnPoint ,nextPointToPlace);
       
        if (NewPathOverlap(newPath.GetComponent<Pathway>()))
        {
            sectionConnectionPoints.Remove(nextPointToPlace);
            PlaceFloorObject();
            return;
        }     
        allStagePathways.Add(newPath.GetComponent<Pathway>());
        AddConnectionPoints(newPath.GetComponent<Pathway>());

            nextPointToPlace.gameObject.SetActive(false);
        sectionConnectionPoints.Remove(nextPointToPlace);

        newPathConnPoint.gameObject.SetActive(false);
        sectionConnectionPoints.Remove(newPathConnPoint);
        return;
    }
    [Server]
    bool NewPathOverlap(Pathway newPath)
    {
        Bounds bounds = newPath.PathBounds;
        bounds.Expand(-0.1f);

        Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, newPath.transform.rotation, pathLayerMask);
        if(colliders.Length > 0)
        {
            foreach(Collider c in colliders)
            {
                if (c.transform.parent.gameObject.Equals(newPath.gameObject))
                {
                    continue;
                }
                else
                {
                    Destroy(newPath.gameObject);
                    NetworkServer.Destroy(newPath.gameObject);
                    return true;
                }
            }
        }
        return false;
    }
    [Server]
    private void PositionNewPath(GameObject newPath, ConnectionPoint currentPoint, ConnectionPoint pointToPlace)
    {

        Vector3 pointToPlaceEuler = pointToPlace.transform.eulerAngles;
        Vector3 currentPointEuler = currentPoint.transform.eulerAngles;
        float deltaAngle = Mathf.DeltaAngle(currentPointEuler.y, pointToPlaceEuler.y);
        Quaternion currentPathTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
        newPath.transform.rotation = currentPathTargetRotation * Quaternion.Euler(90f, 90f, 0);

        Vector3 pathPositionOffset = currentPoint.transform.position - newPath.transform.position;
        newPath.transform.position = pointToPlace.transform.position - pathPositionOffset;
        return;
    }
    [Server]
    private void AddConnectionPoints(Pathway pathway)
    {
        foreach(ConnectionPoint connpoint in pathway.connectionPoints)
        {
            sectionConnectionPoints.Add(connpoint);          
        }
        return;
    }
    [Server]
    public void PlacePuzzles()
    {  
        puzzlesToBeSolved = 3 + (NumberofPlayers * currentStageNumber);
        List<Pathway> puzzlePathways = new List<Pathway>();
        puzzlePathways.Clear();
        puzzlePathways = allStagePathways;
        for (int i = 0; i < puzzlesToBeSolved; i++)
        {
            GameObject randomPathway = puzzlePathways[Random.Range(0, allStagePathways.Count)].gameObject;
            Transform randomPuzzlePoint = randomPathway.GetComponent<Pathway>().puzzlePoints
                [Random.Range(0, randomPathway.GetComponent<Pathway>().puzzlePoints.Count)].transform;
            GameObject puzzle = Instantiate(PuzzleList.puzzleList.EasyPuzzles
                [Random.Range(0, PuzzleList.puzzleList.EasyPuzzles.Count)].gameObject, randomPuzzlePoint.transform.position, randomPuzzlePoint.transform.rotation);
            NetworkServer.Spawn(puzzle);
            allStagePuzzles.Add(puzzle);
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
            GameObject spawnedMonster = Instantiate(monsterSpawn, allStagePathways[Random.Range(0, allStagePathways.Count)].monsterSpawn.position, Quaternion.identity);
            NetworkServer.Spawn(spawnedMonster);
            allStageMonsters.Add(spawnedMonster);
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