using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Pathway UpStart;
    [SerializeField] private Pathway DownStart;
    [SerializeField] private Pathway RightStart;
    [SerializeField] private Pathway LeftStart;
    [SerializeField] private float gameScaling; //Amount per stage increase in percent .1 - 1;

    private List<Pathway> currentSectionPathways = new List<Pathway>();

    
    private float gameScale;
    public void Awake()
    {
        gameScale = 1;
    }
    public void Start()
    {
        BuildMap();
    }
    public void StartStage()
    {
        // allow player to see and move
    }
    public void StartNewFloor()
    {
        gameScale += gameScaling;
    }
    public void BuildMap()
    {
        int partsPerSection = 5;
        partsPerSection = Mathf.CeilToInt(partsPerSection * gameScale);
        int stageFloorTpye =  Random.Range(0 ,(FloorList.floorList.numberOfFloorTypes + 1));
        for (int o = 0; o <= 3; o++)
        {
            for (int i = 0; i <= partsPerSection; i++)
            {
                switch (o)
                {
                    case 0:
                        PlaceFloorObjects(partsPerSection,UpStart);
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
    public void PlaceFloorObjects(int partsPerSection,Pathway pathComponent)
    {
        currentSectionPathways.Add(pathComponent);
        for(int i = 0; i <= partsPerSection; i++)
        {
            Pathway nextPlacement = currentSectionPathways[Random.Range(0, (currentSectionPathways.Count + 1))];
            
        }
    }

}
