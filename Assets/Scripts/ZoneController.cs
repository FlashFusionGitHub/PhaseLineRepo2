using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ZoneController : MonoBehaviour {

    public List<CaptureZoneActor> zones;

    public float progressTimer;
    public float progressTime = 10;

    public int team1Score, team2Score;
    public Text team1ScoreText, team2ScoreText;

    public int captureZoneScoreAmount;
    public int killScoreAmount;

    // Use this for initialization
    void Start () {

        zones = GetComponentsInChildren<CaptureZoneActor>().ToList();
	}
	
	// Update is called once per frame
	void Update () {

        foreach (CaptureZoneActor zone in zones)
        {
            progressTimer -= Time.deltaTime;
           
            if(progressTimer <= 0)
            {
                if (zone.owner == CaptureZoneActor.Owner.NONE)
                    return;

                if (zone.owner == CaptureZoneActor.Owner.TEAM1)
                {
                    //add points to team 1
                    team1Score += captureZoneScoreAmount;
                }

                if (zone.owner == CaptureZoneActor.Owner.TEAM2)
                {
                    //add points to team 2
                    team2Score += captureZoneScoreAmount;
                }

                progressTimer = progressTime;
            }

            UpdateScoreText();
        }
    }

    public void UpdatePlayer1KillScore()
    {
        team1Score += killScoreAmount;
        UpdateScoreText();
    }

    public void UpdatePlayer2KillScore()
    {
        team2Score += killScoreAmount;
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        team1ScoreText.text = team1Score.ToString();
        team2ScoreText.text = team2Score.ToString();
    }
}
