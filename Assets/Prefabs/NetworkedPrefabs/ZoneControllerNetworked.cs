using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ZoneControllerNetworked : MonoBehaviour {

    public List<CaptureZoneActorNetworked> zones;

    public Image progressBar;

    public float progressTimer;
    public float progressTime = 10;

    public float m_startPercentage = 100;
    private float m_percentage;

    public float amount;

    // Use this for initialization
    void Start()
    {
        zones = GetComponentsInChildren<CaptureZoneActorNetworked>().ToList();

        m_percentage = m_startPercentage / 2;
    }

    // Update is called once per frame
    void Update()
    {

        foreach (CaptureZoneActorNetworked zone in zones)
        {
            progressTimer -= Time.deltaTime;

            if (progressTimer <= 0)
            {
                if (zone.owner == CaptureZoneActorNetworked.Owner.NONE)
                    return;

                if (zone.owner == CaptureZoneActorNetworked.Owner.TEAM1)
                {
                    m_percentage -= amount;

                    progressBar.fillAmount = m_percentage / m_startPercentage;
                }

                if (zone.owner == CaptureZoneActorNetworked.Owner.TEAM2)
                {
                    m_percentage += amount;

                    progressBar.fillAmount = m_percentage / m_startPercentage;
                }

                progressTimer = progressTime;
            }
        }
    }
}
