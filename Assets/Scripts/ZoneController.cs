using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ZoneController : MonoBehaviour {


    public List<CaptureZoneActor> zones;

    public Image progressBar;

    public float progressTimer;
    public float progressTime = 10;

    public float m_startPercentage = 100;
    private float m_percentage;

    public float amount;

    // Use this for initialization
    void Start () {
        zones = GetComponentsInChildren<CaptureZoneActor>().ToList();

        m_percentage = m_startPercentage / 2;
	}
	
	// Update is called once per frame
	void Update () {

        foreach (CaptureZoneActor zone in zones)
        {
            progressTimer -= Time.deltaTime;
            
            if(progressTimer <= 0)
            {
                if (zone.owner == CaptureZoneActor.Owner.NONE)
                    return;

                if (zone.owner == CaptureZoneActor.Owner.TEAM1)
                {
                    m_percentage -= amount;

                    progressBar.fillAmount = m_percentage / m_startPercentage;
                }

                if (zone.owner == CaptureZoneActor.Owner.TEAM2)
                {
                    m_percentage += amount;

                    progressBar.fillAmount = m_percentage / m_startPercentage;
                }

                progressTimer = progressTime;
            }
        }
	}
}
