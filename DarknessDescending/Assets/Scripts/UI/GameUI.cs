using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Text loadingText;

    public Text endingText;
    public Text puzzleText;
    public Text stageText;

    private float timer = 0;
    private int loadingDots = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > .5)
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
