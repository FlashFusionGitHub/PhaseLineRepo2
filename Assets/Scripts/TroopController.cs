﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopController : MonoBehaviour {

    /*Used for indexing through all available units*/
    int index = 0;

    [Header("Selection Circle")]
    [SerializeField]
    private GameObject m_selectionCircle;
    private GameObject m_currentSelectionCircle;

    List<CaptureZoneActor> zonesCaptured; /*A list of all the zones This player has captured*/

    protected Controller m_controller; /*reference to the Controller*/

    public NavigationArrowActor m_navigationArrowActor; /*reference to the navigation marker*/

    public CameraController cameraController; /*reference to the cameraController*/

    public Team team; /*What team does this script belong too*/

    /*Referecne to the object pool game object*/
    public ObjectPool op;

    public List<TroopActor> m_generals; /*A list of all my available generals*/

    public GameObject currentSelectedUnit; /*Reference to the currently selected unit*/

    // Use this for initialization
    protected virtual void Start () {

        op = FindObjectOfType<ObjectPool>();

        if (team == Team.TEAM1)
            m_generals = op.team1Generals;
        if (team == Team.TEAM2)
            m_generals = op.team2Generals;

        m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[0].transform.position, Quaternion.Euler(-90, 0, 0));

        currentSelectedUnit = m_generals[index].gameObject;

        cameraController.MoveCameraTo(m_generals[index].transform.position);
    }

    // Update is called once per frame
    protected virtual void Update () {

        if(m_controller == null)
        {
            foreach (Controller c in FindObjectsOfType<Controller>())
            {
                if (team == Team.TEAM1 && c.m_playerIndex == 0)
                {
                    m_controller = c;
                }

                if (team == Team.TEAM2 && c.m_playerIndex == 1)
                {
                    m_controller = c;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A) && m_generals.Count > 0) {
            foreach (TroopActor gen in m_generals.ToArray()) {
                gen.Die(gen);
            }
        }

        if (m_generals.Count == 0)
            Destroy(m_currentSelectionCircle);

        QuickSelect();

        if (m_controller.DpadLeftWasPress() && m_generals.Count > 1) {
            //destroy the currect circle
            if (m_currentSelectionCircle != null)
                Destroy(m_currentSelectionCircle);

            CheckGeneralState(false, true);
        }

        if (m_controller.DpadRightWasPress() && m_generals.Count > 1) {
            //destory the currect circle
            if (m_currentSelectionCircle != null)
                Destroy(m_currentSelectionCircle);

            CheckGeneralState(true, false);
        }

        if (m_controller.Action1WasPress() && !m_navigationArrowActor.m_airStrikeState)
        {
            m_generals[index].moveTarget.transform.position = m_navigationArrowActor.m_currentMarker.transform.position;
            m_generals[index].moveTarget.transform.rotation = m_navigationArrowActor.m_currentMarker.transform.rotation;
        }

        if (m_currentSelectionCircle != null && index >= 0 && m_generals.Count > 0)
            m_currentSelectionCircle.transform.position = m_generals[index].transform.position;
    }

    /*Set cameras postion to be looking at the current unit, or the current marker*/
    void QuickSelect()
    {
        if (m_controller.RightStickButton())
        {
            cameraController.MoveCameraTo(m_navigationArrowActor.m_currentMarker.transform.position);
        }

        if (m_controller.LeftStickButton())
        {
            cameraController.MoveCameraTo(m_generals[index].transform.position);
        }
    }

    /*Toggle between Generals, decrement or increment the generals list, but setting true or false for the function parameters*/
    void CheckGeneralState(bool increase, bool decrease) {

        if (increase) {
            index++;

            if (index >= m_generals.Count)
                index = 0;

            cameraController.MoveCameraTo(m_generals[index].transform.position);

            currentSelectedUnit = m_generals[index].gameObject;

            m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[index].transform.position, Quaternion.Euler(-90, 0, 0));
        }
        if (decrease) {
            if (index <= 0)
                index = m_generals.Count;

            index--;

            cameraController.MoveCameraTo(m_generals[index].transform.position);

            currentSelectedUnit = m_generals[index].gameObject;

            m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[index].transform.position, Quaternion.Euler(-90, 0, 0));
        }
    }
}
