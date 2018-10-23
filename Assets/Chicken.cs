using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chicken : MonoBehaviour {

    public float duration;    //the max time of a walking session (set to ten)
    float elapsedTime = 0f; //time since started walk
    float wait = 0f; //wait this much time
    float waitTime = 0f; //waited this much time

    bool move = true; //start moving

    bool angered;

    GameObject tank;

    public int chickenChaseSpeed;
    public float chaseTimeout = 1.0f;

    public float chaseTime = 5;
    float chaseTimer;

    public UnityEvent onChaseStart;
    public UnityEvent onChaseEnd;
    public UnityEvent onMoveStart;
    public UnityEvent onMoveEnd;

    Vector3 target;

    void Start()
    {
        target = transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

        chaseTimer = chaseTime;
    }

    void Update()
    {
        if (!angered)
        {
            if (move)
            {
                if (elapsedTime < duration)
                {
                    //if its moving and didn't move too much
                    transform.position = Vector3.MoveTowards(transform.position, target, 5 * Time.deltaTime);

                    transform.LookAt(target);

                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    //do not move and start waiting for random time
                    wait = Random.Range(2, 5);
                    waitTime = 0f;
                    move = false;
                    onMoveEnd.Invoke();
                }
            }

            if(!move)
            {
                waitTime += Time.deltaTime;
           
                if(waitTime > wait)
                {
                    move = true;
                    onMoveStart.Invoke();
                    elapsedTime = 0f;
                    target = transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
                }
            }
        }
        else
        {
            chaseTimer -= Time.deltaTime;

            if(chaseTimer > 0)
            {
                if(Vector3.Distance(this.transform.position, tank.transform.position) > 10)
                {
                    transform.position = Vector3.MoveTowards(transform.position, tank.transform.position, chickenChaseSpeed * Time.deltaTime);
                    transform.LookAt(tank.transform);
                }
            }
            else
            {
                chaseTimer = chaseTime;
                angered = false;
                onChaseEnd.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.GetComponent<TroopActor>())
        {
            tank = collider.gameObject.GetComponent<TroopActor>().gameObject;
            Invoke("MakeAngry", chaseTimeout);
        }
    }

    private void MakeAngry()
    {
        angered = true;
        onChaseStart.Invoke();
    }   
}
