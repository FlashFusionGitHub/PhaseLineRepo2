﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*Global Team Enum, some scripts need to know what team an object belongs too*/
public enum Team { NONE, TEAM1, TEAM2 };

public class NavigationArrowActor : MonoBehaviour
{
    public TroopActor m_tank; /*Store Tank the marker is currently inside off*/

    public GameObject m_airStrikeMarker;
    public GameObject m_navMarker;
    public GameObject m_airStrike;

    /*Min Max values used for clamping, navigation arrow within the map*/
    public float m_minXPos = -100, maxXPos = 100;
    public float m_minZPos = -100, maxZPos = 100;

    public float m_markerSpeed = 2; /*Speed of the marker*/

    public Team m_team; /*The team this Component belongs too*/

    public int playerIndex; /*players controller index used for setting (InControl Device)*/

    public GameObject m_currentMarker; /*The current marker, markers are created and destoryed, so the current marker is stored*/

    public bool m_airStrikeState; /*Are we currently doing an airstrike, if so stop the marker updating*/

    protected Controller m_controller; /*Reference to the controller class*/

    public List<CaptureZoneActor> airstrikes; /*A list of available airstrikes / zones captured*/

    public float floatValue = 1f;

    public LayerMask terrainMask; /*Refernce to the terrain mask, used for raycasting to check ground level*/

    public float maxMarkerDistanceFromUnit = 500.0f; /*Max distance the current navigation marker can move from the selected unit*/

    public TroopController troopController; /*Reference to THIS players the troop controoler*/

    Transform prevPos; /*The previous position of the current marker*/

	ObjectPool op;

    public Transform camTransform;

	public Renderer[] markerRenderers;

    public int zoneSize = 50;

    public CaptureZoneActor closestZone;

    ZoneController zone;

    // Use this for initialization
    protected virtual void Start()
    {
        op = FindObjectOfType<ObjectPool> ();
        zone = FindObjectOfType<ZoneController>();
        GameObject previousPos = new GameObject("previous Pos");
        prevPos = previousPos.transform;
        m_currentMarker = Instantiate(m_navMarker, new Vector3(0, 4, 0), Quaternion.identity);
		markerRenderers = m_currentMarker.GetComponentsInChildren<Renderer> ();

        m_currentMarker.transform.position = troopController.currentSelectedUnit.transform.position;
		
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(m_controller == null)
        {
            foreach (Controller c in FindObjectsOfType<Controller>())
            {
                if (playerIndex == 0 && c.m_playerIndex == 0)
                {
                    m_controller = c;
                }
                if (playerIndex == 1 && c.m_playerIndex == 1)
                {
                    m_controller = c;
                }
            }
        }

        if (prevPos.parent != troopController.currentSelectedUnit.transform)
        {
            prevPos.parent = troopController.currentSelectedUnit.transform;
            prevPos.localPosition = Vector3.zero;
            prevPos.localEulerAngles = Vector3.zero;
        }

        float markerXPos = Mathf.Clamp(m_currentMarker.transform.position.x, m_minXPos, maxXPos);
        float markerZPos = Mathf.Clamp(m_currentMarker.transform.position.z, m_minZPos, maxZPos);

		if (MarkerVisible())
        {
            m_currentMarker.transform.position = new Vector3(markerXPos, m_currentMarker.transform.position.y, markerZPos);

            var objPos = m_currentMarker.transform.position;

            RaycastHit hit;

            /*Cast a ray downward from the centre of the current marker, if the ray the terrain mask, set the current markers Y position the be on top of the terrain mask*/
            if (Physics.Raycast(new Vector3(objPos.x, 500, objPos.z), Vector3.down, out hit, 800f, terrainMask))
            {
                Debug.DrawLine(new Vector3(objPos.x, 500, objPos.z), hit.point);
                m_currentMarker.transform.Translate(0, (floatValue - hit.distance), 0);
                m_currentMarker.transform.position += new Vector3(m_controller.LeftAnalogStick().X, 0, m_controller.LeftAnalogStick().Y) * Time.deltaTime * (PlayerPrefs.GetFloat("MarkerSpeedPlayer" + playerIndex) * m_markerSpeed);
                m_currentMarker.transform.position = new Vector3(m_currentMarker.transform.position.x, hit.point.y, m_currentMarker.transform.position.z);
            }
            else
            {
                m_currentMarker.transform.position += new Vector3(m_controller.LeftAnalogStick().X, 0, m_controller.LeftAnalogStick().Y) * Time.deltaTime * (PlayerPrefs.GetFloat("MarkerSpeedPlayer" + playerIndex) * m_markerSpeed);
            }

            prevPos.position = m_currentMarker.transform.position;
        }
        else
        {
			minMoveTimer = minMoveTime;
            if (prevPos.position == Vector3.zero)
            {
                prevPos.position = troopController.currentSelectedUnit.transform.position;
            }
            
            //m_currentMarker.transform.position = prevPos.position - ((m_currentMarker.transform.position - troopController.currentSelectedUnit.transform.position).normalized * 10f);
        }

		if (minMoveTimer >= 0) 
		{
			camTransform.position = Vector3.Lerp(camTransform.position, new Vector3(m_currentMarker.transform.position.x, camTransform.position.y, m_currentMarker.transform.position.z), Time.deltaTime * (PlayerPrefs.GetFloat("MarkerSpeedPlayer" + playerIndex)) *2);
			m_currentMarker.transform.position = Vector3.Lerp(m_currentMarker.transform.position, new Vector3(camTransform.position.x, m_currentMarker.transform.position.y, camTransform.position.z), Time.deltaTime * (PlayerPrefs.GetFloat("MarkerSpeedPlayer" + playerIndex)));
			minMoveTimer -= Time.deltaTime;
		}

        ClosestEnemyUnit ();
        AirStrikeControls();
		if (ChangeSate) 
		{
			ChangeSate = false;
		}
    }

