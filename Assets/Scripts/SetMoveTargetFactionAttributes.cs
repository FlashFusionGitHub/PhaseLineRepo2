using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMoveTargetFactionAttributes : MonoBehaviour {

    public GameObject[] Stars = new GameObject[7];

    public Team team;

    SelectedFactions selectedFactions;
    private void Awake()
    {
        try
        {
            selectedFactions = FindObjectOfType<SelectedFactions>();

            if (team == Team.TEAM1)
            {
                for (int i = 0; i < Stars.Length; i++)
                    Stars[i].GetComponent<Renderer>().material.SetColor(Shader.PropertyToID("_Color"), selectedFactions.team1.primaryColour);
            }
            else
            {
                for (int i = 0; i < Stars.Length; i++)
                    Stars[i].GetComponent<Renderer>().material.SetColor(Shader.PropertyToID("_Color"), selectedFactions.team2.primaryColour);
            }
        }
        catch(System.Exception)
        {
            return;
        }
    }
}
