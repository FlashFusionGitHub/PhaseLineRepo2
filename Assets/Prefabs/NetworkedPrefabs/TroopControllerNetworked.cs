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
    bool moveToSwitch;

    [SerializeField] GameObject selectionCircle;
    public GameObject currentSelectionCircle;

    [SerializeField] CameraController cameraController;

    // Use this for initialization
    void Start () {
         m_op = FindObjectOfType<ObjectPoolNetworked>();
         m_nmn = GetComponent<NavigationMarkerNetworked>();

         if (isServer)
             m_generals = m_op.team1Generals;
         else
             m_generals = m_op.team2Generals;

         currentSelectionCircle = Instantiate(selectionCircle, m_generals[0].transform.position, Quaternion.Euler(-90, 0, 0));
    }
	
	// Update is called once per frame
	void Update () {

        m_controller = InputManager.Devices[0];

        if (m_generals.Count == 0)
            Destroy(currentSelectionCircle);

        if (m_controller.RightStickButton.WasPressed)
        {
            QuickSelect();
        }

        if (m_controller.Action1.WasPressed)
        {
            m_generals[tankIndex].GetComponent<NavMeshAgent>().SetDestination(m_nmn.m_currentMarker.transform.position);
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

        if (currentSelectionCircle != null && tankIndex >= 0 && m_generals.Count > 0)
            currentSelectionCircle.transform.position = m_generals[tankIndex].transform.position;
    }

    void QuickSelect()
    {
        if (moveToSwitch)
        {
            moveToSwitch = false;
            cameraController.MoveCameraTo(m_generals[tankIndex].transform.position.x, m_generals[tankIndex].transform.position.z);
        }
        else
        {
            moveToSwitch = true;
            cameraController.MoveCameraTo(m_nmn.m_currentMarker.transform.position.x, m_nmn.m_currentMarker.transform.position.z);
        }
    }

    void CheckGeneralState(bool increase, bool decrease)
    {
        if (increase)
        {
            tankIndex++;

            if (tankIndex >= m_generals.Count)
                tankIndex = 0;

            cameraController.MoveCameraTo(m_generals[tankIndex].transform.position.x, m_generals[tankIndex].transform.position.z - 10);

            currentSelectionCircle = Instantiate(selectionCircle, m_generals[tankIndex].transform.position, Quaternion.Euler(-90, 0, 0));
        }
        if (decrease)
        {
            if (tankIndex <= 0)
                tankIndex = m_generals.Count;

            tankIndex--;

            cameraController.MoveCameraTo(m_generals[tankIndex].transform.position.x, m_generals[tankIndex].transform.position.z - 10);

            currentSelectionCircle = Instantiate(selectionCircle, m_generals[tankIndex].transform.position, Quaternion.Euler(-90, 0, 0));
        }
    }
}
