using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public Team team; /*The team the explosion can damage*/

    public int damage; /*The amount of damage the explosion will do*/

    private void OnTriggerEnter(Collider other)
    {

        /*If a gamobject with a Troopactor component is within the blast radius, then apply damage the the corresponding team*/
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
    }
}
