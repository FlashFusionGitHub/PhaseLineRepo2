using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.UI;

public class TroopActor : MonoBehaviour {

    [System.Serializable]
    public enum UnitClasses
    {
        Tank,
        AntAir,
        Helicopter
    }

    [System.Serializable]
    public enum RankState
    {
        LookingForGeneral,
        FollowingGeneral,
        IsGeneral,
        dead
    }

    [System.Serializable]
    public enum MovementTypes
    {
        Air,
        Ground
    }

    [System.Serializable]
    public class GunSettings
    {
        [Header("Gun Stuff")]
        public float damage;
        public float TimeBetweenShots;
        public float m_gunTimerRandomiser;
        public float m_gunTimer;
        public float attackRangeMin;
        public float attackRangeMax;
        public TroopActor attackTarget;

        [Header("Turret Settings")]
        public Transform turret;
        public float turretAimSpeed;

        [Header("Barrel Settings")]
        public Transform barrel;
        public UnityEvent onShoot;
        public UnityEvent onKillShot;
        public float barrelAimSpeed;
    }
    [System.Serializable]
    public class FormationPosition
    {
        public Transform fromPos;
        public TroopActor assignedUnit;
        public bool taken;
    }
    [System.Serializable]
    public enum PlacementDirection
    {
        left,
        right
    }

    [Header("Unit Settings")]
    [SerializeField] public UnitClasses unitClass;
    [SerializeField] public Team team;
    [SerializeField] private UnitClasses[] strengths;
    [SerializeField] private UnitClasses[] vulnerabilities;

    [Header("Rank Settings")]
    [SerializeField] public RankState rankState;
    [SerializeField] public TroopActor myGeneral;
    float killMeAfter = 0.5f;

    [Header("Formation Settings")]
    [SerializeField] private PlacementDirection placementDirection;
    [SerializeField] private float numOfRows;
    [SerializeField] private float maxNumOfColumns;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector2 distanceBetweenPoints;
    [SerializeField] public List<FormationPosition> formationPositions = new List<FormationPosition>();
    [SerializeField] private Transform oldPos;
    [SerializeField] private PlacementDirection pd;
    [SerializeField] private int count;


    [Header("Promotion Settings")]
    [SerializeField] private int rank;
    [SerializeField] private UnityEvent OnGainExperience;
    [SerializeField] private UnityEvent OnPromotion;
    [SerializeField] private UnityEvent OnBecomeGeneral;
    [SerializeField] private string[] rankTitles;
    [SerializeField] private float experienceRequiredForNextRank;
    [SerializeField] private float currentExperience;
    [SerializeField] private float percentageIncreasePerRank;
    [SerializeField] private float influenceRadius;

    [Header("Name Settings")]
    [SerializeField] public string unitName;
    [SerializeField] private string[] possibleNames;

    [Header("Health Settings")]
    public float maxHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private UnityEvent onTakeDamage;
    [SerializeField] private UnityEvent onDie;
    public Image m_healthBar;

    [Header("Movement Settings")]
    [SerializeField] private bool moving;
    [SerializeField] private UnityEvent OnMove;
    public Transform moveTarget;
    [SerializeField] private MovementTypes movementType;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    public GameObject moveTargetPrefab;
    public GameObject generalMoveTargetPrefab;

    [Header("Air Movement Settings")]
    [SerializeField] private float hoverHeight;
    [SerializeField] private float verticleSpeed;

    [Header("Ground Movement Settings")]
    [SerializeField] private NavMeshAgent m_navAgent;

    [Header("Object Avoidance Settings")]
    [SerializeField] private float avoidanceRange;

    [Header("Attack Settings")]
    [SerializeField] private GunSettings[] guns;

