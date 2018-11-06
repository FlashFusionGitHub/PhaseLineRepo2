using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMoveTargetFactionAttributes : MonoBehaviour {

    public GameObject[] Stars;

    public Team team;

    public SelectedFactions selectedFactions;

    public void Awake()
    {
        selectedFactions = FindObjectOfType<SelectedFactions>();
    }

    public void SetColor()
    {
        try
        {
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
