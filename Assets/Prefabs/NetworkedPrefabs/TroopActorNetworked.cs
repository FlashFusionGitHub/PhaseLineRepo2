using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;

public class TroopActorNetworked : NetworkBehaviour
{
    [Header("Health Settings")]
    public const int maxHealth = 100;
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
    public RectTransform healthBar;

    public Team team;
    public RankState rankState;

    /*[Header("Unit Settings")]
    public UnitClasses unitClass;
    public Team team;
    [SerializeField] UnitClasses[] strengths;
    [SerializeField] UnitClasses[] vulnerabilities;

    [Header("Rank Settings")]
    public RankState rankState;
    public TroopActor myGeneral;
    float killMeAfter = 0.5f; //?????? wtf

    [Header("Formation Settings")]
    [SerializeField] PlacementDirection placementDirection;
    [SerializeField] float numOfRows;
    [SerializeField] float maxNumOfColumns;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] Vector2 distanceBetweenPoints;
    public List<FormationPosition> formationPositions = new List<FormationPosition>();
    [SerializeField] Transform oldPos;
    [SerializeField] PlacementDirection pd;
    [SerializeField] int count;

    [Header("Promotion Settings")]
    [SerializeField] int rank;
    [SerializeField] UnityEvent OnGainExperience;
    [SerializeField] UnityEvent OnPromotion;
    [SerializeField] UnityEvent OnBecomeGeneral;
    [SerializeField] string[] rankTitles;
    [SerializeField] float experienceRequiredForNextRank;
    [SerializeField] float currentExperience;
    [SerializeField] float percentageIncreasePerRank;
    [SerializeField] float influenceRadius;

    [Header("Name Settings")]
    public string unitName;
    [SerializeField] string[] possibleNames;

    [Header("Health Settings")]
    public const int maxHealth = 100;
    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;
    public RectTransform healthBar;
    [SerializeField] UnityEvent onTakeDamage;
    [SerializeField] UnityEvent onDie;

    [Header("Movement Settings")]
    [SerializeField] bool moving;
    [SerializeField] UnityEvent OnMove;
    public Transform moveTarget;
    [SerializeField] MovementTypes movementType;
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed;
    public GameObject moveTargetPrefab;
    public GameObject generalMoveTargetPrefab;

    [Header("Air Movement Settings")]
    [SerializeField] float hoverHeight;
    [SerializeField] float verticleSpeed;

    [Header("Ground Movement Settings")]
    //[SerializeField] NavMeshAgent m_navAgent; ??

    [Header("Object Avoidance Settings")]
    [SerializeField] float avoidanceRange;

    [Header("Attack Settings")]
    [SerializeField] GunSettings[] guns;*/

    //[Header("Important Optimisation List")]
    //public ObjectPool op;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
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
