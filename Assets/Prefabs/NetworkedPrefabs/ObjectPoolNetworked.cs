using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPoolNetworked : MonoBehaviour {

    [System.Serializable]
    public class FactoryPrefab
    {
        public GameObject prefab;
        public int amount = 20;
    }


    public List<TroopActorNetworked> allTroopActors = new List<TroopActorNetworked>();

    public List<TroopActorNetworked> team1Troops = new List<TroopActorNetworked>();
    public List<TroopActorNetworked> team2Troops = new List<TroopActorNetworked>();

    public List<TroopActorNetworked> team1Generals = new List<TroopActorNetworked>();
    public List<TroopActorNetworked> team2Generals = new List<TroopActorNetworked>();

    [Header("P1's Factory")]
    public Transform p1Group;
    public FactoryPrefab p1Tanks;
    public FactoryPrefab p1AATanks;
    public FactoryPrefab p1Helis;

    [Header("P2's Factory")]
    public Transform p2Group;
    public FactoryPrefab p2Tanks;
    public FactoryPrefab p2AATanks;
    public FactoryPrefab p2Helis;

    // Use this for initialization
    void Start () {
        GenerateTroops();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateTroops()
    {
        for (int i = 0; i < p1Tanks.amount; i++)
        {
            Instantiate(p1Tanks.prefab, p1Group);
        }

        for (int i = 0; i < p1AATanks.amount; i++)
        {
            Instantiate(p1AATanks.prefab, p1Group);
        }

        for (int i = 0; i < p1Helis.amount; i++)
        {
            Instantiate(p1Helis.prefab, p1Group);
        }

        for (int i = 0; i < p2Tanks.amount; i++)
        {
            Instantiate(p2Tanks.prefab, p2Group);
        }

        for (int i = 0; i < p2AATanks.amount; i++)
        {
            Instantiate(p2AATanks.prefab, p2Group);
        }

        for (int i = 0; i < p2Helis.amount; i++)
        {
            Instantiate(p2Helis.prefab, p2Group);
        }
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
            if (troop.team == Team.TEAM1 && troop.rankState == RankState.IsGeneral)
            {
                team1Generals.Add(troop);
            }

            if (troop.team == Team.TEAM2 && troop.rankState == RankState.IsGeneral)
            {
                team2Generals.Add(troop);
            }
        }
    }
}
