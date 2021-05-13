using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTExt : MonoBehaviour
{
    private float timer;
    private int loadingDots;
    [SerializeField] private Text loadingText;
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > .5)
        {
            switch (loadingDots)
            {
                case 0:
                    loadingDots += 1;
                    loadingText.text = "Loading .";
                    break;
                case 1:
                    loadingDots += 1;
                    loadingText.text = "Loading . .";
                    break;
                case 2:
                    loadingDots += 1;
                    loadingText.text = "Loading . . .";
                    break;
                case 3:
                    loadingDots = 0;
                    loadingText.text = "Loading";
                    break;
            }
            timer = 0;
        }
    }
}
