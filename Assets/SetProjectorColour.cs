using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetProjectorColour : MonoBehaviour {

    public Team team;

    public SelectedFactions selectedFactions;

    public Projector projector;

    private void Awake()
    {
        selectedFactions = FindObjectOfType<SelectedFactions>();

        if (team == Team.TEAM1)
            projector.material.SetColor(Shader.PropertyToID("_Color"), selectedFactions.team1.primaryColour);
        if (team == Team.TEAM2)
            projector.material.SetColor(Shader.PropertyToID("_Color"), selectedFactions.team2.primaryColour);
    }
}
