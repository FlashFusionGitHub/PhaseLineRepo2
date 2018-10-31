using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddScoreToZoneController : MonoBehaviour {

    public ZoneController zc;
	// Use this for initialization
	void Start () {
        zc = FindObjectOfType<ZoneController>();
	}

    // Update is called once per frame
    public void UpdateScoreTeam1(int ini)
    {
        zc.UpdatePlayer1Score(ini);
    }

    public void UpdateScoreTeam2(int ini)
    {
        zc.UpdatePlayer2Score(ini);
    }
}
