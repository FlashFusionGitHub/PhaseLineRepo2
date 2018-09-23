using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TroopActorNetworked : NetworkBehaviour
{
    public enum Rank
    {
        LookingForGeneral,
        FollowingGeneral,
        IsGeneral,
        dead
    };

    public Team team;
    public Rank rank;

    [Header("Health Settings")]
    public const int maxHealth = 100;
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
    public RectTransform healthBar;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer)
            CmdSyncPos(transform.localPosition, transform.localRotation);
    }

    [Command]
    protected void CmdSyncPos(Vector3 localPos, Quaternion localRotation)
    {
        transform.localPosition = localPos;
        transform.localRotation = localRotation;
    }

    public void TakeDamage(int damageToTake)
    {
        //onTakeDamage.Invoke();
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            //Die(this);
        }
    }

    void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
    }
}
