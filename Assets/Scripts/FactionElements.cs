﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionElements : MonoBehaviour{

    [Header("Faction Elements")]
    public Commentator commentator;
    public string name;
    public Color primaryColour;
    public Color secondaryColour;
    public GameObject bigBase;
	public Sprite baseFace;
	public string description;
    public Text crossText;

    public void SetTeam(Team t)
    {
        TroopActor ta = bigBase.GetComponentInChildren<TroopActor>();
        HangerSpawner[] hangers = bigBase.GetComponentsInChildren<HangerSpawner>();
        TriggerWin tw = bigBase.GetComponentInChildren<TriggerWin>();
        PortraitData pd = bigBase.GetComponentInChildren<PortraitData>();

        if (pd)
        {
            pd.team = t;
            pd.TeamColor = primaryColour;
        }
        else
        {
            Debug.LogWarning("Please assign a PortraitData Component to " + bigBase.name);
        }

        if (ta)
        {
            ta.team = t;
        }
        else
        {
            Debug.LogWarning("please assign a Troop Actor Component to " + bigBase.name);
        }
        if (hangers.Length > 0)
        {
            foreach (HangerSpawner hanger in hangers)
            {
                hanger.team = t;
            }
        }
        else
        {
            Debug.LogWarning("please assign at least one HangerSpawner Component to " + bigBase.name);
        }
        if (tw)
        {
            tw.team = t;
        }
        else
        {
            Debug.LogWarning("please assign a TriggerWin Component to " + bigBase.name);
        }
    }
}
