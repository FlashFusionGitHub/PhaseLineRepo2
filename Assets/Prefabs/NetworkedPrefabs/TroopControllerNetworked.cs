using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.AI;
using UnityEngine.Networking;

public class TroopControllerNetworked : NetworkBehaviour {

    [SerializeField] ObjectPoolNetworked m_op;
    [SerializeField] NavigationMarkerNetworked m_nmn;

    InputDevice m_controller;

    [SerializeField] List<TroopActorNetworked> m_generals = new List<TroopActorNetworked>();

    int tankIndex = 0;

    [SerializeField] GameObject selectionCircle;
    public GameObject currentSelectionCircle;

    [SerializeField] CameraControllerNetworked cameraController;

    public GameObject currentSelectedUnit; /*Reference to the currently selected unit*/

    public TroopActorNetworked UnitToAttack;


    // Use this for initialization
    void Start () {
         m_op = FindObjectOfType<ObjectPoolNetworked>();
         m_nmn = GetComponent<NavigationMarkerNetworked>();

        if (isServer)
            m_generals = m_op.team1Generals;
        else
            m_generals = m_op.team2Generals;

        currentSelectedUnit = m_generals[tankIndex].gameObject;

        cameraController.MoveCameraTo(m_generals[tankIndex].transform.position);

        currentSelectionCircle = Instantiate(selectionCircle, m_generals[0].transform.position, Quaternion.Euler(-90, 0, 0));
    }
	
	// Update is called once per frame
	void Update () {

        try
        {
            m_controller = InputManager.Devices[0];
        }
        catch (System.Exception)
        {
            return;
        }

        if (m_generals.Count == 0)
            Destroy(currentSelectionCircle);

        QuickSelect();

        if (isClient && !isServer)
        {
            if (m_controller.Action1.WasPressed)
            {
                m_generals[tankIndex].moveTarget.transform.position = m_nmn.m_currentMarker.transform.position;
                m_generals[tankIndex].CmdUpdateMoveTargetPosition(m_nmn.m_currentMarker.transform.position, m_nmn.m_currentMarker.transform.rotation);
            }
        }
        else
        {
            if (m_controller.Action1.WasPressed)
            {
                m_generals[tankIndex].RpcUpdateMoveTargetPosition(m_nmn.m_currentMarker.transform.position, m_nmn.m_currentMarker.transform.rotation);
            }
        }

        if (m_controller.DPadLeft.WasPressed && m_generals.Count > 1)
        {
            //destory the currect circle
            if (currentSelectionCircle != null)
                Destroy(currentSelectionCircle);

            CheckGeneralState(false, true);
        }

        if (m_controller.DPadRight.WasPressed && m_generals.Count > 1)
        {
            //destory the currect circle
            if (currentSelectionCircle != null)
                Destroy(currentSelectionCircle);

            CheckGeneralState(true, false);
        }

        if (m_nmn.m_tank != null && m_controller.Action1.WasPressed && !m_nmn.m_airStrikeState)
        {
            UnitToAttack = m_nmn.m_tank;
        }
        else if (m_nmn.m_tank == null && m_controller.Action1.WasPressed && !m_nmn.m_airStrikeState)
        {
            UnitToAttack = null;

            m_generals[tankIndex].targetToAttack = null;
            m_generals[tankIndex].SetAttackType(AttackType.AUTO);
            foreach (TroopActorNetworked T in m_op.allTroopActors)
            {
                if (T.myGeneral == m_generals[tankIndex])
                {
                    T.targetToAttack = null;
                    T.SetAttackType(AttackType.AUTO);
                }
            }

            m_generals[tankIndex].moveTarget.transform.position = m_nmn.m_currentMarker.transform.position;
            m_generals[tankIndex].moveTarget.transform.rotation = m_nmn.m_currentMarker.transform.rotation;
        }

        SquadGangAttackSelectedUnit();

        if (currentSelectionCircle != null && tankIndex >= 0 && m_generals.Count > 0)
            currentSelectionCircle.transform.position = m_generals[tankIndex].transform.position;
    }

    int underlingsCount;
    void SquadGangAttackSelectedUnit()
    {
        //foreach Troopactor (T) under this generals control
        //set (T) targetToAttack to the generals attackTarget
        //attack until the targetToAttack is dead or the generals attackTarget != null

        if (UnitToAttack != null)
        {

            m_generals[tankIndex].targetToAttack = UnitToAttack;
            underlingsCount = 0;
            foreach (TroopActorNetworked T in m_op.allTroopActors)
            {
                if (T.myGeneral == m_generals[tankIndex])
                {
                    Debug.Log(UnitToAttack);
                    T.targetToAttack = UnitToAttack;
                    T.SetAttackType(AttackType.SELECTED);
                    underlingsCount++;
                }
            }

            if (underlingsCount == 0)
            {
                m_generals[tankIndex].SetAttackType(AttackType.SELECTED);
            }
        }
    }

    void QuickSelect()
    {
        if (m_controller.RightStickButton.WasPressed)
        {
            cameraController.MoveCameraTo(m_nmn.m_currentMarker.transform.position);
        }

        if (m_controller.LeftStickButton.WasPressed)
        {
            cameraController.MoveCameraTo(m_generals[tankIndex].transform.position);
        }
    }

    void CheckGeneralState(bool increase, bool decrease)
    {
        if (increase)
        {
            tankIndex++;

            if (tankIndex >= m_generals.Count)
                tankIndex = 0;

            cameraController.MoveCameraTo(m_generals[tankIndex].transform.position);

            currentSelectedUnit = m_generals[tankIndex].gameObject;

            currentSelectionCircle = Instantiate(selectionCircle, m_generals[tankIndex].transform.position, Quaternion.Euler(-90, 0, 0));
        }
        if (decrease)
        {
            if (tankIndex <= 0)
                tankIndex = m_generals.Count;

            tankIndex--;

            cameraController.MoveCameraTo(m_generals[tankIndex].transform.position);

            currentSelectedUnit = m_generals[tankIndex].gameObject;

            currentSelectionCircle = Instantiate(selectionCircle, m_generals[tankIndex].transform.position, Quaternion.Euler(-90, 0, 0));
        }
    }
}
