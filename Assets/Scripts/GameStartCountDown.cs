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

    public GameObject[] componentsToDisable; 

    // Use this for initialization
    void Start () {
        SAanim();
        countDownTimer = countDownTime;
        Time.timeScale = 0;

        for(int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].SetActive(false);
        }
	}

    void SAanim()
    {
        GameObject b1 = FindObjectOfType<SelectedFactions>().team1.bigBase;
        GameObject b2 = FindObjectOfType<SelectedFactions>().team2.bigBase;
        
        b1.SetActive(true);
        b2.SetActive(true);
      // Animator b1anim = b1.GetComponentInChildren<Animator>();
      // Animator b2anim = b2.GetComponentInChildren<Animator>();
      // b1anim.updateMode = AnimatorUpdateMode.UnscaledTime;
      // b2anim.updateMode = AnimatorUpdateMode.UnscaledTime;
       // Debug.Log("B1 =" + b1.name + ", " + b1anim.gameObject.name + ", b2 = " + b2.name + ", " + b2anim.gameObject.name);
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

                for (int i = 0; i < componentsToDisable.Length; i++)
                {
                    componentsToDisable[i].SetActive(true);
                }

                Destroy(this.gameObject);
            }
        }
    }
}
