using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerColourChanger : MonoBehaviour {

    public Renderer[] renderers;
    public Team m_team;

	public Shader shader;

    SelectedFactions selectedFactions;

	// Use this for initialization
	void Awake () {
        selectedFactions = FindObjectOfType<SelectedFactions>();


        if(m_team == Team.TEAM1)
        {
            for(int i = 0; i < renderers.Length; i++)
            {
				if (renderers [i].material.shader == shader) {
					renderers [i].material.SetColor (Shader.PropertyToID ("_TeamColor"), selectedFactions.team1.primaryColour);
				} else {
					renderers [i].material.SetColor (Shader.PropertyToID ("_Color"), selectedFactions.team1.primaryColour);
				}
            }
        }
        else
        {
            for (int i = 0; i < renderers.Length; i++)
            {
				if (renderers [i].material.shader == shader) {
					renderers [i].material.SetColor (Shader.PropertyToID ("_TeamColor"), selectedFactions.team2.primaryColour);
				} else {
					renderers [i].material.SetColor (Shader.PropertyToID ("_Color"), selectedFactions.team2.primaryColour);
				}
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
