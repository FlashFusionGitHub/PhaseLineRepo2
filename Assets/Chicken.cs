using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*NPC Chicken brain*/
public class Chicken : MonoBehaviour {

    public float duration;    //the max time of a walking session (set to ten)
    float elapsedTime = 0f; //time since started walk
    public float wait = 0f; //wait this much time
    float waitTime = 0f; //waited this much time

    bool move = true; //start moving

    bool angered; //the chickens angered state

    GameObject tank; //the object to chase

    public int chickenSpeed; // chickens happy speed (set to five)
    public int chickenChaseSpeed; // chickens angry speed (set to twenty)
    public float chaseTimeout = 1.0f; //time to wait before chicken starts chasing (This prevents chicken getting stuck inside of tank)

    public float chaseTime; //The time the chicken will chase a tank(set to ten)
    float chaseTimer;

    /*Unity events for triggering animations*/
    public UnityEvent onChaseStart;
    public UnityEvent onChaseEnd;
    public UnityEvent onMoveStart;
    public UnityEvent onMoveEnd;

    Vector3 target; //The next target the chicken will translate too

    void Start()
    {
        target = transform.position;
        chaseTimer = chaseTime;
    }

    void Update()
    {
        //Chicken has fallen of the map, destory the chicken
        if(transform.position.y < -500.0f) {
            Destroy(gameObject);
        }

        if (!angered)
        {
            if (move)
            {
                if (elapsedTime < duration)
                {
                    //move to the target location, look at the target location, increment time
                    transform.position = Vector3.MoveTowards(transform.position, target, chickenSpeed * Time.deltaTime);
                    transform.LookAt(target);
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    //do not move and start waiting for random time
                    wait = Random.Range(2, 4);
                    waitTime = 0f;
                    move = false;
                    onMoveEnd.Invoke();
                }
            }

            if(!move)
            {
                //increment wait time
                waitTime += Time.deltaTime;
           
                if(waitTime > wait)
                {
                    move = true;
                    onMoveStart.Invoke();
                    elapsedTime = 0f;
                    target = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
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
        //did we collide with a troop actor?
        if(collider.gameObject.GetComponent<TroopActor>())
        {
            //set tank to the collision entity and invoke MakeAngry()
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
