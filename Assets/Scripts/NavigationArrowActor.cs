using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Team { NONE, TEAM1, TEAM2 };

public class NavigationArrowActor : MonoBehaviour
{
    public TroopActor m_tank;
    public GameObject m_airStrikeMarker;
    public GameObject m_navMarker;
    public GameObject m_airStrike;

    public float m_minXPos = -100, maxXPos = 100;
    public float m_minZPos = -100, maxZPos = 100;

    public float m_markerSpeed = 2;

    public Team m_team;

    public int playerIndex;

    public GameObject m_currentMarker;

    public bool m_airStrikeState;

    protected Controller m_controller;

    public List<AirStrike> airstrikes;

    public float floatValue = 1f;

    public LayerMask terrainMask;

    public float maxMarkerDistanceFromUnit = 500.0f;

    public TroopController troopController;

    Transform prevPos;

    // Use this for initialization
    protected virtual void Start()
    {
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
            
            m_currentMarker.transform.position = prevPos.position - ((m_currentMarker.transform.position - troopController.currentSelectedUnit.transform.position).normalized * 10f);
        }

        AirStrikeControls();

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

        if (Input.GetKeyDown(KeyCode.G) && !m_airStrikeState && airstrikes.Count > 0)
        {
            EnableAirStrikeMarker();

            Destroy(nearestCaptureZone.gameObject);
            airstrikes.Remove(nearestCaptureZone.GetComponent<AirStrike>());

            Instantiate(m_airStrike, m_currentMarker.transform.position, m_currentMarker.transform.rotation);
            EnableNavigationMarker();
        }

        if (m_airStrikeState && m_controller.Action1WasPress())
        {

            Instantiate(m_airStrike, m_currentMarker.transform.position, m_currentMarker.transform.rotation);
            EnableNavigationMarker();

            airstrikes.Remove(nearestCaptureZone.airStrike);
            Destroy(nearestCaptureZone.gameObject);
        }
    }

    void EnableAirStrikeMarker()
    {
        m_airStrikeState = true;
        GameObject prevMarker = m_currentMarker;
        m_currentMarker = Instantiate(m_airStrikeMarker, new Vector3(prevMarker.transform.position.x, 1, prevMarker.transform.position.z), Quaternion.identity);
        Destroy(prevMarker);
    }

    void EnableNavigationMarker()
    {
        m_airStrikeState = false;
        GameObject prevMarker = m_currentMarker;
        m_currentMarker = Instantiate(m_navMarker, new Vector3(prevMarker.transform.position.x, 4, prevMarker.transform.position.z), Quaternion.identity);
        Destroy(prevMarker);
    }

    public TroopActor GetEnemyToAttack()
    {
        if (m_tank != null)
            return m_tank;
        else
            return null;
    }

    void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.GetComponent<TroopActor>().team == Team.TEAM2)
        // {
        //     m_tank = other.GetComponent<TroopActor>();
        // }
    }

    void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.GetComponent<TroopActor>().team == Team.TEAM2)
        //{
        //    m_tank = other.GetComponent<TroopActor>();
        // }
    }

    private void OnDrawGizmosSelected()
    {
        if(troopController.currentSelectedUnit != null) 
            Gizmos.DrawWireSphere(troopController.currentSelectedUnit.transform.position, maxMarkerDistanceFromUnit);
    }
}
