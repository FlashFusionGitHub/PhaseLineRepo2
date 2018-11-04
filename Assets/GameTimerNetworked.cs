using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameTimerNetworked : NetworkBehaviour
{
    public const float maxMatchTime = 0.5f;
    [SyncVar(hook = "OnUpdateTimer")]
    public float matchTime = maxMatchTime; /*Amount of time a match will last*/
    float currentTime; /*Current time of the match*/
    public Image timerSlider; /*Reference to the slider*/

    public bool gameEnd;

    // Use this for initialization
    void Start () {
        matchTime *= 60; /*convert seconds into minutes*/
	}
	
	// Update is called once per frame
	void Update () {
        UpdateTimer();
	}

    float percentage;
    void UpdateTimer()
    {
        if (percentage < 100)
        {
            currentTime += Time.deltaTime;

            percentage = currentTime / matchTime * 100;

            OnUpdateTimer(percentage);
        }
        else
        {
            gameEnd = true;
        }
    }

    void OnUpdateTimer(float amount)
    {
        timerSlider.fillAmount = amount / 100;
    }
}
