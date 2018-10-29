using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerColourChanger : MonoBehaviour {

    public Renderer[] renderers;
    public Team m_team;

    SelectedFactions selectedFactions;

	// Use this for initialization
	void Awake () {

        try
        {
            selectedFactions = FindObjectOfType<SelectedFactions>();

            if (m_team == Team.TEAM1)
            {
                for(int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.primaryColour);
                }
            }
            else
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.primaryColour);
                }
            }
        }
        catch (System.Exception)
        {
            return;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
