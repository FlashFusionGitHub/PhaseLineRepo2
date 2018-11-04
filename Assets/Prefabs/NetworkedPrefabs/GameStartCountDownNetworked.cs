using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameStartCountDownNetworked : NetworkBehaviour {

    public GameObject[] componentsToDisable;
    float countDownTimer;
    public float countDownTime = 1.0f;
    int num = 6;

    // Use this for initialization
    void Start () {
        countDownTimer = countDownTime;
        Time.timeScale = 0;

        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        countDownTimer -= Time.unscaledDeltaTime;

        if (num > 0)
        {
            if (countDownTimer <= 0.0f)
            {
                --num;
                GetComponent<Text>().text = num.ToString();
                countDownTimer = countDownTime;
            }
        }
        else
        {
            GetComponent<Text>().text = "GO!";

            if (countDownTimer <= 0.0f)
            {
                Time.timeScale = 1;

                for (int i = 0; i < componentsToDisable.Length; i++)
                {
                    componentsToDisable[i].SetActive(true);
                }

                Destroy(this.gameObject);
            }
        }
    }
}