	Camera cam;
	float minMoveTime = 0.5f;
	float minMoveTimer;
	bool MarkerVisible()
	{
        if (cam)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(m_currentMarker.transform.position);
            return viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1;
        }
        else
        {
            cam = camTransform.GetComponent<CameraController>().camera;
            return false;
        }
	}

    void ClosestEnemyUnit()
    {
        float dis = 0;
        foreach (TroopActor T in op.allTroopActors)
        {
            if (m_tank == null && T.team != m_team && T.rankState != RankState.dead)
            {
                if (Vector3.Distance(m_currentMarker.transform.position, T.transform.position) < 20f)
                {
                    if (dis == 0 || Vector3.Distance(m_currentMarker.transform.position, T.transform.position) < dis)
                    {
                        dis = Vector3.Distance(m_currentMarker.transform.position, T.transform.position);
                        m_tank = T;
                    }
                }
            }
        }

        if (m_tank != null)
        {
            if (Vector3.Distance(m_currentMarker.transform.position, m_tank.transform.position) > 20f)
            {
                m_tank = null;
                SimpleAnimate sa = m_currentMarker.GetComponentInChildren<SimpleAnimate>();
                if (sa)
                {
                    sa.speedModifier = 1;
                }
            }
            else
            {
				SimpleAnimate sa = m_currentMarker.GetComponentInChildren <SimpleAnimate>();
                if (sa)
                {
                    sa.speedModifier = 5f;
                }
            }
        }
    }

    float closestDistanceSqr = Mathf.Infinity;
    public void AirStrikeControls()
    {
        if (m_controller.Action4WasPress() && !m_airStrikeState)
        {
            EnableAirStrikeMarker();
        }
        else if (m_controller.Action4WasPress() && m_airStrikeState)
        {
            EnableNavigationMarker();
        }

		if (m_airStrikeState && m_controller.Action1WasPress() && airstrikes.Count > 0)
        {
            Instantiate(m_airStrike, m_currentMarker.transform.position, m_currentMarker.transform.rotation);

            if (airstrikes[0].gameObject.activeInHierarchy)
            {
                airstrikes[0].capturePercentage = 0;
                airstrikes[0].owner = Team.NONE;
				airstrikes [0].captureZoneText.text = "";
                
            }
            airstrikes.Remove(airstrikes[0]);

            EnableNavigationMarker();
        }
    }

    bool colored;
    void ColorMeBoi(Color c)
    {
        colored = true;
        m_currentMarker.GetComponent<MarkerColourChanger>().ChangeColour(c);
    }

    void ResetMyColor()
    {
        colored = false;
        m_currentMarker.GetComponent<MarkerColourChanger>().ResetColor();
    }

    /*Enable the airstrike maker, set airstrike state to true which stops the navigation marker script to stop updating, adn the airstrike script to begin updating*/
    void EnableAirStrikeMarker()
    {
        m_airStrikeState = true;
        GameObject prevMarker = m_currentMarker;
        m_currentMarker = Instantiate(m_airStrikeMarker, new Vector3(prevMarker.transform.position.x, 1, prevMarker.transform.position.z), Quaternion.identity);
        Destroy(prevMarker);
    }

	public bool ChangeSate = false;
    /*The reverse of EnableAirStrikeMarker()*/
    void EnableNavigationMarker()
    {
		ChangeSate = true;
		m_airStrikeState = false;
        GameObject prevMarker = m_currentMarker;
        m_currentMarker = Instantiate(m_navMarker, new Vector3(prevMarker.transform.position.x, 4, prevMarker.transform.position.z), Quaternion.identity);
        Destroy(prevMarker);
    }

    /*Which enemy do we attack, if tank is not null*/
    public TroopActor GetEnemyToAttack()
    {
        if (m_tank != null)
            return m_tank;
        else
            return null;
    }

    private void OnDrawGizmosSelected()
    {
        if(troopController.currentSelectedUnit != null) 
            Gizmos.DrawWireSphere(troopController.currentSelectedUnit.transform.position, maxMarkerDistanceFromUnit);

		Gizmos.color = Color.green;
		if (m_currentMarker != null)
		Gizmos.DrawWireSphere (m_currentMarker.transform.position, 20.0f);
    }

    public void moveMarkerToMyBoy()
    {
        m_currentMarker.transform.position = troopController.currentSelectedUnit.transform.position;
    }
}