    [Header("Important Optimisation List")]
    public ObjectPool op;

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                      / START FUNCTION BELOW \
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start () {

        op = FindObjectOfType<ObjectPool>();
        SetHealth(maxHealth);
        NameUnit();
	}
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                     \ START FUNCTION ABOVE /
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetHealth(float newHealth)
    {
        currentHealth = newHealth;

        m_healthBar.fillAmount = currentHealth / maxHealth;
    }
    void NameUnit()
    {
        if (possibleNames.Length > 0)
        {
            unitName = possibleNames[Random.Range(0, possibleNames.Length)];
        }
        else
        {
            unitName = "Unit_" + Random.Range (0, 1000);
        }
        name = unitName;
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                     /  UPDATE FUNCTION BELOW \
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        RankAction();
        if (rankState != RankState.dead)
        {
            Move();
            AttackClosestEnemy();
        }

    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                      \ UPDATE FUNCTION ABOVE /
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    void RankAction()
    {
        if (rankState == RankState.FollowingGeneral)
        {
            if (myGeneral.rankState != RankState.IsGeneral)
            {
                rankState = RankState.LookingForGeneral;
                killMeAfter = 0.5f;
            }
        }
        if (rankState == RankState.LookingForGeneral)
        {
            if (ClosestGeneral())
                AssignToGeneral(ClosestGeneral());
            if (killMeAfter <= 0)
            {
                Die(this);
            }
            else
            {
                killMeAfter -= Time.deltaTime;
            }
        }
        if (rankState == RankState.dead && gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
    void AssignToGeneral(TroopActor ta)
    {
        rankState = RankState.FollowingGeneral;
        myGeneral = ta;
        if (moveTarget)
        Destroy(moveTarget.gameObject);
        moveTarget = ta.AllocateTarget(this);
    }

    public Transform AllocateTarget(TroopActor ta)
    {
        foreach (FormationPosition fp in formationPositions)
        {
            if (fp.assignedUnit.rankState == RankState.dead)
            {
                fp.taken = false;
            }
            if (!fp.taken)
            {
                fp.assignedUnit = ta;
                fp.taken = true;
                return fp.fromPos;
            }
        }
        FormationPosition newFP = new FormationPosition();
        {
            GameObject newFormPos = new GameObject("dad");
            GameObject trackedFormPos = Instantiate(newFormPos, NextFormPos() + positionOffset, transform.rotation);
            Destroy(newFormPos);
            trackedFormPos.name = "Formation Position " + formationPositions.Count +1;
            if (!moveTarget)
            {
                CreateMoveTarget();
            }
            trackedFormPos.transform.parent = moveTarget.transform;
            newFP.fromPos = trackedFormPos.transform;
            newFP.taken = true;
            newFP.assignedUnit = ta;
            formationPositions.Add(newFP);
            return newFP.fromPos;
        }

    }

    Vector3 NextFormPos()
    {
        //making Sure Old Pos Exists and is parented correctly
        if (!oldPos)
        {
            oldPos = new GameObject("oldPos " + gameObject.name).transform;
            oldPos.parent = moveTarget;
            oldPos.localPosition = Vector3.zero;
            oldPos.localPosition -= new Vector3(0, 0, distanceBetweenPoints.y);
            count = 0;
            return oldPos.position;
        }
        else if (oldPos.parent != moveTarget)
        {
            oldPos.parent = moveTarget;
            oldPos.localPosition = Vector3.zero;
            oldPos.localPosition -= new Vector3(0, 0, distanceBetweenPoints.y);
            count = 0;
            return oldPos.position;
        }


        if (count < maxNumOfColumns / 2)
        {
            count++;
            oldPos.localPosition += (pd == PlacementDirection.right) ? new Vector3(distanceBetweenPoints.x, 0, 0) : new Vector3(-distanceBetweenPoints.x, 0, 0);

        }
        else
        {
            count = 0;
            if (pd == PlacementDirection.left)
                oldPos.localPosition -= new Vector3(0, 0, distanceBetweenPoints.y);


            pd = (pd == PlacementDirection.right) ? PlacementDirection.left : PlacementDirection.right;
            oldPos.localPosition = (pd == PlacementDirection.right) ? new Vector3(0, 0, oldPos.localPosition.z) : new Vector3(-distanceBetweenPoints.x, 0, oldPos.localPosition.z);


        }

        return oldPos.position;
    }

    void AttackClosestEnemy()
    {
        
        foreach (GunSettings gun in guns)
        {
            if (ClosestEnemy(gun))
            {
                gun.attackTarget = ClosestEnemy(gun);
                gun.turret.rotation = Quaternion.Slerp(gun.turret.rotation, Quaternion.LookRotation(gun.attackTarget.transform.position - gun.turret.position), gun.turretAimSpeed * Time.deltaTime);
                gun.turret.localEulerAngles = new Vector3(0, gun.turret.localEulerAngles.y, 0);


                gun.barrel.rotation = Quaternion.Slerp(gun.barrel.rotation, Quaternion.LookRotation(gun.attackTarget.transform.position - gun.barrel.position), gun.barrelAimSpeed * Time.deltaTime);
                gun.barrel.localEulerAngles = new Vector3(gun.barrel.localEulerAngles.x, 0, 0);
                gun.m_gunTimer -= Time.deltaTime;
                if (gun.m_gunTimer < Time.deltaTime)
                {
                    ResetGunTimer(gun);
                    Fire(gun);
                }
            }

        }
       
    }
    void ResetGunTimer(GunSettings gun)
    {
        gun.m_gunTimer = Random.Range ((gun.TimeBetweenShots - (gun.TimeBetweenShots * gun.m_gunTimerRandomiser)),(gun.TimeBetweenShots + (gun.TimeBetweenShots * gun.m_gunTimerRandomiser)));
    }

    void Fire(GunSettings gun)
    {
        gun.onShoot.Invoke();

        if (EnemyInSights(gun) && CanHitTarget(gun.attackTarget))
        {
            if (gun.attackTarget.currentHealth - gun.damage <= 0)
            {
                gun.onKillShot.Invoke();
            }
            gun.attackTarget.TakeDamage(gun.damage);
        }
    }

    bool CanHitTarget(TroopActor checkThis)
    {
        bool canHit = false;
        foreach (UnitClasses uc in strengths)
        {
            if (checkThis.unitClass == uc)
            {
                canHit = true;
            }
        }
        return canHit;
    }

    bool EnemyInSights(GunSettings gun)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (gun.attackTarget.transform.position - transform.position), out hit, Vector3.Distance(transform.position, gun.attackTarget.transform.position)))
        {
            if (hit.transform != gun.attackTarget.transform)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    TroopActor ClosestEnemy(GunSettings gun)
    {
        float dis = 0f;
        TroopActor closestEnemy = null; 
        foreach (TroopActor ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead)
            {
                if ((dis == 0f || Vector3.Distance(ta.transform.position, transform.position) < dis) && EnemyInRange(ta.transform, gun) && ta != this && ta.team != team)
                {
                    dis = Vector3.Distance(ta.transform.position, transform.position);
                    closestEnemy = ta;
                }
            }
        }
        return closestEnemy;
    }
    TroopActor ClosestAlly()
    {
        float dis = 0f;
        TroopActor closestAlly = null;
        foreach (TroopActor ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead)
            if ((dis == 0f || Vector3.Distance(ta.transform.position, transform.position) < dis) && Vector3.Distance(ta.transform.position, transform.position) < influenceRadius && ta != this && ta.team == team &&ta.rankState != RankState.IsGeneral)
            {
                dis = Vector3.Distance(ta.transform.position, transform.position);
                closestAlly = ta;
            }
        }
        return closestAlly;
    }
    TroopActor ClosestGeneral()
    {
        float dis = 0f;
        TroopActor closestAlly = null;
        foreach (TroopActor ta in op.allTroopActors)
        {
            if (ta.rankState != RankState.dead)
            if ((dis == 0f || Vector3.Distance(ta.transform.position, transform.position) < dis) && ta != this && ta.team == team && ta.rankState == RankState.IsGeneral)
            {
                dis = Vector3.Distance(ta.transform.position, transform.position);
                closestAlly = ta;
            }
        }
        return closestAlly;
    }
    bool EnemyInRange(Transform enemy, GunSettings gun)
    {
        if (Vector3.Distance(enemy.position, transform.position) > gun.attackRangeMin && Vector3.Distance(enemy.position, transform.position) < gun.attackRangeMax)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //                                                                     MOVE STUFF
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
    void Move()
    {
        if (!moveTarget)
        {
            CreateMoveTarget();
        }
        if (movementType == MovementTypes.Ground)
        {
            if (!m_navAgent)
            {
                m_navAgent = gameObject.AddComponent<NavMeshAgent>();
                m_navAgent.speed = moveSpeed;
                m_navAgent.angularSpeed = turnSpeed;
            }
            else if (!m_navAgent.isActiveAndEnabled)
            {
                m_navAgent.enabled = true;
            }
            else if (m_navAgent && moveTarget)
            {
                Debug.DrawLine(transform.position, moveTarget.position, Color.red);
                m_navAgent.SetDestination(moveTarget.position);
                if (Vector3.Distance(new Vector3(moveTarget.position.x, transform.position.y, moveTarget.position.z), transform.position) > moveSpeed * Time.deltaTime)

                    moving = true;

                else
                    moving = false;
            }
           
            LookAtMoveTarget();
        }
        if (movementType == MovementTypes.Air)
        {
            if (m_navAgent)
            {
                m_navAgent.enabled = false;
            }
            LookAtMoveTarget();
            if (ClearLineOfSight())
            {
                MoveTowardsMoveTarget();
            }
            else
            {
                Evade();
            }
        }
        if (moving)
        {
            OnMove.Invoke();
        }
    }

 
    void CreateMoveTarget()
    {
        if (!moveTargetPrefab && !generalMoveTargetPrefab)
        {
            GameObject killMePls = new GameObject("Kill me pls");
            killMePls.name = gameObject.name + "'s MoveTarget";
            killMePls.transform.position = transform.position;
            killMePls.transform.rotation = transform.rotation;
            moveTarget = killMePls.transform;
        }
        else if (generalMoveTargetPrefab)
        {
            GameObject keepThisAlive = Instantiate(generalMoveTargetPrefab, transform.position, transform.rotation);
            keepThisAlive.name = gameObject.name + "'s MoveTarget";
            moveTarget = keepThisAlive.transform;
        }
        else if (!generalMoveTargetPrefab && moveTargetPrefab)
        {
            GameObject keepThisAlive = Instantiate(moveTargetPrefab, transform.position, transform.rotation);
            keepThisAlive.name = gameObject.name + "'s MoveTarget";
            moveTarget = keepThisAlive.transform;
        }

    }

    bool ClearLineOfSight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (moveTarget.position - transform.position), out hit, avoidanceRange))
        {
            if (hit.transform != moveTarget)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                return false;
            }
            else
            {
                Debug.DrawLine(transform.position, moveTarget.position, Color.grey);
                return true;
            }
        }
        else
        {
            Debug.DrawLine(transform.position, moveTarget.position, Color.grey);
            return true;
        }
    }
    public Vector3 previousPos;
    void LookAtMoveTarget()
    {
        Vector3 lookRotation = transform.position - previousPos;

        if (moveTarget && moving)
            if(lookRotation != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((transform.position - previousPos)), turnSpeed * Time.deltaTime);

        previousPos = transform.position;

        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }
    void Evade()
    {
        transform.position += Vector3.up * verticleSpeed * Time.deltaTime;
    }

    void MoveTowardsMoveTarget()
    {
        if (Vector3.Distance(new Vector3(moveTarget.position.x, transform.position.y, moveTarget.position.z), transform.position) > avoidanceRange)
        {
            transform.position += (new Vector3(moveTarget.position.x, transform.position.y, moveTarget.position.z) - transform.position).normalized * moveSpeed * Time.deltaTime;
            moving = true;
        }
        else if (Vector3.Distance(new Vector3(moveTarget.position.x, moveTarget.position.y + hoverHeight, moveTarget.position.z), transform.position) > moveSpeed * Time.deltaTime)
        {
            transform.position += (new Vector3(moveTarget.position.x, moveTarget.position.y + hoverHeight, moveTarget.position.z) - transform.position).normalized * verticleSpeed * Time.deltaTime;
            moving = true;
        }
        else
        {
            moving = false;
        }
    }

    public void TakeDamage(float damageToTake)
    {
        onTakeDamage.Invoke();
        currentHealth -= damageToTake;
        if (currentHealth <= 0)
        {
            Die(this);
        }

        m_healthBar.fillAmount = currentHealth / maxHealth;
    }
    public void Die(TroopActor ta)
    {
        ta.onDie.Invoke();
        if (ta.rankState == RankState.IsGeneral)
        {
            if (ClosestAlly())
            ta.PromoteToGeneral(ClosestAlly());
            ta.rankState = RankState.dead;
            if (moveTarget)
            Destroy(moveTarget.gameObject);
        }
        ta.rankState = RankState.dead;

        if (ta.team == Team.TEAM1)
            op.team1Generals.Remove(ta);

        if (ta.team == Team.TEAM2)
            op.team2Generals.Remove(ta);

        gameObject.SetActive(false);
    }
    private void OnDrawGizmosSelected()
    {
        
        foreach (GunSettings gun in guns)
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, gun.attackRangeMin);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, gun.attackRangeMax);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, influenceRadius);

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, avoidanceRange);
    }

    public void grantExperience(float expToGrant)
    {
        OnGainExperience.Invoke();
        currentExperience += expToGrant;
        if (currentExperience >= experienceRequiredForNextRank)
        {
            RankUp();
        }
    }
    void RankUp()
    {

        if (rank + 1 < rankTitles.Length)
        {
            OnPromotion.Invoke();
            rank++;
            name = rankTitles[rank] + " " + unitName;
            experienceRequiredForNextRank += experienceRequiredForNextRank * percentageIncreasePerRank / 100f;
            currentExperience = 0f;
            influenceRadius += influenceRadius * percentageIncreasePerRank / 100f;
        }
    }

    public void PromoteToGeneral(TroopActor ta)
    {
        ta.OnBecomeGeneral.Invoke();
        ta.rankState = RankState.IsGeneral;
        ta.moveTarget = null;

        if (ta.team == Team.TEAM1)
            op.team1Generals.Add(ta);

        if (ta.team == Team.TEAM2)
            op.team2Generals.Add(ta);
    }
}
