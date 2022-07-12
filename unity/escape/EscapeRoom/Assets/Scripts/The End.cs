using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TheEnd : MonoBehaviour
{

    public TextMeshProUGUI end;
    private float time = 0;
    private int mintunes = 0;
    private int seconds = 0;
    private string mintunesText;
    private string secondsText;

    // Start is called before the first frame update
    void Start()
    {
       // end = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManger.Instance.IsLazerComplete())
        {
            time += Time.deltaTime;
            mintunes = (int)System.Math.Floor(time / 60);
            seconds = (int)time % 60;

            if (mintunes < 10)
            {
                mintunesText = "0" + mintunes;
            }
            else
            {
                mintunesText = "" + mintunes;
            }

            if (seconds < 10)
            {
                secondsText = "0" + seconds;
            }
            else
            {
                secondsText = "" + seconds;
            }

            end.text = "Congrats! You escaped!\nFinish time: " + mintunesText + ":" + secondsText;
        }
    
    }
}
