using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameProgressBar : MonoBehaviour {

    public RectTransform progressBar;

    public float progressTimer;
    public float progressTime = 10;

    public int currentProgressAmount = 200;
    public int amount;

    public ZoneControllerNetworked zcn;

    // Use this for initialization
    void Start () {
        zcn = FindObjectOfType<ZoneControllerNetworked>();
    }
	
	// Update is called once per frame
	void Update () {

        foreach (CaptureZoneActorNetworked zone in zcn.zones)
        {
            progressTimer -= Time.deltaTime;

            if (progressTimer <= 0.0f)
            {
                if (zone.owner == CaptureZoneActorNetworked.Owner.NONE)
                    return;

                if (zone.owner == CaptureZoneActorNetworked.Owner.TEAM1)
                {
                    currentProgressAmount -= amount;

                    OnChangedAmount(currentProgressAmount);
                }

                if (zone.owner == CaptureZoneActorNetworked.Owner.TEAM2)
                {
                    currentProgressAmount += amount;

                    OnChangedAmount(currentProgressAmount);
                }

                progressTimer = progressTime;
            }
        }
    }

    void OnChangedAmount(int amount)
    {
        progressBar.sizeDelta = new Vector2(amount, progressBar.sizeDelta.y);
    }
}
