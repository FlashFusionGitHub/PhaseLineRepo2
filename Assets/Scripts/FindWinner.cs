using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Simple class that will find the winner of the match*/
public class FindWinner : MonoBehaviour {

    public Text team1, team2; /*reference to the team1 and team2 text component*/
    public TweenAnimator tweenAnimator1, tweenAnimator2, tweenAnimator3; /*reference to the available tween animators for (team1, team2, and Draw)*/

    ZoneController zoneController; /*reference to the zoneController, used for checking score*/

    bool winnerFound;
    bool winnerCalled = false; /*used for stopping the update loop after the winner wass called*/

    public GameObject[] componentsToDisable; /*list of canvas component to disable when a winner is found (reduces clutter)*/

    public GameObject quitToMenuButton; /*reference to the quit button*/
	public GameObject rematchButton; /*reference to the restart button*/

    public Team winner; /*The winner*/

    public GameTimer gameTimer; /*The match timer*/

    bool isDraw; /*was the game a draw*/

    int player1Score, player2Score; /*The score of both players, use to determine a winner if time runs out*/

    // Use this for initialization
    void Start () {
        zoneController = FindObjectOfType<ZoneController>();
    }

    // Update is called once per frame
    void Update() {

        if(!winnerFound || winnerCalled == true)
        {
            if (winnerCalled)
            {
                winnerCalled = false;
            }

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

                //Time.timeScale = 0;

                quitToMenuButton.SetActive(true);
				rematchButton.SetActive (true);
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

                //Time.timeScale = 0;

                quitToMenuButton.SetActive(true);
				rematchButton.SetActive (true);
            }
            else if(isDraw)
            {
                tweenAnimator3.TweenToInPos();

                winnerFound = true;

                for (int i = 0; i < componentsToDisable.Length; i++)
                {
                    componentsToDisable[i].SetActive(false);
                }

                //Time.timeScale = 0;

                quitToMenuButton.SetActive(true);
				rematchButton.SetActive (true);
            }
        }
        else if (winnerFound)
        {
            quitToMenuButton.SetActive(true);
			rematchButton.SetActive (true);
        }
    }

    void CalculateWinnerFromScore()
    {
        if(gameTimer.gameEnd)
        {
            if (zoneController.team1Score > zoneController.team2Score)
            {
                TriggerTeam1Win();
            }
            else if(zoneController.team1Score < zoneController.team2Score)
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
        winner = Team.TEAM1;
        winnerFound = true;
        winnerCalled = true;
        
    }

    public void TriggerTeam2Win()
    {
        winner = Team.TEAM2;
        winnerFound = true;
        winnerCalled = true;
        
    }

    public void TriggerDraw()
    {
        isDraw = true;
        winnerFound = true;
    }
}
