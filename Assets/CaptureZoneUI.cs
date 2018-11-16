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

	public PortraitData pdTeam1;

	public PortraitData pdTeam2;
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

		if (!pdTeam1 || !pdTeam2) 
		{
			foreach (PortraitData pd in FindObjectsOfType<PortraitData>()) 
			{
				if (pd.team == Team.TEAM1) {
					pdTeam1 = pd;
				} else 
				{
					pdTeam2 = pd;
				}
			}
			return;
		}

        

       if (CZA)
        {
			Overlay.color = Color.Lerp(Color.white, (CZA.owner == Team.TEAM1 || CZA.team1unitsInZone.Count > CZA.team2unitsInZone.Count) ? pdTeam1.TeamColor : pdTeam2.TeamColor, CZA.capturePercentage /100f);
			if (CZA.capturePercentage == 100f)
            {
                if (Overlay.sprite != Captured)
                {
                    Overlay.sprite = Captured;
                }
			}

			if (CZA.team1unitsInZone.Count > 0 && CZA.team2unitsInZone.Count > 0)
                {
                    if (CZA.team2unitsInZone.Count > 0)
                    {
                        if (Overlay.sprite != Alert)
                            Overlay.sprite = Alert;
                    }
                }
            }
			else if (Overlay.sprite != Captured)
            {
                if (Overlay.sprite != Empty)
                {
                    Overlay.sprite = Empty;
                }
            }
	}
}
