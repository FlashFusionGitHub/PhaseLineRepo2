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

    public Transform BigBasePosition; /*Base Place Holder*/

    // Use this for initialization
    void Start () {
        selectedFactions = FindObjectOfType<SelectedFactions>();

        selectedFactions.team1.bigBase.SetActive(true);
        selectedFactions.team2.bigBase.SetActive(true);

        if (team == Team.TEAM1)
        {
            selectedFactions.team1.bigBase.transform.SetPositionAndRotation(BigBasePosition.position, BigBasePosition.rotation);

            for (int i = 0; i < primaryRenderers.Length; i++)
                primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.primaryColour);

            for (int i = 0; i < secondaryRenderers.Length; i++)
                secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team1.secondaryColour);
        }
        else
        {
            selectedFactions.team2.bigBase.transform.SetPositionAndRotation(BigBasePosition.position, BigBasePosition.rotation);

            for (int i = 0; i < primaryRenderers.Length; i++)
                primaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.primaryColour);

            for (int i = 0; i < secondaryRenderers.Length; i++)
                secondaryRenderers[i].material.SetColor(Shader.PropertyToID("_TeamColor"), selectedFactions.team2.secondaryColour);
        }
	}
}
