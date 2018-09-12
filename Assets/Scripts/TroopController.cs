using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class TroopController : MonoBehaviour {

    int index = 0;

    [Header("Selection Circle")]
    [SerializeField]
    private GameObject m_selectionCircle;
    private GameObject m_currentSelectionCircle;

    List<CaptureZoneActor> zonesCaptured;

    protected InputDevice m_controller;

    public NavigationArrowActor m_navigationArrowActor;

    public CameraController cameraController;

    public int playerIndex;

    public ObjectPool op;

    public List<TroopActor> m_generals;

    public int tankSize;

    // Use this for initialization
    protected virtual void Start () {

        if (playerIndex == 0)
            m_generals = op.team1Generals;
        if (playerIndex == 1)
            m_generals = op.team2Generals;

        m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[0].transform.position, Quaternion.Euler(-90, 0, 0));
        cameraController.MoveCameraTo(m_generals[index].transform.position.x, m_generals[index].transform.position.z - 10);
    }

    // Update is called once per frame
    protected virtual void Update () {

        m_controller = InputManager.Devices[playerIndex];

        if (Input.GetKeyDown(KeyCode.A) && m_generals.Count > 0) {
            foreach (TroopActor gen in m_generals.ToArray()) {
                gen.Die(gen);
            }
        }

        if (m_generals.Count == 0)
            Destroy(m_currentSelectionCircle);

        if (m_controller.DPadLeft.WasPressed && m_generals.Count > 1) {
            //destory the currect circle
            if (m_currentSelectionCircle != null)
                Destroy(m_currentSelectionCircle);

            CheckGeneralState(false, true);
        }

        if (m_controller.DPadRight.WasPressed && m_generals.Count > 1) {
            //destory the currect circle
            if (m_currentSelectionCircle != null)
                Destroy(m_currentSelectionCircle);

            CheckGeneralState(true, false);
        }

        if (m_controller.Action1.WasPressed && !m_navigationArrowActor.m_airStrikeState)
        {
            foreach(TroopActor tank in m_generals)
            {
                if(Vector3.Distance(m_generals[index].moveTarget.transform.position, tank.transform.position) < tankSize)
                {
                    Debug.Log("Space Occupied");
                    return;
                }
            }

            m_generals[index].moveTarget.transform.position = m_navigationArrowActor.m_currentMarker.transform.position;
        }

        if (m_currentSelectionCircle != null && index >= 0 && m_generals.Count > 0)
            m_currentSelectionCircle.transform.position = m_generals[index].transform.position;
    }

    void CheckGeneralState(bool increase, bool decrease) {
        if (increase) {
            index++;

            if (index >= m_generals.Count)
                index = 0;

            cameraController.MoveCameraTo(m_generals[index].transform.position.x, m_generals[index].transform.position.z - 10);

            m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[index].transform.position, Quaternion.Euler(-90, 0, 0));
        }
        if (decrease) {
            if (index <= 0)
                index = m_generals.Count;

            index--;

            cameraController.MoveCameraTo(m_generals[index].transform.position.x, m_generals[index].transform.position.z - 10);

            m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[index].transform.position, Quaternion.Euler(-90, 0, 0));
        }
    }
}
