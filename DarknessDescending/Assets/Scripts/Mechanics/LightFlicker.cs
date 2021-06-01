using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light lightTo;
     private float flickerTime = 2f;
     private float flickerTimerOffset = 1.95f;
    [SerializeField] private float normalTime;
    [SerializeField] private float wierdTime;
    [SerializeField] private GameObject monster;
    private float timer;
    private float flickerTimer;
    private bool isWeird;
    private void Update()
    {
        timer += Time.deltaTime;
        flickerTimer += Time.deltaTime;
        if(timer > Random.Range(normalTime - 1, normalTime + 1) && !isWeird)
        {
            isWeird = true;
            monster.SetActive(true);
            timer = 0;
        }

        if (flickerTimer > Random.Range(flickerTime - flickerTimerOffset, flickerTime + flickerTimerOffset) && isWeird &&  timer < wierdTime)
        {
            flickerTimer = 0;
            lightTo.enabled = !lightTo.enabled;
        }
        else if (isWeird && timer > wierdTime)
        {
            timer = 0;
            isWeird = false;
            monster.SetActive(false);
        }
    }

}
