using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckForGrounded : MonoBehaviour {

    Transform origin;

    Vector3 prevPos;
    Vector3 prevPosTarget;

    ObjectPool op;

    PointOfInterest myPoint;

    Transform attackTarget;
	// Use this for initialization
	void Awake () {
        op = FindObjectOfType<ObjectPool>();
        origin = new GameObject(gameObject.name + "'s Origin").transform;

        origin.parent = transform.parent;
        origin.localPosition = transform.localPosition;
        origin.localRotation = transform.localRotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (origin.parent != transform.parent)
        {
            origin.parent = transform.parent;
            origin.localPosition = transform.localPosition;
        }
        if (!myPoint)
        {
            myPoint = op.NearestPOI(origin.position);
            //Debug.Log("my Point = " + myPoint);
            myPoint.assignMoveTarget(this);
        }
        if (!attackTarget || !attackTarget.gameObject.activeInHierarchy)
            attacking = false;


        if (attacking)
        {
            if (attackTarget.position != prevPosTarget)
            {
                if (myPoint)
                {
                    myPoint.clearPoI();
                }
                myPoint = op.NearestPOI(attackTarget.position);
                myPoint.assignMoveTarget(this);
                transform.position = myPoint.transform.position;
                prevPosTarget = attackTarget.position;
            }
        }
        else
        {
            if (origin.position != prevPos)
            {
                if (myPoint)
                {
                    myPoint.clearPoI();
                }
                myPoint = op.NearestPOI(origin.position);
                myPoint.assignMoveTarget(this);
                transform.position = myPoint.transform.position;
                prevPos = origin.position;
            }
        }
    }

    bool attacking = false;
    public void AssignAttackTarget(Transform aT)
    {
        attackTarget = aT;
        attacking = true;
        Debug.Log(gameObject.name + " is in attackPosition against " + attackTarget.name);
        prevPosTarget = Vector3.zero;
    }

    public void StopAttacking()
    {
        attacking = false;
    }
}
