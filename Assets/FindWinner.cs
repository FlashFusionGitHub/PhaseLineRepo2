using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindWinner : MonoBehaviour {

    ZoneController zc;

    public GameObject TeamOneSliderPanel;
    public GameObject TeamTwoSliderPanel;

    public Text teamOneSliderPanelText;
    public Text teamTwoSliderPanelText;

    public TweenAnimator tA1, tA2;

    bool winnerFound = false;

    enum Winner { player1, player2 };

    Winner winner;

    // Use this for initialization
    void Start () {
        zc = FindObjectOfType<ZoneController>();
	}
	
	// Update is called once per frame
	void Update () {
        FindAWinner();
    }

    void FindAWinner()
    {
        if(!winnerFound)
        {
            if (zc.progressBar.fillAmount == 0)
            {
                teamOneSliderPanelText.text = "<color=lime>WINNER</color>";
                teamTwoSliderPanelText.text = "<color=red>LOSER</color>";

                tA1.ToggleInOut();
                tA2.ToggleInOut();

                winnerFound = true;
            }

            if (zc.progressBar.fillAmount == 1)
            {
                teamOneSliderPanelText.text = "<color=red>LOSER</color>";
                teamTwoSliderPanelText.text = "<color=lime>WINNER</color>";

                tA1.ToggleInOut();
                tA2.ToggleInOut();

                winnerFound = true;
            }
        }
    }
}
