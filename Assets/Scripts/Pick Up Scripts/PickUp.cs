using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUp : MonoBehaviour {

    public UnityEvent OnColEnter;
    public TroopActor lastCollided;

    private void OnTriggerEnter(Collider collision)
    {
        TroopActor ta = collision.gameObject.GetComponent<TroopActor>();
        if (ta)
        {
            lastCollided = ta;
            OnColEnter.Invoke();
        }
    }

    public void RandomBuff(float percentageIncrease)
    {
        if (lastCollided)
            lastCollided.AddRandomBuff(percentageIncrease);
    }
    public void HealthBuff(float percentageIncrease)
    {
        if (lastCollided)
            lastCollided.AddHealth(percentageIncrease);
    }
    public void RangeBuff(float percentageIncrease)
    {
        if (lastCollided)
            lastCollided.AddRange(percentageIncrease);
    }
    public void InfluenceBuff(float percentageIncrease)
    {
        if (lastCollided)
            lastCollided.AddInfluence(percentageIncrease);
    }
    public void DamageBuff(float percentageIncrease)
    {
        if (lastCollided)
            lastCollided.AddDamage(percentageIncrease);
    }
}
