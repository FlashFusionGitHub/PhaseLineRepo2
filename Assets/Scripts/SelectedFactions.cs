using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedFactions : MonoBehaviour {

    public FactionElements team1;
    public FactionElements team2;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetFactionElement(int i, FactionElements factionElement)
    {
        if(i == 0)
        {
            team1 = Instantiate(factionElement);
            team1.SetTeam(Team.TEAM1);
            DontDestroyOnLoad(team1);
        }
        else
        {
            team2 = Instantiate(factionElement);
            team2.SetTeam(Team.TEAM2);
            DontDestroyOnLoad(team2);
        }
    }
}
