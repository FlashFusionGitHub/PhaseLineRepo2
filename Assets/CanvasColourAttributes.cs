using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasColourAttributes : MonoBehaviour {

    public Image Team1;
    public Image Team2;

    [Header("Chosen Factions")]
    public SelectedFactions selectedFactions;

    private void Awake()
    {
        try
        {
            selectedFactions = FindObjectOfType<SelectedFactions>();

            Team1.color = selectedFactions.team1.primaryColour;
            Team2.color = selectedFactions.team2.primaryColour;
        }
        catch (System.Exception)
        {
            Team1.color = Color.blue;
            Team2.color = Color.red;
        }
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
