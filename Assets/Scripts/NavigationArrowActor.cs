using System.Collections;
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

    public List<AirStrike> airstrikes; /*A list of avaiable airstrikes*/

    public float floatValue = 1f;

    public LayerMask terrainMask; /*Refernce to the terrain mask, used for raycasting to check ground level*/

    public float maxMarkerDistanceFromUnit = 500.0f; /*Max distance the current navigation marker can move from the selected unit*/

    public TroopController troopController; /*Reference to THIS players the troop controoler*/

    Transform prevPos; /*The previous position of the current marker*/

	ObjectPool op;

    // Use this for initialization
    protected virtual void Start()
    {
		op = FindObjectOfType<ObjectPool> ();

        GameObject previousPos = new GameObject("previous Pos");
        prevPos = previousPos.transform;
        m_currentMarker = Instantiate(m_navMarker, new Vector3(0, 4, 0), Quaternion.identity);
        airstrikes = new List<AirStrike>();

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

        if (Vector3.Distance(m_currentMarker.transform.position, troopController.currentSelectedUnit.transform.position) <= maxMarkerDistanceFromUnit)
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
            if (prevPos.position == Vector3.zero)
            {
                prevPos.position = troopController.currentSelectedUnit.transform.position;
            }
            
			m_currentMarker.transform.position = Vector3.Lerp(m_currentMarker.transform.position, troopController.currentSelectedUnit.transform.position, Time.deltaTime * (PlayerPrefs.GetFloat("MarkerSpeedPlayer" + playerIndex)));
            //m_currentMarker.transform.position = prevPos.position - ((m_currentMarker.transform.position - troopController.currentSelectedUnit.transform.position).normalized * 10f);
        }

        AirStrikeControls();

		ClosestEnemyUnit ();
    }
		
	void ClosestEnemyUnit() {
		float dis = 0;
		foreach (TroopActor T in op.allTroopActors) {
			if (m_tank == null && T.team != m_team && T.rankState != RankState.dead) {
				if (Vector3.Distance (m_currentMarker.transform.position, T.transform.position) < 20f) {
					if (dis == 0 || Vector3.Distance (m_currentMarker.transform.position, T.transform.position) < dis) {
						dis = Vector3.Distance (m_currentMarker.transform.position, T.transform.position);
						m_tank = T;
					}
				}
			}
		}

		if (m_tank != null) {
			if (Vector3.Distance (m_currentMarker.transform.position, m_tank.transform.position) > 20f)
				m_tank = null;
		}
	}

    public void AirStrikeControls()
    {
        if (m_controller.Action3WasPress() && !m_airStrikeState && airstrikes.Count > 0)
        {
            EnableAirStrikeMarker();
        }
        else if (m_controller.Action3WasPress() && m_airStrikeState)
        {
            EnableNavigationMarker();
        }

        CaptureZoneActor nearestCaptureZone = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach(AirStrike a in airstrikes)
        {
            if(a.captureZone != null)
            {
                Vector3 directionToTarget = a.captureZone.transform.position - m_currentMarker.transform.position;
                float sqrToTarget = directionToTarget.sqrMagnitude;

                if (sqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = sqrToTarget;
                    nearestCaptureZone = a.captureZone;
                }
            }
        }

        if (m_airStrikeState && m_controller.Action1WasPress())
        {

            Instantiate(m_airStrike, m_currentMarker.transform.position, m_currentMarker.transform.rotation);
            EnableNavigationMarker();

            airstrikes.Remove(nearestCaptureZone.airStrike);
            Destroy(nearestCaptureZone.gameObject);
        }
    }

    /*Enable the airstrike maker, set airstrike state to true which stops the navigation marker script to stop updating, adn the airstrike script to begin updating*/
    void EnableAirStrikeMarker()
    {
        m_airStrikeState = true;
        GameObject prevMarker = m_currentMarker;
        m_currentMarker = Instantiate(m_airStrikeMarker, new Vector3(prevMarker.transform.position.x, 1, prevMarker.transform.position.z), Quaternion.identity);
        Destroy(prevMarker);
    }

    /*The reverse of EnableAirStrikeMarker()*/
    void EnableNavigationMarker()
    {
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

		Gizmos.DrawWireSphere (m_currentMarker.transform.position, 20.0f);
    }

    public void moveMarkerToMyBoy()
    {
        m_currentMarker.transform.position = troopController.currentSelectedUnit.transform.position;
    }
}
