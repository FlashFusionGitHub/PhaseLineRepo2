using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

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

    public GameObject m_currentMarker;

    public bool m_airStrikeState;

    protected InputDevice m_controller;

    public int playerIndex;

    public int AirStrikeCount;

    public float floatValue = 1f;

    public LayerMask terrainMask;

    // Use this for initialization
    protected virtual void Start()
    {
        m_currentMarker = Instantiate(m_navMarker, new Vector3(0, 4, 0), Quaternion.identity);
    }

    // Update is called once per frame
    protected virtual void Update()
    {

        Debug.Log(AirStrikeCount);

        m_controller = InputManager.Devices[playerIndex];

        float markerXPos = Mathf.Clamp(m_currentMarker.transform.position.x, m_minXPos, maxXPos);
        float markerZPos = Mathf.Clamp(m_currentMarker.transform.position.z, m_minZPos, maxZPos);

        m_currentMarker.transform.position = new Vector3(markerXPos, m_currentMarker.transform.position.y, markerZPos);

        var objPos = m_currentMarker.transform.position;

        //For the Optional solution Delete from Here
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(objPos.x, 500, objPos.z), Vector3.down, out hit, 800f, terrainMask))
        {
            Debug.DrawLine(new Vector3(objPos.x, 500, objPos.z), hit.point);
            m_currentMarker.transform.Translate(0, (floatValue - hit.distance), 0);
            m_currentMarker.transform.position += new Vector3(m_controller.LeftStickX, 0, m_controller.LeftStickY) * m_markerSpeed;
            m_currentMarker.transform.position = new Vector3(m_currentMarker.transform.position.x, hit.point.y, m_currentMarker.transform.position.z);
        }
        else
        {
            m_currentMarker.transform.position += new Vector3(m_controller.LeftStickX, 0, m_controller.LeftStickY) * m_markerSpeed;
        }

        AirStrikeControls();

    }

    public void AirStrikeControls()
    {
        if (m_controller.Action3.WasPressed && !m_airStrikeState && AirStrikeCount > 0)
        {
            EnableAirStrikeMarker();
        }
        else if (m_controller.Action3.WasPressed && m_airStrikeState)
        {
            EnableNavigationMarker();
        }

        if (m_airStrikeState && m_controller.Action1.WasPressed)
        {
            AirStrikeCount--;
            Instantiate(m_airStrike, m_currentMarker.transform.position, m_currentMarker.transform.rotation);
            EnableNavigationMarker();
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
        // if (other.gameObject.GetComponent<TroopActor>().team == Team.TEAM2)
        //{
        //    m_tank = other.GetComponent<TroopActor>();
        // }
    }
}
