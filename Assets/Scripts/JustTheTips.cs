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
        tipText.text = tips[Random.Range(0, tips.Length)];
        tipTimer = tipTime;
        alpha = 1;
        fading = true;
    }

	// Update is called once per frame
	void Update () {
       
        if (tipTimer <= 0)
        {

            PickATip();
        }
        else
        {
            tipTimer -= Time.deltaTime;
        }
    }

    float alpha = 0;
    bool fading;
    void PickATip()
    {
        if (fading)
        {
            alpha -= Time.deltaTime / fadeTime;
            tipText.color = new Color(1, 1, 1, alpha);
            if (alpha <= 0)
            {
                fading = false;
            }
        }
        else
        { 
            alpha += Time.deltaTime / fadeTime;
            tipText.color = new Color(1, 1, 1, alpha);


            if (alpha >= 1)
            {
                tipText.text = tips[Random.Range(0, tips.Length)];
                tipTimer = tipTime;
                fading = true;
              
            }
        }
    }
}
