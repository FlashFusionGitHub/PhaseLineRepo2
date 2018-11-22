using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassColour : MonoBehaviour {

    /*simply colour changer class, changes the colour of the compass according the the selected team chosen*/
    SelectedFactions selectedFactions; /*reference to the selected factions script*/
    public Team team; /*The team the compass belongs too*/
    public Image compass; /*The iamge of the compass*/

    // Use this for initialization
    void Start () {

        /*try seraching for a Selected Factions component, if it doesnt exist just set the colour to black*/
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
