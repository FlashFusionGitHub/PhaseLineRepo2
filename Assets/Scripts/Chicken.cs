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

    bool move = false; //start moving

    bool angered; //the chickens angered state

    public GameObject tank; //the object to chase

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

    Chicken[] myFriends;

    public Color[] potentialColours;
    public Renderer[] renderer;

    public bool randomiseMySize;

    void Start()
    {

        ChickidyRandomiser();

        myFriends = FindObjectsOfType<Chicken>();
        target = transform.position;
        chaseTimer = chaseTime;
    }

    void ChickidyRandomiser()
    {
        if(randomiseMySize)
        {
            float chickenSize = Random.Range(0.5f, 2.5f);
            this.gameObject.transform.localScale = new Vector3(chickenSize, chickenSize, chickenSize);
        }

        Color colour = potentialColours[Random.Range(0, 3)];

        for(int i = 0; i < renderer.Length; i++)
            renderer[i].material.SetColor(Shader.PropertyToID("_TeamColor"), colour);
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
                    if (Vector3.Distance(transform.position, target) <= chickenSpeed * Time.deltaTime)
                    {
                        elapsedTime = duration;

                    }
                    transform.LookAt(target);
                    elapsedTime += Time.deltaTime;
                }
                else
                {

                    //do not move and start waiting for random time
                    wait = Random.Range(wait + wait * 0.05f, wait - wait * 0.05f);
                    waitTime = 0f;
                    onMoveEnd.Invoke();
                    move = false;
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
                    target = transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
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
            TriggerMyFriends();
        }
    }

    public float friendZone = 50f;
    void TriggerMyFriends()
    {
        Transform me = this.transform;
        foreach (Chicken myFriend in myFriends)
        {
            if(myFriend != null)
            {
                if (Vector3.Distance(me.position, myFriend.transform.position) < friendZone)
                {
                    myFriend.tank = tank;
                    myFriend.Invoke("MakeAngry", myFriend.chaseTimeout);
                }
            }
        }
    }

    private void MakeAngry()
    {
        angered = true;
        onChaseStart.Invoke();
    }   
}
