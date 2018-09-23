using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUp : MonoBehaviour {

    public UnityEvent OnColEnter;
    public TroopActor lastCollided;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<TroopActor>())
        {
            lastCollided = collision.gameObject.GetComponent<TroopActor>();
            OnColEnter.Invoke();
        }
    }

    public void RandomBuff(float percentageIncrease)
    {
        lastCollided.AddRandomBuff(percentageIncrease);
    }
    public void HealthBuff(float percentageIncrease)
    {
        lastCollided.AddHealth(percentageIncrease);
    }
    public void RangeBuff(float percentageIncrease)
    {
        lastCollided.AddRange(percentageIncrease);
    }
    public void InfluenceBuff(float percentageIncrease)
    {
        lastCollided.AddInfluence(percentageIncrease);
    }
    public void DamageBuff(float percentageIncrease)
    {
        lastCollided.AddDamage(percentageIncrease);
    }
}
