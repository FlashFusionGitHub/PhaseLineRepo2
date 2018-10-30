using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {


    public float matchTime; /* 1 = 1 minute*/

    float currentTime;

    public Image timerSlider;

    public bool gameEnd;

    // Use this for initialization
    void Start () {
        matchTime *= 60;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateTimer();
	}


    void UpdateTimer() {

        currentTime += Time.deltaTime;

        float percentage = currentTime / matchTime * 100;

        timerSlider.fillAmount = percentage / 100;

        if(timerSlider.fillAmount >= 1.0f)
        {
            gameEnd = true;
        }
    }
}
