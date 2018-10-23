using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour {

    public float duration;    //the max time of a walking session (set to ten)
    float elapsedTime = 0f; //time since started walk
    float wait = 0f; //wait this much time
    float waitTime = 0f; //waited this much time

    float randomX;  //randomly go this X direction
    float randomZ;  //randomly go this Z direction

    bool move = true; //start moving

    bool angered;

    TroopActor tank;

    public float chaseTime = 5;
    float chaseTimer;
    void Start()
    {
        randomX = Random.Range(-5, 5);
        randomZ = Random.Range(-5, 5);

        chaseTimer = chaseTime;
    }

    void Update()
    {
        if(!angered)
        {
            if (move)
            {
                if (elapsedTime < duration)
                {
                    //if its moving and didn't move too much
                    transform.Translate(new Vector3(randomX, 0, randomZ) * Time.deltaTime);
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    //do not move and start waiting for random time
                    wait = Random.Range(2, 5);
                    waitTime = 0f;
                    move = false;
                }
            }

            if(!move)
            {
                waitTime += Time.deltaTime;
           
                if(waitTime > wait)
                {
                    move = true;
                    elapsedTime = 0f;
                    randomX = Random.Range(-3, 3);
                    randomZ = Random.Range(-3, 3);
                }
            }

            Debug.Log(wait);
        }
        else
        {
            chaseTime -= Time.deltaTime;

            if(chaseTimer > 0)
            {
                moveTowards((tank.transform.position) * Time.deltaTime);
            }
            else
            {
                chaseTimer = chaseTime;
                angered = false;
            }
        }
    }

    void moveTowards(Vector3 position)
    {
        transform.Translate(position * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<TroopActor>() == true)
        {
            angered = true;
            tank = collision.gameObject.GetComponent<TroopActor>();
        }
    }
}
