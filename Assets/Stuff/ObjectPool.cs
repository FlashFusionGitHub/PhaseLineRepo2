using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour {


    public List<TroopActor> allTroopActors = new List<TroopActor>();

    public List<TroopActor> team1Troops = new List<TroopActor>();
    public List<TroopActor> team2Troops = new List<TroopActor>();

    public List<TroopActor> team1Generals = new List<TroopActor>();
    public List<TroopActor> team2Generals = new List<TroopActor>();

    // Use this for initialization
    void Start () {
        FindAllTroopTargets();
        AddGeneralsToList();
    }

    private void Update()
    {

    }

    public void FindAllTroopTargets()
    {
        allTroopActors = FindObjectsOfType<TroopActor>().ToList();
    }

    public void AddGeneralsToList()
    {
        team1Generals.Clear();
        team2Generals.Clear();

        foreach (TroopActor troop in allTroopActors)
        {
            if (troop.team == Team.TEAM1 && troop.rankState == RankState.IsGeneral)
            {
                team1Generals.Add(troop);
            }

            if (troop.team == Team.TEAM1)
            {
                team1Troops.Add(troop);
            }

            if (troop.team == Team.TEAM2 && troop.rankState == RankState.IsGeneral)
            {
                team2Generals.Add(troop);
            }

            if (troop.team == Team.TEAM2)
            {
                team2Troops.Add(troop);
            }
        }
    }
}
