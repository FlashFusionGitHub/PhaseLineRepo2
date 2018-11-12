using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JustTheTips : MonoBehaviour {

    public Text tipText;

    //All the Tips
    public string[] tips;

    //Tip intervals
    public int tipTime;
    float tipTimer;

    //Time Tip is Displayed
    public int displayTime;
    float displayTimer;

    //Fade Time
    public float fadeTime;

    // Use this for initialization
    void Start () {
        //Pick a tip to display
        tipText.text = tips[Random.Range(0, tips.Length)];
        //set timer
        tipTimer = tipTime;
        displayTimer = displayTime;
    }

    float alpha = 0;
    bool fadingOut;
	// Update is called once per frame
	void Update () {
        //Decrease timer
        tipTimer -= Time.deltaTime;

        if (tipTimer <= 0)
        {
            if(!fadingOut)
            {
                if (alpha < 1)
                    tipText.color = new Color(1, 1, 1, alpha += Time.deltaTime / fadeTime);
                else
                    displayTimer -= Time.deltaTime;
            }

            if(displayTimer <= 0)
            {
                fadingOut = true;

                tipText.color = new Color(1, 1, 1, alpha -= Time.deltaTime / fadeTime);
                
                if(alpha <= 0)
                {
                    tipText.text = tips[Random.Range(0, tips.Length)];
                    tipTimer = tipTime;
                    displayTimer = displayTime;
                    fadingOut = false;
                }
            }
        }
    }
}
