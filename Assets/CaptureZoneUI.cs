using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureZoneUI : MonoBehaviour {

    public CaptureZoneActor CZA;

    public Team team;

    public Sprite Empty;

    public Sprite Captured;

    public Sprite Destroyed;

    public Sprite Alert;

    public Image Overlay;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        if (!CZA || !CZA.gameObject.activeInHierarchy)
        {
            if (Overlay.sprite != Destroyed)
                Overlay.sprite = Destroyed;
        }

        

        else if (CZA)
        {
            if (CZA.owner == team)
            {
                if (Overlay.sprite != Captured)
                {
                    Overlay.sprite = Captured;
                }

                if (team == Team.TEAM1)
                {
                    if (CZA.team2unitsInZone.Count > 0)
                    {
                        if (Overlay.sprite != Alert)
                            Overlay.sprite = Alert;
                    }
                }
                else
                {
                    if (CZA.team1unitsInZone.Count > 0)
                    {
                        if (Overlay.sprite != Alert)
                            Overlay.sprite = Alert;
                    }

                }
            }
            else if (CZA.owner == Team.NONE)
            {
                if (Overlay.sprite != Empty)
                {
                    Overlay.sprite = Empty;
                }
            }
            else
            {
                if (Overlay.sprite != Destroyed)
                {
                    Overlay.sprite = Destroyed;
                }
            }
            
        }


	}
}
