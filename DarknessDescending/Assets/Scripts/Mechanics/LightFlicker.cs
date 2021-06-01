using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light lightTo;
    [SerializeField] private float flickerTime;
    [SerializeField] private float flickerTimerOffset;
    private float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > Random.Range(flickerTime - flickerTimerOffset, flickerTime + flickerTimerOffset))
        {
            timer = 0;
            lightTo.enabled = !lightTo.enabled;
        }
    }
}
