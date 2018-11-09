using System.Collections;
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

    public List<TroopActor> m_generals = new List<TroopActor>(); /*A list of all my available generals*/

    public GameObject currentSelectedUnit; /*Reference to the currently selected unit*/

	public TroopActor UnitToAttack;

    public TroopActor currentGeneral;

    // Use this for initialization
    protected virtual void Start () {

        op = FindObjectOfType<ObjectPool>();

        if (team == Team.TEAM1)
            m_generals = op.team1Generals;
        if (team == Team.TEAM2)
            m_generals = op.team2Generals;

        m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[0].transform.position, Quaternion.Euler(-90, 0, 0));

        currentSelectedUnit = m_generals[index].gameObject;
        m_navigationArrowActor.moveMarkerToMyBoy();

        cameraController.MoveCameraTo(m_generals[index].transform.position);

        currentGeneral = m_generals[index];

        DisablesMoveTargets();
    }

    TroopActor checkThisGeneral;
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

        if(currentGeneral != m_generals[index])
        {
            Destroy(m_currentSelectionCircle);
        }

        if(m_generals[index].rankState == RankState.dead)
        {
            currentSelectedUnit = null;
        }

        if (m_generals.Count == 0)
        {
            Destroy(m_currentSelectionCircle);
        }
        else
        {
            QuickSelect();

            if (m_controller.DpadLeftWasPress() && m_generals.Count > 0)
            {
                //destroy the currect circle
                if (m_currentSelectionCircle != null)
                    Destroy(m_currentSelectionCircle);

                CheckGeneralState(false, true);
            }

            if (m_controller.DpadRightWasPress() && m_generals.Count > 0)
            {
                //destory the currect circle
                if (m_currentSelectionCircle != null)
                    Destroy(m_currentSelectionCircle);

                CheckGeneralState(true, false);
            }

            if (m_navigationArrowActor.m_tank != null && m_controller.Action3WasPress() && !m_navigationArrowActor.m_airStrikeState)
            {
                if (UnitToAttack != m_navigationArrowActor.m_tank)
                {
                    UnitToAttack = m_navigationArrowActor.m_tank;
                }
            }
            else if (m_controller.Action1WasPress() && !m_navigationArrowActor.m_airStrikeState)
            {
                if (UnitToAttack != null)
                {
                    UnitToAttack = null;
                }

                m_generals[index].targetToAttack = null;
                m_generals[index].SetAttackType(AttackType.AUTO);
                foreach (TroopActor T in op.allTroopActors)
                {
                    if (T.myGeneral == m_generals[index])
                    {
                        T.targetToAttack = null;
                        T.SetAttackType(AttackType.AUTO);
                    }
                }

                m_generals[index].moveTarget.transform.position = m_navigationArrowActor.m_currentMarker.transform.position;
                m_generals[index].moveTarget.transform.rotation = m_navigationArrowActor.m_currentMarker.transform.rotation;
            }

            SquadGangAttackSelectedUnit();

            if (m_currentSelectionCircle != null && index >= 0 && m_generals.Count > 0)
                m_currentSelectionCircle.transform.position = m_generals[index].transform.position;
        }
    }


    //Disable move targets for unselected general troops
    public void DisablesMoveTargets()
    {
        foreach(TroopActor ta in op.allTroopActors)
        {
            if (ta.rankState == RankState.IsGeneral && ta != currentGeneral)
            {
                foreach (TurnThisOff tto in ta.moveTarget.GetComponentsInChildren<TurnThisOff>())
                {
                    tto.TurnOff();
                }
            }
        }
    }

    void EnableMoveTargetsForSelectedGeneral()
    {
        foreach (TroopActor ta in op.allTroopActors)
        {
            if (ta == currentGeneral)
            {
                foreach (TurnThisOff tto in ta.moveTarget.GetComponentsInChildren<TurnThisOff>())
                {
                    tto.TurnOn();
                }
            } 
        }
    }

	int underlingsCount;
	void SquadGangAttackSelectedUnit() {
		//foreach Troopactor (T) under this generals control
		//set (T) targetToAttack to the generals attackTarget
		//attack until the targetToAttack is dead or the generals attackTarget != null

		if (UnitToAttack != null) {
			m_generals [index].targetToAttack = UnitToAttack;
			underlingsCount = 0;
			foreach (TroopActor T in op.allTroopActors) {
				if (T.myGeneral == m_generals [index]) {
					T.targetToAttack = UnitToAttack;
					T.SetAttackType (AttackType.SELECTED);
					underlingsCount++;
				}
			}

			if (underlingsCount == 0) {
				m_generals [index].SetAttackType(AttackType.SELECTED);
			}
		}
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
            m_navigationArrowActor.moveMarkerToMyBoy();
        }
    }

    /*Toggle between Generals, decrement or increment the generals list, but setting true or false for the function parameters*/
    void CheckGeneralState(bool increase, bool decrease) {

        if (increase) {
            index++;

            if (index >= m_generals.Count)
                index = 0;


            currentGeneral = m_generals[index];

            currentSelectedUnit = m_generals[index].gameObject;
            m_navigationArrowActor.m_navMarker.transform.position = currentSelectedUnit.transform.position;

            m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[index].transform.position, Quaternion.Euler(-90, 0, 0));

            EnableMoveTargetsForSelectedGeneral();
            DisablesMoveTargets();

           
        }
        if (decrease) {
            if (index <= 0)
                index = m_generals.Count;

            index--;

            currentSelectedUnit = m_generals[index].gameObject;

            currentGeneral = m_generals[index];

            m_currentSelectionCircle = Instantiate(m_selectionCircle, m_generals[index].transform.position, Quaternion.Euler(-90, 0, 0));

            EnableMoveTargetsForSelectedGeneral();
            DisablesMoveTargets();

        }
        
    }
}
