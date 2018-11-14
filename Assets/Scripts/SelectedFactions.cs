﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedFactions : MonoBehaviour {

    public FactionElements team1;
    public FactionElements team2;

    public GameObject light1;
    public GameObject light2;

	public float lifeTime;
    void Awake()
    {
		foreach (SelectedFactions sf in FindObjectsOfType<SelectedFactions>()) {
			if (sf.lifeTime > lifeTime) {
				Destroy (sf.gameObject);
			}
		}

        DontDestroyOnLoad(this.gameObject);
    }

    public void SetFactionElement(int i, FactionElements factionElement)
    {
        if(i == 0)
        {
            light1.SetActive(false);
            team1 = Instantiate(factionElement);
            team1.SetTeam(Team.TEAM1);
            DontDestroyOnLoad(team1);
        }
        else
        {
            light2.SetActive(false);
            team2 = Instantiate(factionElement);
            team2.SetTeam(Team.TEAM2);
            DontDestroyOnLoad(team2);
        }
    }

	public void Update() {
		lifeTime += Time.deltaTime;
	}
}
