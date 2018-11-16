using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneColorChanger : MonoBehaviour
{

    [SerializeField] private Color Team1Color;
    PortraitData pdTeam1;
    PortraitData pdTeam2;
    [SerializeField] private Color Team2Color;
    [SerializeField] private CaptureZoneActor m_captureZoneActor;
    [SerializeField] private ChangeColor m_changeColor;
    [SerializeField] private float m_progress;

    // Use this for initialization
    void Start()
    {
        if (!m_captureZoneActor)
        {
            m_captureZoneActor = GetComponent<CaptureZoneActor>();
        }
        if (!m_changeColor)
        {
            m_changeColor = GetComponent<ChangeColor>();
            if (!m_changeColor)
            {
                print("no ColorChanging Script Found, please add it!");
                this.enabled = false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!pdTeam1 || !pdTeam2)
        {
            foreach (PortraitData pd in FindObjectsOfType<PortraitData>())
            {
                if (pd.team == Team.TEAM1)
                {
                    pdTeam1 = pd;
                    Team1Color = pd.TeamColor;
                }
                else if (pd.team == Team.TEAM2)
                {
                    pdTeam2 = pd;
                    Team2Color = pd.TeamColor;
                }
            }
            return;
        }
        if (m_captureZoneActor)
        {
            m_progress = m_captureZoneActor.capturePercentage / 100;
            if (m_captureZoneActor.team1unitsInZone.Count >= m_captureZoneActor.team2unitsInZone.Count)
            {
                m_changeColor.ChangeColorTo(Color.Lerp(Color.white, Team1Color, m_progress));
            }
            else
            {
                m_changeColor.ChangeColorTo(Color.Lerp(Color.white, Team2Color, m_progress));
            }
        }
        else
        {
            m_captureZoneActor = GetComponent<CaptureZoneActor>();
        }
    }
}
