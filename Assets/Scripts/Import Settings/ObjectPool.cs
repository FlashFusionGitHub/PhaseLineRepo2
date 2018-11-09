using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

	[System.Serializable]
	public class FactoryPrefab
	{
		public GameObject prefab;
		public int amount = 20;
	}

    public List<TroopActor> allTroopActors = new List<TroopActor>();

    public List<TroopActor> team1Troops = new List<TroopActor>();
    public List<TroopActor> team2Troops = new List<TroopActor>();

    public List<TroopActor> team1Generals = new List<TroopActor>();
    public List<TroopActor> team2Generals = new List<TroopActor>();

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
		GenerateTroops ();
        FindAllTroopTargets();
        AddGeneralsToList();
    }

	void GenerateTroops()
	{
		for (int i = 0; i < p1Tanks.amount; i++) 
		{
			Instantiate (p1Tanks.prefab, p1Group);
		}

		for (int i = 0; i < p1AATanks.amount; i++) 
		{
			Instantiate (p1AATanks.prefab, p1Group);
		}

		for (int i = 0; i < p1Helis.amount; i++) 
		{
			Instantiate (p1Helis.prefab, p1Group);
		}




		for (int i = 0; i < p2Tanks.amount; i++) 
		{
			Instantiate (p2Tanks.prefab, p2Group);
		}

		for (int i = 0; i < p2AATanks.amount; i++) 
		{
			Instantiate (p2AATanks.prefab, p2Group);
		}

		for (int i = 0; i < p2Helis.amount; i++) 
		{
			Instantiate (p2Helis.prefab, p2Group);
		}
	}
    private void Update()
    {
        if (!WeHaveBases())
        {
            FindAllTroopTargets();
        }
    }

    public void FindAllTroopTargets()
    {
        allTroopActors = FindObjectsOfType<TroopActor>().ToList();
    }

    bool WeHaveBases()
    {
        int baseNumber = 2;
        foreach (TroopActor t in allTroopActors)
        {
            if (t.rankState == RankState.Base)
            {
                baseNumber--;
                if (baseNumber <= 0)
                {
                    return true;
                }
            }
        }
        if (baseNumber <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
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
