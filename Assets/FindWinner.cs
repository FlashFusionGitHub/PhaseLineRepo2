using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindWinner : MonoBehaviour {

    public Text team1, team2;
    public TweenAnimator tweenAnimator1, tweenAnimator2, tweenAnimator3;

    ZoneController zoneController;

    bool winnerFound;

    public GameObject[] componentsToDisable;

    public GameObject quitToMenuScreen;

    Team winner;

    public GameTimer gameTimer;

    int team1Score, team2Score;

    bool isDraw;

    // Use this for initialization
    void Start () {
        zoneController = FindObjectOfType<ZoneController>();
	}

    // Update is called once per frame
    void Update() {

        if(!winnerFound)
        {
            CalculateWinnerFromScore();

            if (winner == Team.TEAM1)
            {
                team1.color = Color.green;
                team1.text = "WINNER";

                team2.color = Color.red;
                team2.text = "LOSER";

                tweenAnimator1.TweenToInPos();
                tweenAnimator2.TweenToInPos();

                winnerFound = true;

                for(int i = 0; i < componentsToDisable.Length; i++)
                {
                    componentsToDisable[i].SetActive(false);
                }

                Time.timeScale = 0;

                quitToMenuScreen.SetActive(true);
            }
            else if (winner == Team.TEAM2)
            {
                team1.color = Color.red;
                team1.text = "LOSER";

                team2.color = Color.green;
                team2.text = "WINNER";

                tweenAnimator1.TweenToInPos();
                tweenAnimator2.TweenToInPos();

                winnerFound = true;

                for (int i = 0; i < componentsToDisable.Length; i++)
                {
                    componentsToDisable[i].SetActive(false);
                }

                Time.timeScale = 0;

                quitToMenuScreen.SetActive(true);
            }
            else if(isDraw)
            {
                tweenAnimator3.TweenToInPos();

                winnerFound = true;

                for (int i = 0; i < componentsToDisable.Length; i++)
                {
                    componentsToDisable[i].SetActive(false);
                }

                Time.timeScale = 0;

                quitToMenuScreen.SetActive(true);
            }
        }
        else
        {
            quitToMenuScreen.SetActive(true);
        }
    }

    void CalculateWinnerFromScore()
    {
        if(gameTimer.gameEnd)
        {
            if (team1Score > team2Score)
            {
                TriggerTeam1Win();
            }
            else if(team2Score > team1Score)
            {
                TriggerTeam2Win();
            }
            else
            {
                TriggerDraw();
            }
        }
    }

    public void TriggerTeam1Win()
    {
        winnerFound = true;
        winner = Team.TEAM1;
    }

    public void TriggerTeam2Win()
    {
        winnerFound = true;
        winner = Team.TEAM2;
    }

    public void TriggerDraw()
    {
        isDraw = true;
        winnerFound = true;
    }
}
