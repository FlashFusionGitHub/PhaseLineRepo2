using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPoolNetworked : MonoBehaviour {

    public List<TroopActorNetworked> allTroopActors = new List<TroopActorNetworked>();

    public List<TroopActorNetworked> team1Troops = new List<TroopActorNetworked>();
    public List<TroopActorNetworked> team2Troops = new List<TroopActorNetworked>();

    public List<TroopActorNetworked> team1Generals = new List<TroopActorNetworked>();
    public List<TroopActorNetworked> team2Generals = new List<TroopActorNetworked>();

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FindAllTroopTargets()
    {
        allTroopActors = FindObjectsOfType<TroopActorNetworked>().ToList();
    }

    public void SplitTroops()
    {

        team1Troops.Clear();
        team2Troops.Clear();

        foreach (TroopActorNetworked troop in allTroopActors)
        {
            if (troop.team == Team.TEAM1)
            {
                team1Troops.Add(troop);
            }

            if (troop.team == Team.TEAM2)
            {
                team2Troops.Add(troop);
            }
        }
    }

    public void FindAllGenerals()
    {
        team1Generals.Clear();
        team2Generals.Clear();

        foreach (TroopActorNetworked troop in allTroopActors)
        {
            if (troop.team == Team.TEAM1 && troop.rank == TroopActorNetworked.Rank.IsGeneral)
            {
                team1Generals.Add(troop);
            }

            if (troop.team == Team.TEAM2 && troop.rank == TroopActorNetworked.Rank.IsGeneral)
            {
                team2Generals.Add(troop);
            }
        }
    }
}
