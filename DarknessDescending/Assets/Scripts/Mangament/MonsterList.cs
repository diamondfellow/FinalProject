using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterList : MonoBehaviour
{
    public static MonsterList monsterList;
    public void Awake()
    {
        monsterList = this;
    }
    public List<GameObject> Monsters = new List<GameObject>();
}
