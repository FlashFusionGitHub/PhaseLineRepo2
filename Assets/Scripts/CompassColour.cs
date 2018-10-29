using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassColour : MonoBehaviour {

    SelectedFactions selectedFactions;
    public Team team;
    public Image compass;

    // Use this for initialization
    void Start () {
        try
        {
            selectedFactions = FindObjectOfType<SelectedFactions>();

            if (team == Team.TEAM1)
            {
                compass.color = selectedFactions.team1.primaryColour;
            }
            else
            {
                compass.color = selectedFactions.team2.primaryColour;
            }
        }
        catch (System.Exception)
        {
            compass.color = Color.black;
        }
    }
	
}
