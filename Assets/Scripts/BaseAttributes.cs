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
            selectedFactions.team1.bigBase.transform.position = new Vector3(398.8f, 87.8f, 407.5f);
            selectedFactions.team1.bigBase.transform.eulerAngles = new Vector3(0f, -123.525f, 0f);

            for (int i = 0; i < primaryRenderers.Length; i++)
                primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.primaryColour);

            for (int i = 0; i < secondaryRenderers.Length; i++)
                secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.secondaryColour);
        }
        else
        {
            selectedFactions.team2.bigBase.transform.position = new Vector3(-630.5f, 85.4f, -619.2f);
            selectedFactions.team2.bigBase.transform.eulerAngles = new Vector3(0f, -123.525f, 0f);

            for (int i = 0; i < primaryRenderers.Length; i++)
                primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.primaryColour);

            for (int i = 0; i < secondaryRenderers.Length; i++)
                secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.secondaryColour);
        }
	}
}
