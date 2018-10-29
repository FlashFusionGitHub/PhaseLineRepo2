using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public Team team;

    public int damage;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<TroopActor>())
        {
            if (team == Team.TEAM2)
            {
                if (other.GetComponent<TroopActor>().team == Team.TEAM1)
                {
                    other.gameObject.GetComponent<TroopActor>().TakeDamage(damage);
                }
            }

            if (team == Team.TEAM1)
            {
                if (other.gameObject.GetComponent<TroopActor>().team == Team.TEAM2)
                {
                    other.gameObject.GetComponent<TroopActor>().TakeDamage(damage);
                }
            }
        }

        if (team == Team.NONE)
        {
            //Damage objects in scene with explosions
        }
    }
}
