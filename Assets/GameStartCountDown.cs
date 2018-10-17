using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameStartCountDown : MonoBehaviour {

    public Text countDownText;
    float countDownTimer;
    public float countDownTime = 1.0f;
    int num = 6;

    // Use this for initialization
    void Start () {
        countDownTimer = countDownTime;
        Time.timeScale = 0;
	}
	
	// Update is called once per frame
	void Update () {

        countDownTimer -= Time.unscaledDeltaTime;

        if(num > 0)
        {
            if (countDownTimer <= 0.0f)
            {
                --num;
                countDownText.text = num.ToString();
                countDownTimer = countDownTime;
            }
        }
        else
        {
            countDownText.text = "GO!";

            if (countDownTimer <= 0.0f)
            {
                Time.timeScale = 1;
                Destroy(this.gameObject);
            }
        }
    }
}
