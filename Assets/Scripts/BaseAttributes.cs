using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttributes : MonoBehaviour {


    [Header("Renderer")]
    public Renderer[] primaryRenderers;
    public Renderer[] secondaryRenderers;

    [Header("Chosen Factions")]
    public SelectedFactions selectedFactions;

    [Header("Team")]
    public Team team;

    // Use this for initialization
    void Start () {
        selectedFactions = FindObjectOfType<SelectedFactions>();

        if (team == Team.TEAM1)
        {
            for (int i = 0; i < primaryRenderers.Length; i++)
                primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.primaryColour);

            for (int i = 0; i < secondaryRenderers.Length; i++)
                secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.secondaryColour);
        }
        else
        {
            for (int i = 0; i < primaryRenderers.Length; i++)
                primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.primaryColour);

            for (int i = 0; i < secondaryRenderers.Length; i++)
                secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.secondaryColour);
        }
	}
}
