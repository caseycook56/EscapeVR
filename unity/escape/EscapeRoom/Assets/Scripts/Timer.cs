using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI timer;
    private float time = 0;
    private int mintunes =0;
    private int seconds =0;
    private string mintunesText;
    private string secondsText;
    private bool escape = false;
    void Start()
    {
        timer = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if( escape == false)
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

            timer.text = "Time: " + mintunesText + ":" + secondsText;
        }
        
        if(GameManger.Instance.IsLazerComplete() && escape == false)
        {
            GameManger.Instance.timer = time;
            escape = true;
        }
       
    }
}
